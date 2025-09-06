using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Application.Services.Auth;
using UcarMobileApi.Application.Services.Security;
using UcarMobileApi.Core.Entities.Users;
using UcarMobileApi.Tests.TestHelpers;
using Xunit;

namespace UcarMobileApi.Tests.Unit.Services;

public class AuthServiceTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public async Task RegisterUserAsync_Creates_User_And_Assigns_Customer_Role()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var context = TestDbContextFactory.CreateInMemoryContext(dbName);

        // Seed: Customer role required by the service
        context.Set<Role>().Add(new Role { Name = "Customer", Description = "Default role" });
        await context.SaveChangesAsync();

        var mapper = TestMapperFactory.CreateMapper();

        // Authorization mock: only used in GetCurrentUserAsync, so not relevant here
        var auth = new Mock<IUserAuthorizationService>();

        var service = new AuthService(context, auth.Object, mapper);

        var dto = new UserDto
        {
            CognitoId = "cog-123",
            Email = "john.doe@example.com",
            FirstName = "John",
            LastName = "Doe"
        };

        // Act
        await service.RegisterUserAsync(dto, CancellationToken.None);

        // Verify in DB
        var inDb = await context.Set<User>()
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == "john.doe@example.com");

        Assert.NotNull(inDb);
        Assert.Single(inDb!.UserRoles);
        Assert.Equal("Customer", inDb.UserRoles.First().Role.Name);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetCurrentUserAsync_Returns_User_Info_And_Permissions()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var context = TestDbContextFactory.CreateInMemoryContext(dbName);

        // Seed: role + user with that role
        var customer = new Role { Name = "Customer", Description = "Default role" };
        context.Set<Role>().Add(customer);
        await context.SaveChangesAsync();

        var user = new User
        {
            CognitoId = "cog-abc",
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@demo.com",
            IsActive = true,
            UserRoles = { new UserRole { RoleId = customer.Id } }
        };
        context.Set<User>().Add(user);
        await context.SaveChangesAsync();

        // Authorization mock: this IS used here
        var auth = new Mock<IUserAuthorizationService>();
        auth.Setup(a => a.GetUserPermissionsAsync("cog-abc", CancellationToken.None))
            .ReturnsAsync(["appointment.schedule", "appointment.view.own"]);

        var mapper = TestMapperFactory.CreateMapper();

        var service = new AuthService(context, auth.Object, mapper);

        // Act
        dynamic result = await service.GetCurrentUserAsync("cog-abc", CancellationToken.None);

        // Assert (dynamic to access properties of the anonymous object)
        Assert.Equal("cog-abc", (string)result.CognitoId);
        Assert.Equal("Jane", (string)result.FirstName);
        Assert.Equal("jane@demo.com", (string)result.Email);

        var roles = (IEnumerable<string>)result.Roles;
        Assert.Single(roles);
        Assert.Contains("Customer", roles);

        var permissions = (IEnumerable<string>)result.Permissions;
        Assert.Contains("appointment.schedule", permissions);
        Assert.Contains("appointment.view.own", permissions);
    }

}
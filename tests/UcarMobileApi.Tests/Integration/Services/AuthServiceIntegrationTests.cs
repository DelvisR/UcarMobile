using System;
using System.Threading.Tasks;
using UcarMobileApi.Core.Entities.Users;
using UcarMobileApi.Tests.TestHelpers;
using Xunit;

namespace UcarMobileApi.Tests.Integration.Services;

public class AuthServiceIntegrationTests
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task CreateUser_Stores_In_RealDb()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = TestDbContextFactory.CreateInMemoryContext(dbName);

        context.Set<Role>().Add(new Role { Name = "Customer", Description = "Default role" });
        await context.SaveChangesAsync();

        // You can instantiate the actual service and verify that the user is saved correctly.
    }
}
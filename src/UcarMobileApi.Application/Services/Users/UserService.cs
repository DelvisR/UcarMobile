using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Gridify;
using Gridify.EntityFramework;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.Common.Models;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Application.Utilities;
using UcarMobileApi.Application.Validators.Users;
using UcarMobileApi.Core.Entities.Users;

namespace UcarMobileApi.Application.Services.Users;

public class UserService(IMapper mapper, IAppDbContext context, IGridifyMapper<User> gridifymapper)
{
    public async Task<(IHeaderDictionary, IEnumerable<UserAccountDto>)> GetUsersAsync(QueryFilter query, CancellationToken ct)
    {
        var users = context.Set<User>().AsNoTracking();

        // AutoMapper ProjectTo + Filtering + Ordering + Paging
        var qp = await users.GridifyQueryableAsync(query, gridifymapper, ct);

        return (qp.GeneratePaginationHttpHeaders(), await qp.Query.ProjectTo<UserAccountDto>(mapper.ConfigurationProvider).ToListAsync(ct));
    }

    public async Task<UserDto?> GetUserAsync(int id, CancellationToken ct)
    {
        return await context.Set<User>()
            .AsNoTracking()
            .Where(u => u.Id == id)
            .ProjectTo<UserAccountDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }

    public async Task CreateUserAsync(UserAccountDto dto, CancellationToken ct)
    {
        // Validation
        var validator = new UserAccountValidator(context);
        await validator.ValidateAndThrowAsync(dto, ct);

        var user = mapper.Map<User>(dto);

        context.Set<User>().Add(user);
        await context.SaveChangesAsync(ct);
    }

    public async Task UpdateUserAsync(int id, UserAccountDto dto, CancellationToken ct)
    {
        // Validation
        var validator = new UserAccountValidator(context);
        await validator.ValidateAndThrowAsync(dto, ct);

        var user = await context.Set<User>()
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id, ct);

        if (user != null)
        {
            mapper.Map(dto, user);

            await context.SaveChangesAsync(ct);
        }
        else
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }
    }

    public async Task ActivateUserAsync(int id, bool active, CancellationToken ct)
    {
        var user = await context.Set<User>().FindAsync([id], ct);
        if (user != null)
        {
            user.IsActive = active;
            await context.SaveChangesAsync(ct);
        }
        else
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }
    }

    /// <summary>
    /// Assigns or updates the roles for a specific user.
    /// Uses AutoMapper.Collection to synchronize the UserRoles collection:
    /// - Adds new roles
    /// - Removes roles not included in the DTO list
    /// - Avoids duplicate Role insertions
    /// </summary>
    /// <param name="userId">The ID of the user to update.</param>
    /// <param name="roles">List of roles to assign.</param>
    /// <param name="ct">Request cancellation token.</param>
    /// <exception cref="KeyNotFoundException">Thrown if the user is not found.</exception>
    public async Task AssignRolesAsync(int userId, List<RoleDto> roles, CancellationToken ct)
    {
        // Validate roles list before applying changes
        var validator = new UserRoleListValidator();
        await validator.ValidateAndThrowAsync(roles, ct);

        // Load the user with UserRoles to allow AutoMapper.Collection to sync the collection
        var user = await context.Set<User>()
                       .Include(u => u.UserRoles)
                       .ThenInclude(ur => ur.Role)
                       .FirstOrDefaultAsync(u => u.Id == userId, ct)
                   ?? throw new KeyNotFoundException($"User with ID {userId} not found.");

        // Synchronize the UserRoles collection
        // AutoMapper.Collection performs:
        // - Add if Role not present
        // - Remove missing entries
        // - Match based on EqualityComparison in AutoMapper Profile
        mapper.Map(roles, user.UserRoles);

        await context.SaveChangesAsync(ct);
    }

}

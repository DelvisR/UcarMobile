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
}

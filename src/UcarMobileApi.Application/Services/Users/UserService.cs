using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Application.Validators.Users;
using UcarMobileApi.Core.Entities.Users;

namespace UcarMobileApi.Application.Services.Users;

public class UserService(IMapper mapper, IAppDbContext context)
{
    public async Task<List<UserDto>> GetUsersAsync(CancellationToken ct)
    {
        return await context.Set<User>()
            .AsNoTracking()
            .ProjectTo<UserDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<UserDto?> GetUserAsync(int id, CancellationToken ct)
    {
        return await context.Set<User>()
            .AsNoTracking()
            .Where(u => u.Id == id)
            .ProjectTo<UserDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }

    public async Task CreateUserAsync(UserDto dto, CancellationToken ct)
    {
        // Validation
        var validator = new UserValidator<UserDto>(context);
        await validator.ValidateAndThrowAsync(dto, ct);

        var user = mapper.Map<User>(dto);

        context.Set<User>().Add(user);
        await context.SaveChangesAsync(ct);
    }

    public async Task UpdateUserAsync(int id, UserDto dto, CancellationToken ct)
    {
        // Validation
        var validator = new UserValidator<UserDto>(context);
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

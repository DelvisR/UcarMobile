using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Application.Validators.Users;
using UcarMobileApi.Core.Entities.Users;
using UcarMobileApi.Infrastructure.Data;

namespace UcarMobileApi.Application.Services.Users;

public class UserService(IMapper mapper, AppDbContext context)
{
    public async Task<List<UserDto>> GetUsersAsync(CancellationToken ct)
    {
        return await context.Set<User>()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Select(u => mapper.Map<UserDto>(u))
            .ToListAsync(ct);
    }

    public async Task<UserDto?> GetUserAsync(int id, CancellationToken ct)
    {
        var user = await context.Set<User>()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id, ct);

        return user == null ? null : mapper.Map<UserDto>(user);
    }

    public async Task CreateUserAsync(UserDto dto, CancellationToken ct)
    {
        // Validation
        var validator = new CreateUserValidator(context);
        await validator.ValidateAndThrowAsync(dto, ct);

        var user = mapper.Map<User>(dto);

        context.Add(user); // same to write context.Set<User>().Add(user);
        await context.SaveChangesAsync(ct);
    }

    public async Task UpdateUserAsync(int id, UserDto dto, CancellationToken ct)
    {
        // Validation
        var validator = new UpdateUserValidator(context);
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
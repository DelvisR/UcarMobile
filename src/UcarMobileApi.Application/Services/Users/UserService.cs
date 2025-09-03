using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Application.Services.Security;
using UcarMobileApi.Application.Validators.Users;
using UcarMobileApi.Core.Entities.Users;
using UcarMobileApi.Infrastructure.Data;

namespace UcarMobileApi.Application.Services.Users;

public class UserService(IMapper mapper, AppDbContext context, IUserAuthorizationService authorizationService)
{
    public async Task<List<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        return await context.Set<User>()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Select(u => mapper.Map<UserDto>(u))
            .ToListAsync(cancellationToken);
    }

    public async Task<UserDto?> GetUserAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await context.Set<User>()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        return user == null ? null : mapper.Map<UserDto>(user);
    }

    public async Task UpdateUserAsync(int id, UpdateUserDto updateUserDto, string cognitoId, CancellationToken cancellationToken = default)
    {
        // Validation
        var validator = new UpdateUserValidator();
        await validator.ValidateAndThrowAsync(updateUserDto);

        var user = await context.Set<User>()
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user == null)
            throw new KeyNotFoundException($"User with ID {id} not found.");

        if (!string.IsNullOrEmpty(updateUserDto.FirstName)) user.FirstName = updateUserDto.FirstName;
        if (!string.IsNullOrEmpty(updateUserDto.LastName)) user.LastName = updateUserDto.LastName;
        if (updateUserDto.IsActive.HasValue) user.IsActive = updateUserDto.IsActive.Value;

        if (updateUserDto.RoleIds != null)
        {
            if (!await authorizationService.HasPermissionAsync(cognitoId, "user.assign_role"))
                throw new UnauthorizedAccessException("You do not have permission to assign roles.");

            var existingRoleIds = user.UserRoles.Select(ur => ur.RoleId).ToList();
            var rolesToAdd = updateUserDto.RoleIds.Except(existingRoleIds).ToList();
            var rolesToRemove = existingRoleIds.Except(updateUserDto.RoleIds).ToList();

            foreach (var roleId in rolesToAdd)
            {
                if (!await context.Set<Role>().AnyAsync(r => r.Id == roleId, cancellationToken))
                    throw new InvalidOperationException($"Role with ID {roleId} not found.");

                user.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = roleId });
            }

            foreach (var roleId in rolesToRemove)
            {
                var userRole = user.UserRoles.FirstOrDefault(ur => ur.RoleId == roleId);
                if (userRole != null)
                    context.Set<UserRole>().Remove(userRole);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task ActivateUserAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await context.Set<User>().FindAsync(id);
        if (user == null) throw new KeyNotFoundException($"User with ID {id} not found.");

        user.IsActive = true;
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateUserAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await context.Set<User>().FindAsync(id);
        if (user == null) throw new KeyNotFoundException($"User with ID {id} not found.");

        user.IsActive = false;
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task AssignRolesAsync(int id, List<int> roleIds, string cognitoId, CancellationToken cancellationToken = default)
    {
        var updateDto = new UpdateUserDto { RoleIds = roleIds };
        await UpdateUserAsync(id, updateDto, cognitoId, cancellationToken);
    }
}
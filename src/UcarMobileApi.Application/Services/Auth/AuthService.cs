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

namespace UcarMobileApi.Application.Services.Auth;

public class AuthService(AppDbContext context, IUserAuthorizationService authorizationService, IMapper mapper) : IAuthService
{
    public async Task RegisterUserAsync(UserDto userDto, CancellationToken ct)
    {
        // Validate input using FluentValidation
        var validator = new UpdateUserValidator(context);
        await validator.ValidateAndThrowAsync(userDto, ct);

        // Load default roles for (Customer) 
        var customerRole = await context.Set<Role>()
            .Where(r => r.Name == "Customer")
            .Select(r => mapper.Map<RoleDto>(r))
            .ToListAsync(ct);

        userDto.Roles = customerRole;

        // Map DTO to entity and roles
        var user = mapper.Map<User>(userDto);
        user.IsActive = true;

        // Save User and UserRole in a single transaction
        await context.AddAsync(user, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<object> GetCurrentUserAsync(string cognitoId, CancellationToken ct)
    {
        // Project user and roles directly without tracking
        var userProjection = await context.Set<User>()
            .AsNoTracking()
            .Where(u => u.CognitoId == cognitoId)
            .Select(u => new
            {
                u.Id,
                u.CognitoId,
                u.FirstName,
                u.LastName,
                u.Email,
                u.IsActive,
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
            })
            .FirstOrDefaultAsync(ct);

        if (userProjection != null)
        {
            if (!userProjection.IsActive)
                throw new UnauthorizedAccessException("User is inactive.");

            // Retrieve permissions using the authorization service
            var permissions = await authorizationService.GetUserPermissionsAsync(cognitoId, ct);

            return new
            {
                userProjection.Id,
                userProjection.CognitoId,
                userProjection.FirstName,
                userProjection.LastName,
                userProjection.Email,
                userProjection.IsActive,
                userProjection.Roles,
                Permissions = permissions
            };
        }

        throw new KeyNotFoundException("User not found.");
    }
}
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Application.Validators.Users;
using UcarMobileApi.Core.Entities.Users;
using UcarMobileApi.Infrastructure.Data;

namespace UcarMobileApi.Application.Services.Security;

public class RoleService(IUserAuthorizationService authorizationService, AppDbContext context, IMapper mapper)
{
    private readonly AppDbContext _context = context;
    private readonly IUserAuthorizationService _authorizationService = authorizationService;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<RoleDto>> GetRolesAsync()
    {
        return await _context.Set<Role>()
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .Select(r => _mapper.Map<RoleDto>(r))
            .ToListAsync();
    }

    public async Task<RoleDto?> GetRoleAsync(int id)
    {
        var role = await _context.Set<Role>()
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == id);

        return role == null ? null : _mapper.Map<RoleDto>(role);
    }

    public async Task<RoleDto> CreateRoleAsync(CreateRoleDto dto)
    {
        // Validation
        var validator = new CreateRoleValidator();
        await validator.ValidateAndThrowAsync(dto);

        if (await _context.Set<Role>().AnyAsync(r => r.Name == dto.Name))
            throw new InvalidOperationException("Role with this name already exists.");

        var role = _mapper.Map<Role>(dto);

        _context.Set<Role>().Add(role);
        await _context.SaveChangesAsync();

        // Assign permissions if any
        if (dto.PermissionIds != null && dto.PermissionIds.Any())
        {
            var rolePermissions = dto.PermissionIds.Select(pid => new RolePermission
            {
                RoleId = role.Id,
                PermissionId = pid
            }).ToList();

            _context.Set<RolePermission>().AddRange(rolePermissions);
            await _context.SaveChangesAsync();
        }

        return await GetRoleAsync(role.Id) ?? throw new InvalidOperationException("Role creation failed.");
    }

    public async Task UpdateRoleAsync(int id, UpdateRoleDto dto, string cognitoId)
    {
        var role = await _context.Set<Role>()
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (role == null)
            throw new KeyNotFoundException("Role not found.");

        if (!string.IsNullOrEmpty(dto.Name)) role.Name = dto.Name;
        if (!string.IsNullOrEmpty(dto.Description)) role.Description = dto.Description;

        // Permissions update
        if (dto.PermissionIds != null)
        {
            if (!await _authorizationService.HasPermissionAsync(cognitoId, "role.assign_permission"))
                throw new UnauthorizedAccessException("You do not have permission to assign permissions to roles.");

            var existingIds = role.RolePermissions.Select(rp => rp.PermissionId).ToList();
            var toAdd = dto.PermissionIds.Except(existingIds).ToList();
            var toRemove = existingIds.Except(dto.PermissionIds).ToList();

            foreach (var idAdd in toAdd)
                role.RolePermissions.Add(new RolePermission { RoleId = role.Id, PermissionId = idAdd });

            foreach (var idRemove in toRemove)
            {
                var rp = role.RolePermissions.FirstOrDefault(rp => rp.PermissionId == idRemove);
                if (rp != null) _context.Set<RolePermission>().Remove(rp);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteRoleAsync(int id)
    {
        var role = await _context.Set<Role>().FindAsync(id);
        if (role == null) throw new KeyNotFoundException("Role not found.");

        _context.Set<Role>().Remove(role);
        await _context.SaveChangesAsync();
    }

    public async Task AssignPermissionsAsync(int id, List<int> permissionIds)
    {
        var role = await _context.Set<Role>()
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (role == null) throw new KeyNotFoundException("Role not found.");

        _context.Set<RolePermission>().RemoveRange(role.RolePermissions);

        var newRps = permissionIds.Select(pid => new RolePermission { RoleId = id, PermissionId = pid });
        _context.Set<RolePermission>().AddRange(newRps);

        await _context.SaveChangesAsync();
    }
}

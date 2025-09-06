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

public class RoleService(AppDbContext context, IMapper mapper)
{
    public async Task<IEnumerable<RoleDto>> GetRolesAsync(CancellationToken ct)
    {
        return await context.Set<Role>()
            .Select(r => mapper.Map<RoleDto>(r))
            .ToListAsync(ct);
    }

    public async Task<RoleDto?> GetRoleAsync(int id, CancellationToken ct)
    {
        var role = await context.Set<Role>()
            .FirstOrDefaultAsync(r => r.Id == id, ct);

        return role == null ? null : mapper.Map<RoleDto>(role);
    }

    public async Task CreateRoleAsync(RoleDto dto, CancellationToken ct)
    {
        // Validation
        var validator = new RoleValidator(context);
        await validator.ValidateAndThrowAsync(dto, ct);

        var role = mapper.Map<Role>(dto);

        context.Add(role);
        await context.SaveChangesAsync(ct);
    }

    public async Task UpdateRoleAsync(int id, RoleDto dto, CancellationToken ct)
    {
        // Validation
        var validator = new RoleValidator(context);
        await validator.ValidateAndThrowAsync(dto, ct);

        var role = await context.Set<Role>()
            .FirstOrDefaultAsync(r => r.Id == id, ct);

        if (role == null) throw new KeyNotFoundException($"Role with ID {id} not found.");

        mapper.Map(dto, role);

        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteRoleAsync(int id, CancellationToken ct)
    {
        var role = await context.Set<Role>().FindAsync(id, ct);
        if (role == null) throw new KeyNotFoundException("Role not found.");

        context.Set<Role>().Remove(role);
        await context.SaveChangesAsync(ct);
    }

    public async Task AssignPermissionsAsync(int id, List<PermissionDto> permissions, CancellationToken ct)
    {
        var role = await context.Set<Role>()
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

        if (role == null) throw new KeyNotFoundException($"Role with ID {id} not found.");

        // We use AutoMapper.Collection to synchronize permissions.
        mapper.Map(permissions, role.RolePermissions);

        await context.SaveChangesAsync(ct);
    }
}

using AutoMapper;
using AutoMapper.EquivalencyExpression;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Core.Entities.Users;

namespace UcarMobileApi.Application.Mapping.Users;

public class RoleProfile : Profile
{
    public RoleProfile()
    {
        // Role <-> RoleDto (no Permissions)
        CreateMap<Role, RoleDto>();
        CreateMap<RoleDto, Role>()
            .EqualityComparison((dto, entity) => dto.Id == entity.Id)
            .ForMember(dest => dest.RolePermissions, opt => opt.Ignore());

        // Permission <-> PermissionDto
        CreateMap<Permission, PermissionDto>();
        CreateMap<PermissionDto, Permission>()
            .EqualityComparison((dto, entity) => dto.Id == entity.Id);

        // PermissionDto -> RolePermission (join)
        CreateMap<PermissionDto, RolePermission>()
            .ConstructUsing(dto => new RolePermission { PermissionId = dto.Id })
            .EqualityComparison((dto, entity) => dto.Id == entity.PermissionId);
    }
}
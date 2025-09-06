using AutoMapper;
using AutoMapper.EquivalencyExpression;
using System.Linq;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Core.Entities.Users;

namespace UcarMobileApi.Application.Mapping.Users;

public class UserProfile : Profile
{
    public UserProfile()
    {
        // User -> UserDto
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role)));

        // UserDto -> User
        CreateMap<UserDto, User>()
            .EqualityComparison((dto, entity) => dto.Id == entity.Id)
            .ForMember(dest => dest.UserRoles, opt => opt.MapFrom(src => src.Roles));

        // RoleDto -> UserRole (join)
        CreateMap<RoleDto, UserRole>()
            .ConstructUsing(dto => new UserRole { RoleId = dto.Id })
            .EqualityComparison((dto, entity) => dto.Id == entity.RoleId);

        // Role -> RoleDto (sin permisos)
        CreateMap<Role, RoleDto>();
        CreateMap<RoleDto, Role>()
            .EqualityComparison((dto, entity) => dto.Id == entity.Id);
    }
}
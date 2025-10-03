using System.Linq;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Core.Entities.Users;

namespace UcarMobileApi.Application.Mapping.Users;

public class UserProfile : Profile
{
    public UserProfile()
    {
        // User -> UserDto
        CreateMap<User, UserDto>()
        //.ForMember(dest => dest.Roles, opt => opt.Ignore());
        .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role)));

        // UserDto -> User
        CreateMap<UserDto, User>()
            .EqualityComparison((dto, entity) => dto.Id == entity.Id)
            .ForMember(dest => dest.UserRoles, opt => opt.MapFrom(src => src.Roles));

        // User -> CurrentUserDto
        CreateMap<User, CurrentUserDto>()
            //.ForMember(dest => dest.Roles, opt => opt.Ignore());
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role)));

        // Map Role -> RoleNameDto (only the Name field)
        CreateMap<Role, RoleNameDto>();

        // RoleDto -> UserRole (join)
        CreateMap<RoleDto, UserRole>()
            .ConstructUsing(dto => new UserRole { RoleId = dto.Id })
            .EqualityComparison((dto, entity) => dto.Id == entity.RoleId);

        // Role -> RoleDto (without actions)
        CreateMap<Role, RoleDto>();
        CreateMap<RoleDto, Role>()
            .EqualityComparison((dto, entity) => dto.Id == entity.Id);
    }
}

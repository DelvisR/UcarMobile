using System.Linq;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Application.Mapping.Common;
using UcarMobileApi.Core.Entities.Users;

namespace UcarMobileApi.Application.Mapping.Users;

public class UserProfile : Profile
{
    public UserProfile()
    {
        // User -> UserDto
        CreateMap<User, UserDto>()
            .ForMember(d => d.File, o => o.MapFrom(s => s.ImageStoredFile))
            .ReverseMap();

        CreateMap<UserUpdateDto, User>()
            .IgnoreNullValuesForPatch();

        // User -> CurrentUserDto
        CreateMap<User, UserAccountDto>()
            .IncludeBase<User, UserDto>()
            .ForMember(dest => dest.AuthProviderId, opt => opt.Ignore())
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role)));

        // UserDto -> User
        CreateMap<UserAccountDto, User>()
            .ForMember(dest => dest.AuthProviderId, opt => opt.Ignore())
            .EqualityComparison((dto, entity) => dto.Id == entity.Id)
            .ForMember(dest => dest.UserRoles, opt => opt.MapFrom(src => src.Roles));

        // User -> CurrentUserDto
        CreateMap<User, CurrentUserDto>()
            .IncludeBase<User, UserDto>()
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role)));

        // Map Role -> RoleNameDto (only the Name field)
        CreateMap<Role, RoleNameDto>();

        // RoleDto -> UserRole (join)
        CreateMap<RoleDto, UserRole>()
            .ConstructUsing(dto => new UserRole { RoleId = dto.Id })
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.Ignore()) // avoid duplicate insertion
            .EqualityComparison((dto, entity) => dto.Id == entity.RoleId);

        CreateMap<UserRole, RoleDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RoleId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Role.Name));

        // Role -> RoleDto (without actions)
        CreateMap<Role, RoleDto>();
        CreateMap<RoleDto, Role>()
            .EqualityComparison((dto, entity) => dto.Id == entity.Id);
    }
}

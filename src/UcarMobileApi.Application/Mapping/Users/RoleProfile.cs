using AutoMapper;
using AutoMapper.EquivalencyExpression;
using UcarMobileApi.Application.DTOs.Users;
using UcarMobileApi.Core.Entities.Users;

namespace UcarMobileApi.Application.Mapping.Users;

public class RoleProfile : Profile
{
    public RoleProfile()
    {
        // Role <-> RoleDto (no Actions)
        CreateMap<Role, RoleDto>();
        CreateMap<RoleDto, Role>()
            .EqualityComparison((dto, entity) => dto.Id == entity.Id)
            .ForMember(dest => dest.RoleActions, opt => opt.Ignore());

        // Action <-> ActionDto
        CreateMap<Action, ActionDto>();
        CreateMap<ActionDto, Action>()
            .EqualityComparison((dto, entity) => dto.Id == entity.Id);

        // ActionDto -> RoleAction (join)
        CreateMap<ActionDto, RoleAction>()
            .ConstructUsing(dto => new RoleAction { ActionId = dto.Id })
            .ForMember(dest => dest.RoleId, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.Action, opt => opt.Ignore())
            .EqualityComparison((dto, entity) => dto.Id == entity.ActionId);
    }
}

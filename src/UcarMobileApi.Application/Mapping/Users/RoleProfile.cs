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
            .EqualityComparison((dto, entity) => dto.Id == entity.ActionId);
    }
}

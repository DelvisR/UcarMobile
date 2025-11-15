using AutoMapper;
using UcarMobileApi.Application.DTOs.Clients;
using UcarMobileApi.Core.Entities.Clients;

namespace UcarMobileApi.Application.Mapping.Clients;

/// <summary>
/// AutoMapper profile to map Lead entity to/from DTOs.
/// </summary>
public class LeadProfile : Profile
{
    public LeadProfile()
    {
        // Entity -> DTO
        CreateMap<Lead, LeadDto>().ReverseMap();
    }
}

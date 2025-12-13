using AutoMapper;
using UcarMobileApi.Application.DTOs.Services;
using UcarMobileApi.Core.Entities.Services;

namespace UcarMobileApi.Application.Mapping.Services;

public class EstimateMappingProfile : Profile
{
    public EstimateMappingProfile()
    {
        CreateMap<Estimate, EstimateDto>()
            .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service.Name))
            .ForMember(dest => dest.ServiceCategoryId, opt => opt.MapFrom(src => src.Service.ServiceCategoryId))
            .ForMember(dest => dest.ServiceCategoryName, opt => opt.MapFrom(src => src.Service.ServiceCategory.Name));
    }
}

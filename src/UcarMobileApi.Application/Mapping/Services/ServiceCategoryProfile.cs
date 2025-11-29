using AutoMapper;
using UcarMobileApi.Application.DTOs.Services;
using UcarMobileApi.Core.Entities.Services;

namespace UcarMobileApi.Application.Mapping.Services;

public class ServiceCategoryProfile : Profile
{
    public ServiceCategoryProfile()
    {
        CreateMap<ServiceCategory, ServiceCategoryDto>().ReverseMap();
    }
}

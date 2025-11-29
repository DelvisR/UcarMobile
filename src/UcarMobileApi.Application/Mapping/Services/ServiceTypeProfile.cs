using AutoMapper;
using UcarMobileApi.Application.DTOs.Services;
using UcarMobileApi.Core.Entities.Services;

namespace UcarMobileApi.Application.Mapping.Services;

public class ServiceTypeProfile : Profile
{
    public ServiceTypeProfile()
    {
        CreateMap<ServiceType, ServiceTypeDto>().ReverseMap();
    }
}

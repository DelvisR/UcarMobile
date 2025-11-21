using AutoMapper;
using UcarMobileApi.Application.DTOs.ServiceZone;
using UcarMobileApi.Core.Entities.Services;

namespace UcarMobileApi.Application.Mapping;

public class ServiceZoneProfile : Profile
{
    public ServiceZoneProfile()
    {
        CreateMap<ServiceZone, ServiceZoneDto>().ReverseMap();
    }
}

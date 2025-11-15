using AutoMapper;
using UcarMobileApi.Application.DTOs.ServiceZone;
using UcarMobileApi.Core.Entities.ServiceZone;

namespace UcarMobileApi.Application.Mapping;

public class ServiceZoneProfile : Profile
{
    public ServiceZoneProfile()
    {
        CreateMap<ServiceZone, ServiceZoneDto>().ReverseMap();
    }
}

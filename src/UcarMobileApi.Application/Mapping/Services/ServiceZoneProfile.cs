using AutoMapper;
using UcarMobileApi.Application.DTOs;
using UcarMobileApi.Core.Entities.Services;

namespace UcarMobileApi.Application.Mapping.Services;

public class ServiceZoneProfile : Profile
{
    public ServiceZoneProfile()
    {
        CreateMap<ServiceZone, ServiceZoneDto>().ReverseMap();
    }
}

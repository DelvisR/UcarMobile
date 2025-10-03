using AutoMapper;
using UcarMobileApi.Application.DTOs;
using UcarMobileApi.Core.Entities;

namespace UcarMobileApi.Application.Mapping;

public class VehicleProfile : Profile
{
    public VehicleProfile()
    {
        CreateMap<Vehicle, VehicleDto>().ReverseMap();
    }
}

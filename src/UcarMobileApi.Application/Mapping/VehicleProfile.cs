using AutoMapper;
using UcarMobileApi.Application.DTOs.Vehicles;
using UcarMobileApi.Core.Entities.Vehicles;

namespace UcarMobileApi.Application.Mapping;

public class VehicleProfile : Profile
{
    public VehicleProfile()
    {
        CreateMap<Vehicle, VehicleDto>().ReverseMap();

        CreateMap<Vehicle, ModelDto>()
            .ForMember(dest => dest.VehicleId, opt => opt.MapFrom(src => src.Id));

        CreateMap<VehicleSubModel, SubModelDto>();

        CreateMap<VehicleEngine, EngineDto>();
    }
}

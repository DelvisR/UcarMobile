using System.Linq;
using AutoMapper;
using UcarMobileApi.Application.DTOs.Clients;
using UcarMobileApi.Application.Mapping.Common;
using UcarMobileApi.Core.Entities.Clients;

namespace UcarMobileApi.Application.Mapping.Clients;

public class ClientVehicleProfile : Profile
{
    public ClientVehicleProfile()
    {
        CreateMap<ClientVehicleBaseDto, ClientVehicle>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Appointments, opt => opt.Ignore())
            .ForMember(dest => dest.Client, opt => opt.Ignore())
            .ForMember(dest => dest.Vehicle, opt => opt.Ignore());

        CreateMap<ClientVehicleUpsertDto, ClientVehicle>()
            .IncludeBase<ClientVehicleBaseDto, ClientVehicle>();

        CreateMap<ClientVehicleUpdateDto, ClientVehicle>()
            .IncludeBase<ClientVehicleBaseDto, ClientVehicle>()
            .IgnoreNullValuesForPatch();

        CreateMap<ClientVehicle, ClientVehicleDto>()
            .ForMember(dest => dest.LastServiceDate, opt => opt.MapFrom(src => src.Appointments
            .Where(av => av.Appointment.CompletedAt != null)
            .Max(av => (System.DateTime?)av.Appointment.CompletedAt)));

        CreateMap<ClientVehicle, ClientVehicleBasicDto>()
            .ForMember(dest => dest.Year, opt => opt.MapFrom(v => v.Vehicle.Year))
            .ForMember(dest => dest.Make, opt => opt.MapFrom(v => v.Vehicle.Make))
            .ForMember(dest => dest.Model, opt => opt.MapFrom(v => v.Vehicle.Model));

    }
}

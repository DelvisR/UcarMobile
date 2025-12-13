using AutoMapper;
using UcarMobileApi.Application.DTOs.Appointments;
using UcarMobileApi.Application.DTOs.Clients;
using UcarMobileApi.Core.Entities.Appointments;
using UcarMobileApi.Core.Entities.Clients;

namespace UcarMobileApi.Application.Mapping.Appointments;

public class AppointmentProfile : Profile
{
    public AppointmentProfile()
    {
        // Appointment Mappings
        CreateMap<AppointmentCreateDto, Appointment>()
            .ForMember(dest => dest.Vehicles, opt => opt.Ignore()) // Handled manually in Service
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => AppointmentStatus.Requested))
            .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => Core.Enums.PaymentStatus.Unpaid));

        CreateMap<Appointment, AppointmentDto>();

        // Appointment Vehicle Mappings
        CreateMap<AppointmentVehicle, AppointmentVehicleDto>();

        // Appointment Service/Part Mappings
        CreateMap<AppointmentServiceCreateDto, AppointmentService>();
        CreateMap<AppointmentService, AppointmentServiceDto>()
            .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service.Name));

        CreateMap<AppointmentPartCreateDto, AppointmentPart>();
        CreateMap<AppointmentPart, AppointmentPartDto>();

        // Note Mappings
        CreateMap<AppointmentNote, AppointmentNoteDto>();

        // ClientVehicle Mapping (for creation if needed)
        CreateMap<ClientVehicleCreateDto, ClientVehicle>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Appointments, opt => opt.Ignore())
            .ForMember(dest => dest.Client, opt => opt.Ignore())
            .ForMember(dest => dest.Vehicle, opt => opt.Ignore());
    }
}

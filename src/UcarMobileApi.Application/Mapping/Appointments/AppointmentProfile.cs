using System.Linq;
using AutoMapper;
using UcarMobileApi.Application.DTOs.Appointments;
using UcarMobileApi.Application.Mapping.Common;
using UcarMobileApi.Core.Entities.Appointments;

namespace UcarMobileApi.Application.Mapping.Appointments;

public class AppointmentProfile : Profile
{
    public AppointmentProfile()
    {
        // Appointment Mappings
        CreateMap<AppointmentCreateDto, Appointment>()
            .ForMember(dest => dest.Vehicles, opt => opt.Ignore()) // Handled manually in Service
            .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => Core.Enums.PaymentStatus.Unpaid));

        CreateMap<UpdateAppointmentRequest, Appointment>()
            .IgnoreNullValuesForPatch();

        CreateMap<Appointment, AppointmentDto>();

        CreateMap<AppointmentDiscount, AppointmentDiscountDto>();

        // Appointment Vehicle Mappings
        CreateMap<AppointmentVehicleCreateDto, AppointmentVehicle>()
            .ForMember(d => d.Vehicle, opt => opt.Ignore())
            .ForMember(d => d.Services, opt => opt.MapFrom(s => s.Services));

        CreateMap<AppointmentVehicle, AppointmentVehicleDto>();

        // Appointment Service/Part Mappings
        CreateMap<AppointmentServiceCreateDto, AppointmentService>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.AppointmentVehicleId, opt => opt.Ignore())
            .ForMember(d => d.AppointmentVehicle, opt => opt.Ignore())
            .ForMember(d => d.CustomService, opt => opt.MapFrom(s => s.CustomService!.Trim()))
            .ForMember(d => d.Parts, opt => opt.MapFrom(s => s.Parts));

        CreateMap<AppointmentServiceUpdateDto, AppointmentService>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.AppointmentVehicleId, opt => opt.Ignore())
            .ForMember(d => d.AppointmentVehicle, opt => opt.Ignore())
            .ForMember(d => d.Parts, opt => opt.Ignore())
            .ForMember(d => d.CustomService, opt => opt.MapFrom(s => s.CustomService!.Trim()));

        CreateMap<AppointmentService, AppointmentServiceDto>()
            .ForMember(d => d.ServiceCategory, opt => opt.MapFrom(s => s.Service!.ServiceCategory.Name))
            .ForMember(d => d.ServiceName, opt => opt.MapFrom(s => s.Service != null ? s.Service.Name : s.CustomService))
            .ForMember(d => d.IsWarrantyApplicable,
                opt => opt.MapFrom(s =>
                    (s.Service == null || s.Service.ServiceCategory.ServiceType.IsWarrantyApplicable)
                    && s.Parts.Any(p => !p.IsCustomerProvidedPart)));

        CreateMap<AppointmentPartUpsertDto, AppointmentPart>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.AppointmentServiceId, opt => opt.Ignore())
            .ForMember(d => d.AppointmentService, opt => opt.Ignore());

        CreateMap<AppointmentPartCreateDto, AppointmentPart>()
            .IncludeBase<AppointmentPartUpsertDto, AppointmentPart>();

        CreateMap<AppointmentPartUpdateDto, AppointmentPart>()
            .IncludeBase<AppointmentPartUpsertDto, AppointmentPart>();

        CreateMap<AppointmentPart, AppointmentPartDto>();

        // Document Mappings

        CreateMap<AppointmentDocument, AppointmentDocumentDto>()
            .ForMember(d => d.File, o => o.MapFrom(s => s.StoredFile));

        // Note Mappings

        // AppointmentNote → DTO
        CreateMap<AppointmentNoteBaseDto, AppointmentNote>().ReverseMap();
        CreateMap<AppointmentNoteDto, AppointmentNote>().ReverseMap();

        // AppointmentNoteDocument → AppointmentNoteDocumentDto
        CreateMap<AppointmentNoteDocument, AppointmentNoteDocumentDto>()
            .ForMember(d => d.File, o => o.MapFrom(s => s.StoredFile));
    }
}

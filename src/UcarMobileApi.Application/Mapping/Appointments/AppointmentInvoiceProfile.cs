using System.Linq;
using AutoMapper;
using UcarMobileApi.Application.DTOs.Appointments;
using UcarMobileApi.Core.Entities.Appointments;

namespace UcarMobileApi.Application.Mapping.Appointments;

public class AppointmentInvoiceProfile : Profile
{
    public AppointmentInvoiceProfile()
    {
        CreateMap<Appointment, AppointmentInvoiceDto>()
            .ForMember(d => d.ClientEmail, opt => opt.MapFrom(s => s.Client.Email))
            .ForMember(d => d.ClientPhone, opt => opt.MapFrom(s => s.Client.Phone))
            .ForMember(d => d.ClientFullName, opt => opt.MapFrom(s => $"{s.Client.FirstName} {s.Client.LastName}"))
            .ForMember(d => d.ServiceFullAddress, opt => opt.MapFrom(s => s.ServiceAddress.FullAddress))
            .ForMember(d => d.ServiceAddressLat, opt => opt.MapFrom(s => s.ServiceAddress.Lat))
            .ForMember(d => d.ServiceAddressLng, opt => opt.MapFrom(s => s.ServiceAddress.Lng))
            .ForMember(d => d.Discount, opt => opt.MapFrom(s => s.Discounts.Sum(d => d.Amount)));

        CreateMap<AppointmentVehicle, AppointmentInvoiceVehicleDto>()
            .ForMember(d => d.VehicleName, opt =>
            opt.MapFrom(s => $"{s.Vehicle.Vehicle.Year} {s.Vehicle.Vehicle.Make} {s.Vehicle.Submodel ?? s.Vehicle.Vehicle.Model} {s.Vehicle.Engine}".Trim()))
            .ForMember(d => d.VehicleVIN, opt => opt.MapFrom(s => s.Vehicle.VIN))
            .ForMember(d => d.VehicleLicense, opt => opt.MapFrom(s => s.Vehicle.LicensePlate))
            .ForMember(d => d.TechnicianName, opt => opt.MapFrom(s => s.Technician != null ? $"{s.Technician.FirstName} {s.Technician.LastName}" : ""));
    }
}

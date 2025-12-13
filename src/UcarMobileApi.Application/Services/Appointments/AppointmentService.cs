using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Gridify;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using UcarMobileApi.Application.Common.Helpers;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.Common.Models;
using UcarMobileApi.Application.DTOs.Appointments;
using UcarMobileApi.Application.Utilities;
using UcarMobileApi.Core.Entities.Appointments;
using UcarMobileApi.Core.Entities.Clients;
using UcarMobileApi.Core.Exceptions;

namespace UcarMobileApi.Application.Services.Appointments;

public class AppointmentService(IAppDbContext context, IMapper mapper, IGridifyMapper<Appointment> gridifyMapper)
{
    public async Task<(IHeaderDictionary, IEnumerable<AppointmentDto>)> GetAppointmentsAsync(QueryFilter query, CancellationToken ct)
    {
        var appointments = context.Set<Appointment>().AsNoTracking();

        // AutoMapper ProjectTo + Filtering + Ordering + Paging
        var qp = await appointments.GridifySafeAsync(query, gridifyMapper, ct);

        return (qp.GeneratePaginationHttpHeaders(), await qp.Query.ProjectTo<AppointmentDto>(mapper.ConfigurationProvider).ToListAsync(ct));
    }

    public async Task<AppointmentDto> CreateAppointmentAsync(AppointmentCreateDto dto, CancellationToken ct)
    {
        // 1. Map basic appointment details
        var appointment = mapper.Map<Appointment>(dto);

        // 2. Process Vehicles
        foreach (var vehicleDto in dto.Vehicles)
        {
            // Check if ClientVehicle already exists for this Client and VehicleId (Catalog Vehicle)
            var clientVehicle = await context.Set<ClientVehicle>()
                .FirstOrDefaultAsync(cv => cv.ClientId == dto.ClientId && cv.VehicleId == vehicleDto.VehicleId, ct);

            if (clientVehicle == null)
            {
                // Create new ClientVehicle
                clientVehicle = mapper.Map<ClientVehicle>(vehicleDto);
                clientVehicle.ClientId = dto.ClientId;
                // vehicleDto.VehicleId is the Catalog Vehicle Id
                clientVehicle.VehicleId = vehicleDto.VehicleId;

                context.Set<ClientVehicle>().Add(clientVehicle);
            }

            // Create AppointmentVehicle
            var appointmentVehicle = new AppointmentVehicle
            {
                Vehicle = clientVehicle,
                TechnicianId = null, // No technician assigned yet
                Services = mapper.Map<List<AppointmentService>>(vehicleDto.Services),
                Parts = mapper.Map<List<AppointmentPart>>(vehicleDto.Parts)
            };

            appointment.Vehicles.Add(appointmentVehicle);
        }

        context.Set<Appointment>().Add(appointment);
        await context.SaveChangesAsync(ct);

        // Reload to get full structure with IDs and Names (e.g. Service Names)
        return await GetAppointmentAsync(appointment.Id, ct);
    }

    public async Task<AppointmentDto> GetAppointmentAsync(int id, CancellationToken ct)
    {
        var appointment = await context.Set<Appointment>()
            .Include(a => a.Vehicles)
                .ThenInclude(av => av.Vehicle)
            .Include(a => a.Vehicles)
                .ThenInclude(av => av.Services)
                    .ThenInclude(s => s.Service)
            .Include(a => a.Vehicles)
                .ThenInclude(av => av.Parts)
            .Include(a => a.Notes)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, ct);

        if (appointment == null)
        {
            throw new NotFoundException($"Appointment with ID {id} not found.");
        }

        return mapper.Map<AppointmentDto>(appointment);
    }

    public async Task<List<AppointmentDto>> GetClientAppointmentsAsync(int clientId, CancellationToken ct)
    {
        var appointments = await context.Set<Appointment>()
            .Where(a => a.ClientId == clientId)
            .Include(a => a.Vehicles)
                .ThenInclude(av => av.Vehicle)
            .Include(a => a.Vehicles)
                .ThenInclude(av => av.Services)
                    .ThenInclude(s => s.Service)
            .Include(a => a.Vehicles)
                .ThenInclude(av => av.Parts)
            .Include(a => a.Notes)
            .OrderByDescending(a => a.ScheduledStart)
            .AsNoTracking()
            .ToListAsync(ct);

        return mapper.Map<List<AppointmentDto>>(appointments);
    }
}

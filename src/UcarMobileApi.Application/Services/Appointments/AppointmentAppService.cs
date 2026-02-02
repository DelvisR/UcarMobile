using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Gridify.EntityFramework;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using UcarMobileApi.Application.Common.Helpers;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.Common.Models;
using UcarMobileApi.Application.DTOs.Appointments;
using UcarMobileApi.Application.DTOs.Common;
using UcarMobileApi.Application.DTOs.Payments;
using UcarMobileApi.Application.DTOs.Technicians;
using UcarMobileApi.Application.Services.Security;
using UcarMobileApi.Application.Services.Technicians;
using UcarMobileApi.Application.Utilities;
using UcarMobileApi.Application.Validators.Common;
using UcarMobileApi.Core.Constants;
using UcarMobileApi.Core.Entities.Appointments;
using UcarMobileApi.Core.Entities.Clients;
using UcarMobileApi.Core.Entities.Common;
using UcarMobileApi.Core.Entities.Storage;
using UcarMobileApi.Core.Enums;
using UcarMobileApi.Core.Exceptions;
using PaymentMethod = UcarMobileApi.Core.Entities.Payments.PaymentMethod;

namespace UcarMobileApi.Application.Services.Appointments;

public class AppointmentAppService(IAppDbContext context, IMapper mapper, IGridifyMapperResolver gridifyMapperResolver,
    BusinessParameterService businessParameters, TechnicianService technicianService, FileUploadService fileUploadService,
    IValidatorResolver validatorResolver, IUserAuthorizationService authorizationService, IReportService reportService,
    IPaymentService paymentService)
{
    #region CRUPs

    private async Task<IQueryable<Appointment>> ApplySecurityFilter(IQueryable<Appointment> query, string authProviderId, CancellationToken ct)
    {
        var userId = await authorizationService.GetUserIdAsync(authProviderId, ct);

        if (await authorizationService.HasActionAsync(authProviderId, "ACTION_ALL_APPOINTMENTS", ct))
            return query;

        if (await authorizationService.HasActionAsync(authProviderId, "ACTION_TECH_APPOINTMENTS", ct))
            return query.Where(a => a.Vehicles.Any(v => v.TechnicianId == userId));

        if (await authorizationService.HasActionAsync(authProviderId, "ACTION_CLIENT_APPOINTMENTS", ct))
            return query.Where(a => a.ClientId == userId);

        // No permissions → empty result, but valid query
        return query.Where(_ => false);
    }

    public async Task<(IHeaderDictionary, IEnumerable<AppointmentDto>)> GetAppointmentsAsync(string authProviderId, QueryFilter query, CancellationToken ct)
    {
        var appointments = context.Set<Appointment>().AsNoTracking();

        // Security (pre-filtering)
        appointments = await ApplySecurityFilter(appointments, authProviderId, ct);

        // AutoMapper ProjectTo + Filtering + Ordering + Paging
        var qp = await appointments.GridifySafeAsync(query, gridifyMapperResolver.Get<Appointment>(), ct);

        return (qp.GeneratePaginationHttpHeaders(), await qp.Query.ProjectTo<AppointmentDto>(mapper.ConfigurationProvider).ToListAsync(ct));
    }

    #region CreateAppointment related methods

    private async Task<DateTime> GetAppoinmentDuration(DateTime scheduledStart, CancellationToken ct)
    {
        var apptDurationHours = await businessParameters.GetValueAsync<int>(BusinessParameterKeys.Scheduling.RepairBufferHours, ct);

        return scheduledStart.AddHours(apptDurationHours);
    }

    private async Task<TechnicianDto?> GetNearestAvailableTechnician(AppointmentCreateDto appt, CancellationToken ct)
    {
        // Find technician
        var technicianRequest = new NearestAvailableRequestDto
        {
            Lat = appt.ServiceAddress.Lat,
            Lng = appt.ServiceAddress.Lng,
            ZipCode = appt.ServiceAddress.ZipCode,
            LocalStar = appt.ScheduledStart,
            LocalEnd = (DateTime)appt.ScheduledEnd!,
            Specialties = [.. appt.Vehicles.SelectMany(v => v.Services.Where(s => s.ServiceCategoryId.HasValue).Select(s => s.ServiceCategoryId!.Value)).Distinct()]
        };

        return await technicianService.GetNearestAvailableTechnicianAsync(technicianRequest, ct);
    }

    private async Task<Client> ValidateClientAsync(int clientId, CancellationToken ct)
    {
        var client = await context.Set<Client>().AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == clientId && c.IsActive, ct)
                     ?? throw new BusinessException("Client not found or inactive.");
        return client;
    }

    private async Task<PaymentMethod?> ValidatePaymentMethodAsync(AppointmentCreateDto dto, CancellationToken ct)
    {
        if (!dto.PaymentMethodId.HasValue)
            return null;

        var paymentMethod = await context.Set<PaymentMethod>().AsNoTracking()
            .FirstOrDefaultAsync(pm => pm.Id == dto.PaymentMethodId.Value && pm.ClientId == dto.ClientId && !pm.IsDeleted, ct)
            ?? throw new BusinessException("Invalid payment method selected.");

        if (PaymentHelper.IsCardExpired(paymentMethod.ExpMonth, paymentMethod.ExpYear))
        {
            throw new BusinessException("The selected payment method is expired.");
        }

        return paymentMethod;
    }

    #endregion CreateAppointment related methods

    public async Task CreateAppointmentAsync(AppointmentCreateDto dto, CancellationToken ct)
    {
        // resolve validator from IServiceProvider instead of 'new'
        await validatorResolver.Get<AppointmentCreateDto>().ValidateAndThrowAsync(dto, ct);

        var client = await ValidateClientAsync(dto.ClientId, ct);
        await ValidatePaymentMethodAsync(dto, ct);

        dto.ScheduledEnd = await GetAppoinmentDuration(dto.ScheduledStart, ct);

        var technicianDto = await GetNearestAvailableTechnician(dto, ct);

        if (technicianDto != null)
        {
            foreach (var vehicle in dto.Vehicles)
                vehicle.TechnicianId = technicianDto.Id;
        }

        dto.Status = dto.IsContactCenter ? AppointmentStatus.Requested : AppointmentStatus.Confirmed;

        var appointment = mapper.Map<Appointment>(dto);

        var clientVehicles = await context.Set<ClientVehicle>()
            .Where(cv => cv.ClientId == dto.ClientId)
            .ToListAsync(ct);

        var clientVehicleMap = clientVehicles.ToDictionary(cv => (cv.VehicleId, cv.AzId), cv => cv);

        foreach (var vehicleDto in dto.Vehicles)
        {
            var key = (vehicleDto.Vehicle.VehicleId, vehicleDto.Vehicle.AzId);
            if (!clientVehicleMap.TryGetValue(key, out var clientVehicle))
            {
                clientVehicle = mapper.Map<ClientVehicle>(vehicleDto.Vehicle);
                clientVehicle.ClientId = dto.ClientId;
                context.Set<ClientVehicle>().Add(clientVehicle);
                clientVehicleMap[key] = clientVehicle;
            }

            var appointmentVehicle = mapper.Map<AppointmentVehicle>(vehicleDto);
            appointmentVehicle.Vehicle = clientVehicle;
            appointment.Vehicles.Add(appointmentVehicle);
        }

        // CalculateTotalInMemory(appointment);

        context.Set<Appointment>().Add(appointment);
        await context.SaveChangesAsync(ct);

        if (technicianDto != null)
        {
            // send push notification to technician
        }
    }

    public async Task<AppointmentDto> GetAppointmentAsync(int id, CancellationToken ct)
    {
        return await context.Set<Appointment>()
                   .AsNoTracking()
                   .Where(a => a.Id == id)
                   .ProjectTo<AppointmentDto>(mapper.ConfigurationProvider)
                   .FirstOrDefaultAsync(ct)
               ?? throw new NotFoundException($"Appointment with ID {id} not found.");
    }

    public async Task UpdateAsync(int id, UpdateAppointmentRequest dto, CancellationToken ct)
    {
        await validatorResolver.Get<UpdateAppointmentRequest>().ValidateAndThrowAsync(dto, ct);

        var appointment = await context.Set<Appointment>()
                              .FirstOrDefaultAsync(a => a.Id == id, ct)
                          ?? throw new NotFoundException($"Appointment with ID {id} not found.");

        EnsureAppointmentIsMutable(appointment);

        mapper.Map(dto, appointment);

        await context.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Deletes an appointment.
    /// Loads the appointment aggregate to validate mutability and lets EF Core remove the entity (and cascade children if configured).
    /// </summary>
    public async Task DeleteAppointmentAsync(int appointmentId, CancellationToken ct)
    {
        var appointment = await context.Set<Appointment>()
                              .Include(a => a.Documents)
                              .Include(a => a.Notes).ThenInclude(n => n.Documents)
                              .FirstOrDefaultAsync(a => a.Id == appointmentId, ct)
                          ?? throw new NotFoundException($"Appointment {appointmentId} not found.");

        EnsureAppointmentIsMutable(appointment);

        context.Set<Appointment>().Remove(appointment);
        await context.SaveChangesAsync(ct);
    }

    #endregion CRUPs

    #region Others

    public async Task<(byte[], string)> GenerateAppointmentInvoicePdfAsync(int appointmentId, CancellationToken ct)
    {
        var appointment = await context.Set<Appointment>()
                              .AsNoTracking()
                              .Where(a => a.Id == appointmentId)
                              .ProjectTo<AppointmentInvoiceDto>(mapper.ConfigurationProvider)
                              .FirstOrDefaultAsync(ct)
                          ?? throw new NotFoundException($"Appointment with ID {appointmentId} not found.");

        var tzInfo = TimeZoneHelper.GetTimeZoneInfo(appointment.ServiceAddressLat, appointment.ServiceAddressLng);

        var dateOfService = (appointment.CompletedAt ?? appointment.ScheduledStart).ConvertUtcToLocalTime(tzInfo).Date;

        var invoiceNro = $"{dateOfService:yy}-{appointment.Id:00000}";

        var pdf = await reportService.GeneratePdfAsync(
            templateName: "AppointmentInvoice",
            data: appointment,
            dataSourceName: "Appointment",
            parameters: new Dictionary<string, object>
            {
                ["CompanyName"] = await businessParameters.GetValueAsync<string>(BusinessParameterKeys.Company.Name, ct) ?? string.Empty,
                ["CompanyAddress"] = await businessParameters.GetValueAsync<string>(BusinessParameterKeys.Company.Address, ct) ?? string.Empty,
                ["CompanyPhone"] = await businessParameters.GetValueAsync<string>(BusinessParameterKeys.Company.Phone, ct) ?? string.Empty,
                ["CompanyEmail"] = await businessParameters.GetValueAsync<string>(BusinessParameterKeys.Company.Email, ct) ?? string.Empty,
                ["CompanyWeb"] = await businessParameters.GetValueAsync<string>(BusinessParameterKeys.Company.Web, ct) ?? string.Empty,
                ["DateOfService"] = dateOfService,
                ["InvoiceNro"] = invoiceNro
            });

        var fileName = $"UcarMobile Invoice {invoiceNro}.pdf";

        return (pdf, fileName);
    }

    public async Task<Dictionary<string, List<string>>> GetAvailableSlotsAsync(int appointmentId, bool includeToday, CancellationToken ct)
    {
        var request = await context.Set<Appointment>()
                          .AsNoTracking()
                          .Where(a => a.Id == appointmentId)
                          .Select(a => new AvailableSlotRequestDto
                          {
                              Lat = a.ServiceAddress.Lat,
                              Lng = a.ServiceAddress.Lng,
                              ZipCode = a.ServiceAddress.ZipCode,
                              Specialties = a.Vehicles
                                  .SelectMany(v => v.Services
                                      .Where(s => s.ServiceId.HasValue)
                                      .Select(s => s.Service!.ServiceCategoryId))
                                  .Distinct()
                                  .ToList(),
                              IncludeToday = includeToday
                          })
                          .FirstOrDefaultAsync(ct)
                      ?? throw new NotFoundException($"Appointment with ID {appointmentId} not found.");

        return await technicianService.GetAvailableSlotsAsync(request, ct);
    }

    /// <summary>
    /// Returns the list of available technicians (with distance) that can cover an existing appointment.
    /// Builds a NearestAvailableRequestDto from the stored appointment and calls TechnicianService.GetAvailableTechniciansAsync.
    /// </summary>
    public async Task<IReadOnlyList<TechnicianWithDistanceDto>> GetAvailableTechniciansAsync(int appointmentId, double? searchRadiusMeters, CancellationToken ct)
    {
        var appt = await context.Set<Appointment>()
                       .AsNoTracking()
                       .Where(a => a.Id == appointmentId)
                       .Select(a => new
                       {
                           a.ServiceAddress.Lat,
                           a.ServiceAddress.Lng,
                           a.ServiceAddress.ZipCode,
                           Specialties = a.Vehicles
                               .SelectMany(v => v.Services
                                   .Where(s => s.ServiceId.HasValue)
                                   .Select(s => s.Service!.ServiceCategoryId))
                               .Distinct()
                               .ToList(),
                           a.ScheduledStart,
                           a.ScheduledEnd
                       })
                       .FirstOrDefaultAsync(ct)
                   ?? throw new NotFoundException($"Appointment with ID {appointmentId} not found.");

        var localEnd = appt.ScheduledEnd;
        if (!localEnd.HasValue)
        {
            var apptDurationHours = await businessParameters.GetValueAsync<int>(BusinessParameterKeys.Scheduling.RepairBufferHours, ct);
            localEnd = appt.ScheduledStart.AddHours(apptDurationHours);
        }

        var request = new NearestAvailableRequestDto
        {
            Lat = appt.Lat,
            Lng = appt.Lng,
            ZipCode = appt.ZipCode,
            Specialties = appt.Specialties,
            LocalStar = appt.ScheduledStart,
            LocalEnd = localEnd.Value,
            SearchRadiusMeters = searchRadiusMeters ?? 50_000
        };

        return await technicianService.GetAvailableTechniciansAsync(request, ct);
    }

    #endregion Others

    #region Controlled updates

    private static DateTime ResolveCompletedAt(Appointment appointment, DateTime? completedAt)
    {
        if (completedAt.HasValue)
            return completedAt.Value;

        var tzInfo = TimeZoneHelper.GetTimeZoneInfo(appointment.ServiceAddress.Lat, appointment.ServiceAddress.Lng);

        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tzInfo);
    }

    private async Task<int?> ResolveWarrantyMonthsAsync(int? warrantyMonths, CancellationToken ct)
    {
        if (warrantyMonths.HasValue)
            return warrantyMonths;

        return await businessParameters.GetValueAsync<int>(BusinessParameterKeys.Warranty.WarrantyMonths, ct);
    }

    private async Task<int?> ResolveWarrantyMilesAsync(int? warrantyMiles, CancellationToken ct)
    {
        if (warrantyMiles.HasValue)
            return warrantyMiles;

        return await businessParameters.GetValueAsync<int>(BusinessParameterKeys.Warranty.WarrantyMiles, ct);
    }

    public async Task CompleteAppointmentAsync(int appointmentId, CompleteAppointmentRequest request, CancellationToken ct)
    {
        await validatorResolver.Get<CompleteAppointmentRequest>().ValidateAndThrowAsync(request, ct);

        var appointment = await context.Set<Appointment>()
                              .Include(a => a.Vehicles)
                              .ThenInclude(v => v.Vehicle)
                              .FirstOrDefaultAsync(a => a.Id == appointmentId, ct)
                          ?? throw new NotFoundException($"Appointment {appointmentId} not found.");

        // Valid status
        EnsureAppointmentIsMutable(appointment);

        // Status changes
        appointment.Status = AppointmentStatus.Completed;
        appointment.CompletedAt = ResolveCompletedAt(appointment, request.CompletedAt);

        // Warranty
        appointment.WarrantyMonths = await ResolveWarrantyMonthsAsync(request.WarrantyMonths, ct) ?? 0;
        appointment.WarrantyMiles = await ResolveWarrantyMilesAsync(request.WarrantyMiles, ct) ?? 0;

        // Odometer
        var vehicleMap = appointment.Vehicles.ToDictionary(v => v.Id);

        foreach (var vehicleDto in request.Vehicles)
        {
            if (!vehicleMap.TryGetValue(vehicleDto.Id, out var av))
            {
                throw new BusinessException($"Vehicle {vehicleDto.Id} does not belong to appointment.");
            }

            av.OdometerKm = vehicleDto.OdometerKm ?? av.Vehicle.OdometerKm;
        }

        await context.SaveChangesAsync(ct);

        // Charge customer for the appointment estimated total if applicable.
        // If EstimatedTotal is zero or negative, skip charging.
        if (appointment.EstimatedTotal > 0m)
        {
            var amountCents = (long)Math.Round(appointment.EstimatedTotal * 100m);
            // Attempt to create payment; final result handled via webhook
            await ChargeAppointmentPaymentAsync(appointment, amountCents, "complete", ct);
        }

        // TODO: Notificar
    }

    public async Task CancelAppointmentAsync(int appointmentId, CancelAppointmentRequest? request, CancellationToken ct)
    {
        var appointment = await context.Set<Appointment>()
            .FirstOrDefaultAsync(a => a.Id == appointmentId, ct)
            ?? throw new NotFoundException($"Appointment {appointmentId} not found.");

        // Prevent cancelling completed or immutable appointments
        EnsureAppointmentIsMutable(appointment);

        // Persist cancellation reason if provided
        if (!string.IsNullOrWhiteSpace(request?.CancellationReason))
            appointment.CancellationReason = request.CancellationReason.Trim();

        // Read business parameters (fallback to sensible defaults)
        var windowHours = await businessParameters
            .GetValueAsync<int>(BusinessParameterKeys.Appointment.AdjustmentWindowHours, ct, 4);

        var adjustmentFeeAmount = await businessParameters
            .GetValueAsync<decimal>(BusinessParameterKeys.Appointment.AdjustmentFeeAmount, ct);

        var minServiceFee = await businessParameters
            .GetValueAsync<decimal>(BusinessParameterKeys.Appointment.MinServiceFeeAmount, ct);

        var amountToCharge = 0m;

        // If the technician is en route or already working, apply minimum service fee
        if (appointment.Status is AppointmentStatus.EnRoute or AppointmentStatus.InProgress)
        {
            amountToCharge = minServiceFee;
        }
        else
        {
            // Evaluate remaining time before scheduled start (UTC)
            var hoursUntil = (appointment.ScheduledStart - DateTime.UtcNow).TotalHours;
            if (hoursUntil < windowHours)
                amountToCharge = adjustmentFeeAmount;
        }

        if (amountToCharge > 0m && !appointment.PaymentMethodId.HasValue)
        {
            throw new BusinessException("Cancellation requires a payment method to apply the fee.");
        }

        appointment.Status = AppointmentStatus.Cancelled;

        await context.SaveChangesAsync(ct);

        // Charge cancellation fee if applicable
        if (amountToCharge > 0m)
        {
            var amountCents = (long)Math.Round(amountToCharge * 100m);

            // Attempt to create payment; final outcome will be resolved via webhook
            await ChargeAppointmentPaymentAsync(appointment, amountCents, "cancel", ct);
        }

        // TODO: Notificar
    }

    public async Task UpdateAppointmentAddressAsync(int appointmentId, AddressInfoDto request, CancellationToken ct)
    {
        // Validation using the resolver
        await validatorResolver.Get<AddressInfoDto>().ValidateAndThrowAsync(request, ct);

        var appointment = await context.Set<Appointment>()
                              .FirstOrDefaultAsync(a => a.Id == appointmentId, ct)
                          ?? throw new NotFoundException($"Appointment {appointmentId} not found.");

        // Ensure the appointment can be modified
        EnsureAppointmentIsMutable(appointment);

        // Replace the Value Object
        appointment.ServiceAddress = mapper.Map<AddressInfo>(request);

        // Persist changes
        await context.SaveChangesAsync(ct);

        // TODO: Notify stakeholders
    }

    public async Task RescheduleAppointmentAsync(int appointmentId, RescheduleRequest request, CancellationToken ct)
    {
        // Validate the request
        await validatorResolver.Get<RescheduleRequest>().ValidateAndThrowAsync(request, ct);

        var appointment = await context.Set<Appointment>()
            .FirstOrDefaultAsync(a => a.Id == appointmentId, ct)
            ?? throw new NotFoundException($"Appointment {appointmentId} not found.");

        // Ensure the appointment is in a state that allows modification
        EnsureAppointmentIsMutable(appointment);

        // Business Rule: Penalty window calculation (X hours)
        // We check against the ORIGINAL ScheduledStart

        var adjustmentWindowHours = await businessParameters.GetValueAsync<int>(BusinessParameterKeys.Appointment.AdjustmentWindowHours, ct, 4);
        var hoursUntilOriginalStart = (appointment.ScheduledStart - DateTime.UtcNow).TotalHours;
        var isLateReschedule = hoursUntilOriginalStart < adjustmentWindowHours;

        if (isLateReschedule)
        {
            var rescheduleFee = await businessParameters.GetValueAsync<decimal>(BusinessParameterKeys.Appointment.MinServiceFeeAmount, ct);

            if (rescheduleFee > 0m)
            {
                if (!appointment.PaymentMethodId.HasValue)
                    throw new BusinessException("A payment method is required for late rescheduling (within 4 hours of the service).");

                var amountCents = (long)Math.Round(rescheduleFee * 100m);
                await ChargeAppointmentPaymentAsync(appointment, amountCents, "reschedule_fee", ct);
            }
        }

        // Update the schedule
        appointment.ScheduledStart = request.ScheduledStart;

        // Logic: If ScheduledEnd is null, default to X hours after start
        appointment.ScheduledEnd = request.ScheduledEnd ?? await GetAppoinmentDuration(request.ScheduledStart, ct);

        // Persist changes
        await context.SaveChangesAsync(ct);

        // TODO: Send notifications to both the customer and the technician
    }

    #endregion

    #region Services

    /// <summary>
    /// Returns all services for a given appointment vehicle (validates the vehicle belongs to the appointment).
    /// </summary>
    public async Task<IEnumerable<AppointmentServiceDto>> GetServicesAsync(int appointmentId, int appointmentVehicleId, CancellationToken ct)
    {
        return await context.Set<AppointmentService>()
            .AsNoTracking()
            .Where(s => s.AppointmentVehicleId == appointmentVehicleId &&
                        s.AppointmentVehicle.AppointmentId == appointmentId)
            .ProjectTo<AppointmentServiceDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    /// <summary>
    /// Adds a service to an appointment vehicle (belongs to an appointment).
    /// Validates DTO, ensures vehicle exists and prevents duplicates.
    /// Returns no content.
    /// </summary>
    public async Task AddServiceAsync(int appointmentId, int appointmentVehicleId, AppointmentServiceCreateDto dto, CancellationToken ct)
    {
        // 1. Input validation (fast, CPU-bound if possible)
        await validatorResolver.Get<AppointmentServiceCreateDto>().ValidateAndThrowAsync(dto, ct);

        // 2. Load the necessary graph in a SINGLE query
        // We fetch the Appointment, its Vehicles, and existing Services/Parts.
        // This allows us to perform all validations and calculations in memory.
        var appointment = await GetAppointmentWithFullGraphAsync(appointmentId, ct)
            ?? throw new NotFoundException($"Appointment {appointmentId} not found.");

        // 3. Domain Validations (In-memory - Zero network latency)
        EnsureAppointmentIsMutable(appointment);

        var vehicle = appointment.Vehicles
            .FirstOrDefault(v => v.Id == appointmentVehicleId)
            ?? throw new NotFoundException($"AppointmentVehicle {appointmentVehicleId} not found for appointment {appointmentId}.");

        // In-memory duplication logic
        var serviceExists = vehicle.Services.Any(s =>
            (dto.ServiceId != null && s.ServiceId == dto.ServiceId) ||
            (dto.ServiceId == null && s.ServiceId == null && s.CustomService == dto.CustomService)
        );

        if (serviceExists)
            throw new BusinessException("The requested service is already added.");

        // 4. Mapping and Aggregation
        var newServiceEntity = mapper.Map<AppointmentService>(dto);

        // IMPORTANT: Adding to the navigation collection allows EF Core to track 
        // the relationship automatically. No need to set IDs manually.
        vehicle.Services.Add(newServiceEntity);

        // 5. Total Recalculation
        // By calculating in-memory on tracked entities, the NEW service 
        // (added in step 4) is automatically included in the sum.
        appointment.EstimatedTotal = CalculateTotalInMemory(appointment);

        // 6. Persistence
        await context.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Updates an existing appointment service for a vehicle.
    /// </summary>
    public async Task UpdateServiceAsync(int appointmentId, int appointmentVehicleId, int serviceId, AppointmentServiceUpdateDto dto, CancellationToken ct)
    {
        // 1. Validation
        await validatorResolver.Get<AppointmentServiceUpdateDto>().ValidateAndThrowAsync(dto, ct);

        // 2. Fetch the Root Aggregate (Appointment) including Vehicles, Services, and Parts.
        // We need the full graph to validate the status and recalculate the total accurately.
        var appointment = await GetAppointmentWithFullGraphAsync(appointmentId, ct)
                          ?? throw new NotFoundException($"Appointment {appointmentId} not found.");

        // 3. Domain Logic: Check status
        EnsureAppointmentIsMutable(appointment);

        // 4. Locate the specific Service within the graph
        // This validates hierarchy (Appointment -> Vehicle -> Service) in memory without extra DB calls.
        var vehicle = appointment.Vehicles
            .FirstOrDefault(v => v.Id == appointmentVehicleId)
            ?? throw new NotFoundException($"Vehicle {appointmentVehicleId} does not belong to appointment {appointmentId}.");

        var serviceToUpdate = vehicle.Services
            .FirstOrDefault(s => s.Id == serviceId)
            ?? throw new NotFoundException($"Service {serviceId} not found in vehicle {appointmentVehicleId}.");

        // 5. Check for Duplicates (excluding the current service being updated)
        var duplicateExists = vehicle.Services.Any(s =>
            s.Id != serviceId && // Ignore self
            (
                (dto.ServiceId != null && s.ServiceId == dto.ServiceId) ||
                (dto.ServiceId == null && s.ServiceId == null && s.CustomService == dto.CustomService)
            ));

        if (duplicateExists)
            throw new BusinessException("The requested service already exists on this vehicle.");

        // 6. Map updates
        // EF Core is tracking 'serviceToUpdate', so changes are detected automatically.
        mapper.Map(dto, serviceToUpdate);

        // 7. Recalculate Total
        // Since we updated the entity in memory (Step 6), the helper will calculate the new correct total.
        appointment.EstimatedTotal = CalculateTotalInMemory(appointment);

        // 8. Persist
        await context.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Deletes an appointment service.
    /// </summary>
    public async Task DeleteServiceAsync(int appointmentId, int appointmentVehicleId, int serviceId, CancellationToken ct)
    {
        // 1. Fetch the full graph
        // We need the Appointment to check its status and recalculate the total after removal.
        var appointment = await GetAppointmentWithFullGraphAsync(appointmentId, ct)
                          ?? throw new NotFoundException($"Appointment {appointmentId} not found.");

        // 2. Domain Validation: Check if the appointment can still be modified
        EnsureAppointmentIsMutable(appointment);

        // 3. Locate the Vehicle and the Service within the graph
        var vehicle = appointment.Vehicles
            .FirstOrDefault(v => v.Id == appointmentVehicleId)
            ?? throw new NotFoundException($"Vehicle {appointmentVehicleId} does not belong to appointment {appointmentId}.");

        var serviceToRemove = vehicle.Services
            .FirstOrDefault(s => s.Id == serviceId)
            ?? throw new NotFoundException($"Service {serviceId} not found in vehicle {appointmentVehicleId}.");

        // 4. Remove the service from the collection
        // EF Core will mark this entity as 'Deleted' because it was loaded from the context.
        vehicle.Services.Remove(serviceToRemove);

        // 5. Recalculate Total
        // The CalculateTotalInMemory method will now sum the services excluding the one we just removed.
        appointment.EstimatedTotal = CalculateTotalInMemory(appointment);

        // 6. Persist changes
        await context.SaveChangesAsync(ct);
    }

    #endregion

    #region Parts

    /// <summary>
    /// Returns all parts for a given appointment vehicle (validates the vehicle belongs to the appointment).
    /// </summary>
    public async Task<IEnumerable<AppointmentPartDto>> GetPartsAsync(int appointmentId, int appointmentVehicleId,
        int appointmentServiceId, CancellationToken ct)
    {
        return await context.Set<AppointmentPart>()
            .AsNoTracking()
            .Where(p => p.AppointmentServiceId == appointmentServiceId &&
                        p.AppointmentService.AppointmentVehicleId == appointmentVehicleId &&
                        p.AppointmentService.AppointmentVehicle.AppointmentId == appointmentId)
            .ProjectTo<AppointmentPartDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    /// <summary>
    /// Adds a part to an appointment vehicle (belongs to an appointment).
    /// Validates DTO, ensures vehicle exists and prevents duplicates.
    /// Returns no content.
    /// </summary>
    public async Task AddPartAsync(int appointmentId, int appointmentVehicleId, int appointmentServiceId,
        AppointmentPartCreateDto dto, CancellationToken ct)
    {
        // 1. Input validation
        await validatorResolver.Get<AppointmentPartCreateDto>().ValidateAndThrowAsync(dto, ct);

        // 2. Load Appointment aggregate with Vehicles/Services/Parts
        var appointment = await GetAppointmentWithFullGraphAsync(appointmentId, ct)
                          ?? throw new NotFoundException($"Appointment {appointmentId} not found.");

        // 3. Domain validations
        EnsureAppointmentIsMutable(appointment);

        var vehicle = appointment.Vehicles
            .FirstOrDefault(v => v.Id == appointmentVehicleId)
            ?? throw new NotFoundException($"AppointmentVehicle {appointmentVehicleId} not found for appointment {appointmentId}.");

        var service = vehicle.Services
                          .FirstOrDefault(v => v.Id == appointmentServiceId)
                      ?? throw new NotFoundException($"appointmentService {appointmentServiceId} not found for Vehicle {appointmentVehicleId}.");

        // Duplication logic: prefer PartNumber when available, otherwise PartName
        var partExists = service.Parts.Any(p =>
            (!string.IsNullOrEmpty(dto.PartNumber) && p.PartNumber == dto.PartNumber) ||
            (string.IsNullOrEmpty(dto.PartNumber) && p.PartName == dto.PartName)
        );

        if (partExists)
            throw new BusinessException("The requested part is already added.");

        // 4. Map and add
        var newPartEntity = mapper.Map<AppointmentPart>(dto);
        service.Parts.Add(newPartEntity);

        // 5. Recalculate total
        appointment.EstimatedTotal = CalculateTotalInMemory(appointment);

        // 6. Persist
        await context.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Updates an existing appointment part for a vehicle.
    /// </summary>
    public async Task UpdatePartAsync(int appointmentId, int appointmentVehicleId, int appointmentServiceId,
        int partId, AppointmentPartUpdateDto dto, CancellationToken ct)
    {
        // 1. Validation
        await validatorResolver.Get<AppointmentPartUpdateDto>().ValidateAndThrowAsync(dto, ct);

        // 2. Fetch aggregate
        var appointment = await GetAppointmentWithFullGraphAsync(appointmentId, ct)
                          ?? throw new NotFoundException($"Appointment {appointmentId} not found.");

        // 3. Domain validation
        EnsureAppointmentIsMutable(appointment);

        // 4. Locate vehicle and part
        var vehicle = appointment.Vehicles
            .FirstOrDefault(v => v.Id == appointmentVehicleId)
            ?? throw new NotFoundException($"Vehicle {appointmentVehicleId} does not belong to appointment {appointmentId}.");

        var service = vehicle.Services
                          .FirstOrDefault(v => v.Id == appointmentServiceId)
                      ?? throw new NotFoundException($"appointmentService {appointmentServiceId} not found for Vehicle {appointmentVehicleId}.");

        var partToUpdate = service.Parts
                               .FirstOrDefault(p => p.Id == partId)
                           ?? throw new NotFoundException($"Part {partId} not found in vehicle {appointmentVehicleId}.");

        // 5. Duplicate check excluding self
        var duplicateExists = service.Parts.Any(p =>
            p.Id != partId &&
            (
                (!string.IsNullOrEmpty(dto.PartNumber) && p.PartNumber == dto.PartNumber) ||
                (string.IsNullOrEmpty(dto.PartNumber) && p.PartName == dto.PartName)
            )
        );

        if (duplicateExists)
            throw new BusinessException("The requested part already exists on this service.");

        // 6. Map updates (EF tracked entity)
        mapper.Map(dto, partToUpdate);

        // 7. Recalculate total
        appointment.EstimatedTotal = CalculateTotalInMemory(appointment);

        // 8. Persist
        await context.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Deletes an appointment part.
    /// </summary>
    public async Task DeletePartAsync(int appointmentId, int appointmentVehicleId, int appointmentServiceId,
        int partId, CancellationToken ct)
    {
        var appointment = await GetAppointmentWithFullGraphAsync(appointmentId, ct)
                          ?? throw new NotFoundException($"Appointment {appointmentId} not found.");

        EnsureAppointmentIsMutable(appointment);

        var vehicle = appointment.Vehicles
            .FirstOrDefault(v => v.Id == appointmentVehicleId)
            ?? throw new NotFoundException($"Vehicle {appointmentVehicleId} does not belong to appointment {appointmentId}.");

        var service = vehicle.Services
                          .FirstOrDefault(v => v.Id == appointmentServiceId)
                      ?? throw new NotFoundException($"appointmentService {appointmentServiceId} not found for Vehicle {appointmentVehicleId}.");

        var partToRemove = service.Parts
                               .FirstOrDefault(p => p.Id == partId)
                           ?? throw new NotFoundException($"Part {partId} not found in vehicle {appointmentVehicleId}.");

        service.Parts.Remove(partToRemove);

        appointment.EstimatedTotal = CalculateTotalInMemory(appointment);

        await context.SaveChangesAsync(ct);
    }

    #endregion

    #region Documents

    public async Task<IEnumerable<AppointmentDocumentDto>> GetAppointmentDocumentsAsync(int appointmentId, CancellationToken ct)
    {
        return await context.Set<AppointmentDocument>()
            .AsNoTracking()
            .Where(d => d.AppointmentId == appointmentId)
            .ProjectTo<AppointmentDocumentDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    /// <summary>
    /// Uploads multiple files server-side, persists StoredFile records and links them to the appointment.
    /// Returns the created AppointmentDocumentDto list (StoredFileId + presigned URLs).
    /// </summary>
    public async Task<IReadOnlyCollection<AppointmentDocumentDto>> AddAppointmentDocumentsAsync(int appointmentId, AddAppointmentDocumentsDto dto, CancellationToken ct)
    {
        // resolve validator from IServiceProvider instead of 'new'
        await validatorResolver.Get<AddAppointmentDocumentsDto>().ValidateAndThrowAsync(dto, ct);

        var appointment = await context.Set<Appointment>()
                              .FirstOrDefaultAsync(a => a.Id == appointmentId, ct)
                          ?? throw new NotFoundException($"Appointment {appointmentId} not found.");

        EnsureAppointmentIsMutable(appointment);

        var fileList = dto.Files!.ToList();

        // 1️ Parallel load to S3
        var uploaded = await fileUploadService.UploadFilesAsync(fileList, dto.Prefix ?? "AppointmentDocuments", ct);

        try
        {
            // 2️ EF persistence (sequential)
            var appointmentDocuments = PersistAppointmentDocuments(appointment, uploaded, dto.Source);

            await context.SaveChangesAsync(ct);

            // 3️ DTOs
            return [.. appointmentDocuments.Select(mapper.Map<AppointmentDocumentDto>)];
        }
        catch
        {
            // 4️ Cleanup S3
            await fileUploadService.CleanupUploadedFilesAsync(uploaded, ct);
            throw;
        }
    }

    private List<AppointmentDocument> PersistAppointmentDocuments(Appointment appointment, IReadOnlyCollection<StoredFileMetadata> uploaded, ContentSource source)
    {
        var appointmentDocuments = new List<AppointmentDocument>();
        var storedFiles = context.Set<StoredFile>();
        var appointmentDocumentsSet = context.Set<AppointmentDocument>();

        foreach (var metadata in uploaded)
        {
            var stored = mapper.Map<StoredFile>(metadata);
            storedFiles.Add(stored);

            var appointmentDocument = new AppointmentDocument
            {
                Appointment = appointment,
                StoredFile = stored,
                Source = source
            };

            appointmentDocumentsSet.Add(appointmentDocument);
            appointmentDocuments.Add(appointmentDocument);
        }

        return appointmentDocuments;
    }

    /// <summary>
    /// Removes the AppointmentDocument link (appointmentId + storedFileId).
    /// This unlinks the stored file from the appointment but does NOT delete the StoredFile record nor the object in S3.
    /// </summary>
    public async Task DeleteAppointmentDocumentAsync(int appointmentId, int storedFileId, CancellationToken ct)
    {
        var appointment = await context.Set<Appointment>().FirstOrDefaultAsync(a => a.Id == appointmentId, ct)
                          ?? throw new NotFoundException($"Appointment {appointmentId} not found.");

        EnsureAppointmentIsMutable(appointment);

        var entity = await context.Set<AppointmentDocument>().FindAsync([appointmentId, storedFileId], ct)
                     ?? throw new NotFoundException($"Document with StoredFileId {storedFileId} not found for appointment {appointmentId}.");

        context.Set<AppointmentDocument>().Remove(entity);
        await context.SaveChangesAsync(ct);
    }

    #endregion

    #region Notes

    /// <summary>
    /// Returns all notes for a given appointment.
    /// </summary>
    public async Task<(IHeaderDictionary, IEnumerable<AppointmentNoteDto>)> GetAppointmentNotesAsync(int appointmentId, QueryFilter query, CancellationToken ct)
    {
        var notesQuery = context.Set<AppointmentNote>()
            .AsNoTracking()
            .Where(n => n.AppointmentId == appointmentId);

        // Gridify: filtering + ordering + paging
        if (string.IsNullOrWhiteSpace(query.OrderBy)) query.OrderBy = "CreatedDate desc";

        var qp = await notesQuery.GridifyQueryableAsync(query, gridifyMapperResolver.Get<AppointmentNote>(), ct);

        // ProjectTo at DB level to map entities to DTOs efficiently
        var dtos = await qp.Query
            .ProjectTo<AppointmentNoteDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return (qp.GeneratePaginationHttpHeaders(), dtos);
    }

    public async Task<AppointmentNoteDto> AddAppointmentNoteAsync(int appointmentId, AppointmentNoteCreateDto request, CancellationToken ct)
    {
        var appointment = await context.Set<Appointment>()
                              .FirstOrDefaultAsync(a => a.Id == appointmentId, ct)
                          ?? throw new NotFoundException($"Appointment {appointmentId} not found.");

        EnsureAppointmentIsMutable(appointment);

        // 1️ Create the note (no documents yet)
        var note = new AppointmentNote
        {
            Appointment = appointment,
            Content = request.Content,
            Source = request.Source
        };

        context.Set<AppointmentNote>().Add(note);

        IReadOnlyCollection<StoredFileMetadata> uploaded = [];

        try
        {
            // 2️ Optional upload to S3
            if (request.Files?.Any() == true)
            {
                uploaded = await fileUploadService.UploadFilesAsync([.. request.Files], request.Prefix ?? "AppointmentNotes", ct);

                PersistNoteDocuments(note, uploaded);
            }

            // 3️ Persist together
            await context.SaveChangesAsync(ct);

            return mapper.Map<AppointmentNoteDto>(note);
        }
        catch
        {
            // 4 Cleanup S3
            if (uploaded.Count > 0)
                await fileUploadService.CleanupUploadedFilesAsync(uploaded, ct);

            throw;
        }
    }

    /// <summary>
    /// Deletes a note from an appointment.
    /// This removes the AppointmentNote (and cascades depending on EF configuration for AppointmentNoteDocument).
    /// StoredFile records / objects are NOT deleted here.
    /// </summary>
    public async Task DeleteAppointmentNoteAsync(int appointmentId, int noteId, CancellationToken ct)
    {
        var appointment = await context.Set<Appointment>().FirstOrDefaultAsync(a => a.Id == appointmentId, ct)
                          ?? throw new NotFoundException($"Appointment {appointmentId} not found.");

        EnsureAppointmentIsMutable(appointment);

        var note = await context.Set<AppointmentNote>()
                       .Include(n => n.Documents)
                       .FirstOrDefaultAsync(n => n.Id == noteId && n.AppointmentId == appointmentId, ct)
                   ?? throw new NotFoundException($"Note {noteId} not found for appointment {appointmentId}.");

        context.Set<AppointmentNote>().Remove(note);

        await context.SaveChangesAsync(ct);
    }

    private void PersistNoteDocuments(AppointmentNote note, IReadOnlyCollection<StoredFileMetadata> uploaded)
    {
        var storedFiles = context.Set<StoredFile>();
        var noteDocuments = context.Set<AppointmentNoteDocument>();

        foreach (var metadata in uploaded)
        {
            var stored = mapper.Map<StoredFile>(metadata);
            storedFiles.Add(stored);

            var document = new AppointmentNoteDocument
            {
                AppointmentNote = note,
                StoredFile = stored
            };

            noteDocuments.Add(document);
        }
    }

    #endregion

    #region Discount

    public async Task ApplyDiscountAsync(int appointmentId, ApplyDiscountDto dto, CancellationToken ct)
    {
        await validatorResolver.Get<ApplyDiscountDto>().ValidateAndThrowAsync(dto, ct);

        var appointment = await context.Set<Appointment>()
                              .Include(a => a.Discounts)
                              .FirstOrDefaultAsync(a => a.Id == appointmentId, ct)
                          ?? throw new NotFoundException($"Appointment {appointmentId} not found.");

        EnsureAppointmentIsMutable(appointment);

        var discount = new AppointmentDiscount(
            appointmentId: appointment.Id,
            category: dto.Category,
            type: dto.Type,
            value: dto.Value,
            baseAmount: appointment.EstimatedTotal,
            reason: dto.Reason,
            source: dto.Source,
            code: dto.Code
        );

        appointment.ApplyDiscount(discount);

        context.Set<AppointmentDiscount>().Add(discount);

        await context.SaveChangesAsync(ct);
    }

    public async Task ApplyReviewDiscountAsync(int appointmentId, UcarMobileApi.Core.Entities.Appointments.DiscountSource source, CancellationToken ct)
    {
        var reviewDiscountAmount = await businessParameters.GetValueAsync<decimal>(BusinessParameterKeys.Discount.ReviewDiscountAmount, ct);

        var dto = new ApplyDiscountDto
        {
            Category = DiscountCategory.Review,
            Type = DiscountType.Flat,
            Value = reviewDiscountAmount,
            Reason = "Review discount",
            Code = null,
            Source = source
        };

        await ApplyDiscountAsync(appointmentId, dto, ct);
    }

    public async Task RemoveDiscountAsync(int appointmentId, DiscountCategory category, CancellationToken ct)
    {
        var appointment = await context.Set<Appointment>()
                              .Include(a => a.Discounts)
                              .FirstOrDefaultAsync(a => a.Id == appointmentId, ct)
                          ?? throw new NotFoundException($"Appointment {appointmentId} not found.");

        EnsureAppointmentIsMutable(appointment);

        var discount = appointment.RemoveDiscount(category);
        if (discount != null)
        {
            context.Set<AppointmentDiscount>().Remove(discount);
            await context.SaveChangesAsync(ct);
        }
    }

    #endregion

    #region Helpers

    private async Task<Appointment?> GetAppointmentWithFullGraphAsync(int appointmentId, CancellationToken ct)
    {
        return await context.Set<Appointment>()
            .Include(a => a.Vehicles)
            .ThenInclude(v => v.Services)
            .ThenInclude(v => v.Parts)
            .Include(a => a.Vehicles)
            .FirstOrDefaultAsync(a => a.Id == appointmentId, ct);
    }

    private static void EnsureAppointmentIsMutable(Appointment appointment)
    {
        if (appointment.Status >= AppointmentStatus.Completed)
            throw new BusinessException("The appointment cannot be modified in its current status.");
    }

    // Synchronous helper to calculate total in-memory
    private static decimal CalculateTotalInMemory(Appointment appointment)
    {
        // Calculate total for service prices
        var totalServices = appointment.Vehicles
            .SelectMany(v => v.Services)
            .Sum(s => s.Price);

        // Calculate total for parts across all services
        var totalParts = appointment.Vehicles
            .SelectMany(v => v.Services)
            .SelectMany(s => s.Parts)
            .Sum(p => p.Quantity * p.UnitPrice);

        return totalServices + totalParts;
    }

    /// <summary>
    /// Centralized payment creation for appointment-related charges.
    /// Returns the payment DTO if created.
    /// Throws BusinessException when payment method or client auth id is missing.
    /// </summary>
    private async Task<PaymentDto?> ChargeAppointmentPaymentAsync(Appointment appointment, long amountCents, string idempotencyKeyPrefix, CancellationToken ct)
    {
        // If nothing to charge, skip
        if (amountCents <= 0) return null;

        // Ensure we have a payment method to charge
        if (!appointment.PaymentMethodId.HasValue)
            throw new BusinessException("Payment requires a payment method to apply the charge.");

        // Build a stable idempotency key per appointment and purpose
        var idempotencyKey = $"{idempotencyKeyPrefix}_appt_{appointment.Id}";

        var paymentRequest = new PaymentCreateDto(appointment.PaymentMethodId.Value, idempotencyKey, amountCents);

        // Create payment via payment service
        var paymentResult = await paymentService.CreatePaymentByClientIdAsync(appointment.ClientId, paymentRequest, ct);

        // Accept both succeeded and processing as valid states; other states may require manual handling
        if (paymentResult.Status is not ("succeeded" or "processing"))
        {
            // TODO: Notify admins about failed payment creation for appointment charges
            // logger.LogWarning("Payment created with non-success status. AppointmentId={AppointmentId}, Status={Status}", appointment.Id, paymentResult.Status);
        }

        // If the payment was immediately successful, mark appointment as Paid
        if (paymentResult.Status == "succeeded")
        {
            // TODO: When the payment is not completed at the time, Stripe sends the result to the WebHook, and the webhook must send a
            // notification to change the payment status of the appointment. Here, it may be a good idea to send a notification as well,
            // so you don't have to write directly to the database.

            // Mark appointment payment status as Paid and persist
            appointment.PaymentStatus = PaymentStatus.Paid;
            await context.SaveChangesAsync(ct);
        }

        // Final outcome handled via webhook; return DTO for callers if needed
        return paymentResult;
    }

    #endregion
}

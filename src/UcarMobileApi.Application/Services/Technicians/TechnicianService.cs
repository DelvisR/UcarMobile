using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Gridify;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using UcarMobileApi.Application.Common.Helpers;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.Common.Models;
using UcarMobileApi.Application.DTOs.Technicians;
using UcarMobileApi.Application.Utilities;
using UcarMobileApi.Application.Validators.Technicians;
using UcarMobileApi.Core.Entities.Technicians;

namespace UcarMobileApi.Application.Services.Technicians;

/// <summary>
/// Service for managing Technician entities.
/// Provides CRUD operations with pagination, filtering, and validation.
/// </summary>
public class TechnicianService(IMapper mapper, IAppDbContext context, BusinessParameterService businessParameters, IGridifyMapper<Technician> gridifyMapper)
{
    /// <summary>
    /// Retrieves a paginated list of technicians with filtering and sorting.
    /// </summary>
    /// <param name="query">Query filter containing pagination, filtering, and sorting parameters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A tuple containing pagination headers and the list of technician DTOs.</returns>
    public async Task<(IHeaderDictionary, IEnumerable<TechnicianDto>)> GetTechniciansAsync(QueryFilter query, CancellationToken ct)
    {
        var technicians = context.Set<Technician>()
            .Include(t => t.ServiceZones)
                .ThenInclude(tsz => tsz.ServiceZone)
            .Include(t => t.Specialities)
                .ThenInclude(ts => ts.ServiceCategory)
            .AsNoTracking();

        // Apply Gridify for filtering, ordering, and paging
        var qp = await technicians.GridifySafeAsync(query, gridifyMapper, ct);

        // Project to DTO and generate pagination headers
        return (qp.GeneratePaginationHttpHeaders(), await qp.Query.ProjectTo<TechnicianDto>(mapper.ConfigurationProvider).ToListAsync(ct));
    }

    /// <summary>
    /// Retrieves a specific technician by ID.
    /// </summary>
    /// <param name="id">The technician ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The technician DTO if found, otherwise null.</returns>
    public async Task<TechnicianDto?> GetTechnicianAsync(int id, CancellationToken ct)
    {
        return await context.Set<Technician>()
            .Include(t => t.ServiceZones)
                .ThenInclude(tsz => tsz.ServiceZone)
            .Include(t => t.Specialities)
                .ThenInclude(ts => ts.ServiceCategory)
            .AsNoTracking()
            .Where(t => t.Id == id)
            .ProjectTo<TechnicianDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }

    /// <summary>
    /// Creates a new technician.
    /// Validates the DTO and assigns default technician roles.
    /// </summary>
    /// <param name="dto">The technician DTO to create.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <exception cref="ValidationException">Thrown when validation fails.</exception>
    public async Task CreateTechnicianAsync(TechnicianDto dto, CancellationToken ct)
    {
        // Validate the DTO
        var validator = new TechnicianValidator(context);
        await validator.ValidateAndThrowAsync(dto, ct);

        // Map DTO to entity
        var technician = mapper.Map<Technician>(dto);

        // Add to database
        context.Set<Technician>().Add(technician);
        await context.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Updates an existing technician.
    /// Uses AutoMapper.Collection to synchronize service zones and specialities.
    /// </summary>
    /// <param name="id">The technician ID.</param>
    /// <param name="dto">The updated technician DTO.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <exception cref="ValidationException">Thrown when validation fails.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the technician is not found.</exception>
    public async Task UpdateTechnicianAsync(int id, TechnicianDto dto, CancellationToken ct)
    {
        // Validate the DTO
        var validator = new TechnicianValidator(context);
        await validator.ValidateAndThrowAsync(dto, ct);

        // Find existing technician with related collections
        var technician = await context.Set<Technician>()
                             .Include(t => t.UserRoles)
                             .Include(t => t.ServiceZones)
                             .Include(t => t.Specialities)
                             .FirstOrDefaultAsync(t => t.Id == id, ct)
                         ?? throw new KeyNotFoundException($"Technician with ID {id} not found.");

        // Map changes to entity
        // AutoMapper.Collection will handle adding/removing/updating items in collections
        mapper.Map(dto, technician);

        // Save changes
        await context.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Assigns or updates service zones for a technician.
    /// Uses AutoMapper.Collection to synchronize the collection.
    /// </summary>
    /// <param name="id">The technician ID.</param>
    /// <param name="serviceZones">The list of service zone assignments.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <exception cref="KeyNotFoundException">Thrown when the technician is not found.</exception>
    public async Task AssignServiceZonesAsync(int id, List<TechnicianServiceZoneDto> serviceZones, CancellationToken ct)
    {
        // Validate the DTO
        var validator = new TechnicianServiceZoneListValidator();
        await validator.ValidateAndThrowAsync(serviceZones, ct);

        var technician = await context.Set<Technician>()
                             .Include(t => t.ServiceZones)
                             .FirstOrDefaultAsync(t => t.Id == id, ct)
                         ?? throw new KeyNotFoundException($"Technician with ID {id} not found.");

        // Use AutoMapper.Collection to synchronize service zones
        mapper.Map(serviceZones, technician.ServiceZones);

        await context.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Assigns or updates specialities for a technician.
    /// Uses AutoMapper.Collection to synchronize the collection.
    /// </summary>
    /// <param name="id">The technician ID.</param>
    /// <param name="specialities">The list of speciality assignments.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <exception cref="KeyNotFoundException">Thrown when the technician is not found.</exception>
    public async Task AssignSpecialitiesAsync(int id, List<TechnicianSpecialityDto> specialities, CancellationToken ct)
    {
        // Validate the DTO
        var validator = new TechnicianSpecialityListValidator();
        await validator.ValidateAndThrowAsync(specialities, ct);

        var technician = await context.Set<Technician>()
                             .Include(t => t.Specialities)
                             .FirstOrDefaultAsync(t => t.Id == id, ct)
                         ?? throw new KeyNotFoundException($"Technician with ID {id} not found.");

        // Use AutoMapper.Collection to synchronize specialities
        mapper.Map(specialities, technician.Specialities);

        await context.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Retrieves all freelance technicians.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of freelance technician DTOs.</returns>
    public async Task<IEnumerable<TechnicianDto>> GetFreelanceTechniciansAsync(CancellationToken ct)
    {
        return await context.Set<Technician>()
            .Include(t => t.ServiceZones)
                .ThenInclude(tsz => tsz.ServiceZone)
            .Include(t => t.Specialities)
                .ThenInclude(ts => ts.ServiceCategory)
            .AsNoTracking()
            .Where(t => t.IsFreelance)
            .ProjectTo<TechnicianDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    /// <summary>
    /// Retrieves all employee (non-freelance) technicians.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of employee technician DTOs.</returns>
    public async Task<IEnumerable<TechnicianDto>> GetEmployeeTechniciansAsync(CancellationToken ct)
    {
        return await context.Set<Technician>()
            .Include(t => t.ServiceZones)
                .ThenInclude(tsz => tsz.ServiceZone)
            .Include(t => t.Specialities)
                .ThenInclude(ts => ts.ServiceCategory)
            .AsNoTracking()
            .Where(t => !t.IsFreelance)
            .ProjectTo<TechnicianDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<Dictionary<string, List<string>>> GetAvailableSlotsAsync(AvailableSlotRequestDto request, CancellationToken ct)
    {
        // Validate the DTO
        var validator = new AvailableSlotRequestValidator();
        await validator.ValidateAndThrowAsync(request, ct);

        return await TechnicianAvailability.GetAvailableSlotsSqlAsync(context, businessParameters, request, ct);
    }

    public async Task<TechnicianDto?> GetNearestAvailableTechnicianAsync(NearestAvailableRequestDto request, CancellationToken ct)
    {
        // Validate the DTO
        var validator = new NearestAvailableRequestValidator();
        await validator.ValidateAndThrowAsync(request, ct);

        var technician = await TechnicianAvailability.GetNearestAvailableTechnicianAsync(context, businessParameters, request.Lat, request.Lng,
            request.ZipCode, request.Specialties, request.LocalStar, request.LocalEnd, ct);

        return mapper.Map<TechnicianDto>(technician);
    }
}

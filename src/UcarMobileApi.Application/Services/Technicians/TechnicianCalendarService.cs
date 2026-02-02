using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.DTOs.Technicians;
using UcarMobileApi.Application.Validators.Common;
using UcarMobileApi.Core.Entities.Technicians;

namespace UcarMobileApi.Application.Services.Technicians;

/// <summary>
/// Provides CRUD operations for technician work schedules and calendar blocks.
/// </summary>
public class TechnicianCalendarService(IMapper mapper, IAppDbContext db, IValidatorResolver validatorResolver)
{
    // ---------------------------------------------------------------------
    // WORK SCHEDULES
    // ---------------------------------------------------------------------

    /// <summary>
    /// Creates a new work schedule for a technician.
    /// </summary>
    public async Task<TechnicalWorkScheduleReadDto> CreateScheduleAsync(TechnicalWorkScheduleDto dto, CancellationToken ct)
    {
        await validatorResolver.Get<TechnicalWorkScheduleDto>().ValidateAndThrowAsync(dto, ct);

        var entity = mapper.Map<TechnicalWorkSchedule>(dto);

        db.Set<TechnicalWorkSchedule>().Add(entity);
        await db.SaveChangesAsync(ct);

        return mapper.Map<TechnicalWorkScheduleReadDto>(entity);
    }

    /// <summary>
    /// Creates multiple work schedules for a technician in a single bulk operation.
    /// Validates each item and performs a single database commit.
    /// </summary>
    public async Task CreateManySchedulesAsync(int technicianId, IEnumerable<TechnicalWorkScheduleDto> items, CancellationToken ct)
    {
        var list = items.ToList();

        // Apply technicianId to all items
        foreach (var item in list)
            item.TechnicianId = technicianId;

        // Validate the list
        await validatorResolver.Get<IEnumerable<TechnicalWorkScheduleDto>>().ValidateAndThrowAsync(list, ct);

        // Map to entities
        var entities = mapper.Map<List<TechnicalWorkSchedule>>(list);

        db.Set<TechnicalWorkSchedule>().AddRange(entities);
        await db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Retrieves a work schedule by its identifier (no tracking).
    /// </summary>
    public async Task<TechnicalWorkScheduleReadDto?> GetScheduleAsync(int technicianId, int id, CancellationToken ct)
    {
        var e = await db.Set<TechnicalWorkSchedule>()
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.TechnicianId == technicianId && x.Id == id, ct);

        return e == null ? null : mapper.Map<TechnicalWorkScheduleReadDto>(e);
    }

    /// <summary>
    /// Retrieves all work schedules (no tracking).
    /// </summary>
    public async Task<IEnumerable<TechnicalWorkScheduleReadDto>> GetAllSchedulesAsync(int technicianId, CancellationToken ct)
    {
        var items = await db.Set<TechnicalWorkSchedule>()
            .AsNoTracking()
            .Where(t => t.TechnicianId == technicianId)
            .ToListAsync(ct);

        return mapper.Map<IEnumerable<TechnicalWorkScheduleReadDto>>(items);
    }

    /// <summary>
    /// Updates an existing work schedule.
    /// </summary>
    public async Task<TechnicalWorkScheduleReadDto> UpdateScheduleAsync(int id, TechnicalWorkScheduleDto dto, CancellationToken ct)
    {
        await validatorResolver.Get<TechnicalWorkScheduleDto>().ValidateAndThrowAsync(dto, ct);

        var schedule = await db.Set<TechnicalWorkSchedule>().FindAsync([id], ct)
            ?? throw new KeyNotFoundException($"Schedule {id} not found.");

        mapper.Map(dto, schedule);

        await db.SaveChangesAsync(ct);

        return mapper.Map<TechnicalWorkScheduleReadDto>(schedule);
    }

    /// <summary>
    /// Deletes a technician work schedule by its identifier.
    /// </summary>
    public async Task DeleteScheduleAsync(int technicianId, int id, CancellationToken ct)
    {
        var schedule = await db.Set<TechnicalWorkSchedule>().SingleOrDefaultAsync(s => s.TechnicianId == technicianId && s.Id == id, ct);
        if (schedule == null) return;

        db.Set<TechnicalWorkSchedule>().Remove(schedule);
        await db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Toggle Schedule active state.
    /// </summary>
    public async Task ToggleScheduleActivateAsync(int technicianId, int id, bool isActive, CancellationToken ct)
    {
        var entity = await db.Set<TechnicalWorkSchedule>().SingleOrDefaultAsync(x => x.TechnicianId == technicianId && x.Id == id, ct)
                     ?? throw new KeyNotFoundException($"Schedule {id} not found.");

        entity.IsActive = isActive;

        await db.SaveChangesAsync(ct);
    }

    // ---------------------------------------------------------------------
    // CALENDAR BLOCKS
    // ---------------------------------------------------------------------

    /// <summary>
    /// Creates a new calendar block (vacation, appointment, training, etc.).
    /// </summary>
    public async Task<TechnicalCalendarBlockReadDto> CreateBlockAsync(TechnicalCalendarBlockDto dto, CancellationToken ct)
    {
        await validatorResolver.Get<TechnicalCalendarBlockDto>().ValidateAndThrowAsync(dto, ct);

        var entity = mapper.Map<TechnicalCalendarBlock>(dto);

        db.Set<TechnicalCalendarBlock>().Add(entity);
        await db.SaveChangesAsync(ct);

        return mapper.Map<TechnicalCalendarBlockReadDto>(entity);
    }

    /// <summary>
    /// Retrieves a calendar block by its identifier (no tracking).
    /// </summary>
    public async Task<TechnicalCalendarBlockReadDto?> GetBlockAsync(int technicianId, int id, CancellationToken ct)
    {
        var block = await db.Set<TechnicalCalendarBlock>()
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.TechnicianId == technicianId && x.Id == id, ct);

        return block == null ? null : mapper.Map<TechnicalCalendarBlockReadDto>(block);
    }

    /// <summary>
    /// Retrieves all calendar blocks (no tracking).
    /// </summary>
    public async Task<IEnumerable<TechnicalCalendarBlockReadDto>> GetAllBlocksAsync(int technicianId, CancellationToken ct)
    {
        var items = await db.Set<TechnicalCalendarBlock>()
            .AsNoTracking()
            .Where(x => x.TechnicianId == technicianId)
            .ToListAsync(ct);

        return mapper.Map<IEnumerable<TechnicalCalendarBlockReadDto>>(items);
    }

    /// <summary>
    /// Updates an existing calendar block.
    /// </summary>
    public async Task<TechnicalCalendarBlockReadDto> UpdateBlockAsync(int id, TechnicalCalendarBlockDto dto, CancellationToken ct)
    {
        await validatorResolver.Get<TechnicalCalendarBlockDto>().ValidateAndThrowAsync(dto, ct);

        var block = await db.Set<TechnicalCalendarBlock>().FindAsync([id], ct)
            ?? throw new KeyNotFoundException($"Block {id} not found.");

        mapper.Map(dto, block);

        await db.SaveChangesAsync(ct);

        return mapper.Map<TechnicalCalendarBlockReadDto>(block);
    }

    /// <summary>
    /// Deletes a technician calendar block by its identifier.
    /// </summary>
    public async Task DeleteBlockAsync(int technicianId, int id, CancellationToken ct)
    {
        var block = await db.Set<TechnicalCalendarBlock>().SingleOrDefaultAsync(x => x.TechnicianId == technicianId && x.Id == id, ct);
        if (block == null) return;

        db.Set<TechnicalCalendarBlock>().Remove(block);
        await db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Toggle block active state.
    /// </summary>
    public async Task ToggleBlockActivateAsync(int technicianId, int id, bool isActive, CancellationToken ct)
    {
        var entity = await db.Set<TechnicalCalendarBlock>().SingleOrDefaultAsync(x => x.TechnicianId == technicianId && x.Id == id, ct)
                     ?? throw new KeyNotFoundException($"Block {id} not found.");

        entity.IsActive = isActive;

        await db.SaveChangesAsync(ct);
    }
}

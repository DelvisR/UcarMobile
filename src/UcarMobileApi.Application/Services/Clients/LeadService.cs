using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Gridify;
using Gridify.EntityFramework;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.Common.Models;
using UcarMobileApi.Application.DTOs.Clients;
using UcarMobileApi.Application.Utilities;
using UcarMobileApi.Application.Validators.Clients;
using UcarMobileApi.Core.Entities.Clients;

namespace UcarMobileApi.Application.Services.Clients;

/// <summary>
/// Lead service
/// </summary>
public class LeadService(IMapper mapper, IAppDbContext context, IGridifyMapper<Lead> gridifymapper)
{
    /// <summary>
    /// Retrieves all leads.
    /// </summary>
    public async Task<(IHeaderDictionary, IEnumerable<LeadDto>)> GetAllAsync(QueryFilter query, CancellationToken ct)
    {
        var leads = context.Set<Lead>().AsNoTracking();

        // AutoMapper ProjectTo + Filtering + Ordering + Paging
        var qp = await leads.GridifyQueryableAsync(query, gridifymapper, ct);
        return (qp.GeneratePaginationHttpHeaders(), await qp.Query.ProjectTo<LeadDto>(mapper.ConfigurationProvider).ToListAsync(ct));
    }

    /// <summary>
    /// Retrieves a single lead by id or null when not found.
    /// </summary>
    public async Task<LeadDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await context.Set<Lead>()
            .AsNoTracking()
            .Where(u => u.Id == id)
            .ProjectTo<LeadDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }

    /// <summary>
    /// Creates a new lead and returns the created DTO (with Id).
    /// </summary>
    public async Task CreateAsync(LeadDto dto, CancellationToken ct)
    {
        var validator = new LeadValidator();
        await validator.ValidateAndThrowAsync(dto, ct);

        var entity = mapper.Map<Lead>(dto);

        context.Set<Lead>().Add(entity);

        await context.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Updates an existing lead
    /// </summary>
    public async Task UpdateAsync(LeadDto dto, CancellationToken ct)
    {
        var validator = new LeadValidator();
        await validator.ValidateAndThrowAsync(dto, ct);

        var lead = await context.Set<Lead>().FirstOrDefaultAsync(x => x.Id == dto.Id, ct);

        if (lead != null)
        {
            mapper.Map(dto, lead);
            await context.SaveChangesAsync(ct);
        }
        else
        {
            throw new KeyNotFoundException($"Lead with ID {dto.Id} not found.");
        }
    }

    /// <summary>
    /// Deletes an existing lead by id. Returns true if deleted, false if not found.
    /// </summary>
    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var lead = await context.Set<Lead>().FindAsync([id], ct)
                   ?? throw new KeyNotFoundException("Lead not found.");

        context.Set<Lead>().Remove(lead);
        await context.SaveChangesAsync(ct);
    }
}

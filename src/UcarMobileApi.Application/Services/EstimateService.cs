using System;
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
using UcarMobileApi.Application.DTOs.Services;
using UcarMobileApi.Application.Utilities;
using UcarMobileApi.Core.Constants;
using UcarMobileApi.Core.Entities.Services;

namespace UcarMobileApi.Application.Services;

public class EstimateService(IAppDbContext context, IMapper mapper, IGridifyMapper<Estimate> gridifyMapper, BusinessParameterService businessParameters)
{
    /// <summary>
    /// Retrieves all estimate records for a given vehicle, applying filtering,
    /// sorting, and pagination using Gridify.
    /// </summary>
    public async Task<(IHeaderDictionary, IEnumerable<EstimateDto>)> GetEstimatesAsync(int vehicleId, QueryFilter query, CancellationToken ct)
    {
        // Query base data filtered by vehicle ID
        var estimatesQuery = context.Set<Estimate>()
            .AsNoTracking()
            .Where(e => e.VehicleId == vehicleId || e.VehicleId == 0);

        // Apply Gridify (filtering, ordering, paging)
        var qp = await estimatesQuery.GridifySafeAsync(query, gridifyMapper, ct);

        // Project to DTO and execute query
        var estimateDtos = await qp.Query
            .ProjectTo<EstimateDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);

        // Apply discounts and computed cost values
        await ApplyDiscountsAsync(estimateDtos, ct);

        // Return results plus pagination headers
        return (qp.GeneratePaginationHttpHeaders(), estimateDtos);
    }

    /// <summary>
    /// Retrieves the most popular services for a given vehicle,
    /// based on ServicePopularity records. No pagination is used here
    /// because the list is expected to be small.
    /// </summary>
    public async Task<IEnumerable<EstimateDto>> GetPopularEstimateServicesAsync(int vehicleId, CancellationToken ct)
    {
        // Retrieve ordered list of most frequently selected services
        var popularServiceIds = await context.Set<ServicePopularity>()
            .AsNoTracking()
            .Where(sp => sp.VehicleId == vehicleId || sp.VehicleId == 0)
            .OrderByDescending(sp => sp.Count)
            .Select(sp => sp.ServiceId)
            .ToListAsync(ct);

        if (popularServiceIds.Count == 0)
            return [];

        // Fetch corresponding estimates
        var estimateDtos = await context.Set<Estimate>()
            .AsNoTracking()
            .Where(e => e.VehicleId == vehicleId && popularServiceIds.Contains(e.ServiceId))
            .ProjectTo<EstimateDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);

        // Apply discount logic
        await ApplyDiscountsAsync(estimateDtos, ct);

        return estimateDtos;
    }

    /// <summary>
    /// Applies labor and parts discount factors and recalculates
    /// final cost values for all EstimateDto items.
    /// </summary>
    private async Task ApplyDiscountsAsync(IEnumerable<EstimateDto> dtos, CancellationToken ct)
    {
        // Retrieve parameter values from Business Parameters storage
        var laborDiscount = await businessParameters.GetValueAsync<decimal>(BusinessParameterKeys.Estimate.LaborDiscountFactor, ct) / 100;

        var partDiscount = await businessParameters.GetValueAsync<decimal>(BusinessParameterKeys.Estimate.PartDiscountFactor, ct) / 100;

        // Apply discount and recalculate cost for each DTO
        foreach (var dto in dtos)
        {
            CalculateCosts(dto, laborDiscount, partDiscount);
        }
    }

    /// <summary>
    /// Adjusts cost ranges using discount factors and calculates
    /// the final labor, part, and total cost values.
    /// </summary>
    private static void CalculateCosts(EstimateDto dto, decimal laborDiscount, decimal partDiscount)
    {
        // Apply discount percentage to cost ranges
        dto.LaborMaxCost -= (dto.LaborMaxCost * laborDiscount);
        dto.LaborMinCost -= (dto.LaborMinCost * laborDiscount);
        dto.PartMaxCost -= (dto.PartMaxCost * partDiscount);
        dto.PartMinCost -= (dto.PartMinCost * partDiscount);

        // Calculate middle values for labor and part cost
        dto.LaborCost = Math.Round((dto.LaborMaxCost + dto.LaborMinCost) / 2, 2);
        dto.PartCost = Math.Round((dto.PartMaxCost + dto.PartMinCost) / 2, 2);

        // Total = labor + parts
        dto.TotalCost = dto.LaborCost + dto.PartCost;
    }
}

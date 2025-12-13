using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.DTOs.Vehicles;
using UcarMobileApi.Core.Entities.Vehicles;

namespace UcarMobileApi.Application.Services;

public class VehicleService(IAppDbContext context, IMapper mapper)
{
    public async Task<IEnumerable<int>> GetYearsAsync(CancellationToken ct)
    {
        return await context.Set<Vehicle>()
            .AsNoTracking()
            .Select(v => v.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<string>> GetMakesAsync(int year, CancellationToken ct)
    {
        return await context.Set<Vehicle>()
            .AsNoTracking()
            .Where(v => v.Year == year)
            .Select(v => v.Make)
            .Distinct()
            .OrderBy(m => m)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<ModelDto>> GetModelsAsync(int year, string make, CancellationToken ct)
    {
        var models = await context.Set<Vehicle>()
            .AsNoTracking()
            .Where(v => v.Year == year && v.Make == make)
            .ProjectTo<ModelDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return models;
    }

}

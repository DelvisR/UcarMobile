using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.DTOs;
using UcarMobileApi.Application.Services.Security;
using UcarMobileApi.Core.Entities.Configurations;

namespace UcarMobileApi.Application.Services.Configurations;

public class BusinessParameterService(IMapper mapper, IAppDbContext context, ICacheService cache)
{
    private const string CacheKey = "BusinessParameters";

    public async Task<List<BusinessParameterDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var cached = await cache.GetAsync<List<BusinessParameterDto>>(CacheKey);
        if (cached is not null) return cached;

        var list = await context.Set<BusinessParameter>().AsNoTracking()
            .ProjectTo<BusinessParameterDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        await cache.SetAsync(CacheKey, list, TimeSpan.FromHours(24), null); // 24 hours, no sliding

        return list;
    }

    public async Task UpdateAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be empty.", nameof(key));

        if (value is null)
            throw new ArgumentNullException(nameof(value), "Value cannot be null.");

        var parameter = await context.Set<BusinessParameter>().FirstOrDefaultAsync(p => p.Key == key, cancellationToken)
                        ?? throw new KeyNotFoundException($"Parameter '{key}' not found.");

        parameter.Value = value;

        await context.SaveChangesAsync(cancellationToken);

        await cache.InvalidateAsync(CacheKey); //invalidate cache to force load
    }


    public async Task<T?> GetValueAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be empty.", nameof(key));

        // Try from cache (using existing GetAllAsync to ensure cache is warmed)
        var list = await GetAllAsync(cancellationToken);

        var parameter = list.FirstOrDefault(p => p.Key == key)
                        ?? throw new KeyNotFoundException($"Parameter '{key}' not found.");

        if (string.IsNullOrWhiteSpace(parameter.Value)) return default;

        try
        {
            return (T)Convert.ChangeType(parameter.Value, typeof(T));
        }
        catch (Exception ex)
        {
            throw new InvalidCastException($"Failed to convert parameter '{key}' with value '{parameter.Value}' to type {typeof(T).Name}.", ex);
        }
    }


}

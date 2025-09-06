using System;
using System.Threading.Tasks;

namespace UcarMobileApi.Application.Services.Security;

using Microsoft.Extensions.Caching.Memory;

public class MemoryCacheService(IMemoryCache cache) : ICacheService
{
    public Task<T?> GetAsync<T>(string key)
    {
        return Task.FromResult(
            cache.TryGetValue(key, out T? value)
                ? value
                : default
        );
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? ttl, TimeSpan? slidingExpiration)
    {
        var options = new MemoryCacheEntryOptions();

        if (ttl.HasValue)
            options.AbsoluteExpirationRelativeToNow = ttl;

        if (slidingExpiration.HasValue)
            options.SlidingExpiration = slidingExpiration;

        cache.Set(key, value, options);

        return Task.CompletedTask;
    }


    public Task InvalidateAsync(string key)
    {
        cache.Remove(key);
        return Task.CompletedTask;
    }
}

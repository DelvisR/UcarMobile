using System;
using System.Threading.Tasks;

namespace UcarMobileApi.Application.Services.Security
{
    /// <summary>
    /// Defines a generic cache service to store, retrieve, and invalidate items.
    /// Supports absolute and sliding expiration policies.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Retrieves a cached value by its key.
        /// </summary>
        /// <typeparam name="T">The expected type of the cached value.</typeparam>
        /// <param name="key">The unique key that identifies the cache entry.</param>
        /// <returns>
        /// The cached value if found; otherwise, <c>null</c>.
        /// </returns>
        Task<T?> GetAsync<T>(string key);

        /// <summary>
        /// Adds or replaces an item in the cache with the specified expiration options.
        /// </summary>
        /// <typeparam name="T">The type of the value to cache.</typeparam>
        /// <param name="key">The unique key that identifies the cache entry.</param>
        /// <param name="value">The value to be cached.</param>
        /// <param name="ttl">
        /// Absolute expiration time relative to now.
        /// If <c>null</c>, the entry will not expire absolutely.
        /// </param>
        /// <param name="slidingExpiration">
        /// Sliding expiration interval.
        /// If <c>null</c>, the entry will not use sliding expiration.
        /// </param>
        Task SetAsync<T>(string key, T value, TimeSpan? ttl, TimeSpan? slidingExpiration);

        /// <summary>
        /// Removes a cache entry by its key.
        /// </summary>
        /// <param name="key">The unique key that identifies the cache entry.</param>
        Task InvalidateAsync(string key);
    }
}

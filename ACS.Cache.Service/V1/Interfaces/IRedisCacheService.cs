using Microsoft.Extensions.Caching.Distributed;

namespace ACS.Cache.Service.V1.Interfaces
{
    /// <summary>
    /// Defines methods for interacting with a Redis-based distributed cache, providing mechanisms to retrieve cached
    /// values or populate the cache using caller-supplied methods when the value is not already available.
    /// </summary>
    /// <remarks>This interface supports both synchronous and asynchronous retrieval methods, with options for
    /// stateless or stateful value generation. It allows specifying cache entry options, such as expiration policies,
    /// and supports cancellation tokens for asynchronous operations.</remarks>
    public interface IRedisCacheService
    {
        /// <summary>
        /// Gets a value from cache with async stateless fallback method
        /// </summary>
        ValueTask<T> GetAsync<T>(string key, Func<CancellationToken, ValueTask<T>> getMethod,
            DistributedCacheEntryOptions? options = null, CancellationToken cancellation = default);

        /// <summary>
        /// Gets a value from cache with sync stateless fallback method
        /// </summary>
        ValueTask<T> GetAsync<T>(string key, Func<T> getMethod,
            DistributedCacheEntryOptions? options = null, CancellationToken cancellation = default);

        /// <summary>
        /// Gets a value from cache with async stateful fallback method
        /// </summary>
        ValueTask<T> GetAsync<TState, T>(string key, TState state, Func<TState, CancellationToken, ValueTask<T>> getMethod,
            DistributedCacheEntryOptions? options = null, CancellationToken cancellation = default);

        /// <summary>
        /// Gets a value from cache with sync stateful fallback method
        /// </summary>
        ValueTask<T> GetAsync<TState, T>(string key, TState state, Func<TState, T> getMethod,
            DistributedCacheEntryOptions? options = null, CancellationToken cancellation = default);

        /// <summary>
        /// Removes a value from the cache
        /// </summary>
        Task RemoveAsync(string key, CancellationToken cancellation = default);

        /// <summary>
        /// Sets a value in the cache directly
        /// </summary>
        Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions? options = null, CancellationToken cancellation = default);

        /// <summary>
        /// Tries to get a value from cache without a fallback method
        /// </summary>
        ValueTask<T?> TryGetAsync<T>(string key, CancellationToken cancellation = default);
    }
}

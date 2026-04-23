using ACS.Cache.Service.V1.Interfaces;
using ACS.License.V1;
using ACS.Service.V1.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Buffers;
using System.Text.Json;

namespace ACS.Cache.Service.V1.Services
{
    public class RedisCacheService(ILicenseManager license, IDistributedCache cache) : ACS.Service.Service(license), IRedisCacheService
    {
        /// <summary>
        ///  Gets a value from cache, with a caller-supplied <paramref name="getMethod"/> (async, stateless) that is used if the value is not yet available
        /// </summary>
        public ValueTask<T> GetAsync<T>(string key, Func<CancellationToken, ValueTask<T>> getMethod,
            DistributedCacheEntryOptions? options = null, CancellationToken cancellation = default)
            => GetAsyncShared<int, T>(cache, key, state: 0, getMethod, options, cancellation);

        /// <summary>
        /// Gets a value from cache, with a caller-supplied <paramref name="getMethod"/> (sync, stateless) that is used if the value is not yet available
        /// </summary>
        public ValueTask<T> GetAsync<T>(string key, Func<T> getMethod,
            DistributedCacheEntryOptions? options = null, CancellationToken cancellation = default)
            => GetAsyncShared<int, T>(cache, key, state: 0, getMethod, options, cancellation);

        /// <summary>
        ///  Gets a value from cache, with a caller-supplied <paramref name="getMethod"/> (async, stateful) that is used if the value is not yet available
        /// </summary>
        public ValueTask<T> GetAsync<TState, T>(string key, TState state, Func<TState, CancellationToken, ValueTask<T>> getMethod,
            DistributedCacheEntryOptions? options = null, CancellationToken cancellation = default)
            => GetAsyncShared<TState, T>(cache, key, state, getMethod, options, cancellation);

        /// <summary>
        ///  Gets a value from cache, with a caller-supplied <paramref name="getMethod"/> (sync, stateful) that is used if the value is not yet available
        /// </summary>
        public ValueTask<T> GetAsync<TState, T>(string key, TState state, Func<TState, T> getMethod,
            DistributedCacheEntryOptions? options = null, CancellationToken cancellation = default)
            => GetAsyncShared<TState, T>(cache, key, state, getMethod, options, cancellation);

        /// <summary>
        /// Removes a value from the cache
        /// </summary>
        public async Task RemoveAsync(string key, CancellationToken cancellation = default)
        {
            await cache.RemoveAsync(key, cancellation);
        }

        /// <summary>
        /// Sets a value in the cache directly without getting it first
        /// </summary>
        public async Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions? options = null, CancellationToken cancellation = default)
        {
            var bytes = Serialize(value);
            if (options is null)
            {
                await cache.SetAsync(key, bytes, cancellation);
            }
            else
            {
                await cache.SetAsync(key, bytes, options, cancellation);
            }
        }

        /// <summary>
        /// Tries to get a value from cache without a fallback method
        /// </summary>
        public async ValueTask<T?> TryGetAsync<T>(string key, CancellationToken cancellation = default)
        {
            var bytes = await cache.GetAsync(key, cancellation);
            if (bytes is null)
            {
                return default;
            }
            return Deserialize<T>(bytes);
        }

        private static ValueTask<T> GetAsyncShared<TState, T>(IDistributedCache cache, string key, TState state, Delegate getMethod,
            DistributedCacheEntryOptions? options, CancellationToken cancellation)
        {
            var pending = cache.GetAsync(key, cancellation);
            if (!pending.IsCompletedSuccessfully)
            {
                return Awaited(cache, key, pending, state, getMethod, options, cancellation);
            }

            var bytes = pending.GetAwaiter().GetResult();
            if (bytes is null)
            {
                return Awaited(cache, key, null, state, getMethod, options, cancellation);
            }

            return new(Deserialize<T>(bytes));

            static async ValueTask<T> Awaited(
                IDistributedCache cache,
                string key,
                Task<byte[]?>? pending,
                TState state,
                Delegate getMethod,
                DistributedCacheEntryOptions? options,
                CancellationToken cancellation)
            {
                byte[]? bytes;
                if (pending is not null)
                {
                    bytes = await pending;
                    if (bytes is not null)
                    {
                        return Deserialize<T>(bytes);
                    }
                }

                var result = getMethod switch
                {
                    Func<TState, CancellationToken, ValueTask<T>> get => await get(state, cancellation),
                    Func<TState, T> get => get(state),
                    Func<CancellationToken, ValueTask<T>> get => await get(cancellation),
                    Func<T> get => get(),
                    _ => throw new ArgumentException(null, nameof(getMethod)),
                };
                bytes = Serialize<T>(result);
                if (options is null)
                {
                    await cache.SetAsync(key, bytes, cancellation);
                }
                else
                {
                    await cache.SetAsync(key, bytes, options, cancellation);
                }
                return result;
            }
        }

        private static T Deserialize<T>(byte[] bytes) => JsonSerializer.Deserialize<T>(bytes)!;

        private static byte[] Serialize<T>(T value)
        {
            var buffer = new ArrayBufferWriter<byte>();
            using var writer = new Utf8JsonWriter(buffer);
            JsonSerializer.Serialize(writer, value);
            return buffer.WrittenSpan.ToArray();
        }
    }
}

using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using ProjectTemplate.Application.Abstrations.Caching;
using System.Collections.Concurrent;

namespace ProjectTemplate.Infra.ExternalService.Caching
{
    public class CacheService : ICacheService
    {
        private static ConcurrentDictionary<string, bool> CacheKeys = new();
        private readonly IDistributedCache _districtorCache;

        public CacheService(IDistributedCache districtorCache)
        {
            _districtorCache = districtorCache;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class
        {
            string? cachedValue = await _districtorCache.GetStringAsync(key, ct);

            if (cachedValue is null)
            {
                return null;
            }

            T? value = JsonConvert.DeserializeObject<T>(cachedValue);

            return value;
        }
        public async Task<T> GetAsync<T>(string key, Func<Task<T>> factory, double cachedTimeExpirationInSeconds = 60, CancellationToken ct = default) where T : class
        {
            T? cachedValue = await GetAsync<T>(key, ct);

            if (cachedValue is not null)
            {
                return cachedValue;
            }

            cachedValue = await factory();

            await SetAsync(key, cachedValue, cachedTimeExpirationInSeconds, ct);

            return cachedValue;
        }

        public async Task SetAsync<T>(string key, T value, double cachedTimeExpirationInSeconds = 60, CancellationToken ct = default) where T : class
        {
            string cacheValue = JsonConvert.SerializeObject(value);
            await _districtorCache.SetStringAsync(key, cacheValue,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(cachedTimeExpirationInSeconds),
                },
                ct);

            CacheKeys.TryAdd(key, false);
        }

        public async Task RemoveAsync(string key, CancellationToken ct = default)
        {
            await _districtorCache.RemoveAsync(key, ct);
            CacheKeys.TryRemove(key, out bool _);
        }

        public async Task RemoveByPrefixAsync(string prefixKey, CancellationToken ct = default)
        {
            IEnumerable<Task> tasks = CacheKeys
                .Keys
                .Where(key => key.StartsWith(prefixKey))
                .Select(k => RemoveAsync(k, ct));

            await Task.WhenAll(tasks);
        }
    }
}

namespace ProjectTemplate.Application.Abstrations.Caching
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class;
        Task<T> GetAsync<T>(string key, Func<Task<T>> factory, double cachedTimeExpirationInSeconds = 60, CancellationToken ct = default) where T : class;
        Task SetAsync<T>(string key, T value, double cachedTimeExpirationInSeconds = 60, CancellationToken ct = default) where T : class;
        Task RemoveAsync(string key, CancellationToken ct = default);
        Task RemoveByPrefixAsync(string prefixKey, CancellationToken ct = default);
    }
}

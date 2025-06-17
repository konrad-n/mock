namespace SledzSpecke.Application.Abstractions;

/// <summary>
/// Cache service abstraction for performance optimization
/// </summary>
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default) where T : class;
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetKeysAsync(string pattern, CancellationToken cancellationToken = default);
    Task ClearAsync(string pattern, CancellationToken cancellationToken = default);
}
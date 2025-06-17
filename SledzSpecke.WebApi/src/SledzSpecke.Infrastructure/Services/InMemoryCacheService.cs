using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Abstractions;
using System.Text.Json;

namespace SledzSpecke.Infrastructure.Services;

/// <summary>
/// In-memory cache implementation for development and testing
/// In production, this would be replaced with Redis or similar
/// </summary>
public class InMemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<InMemoryCacheService> _logger;
    private readonly HashSet<string> _keys = new();
    private readonly object _lock = new();

    public InMemoryCacheService(IMemoryCache cache, ILogger<InMemoryCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            if (_cache.TryGetValue(key, out var cachedValue))
            {
                if (cachedValue is string json)
                {
                    return Task.FromResult(JsonSerializer.Deserialize<T>(json));
                }
                return Task.FromResult(cachedValue as T);
            }
            return Task.FromResult<T?>(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving value from cache for key: {Key}", key);
            return Task.FromResult<T?>(null);
        }
    }

    public Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var json = JsonSerializer.Serialize(value);
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration,
                PostEvictionCallbacks = { new PostEvictionCallbackRegistration
                {
                    EvictionCallback = (k, v, r, s) => RemoveKeyFromTracking(k?.ToString() ?? string.Empty)
                }}
            };
            
            _cache.Set(key, json, cacheOptions);
            
            lock (_lock)
            {
                _keys.Add(key);
            }
            
            _logger.LogDebug("Cached value for key: {Key} with expiration: {Expiration}", key, expiration);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting value in cache for key: {Key}", key);
            return Task.CompletedTask;
        }
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _cache.Remove(key);
        RemoveKeyFromTracking(key);
        _logger.LogDebug("Removed key from cache: {Key}", key);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_cache.TryGetValue(key, out _));
    }

    public Task<IEnumerable<string>> GetKeysAsync(string pattern, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var matchingKeys = _keys.Where(k => MatchesPattern(k, pattern));
            return Task.FromResult(matchingKeys);
        }
    }

    public async Task ClearAsync(string pattern, CancellationToken cancellationToken = default)
    {
        var keysToRemove = await GetKeysAsync(pattern, cancellationToken);
        foreach (var key in keysToRemove.ToList())
        {
            await RemoveAsync(key, cancellationToken);
        }
        _logger.LogInformation("Cleared {Count} keys matching pattern: {Pattern}", keysToRemove.Count(), pattern);
    }

    private void RemoveKeyFromTracking(string key)
    {
        lock (_lock)
        {
            _keys.Remove(key);
        }
    }

    private bool MatchesPattern(string key, string pattern)
    {
        if (pattern == "*") return true;
        if (pattern.StartsWith("*") && pattern.EndsWith("*"))
        {
            var contains = pattern.Trim('*');
            return key.Contains(contains);
        }
        if (pattern.StartsWith("*"))
        {
            var endsWith = pattern.TrimStart('*');
            return key.EndsWith(endsWith);
        }
        if (pattern.EndsWith("*"))
        {
            var startsWith = pattern.TrimEnd('*');
            return key.StartsWith(startsWith);
        }
        return key == pattern;
    }
}
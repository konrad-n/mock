using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Abstractions;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SledzSpecke.Application.Decorators;

/// <summary>
/// Decorator that adds caching to query handlers
/// Caches query results to improve performance for read-heavy operations
/// </summary>
internal sealed class CachingQueryHandlerDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
    where TQuery : class, IQuery<TResult>
    where TResult : class
{
    private readonly IQueryHandler<TQuery, TResult> _handler;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachingQueryHandlerDecorator<TQuery, TResult>> _logger;
    
    // Default cache duration - can be overridden per query type
    private static readonly TimeSpan DefaultCacheDuration = TimeSpan.FromMinutes(5);
    
    // Query-specific cache durations
    private static readonly Dictionary<Type, TimeSpan> CacheDurations = new()
    {
        { typeof(Queries.GetSpecializationTemplates), TimeSpan.FromHours(24) }, // Templates rarely change
        { typeof(Features.MedicalShifts.Queries.GetMedicalShiftStatistics.GetMedicalShiftStatistics), TimeSpan.FromMinutes(1) }, // Statistics need to be fresh
        { typeof(Queries.GetUserMedicalShifts), TimeSpan.FromMinutes(5) }, // Standard duration
        { typeof(Queries.GetInternshipById), TimeSpan.FromMinutes(10) }, // Internship data is relatively stable
        { typeof(Queries.GetSpecializationSmkDetails), TimeSpan.FromHours(1) }, // SMK details change infrequently
        { typeof(Queries.GetModuleProgress), TimeSpan.FromMinutes(5) }, // Progress updates periodically
    };

    public CachingQueryHandlerDecorator(
        IQueryHandler<TQuery, TResult> handler,
        ICacheService cacheService,
        ILogger<CachingQueryHandlerDecorator<TQuery, TResult>> logger)
    {
        _handler = handler;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<TResult> HandleAsync(TQuery query)
    {
        var queryType = typeof(TQuery);
        
        // Some queries should never be cached (e.g., authentication, real-time data)
        if (ShouldSkipCache(queryType))
        {
            _logger.LogDebug("Skipping cache for query type: {QueryType}", queryType.Name);
            return await _handler.HandleAsync(query);
        }

        var cacheKey = GenerateCacheKey(query);
        
        // Try to get from cache
        var cachedResult = await _cacheService.GetAsync<TResult>(cacheKey);
        if (cachedResult != null)
        {
            _logger.LogDebug("Cache hit for query {QueryType} with key: {CacheKey}", queryType.Name, cacheKey);
            return cachedResult;
        }

        _logger.LogDebug("Cache miss for query {QueryType} with key: {CacheKey}", queryType.Name, cacheKey);
        
        // Execute the query
        var result = await _handler.HandleAsync(query);
        
        // Cache the result
        var cacheDuration = GetCacheDuration(queryType);
        await _cacheService.SetAsync(cacheKey, result, cacheDuration);
        
        _logger.LogDebug("Cached result for query {QueryType} with key: {CacheKey}, duration: {Duration}", 
            queryType.Name, cacheKey, cacheDuration);

        return result;
    }

    private static bool ShouldSkipCache(Type queryType)
    {
        // Add query types that should never be cached
        var noCacheQueries = new[]
        {
            typeof(Queries.GetUser), // User context should always be fresh
            typeof(Queries.GetUserProfile), // User profile should always be fresh
            typeof(Queries.DownloadFile), // File downloads should not be cached
        };

        return noCacheQueries.Contains(queryType);
    }

    private static TimeSpan GetCacheDuration(Type queryType)
    {
        return CacheDurations.TryGetValue(queryType, out var duration) 
            ? duration 
            : DefaultCacheDuration;
    }

    private static string GenerateCacheKey(TQuery query)
    {
        var queryType = typeof(TQuery).Name;
        var queryJson = JsonSerializer.Serialize(query);
        
        // Create a hash of the query to handle complex query objects
        using var sha256 = SHA256.Create();
        var queryHash = Convert.ToBase64String(
            sha256.ComputeHash(Encoding.UTF8.GetBytes(queryJson)));
        
        return $"query:{queryType}:{queryHash}";
    }
}
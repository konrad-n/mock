using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Abstractions;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace SledzSpecke.Application.Decorators;

/// <summary>
/// Decorator that monitors performance of query handlers
/// Tracks execution time, success rate, and provides metrics for monitoring systems
/// </summary>
internal sealed class PerformanceQueryHandlerDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
    where TQuery : class, IQuery<TResult>
{
    private readonly IQueryHandler<TQuery, TResult> _handler;
    private readonly ILogger<PerformanceQueryHandlerDecorator<TQuery, TResult>> _logger;
    private const int SlowOperationThresholdMs = 300; // Queries should be faster than commands
    private static readonly Meter _meter = new("SledzSpecke.Application", "1.0");
    private static readonly Counter<long> _queryCounter = _meter.CreateCounter<long>("queries_executed", "queries", "Number of queries executed");
    private static readonly Histogram<double> _queryDuration = _meter.CreateHistogram<double>("query_duration", "ms", "Query execution duration");
    private static readonly Counter<long> _queryErrors = _meter.CreateCounter<long>("query_errors", "errors", "Number of query execution errors");

    public PerformanceQueryHandlerDecorator(
        IQueryHandler<TQuery, TResult> handler,
        ILogger<PerformanceQueryHandlerDecorator<TQuery, TResult>> logger)
    {
        _handler = handler;
        _logger = logger;
    }

    public async Task<TResult> HandleAsync(TQuery query)
    {
        var queryName = typeof(TQuery).Name;
        var stopwatch = Stopwatch.StartNew();
        var tags = new TagList
        {
            { "query", queryName }
        };

        try
        {
            var result = await _handler.HandleAsync(query);

            stopwatch.Stop();
            var duration = stopwatch.ElapsedMilliseconds;

            // Record metrics
            _queryCounter.Add(1, tags);
            _queryDuration.Record(duration, tags);

            if (duration > SlowOperationThresholdMs)
            {
                _logger.LogWarning(
                    "Slow query detected: Query {QueryName} took {ElapsedMilliseconds}ms (threshold: {Threshold}ms)",
                    queryName,
                    duration,
                    SlowOperationThresholdMs);
            }

            return result;
        }
        catch (Exception)
        {
            stopwatch.Stop();
            
            // Record error metrics
            _queryErrors.Add(1, tags);
            _queryDuration.Record(stopwatch.ElapsedMilliseconds, tags);

            throw;
        }
    }
}
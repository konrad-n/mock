using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Decorators;

public sealed class LoggingQueryHandlerDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
    where TQuery : class, IQuery<TResult>
{
    private readonly IQueryHandler<TQuery, TResult> _handler;
    private readonly ILogger<LoggingQueryHandlerDecorator<TQuery, TResult>> _logger;

    public LoggingQueryHandlerDecorator(
        IQueryHandler<TQuery, TResult> handler,
        ILogger<LoggingQueryHandlerDecorator<TQuery, TResult>> logger)
    {
        _handler = handler;
        _logger = logger;
    }

    public async Task<TResult> HandleAsync(TQuery query)
    {
        var queryName = typeof(TQuery).Name;
        var stopwatch = Stopwatch.StartNew();
        
        _logger.LogInformation(
            "Executing query {QueryName} with data: {QueryData}",
            queryName,
            JsonSerializer.Serialize(query));

        try
        {
            var result = await _handler.HandleAsync(query);
            
            stopwatch.Stop();
            
            _logger.LogInformation(
                "Query {QueryName} executed successfully in {ElapsedMilliseconds}ms",
                queryName,
                stopwatch.ElapsedMilliseconds);
            
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            _logger.LogError(
                ex,
                "Query {QueryName} failed after {ElapsedMilliseconds}ms with error: {ErrorMessage}",
                queryName,
                stopwatch.ElapsedMilliseconds,
                ex.Message);
            
            throw;
        }
    }
}
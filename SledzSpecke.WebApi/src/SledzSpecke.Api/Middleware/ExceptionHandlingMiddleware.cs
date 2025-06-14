using System.Net;
using System.Text.Json;
using SledzSpecke.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace SledzSpecke.Api.Middleware;

public class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An error occurred: {Message}", exception.Message);
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";
        
        var (statusCode, problemDetails) = MapExceptionToProblemDetails(exception, context);
        
        context.Response.StatusCode = statusCode;
        
        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        
        await context.Response.WriteAsync(json);
    }

    private (int statusCode, ProblemDetails problemDetails) MapExceptionToProblemDetails(Exception exception, HttpContext context)
    {
        var problemDetails = new ProblemDetails
        {
            Instance = context.Request.Path,
            Extensions = { ["traceId"] = context.TraceIdentifier }
        };

        if (_environment.IsDevelopment())
        {
            problemDetails.Extensions["exception"] = exception.ToString();
        }

        return exception switch
        {
            // Domain exceptions
            InvalidEntityIdException e => (
                StatusCodes.Status400BadRequest,
                new ProblemDetails
                {
                    Title = "Invalid Entity ID",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = e.Message,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                }.CopyExtensions(problemDetails)
            ),
            
            EntityNotFoundException e => (
                StatusCodes.Status404NotFound,
                new ProblemDetails
                {
                    Title = "Entity Not Found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = e.Message,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
                }.CopyExtensions(problemDetails)
            ),
            
            DomainException e => (
                StatusCodes.Status400BadRequest,
                new ProblemDetails
                {
                    Title = "Domain Rule Violation",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = e.Message,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                }.CopyExtensions(problemDetails)
            ),
            
            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                new ProblemDetails
                {
                    Title = "Unauthorized",
                    Status = StatusCodes.Status401Unauthorized,
                    Detail = "Authentication is required to access this resource",
                    Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
                }.CopyExtensions(problemDetails)
            ),
            
            // Default
            _ => (
                StatusCodes.Status500InternalServerError,
                new ProblemDetails
                {
                    Title = "An error occurred",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = _environment.IsDevelopment() ? exception.Message : "An unexpected error occurred",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
                }.CopyExtensions(problemDetails)
            )
        };
    }
}

public static class ProblemDetailsExtensions
{
    public static ProblemDetails CopyExtensions(this ProblemDetails target, ProblemDetails source)
    {
        foreach (var extension in source.Extensions)
        {
            target.Extensions[extension.Key] = extension.Value;
        }
        target.Instance = source.Instance;
        return target;
    }
}
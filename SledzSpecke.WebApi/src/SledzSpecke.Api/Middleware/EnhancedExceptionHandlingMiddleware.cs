using System.Net;
using System.Text.Json;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace SledzSpecke.Api.Middleware;

/// <summary>
/// Enhanced exception handling middleware that provides comprehensive error handling
/// following REST API best practices with Problem Details (RFC 7807)
/// </summary>
public class EnhancedExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<EnhancedExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public EnhancedExceptionHandlingMiddleware(
        ILogger<EnhancedExceptionHandlingMiddleware> logger, 
        IHostEnvironment environment)
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
            var traceId = context.TraceIdentifier;
            var correlationId = context.Items["CorrelationId"]?.ToString() ?? traceId;
            
            // Structured logging with rich context
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["TraceId"] = traceId,
                ["RequestPath"] = context.Request.Path.ToString(),
                ["RequestMethod"] = context.Request.Method,
                ["UserAgent"] = context.Request.Headers["User-Agent"].ToString(),
                ["RemoteIp"] = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                ["UserId"] = context.User?.FindFirst("sub")?.Value ?? "Anonymous",
                ["ExceptionType"] = exception.GetType().Name,
                ["ExceptionMessage"] = exception.Message
            }))
            {
                _logger.LogError(exception, 
                    "Unhandled exception occurred. Type: {ExceptionType}, Message: {ExceptionMessage}, Path: {Path}", 
                    exception.GetType().Name, 
                    exception.Message, 
                    context.Request.Path);
            }
            
            await HandleExceptionAsync(context, exception, correlationId);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, string correlationId)
    {
        context.Response.ContentType = "application/problem+json";

        var (statusCode, problemDetails) = MapExceptionToProblemDetails(exception, context);
        
        // Add correlation ID to the response
        problemDetails.Extensions["correlationId"] = correlationId;

        context.Response.StatusCode = statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            // Temporarily pretty-print JSON in production - REMOVE BEFORE CUSTOMER RELEASE
            WriteIndented = true // was: _environment.IsDevelopment()
        };

        var json = JsonSerializer.Serialize(problemDetails, options);
        await context.Response.WriteAsync(json);
    }

    private (int statusCode, ProblemDetails problemDetails) MapExceptionToProblemDetails(
        Exception exception, HttpContext context)
    {
        var baseDetails = CreateBaseProblemDetails(context);

        return exception switch
        {
            // Application layer exceptions
            ValidationException e => CreateProblemDetails(
                StatusCodes.Status400BadRequest,
                "Validation Error",
                e.Message,
                "validation-error",
                baseDetails),

            InvalidCredentialsException => CreateProblemDetails(
                StatusCodes.Status401Unauthorized,
                "Invalid Credentials",
                "The provided credentials are invalid",
                "invalid-credentials",
                baseDetails),

            NotFoundException e => CreateProblemDetails(
                StatusCodes.Status404NotFound,
                "Resource Not Found",
                e.Message,
                "not-found",
                baseDetails),

            // UserAlreadyExistsException e => CreateProblemDetails(
            //     StatusCodes.Status409Conflict,
            //     "User Already Exists",
            //     e.Message,
            //     "user-exists",
            //     baseDetails),

            CannotModifySyncedDataException e => CreateProblemDetails(
                StatusCodes.Status409Conflict,
                "Cannot Modify Synced Data",
                e.Message,
                "synced-data-locked",
                baseDetails),

            // Domain layer exceptions - Value Objects
            InvalidEmailException e => CreateProblemDetails(
                StatusCodes.Status400BadRequest,
                "Invalid Email",
                e.Message,
                "invalid-email",
                baseDetails),

            InvalidPersonNameException e => CreateProblemDetails(
                StatusCodes.Status400BadRequest,
                "Invalid Person Name",
                e.Message,
                "invalid-person-name",
                baseDetails),

            InvalidFilePathException e => CreateProblemDetails(
                StatusCodes.Status400BadRequest,
                "Invalid File Path",
                e.Message,
                "invalid-file-path",
                baseDetails),

            InvalidInstitutionNameException e => CreateProblemDetails(
                StatusCodes.Status400BadRequest,
                "Invalid Institution Name",
                e.Message,
                "invalid-institution-name",
                baseDetails),

            InvalidProcedureCodeException e => CreateProblemDetails(
                StatusCodes.Status400BadRequest,
                "Invalid Procedure Code",
                e.Message,
                "invalid-procedure-code",
                baseDetails),

            InvalidDOIException e => CreateProblemDetails(
                StatusCodes.Status400BadRequest,
                "Invalid DOI",
                e.Message,
                "invalid-doi",
                baseDetails),

            InvalidISBNException e => CreateProblemDetails(
                StatusCodes.Status400BadRequest,
                "Invalid ISBN",
                e.Message,
                "invalid-isbn",
                baseDetails),

            InvalidPhoneNumberException e => CreateProblemDetails(
                StatusCodes.Status400BadRequest,
                "Invalid Phone Number",
                e.Message,
                "invalid-phone-number",
                baseDetails),

            InvalidWebUrlException e => CreateProblemDetails(
                StatusCodes.Status400BadRequest,
                "Invalid URL",
                e.Message,
                "invalid-url",
                baseDetails),

            // Domain layer exceptions - General
            InvalidEntityIdException e => CreateProblemDetails(
                StatusCodes.Status400BadRequest,
                "Invalid Entity ID",
                e.Message,
                "invalid-id",
                baseDetails),

            EntityNotFoundException e => CreateProblemDetails(
                StatusCodes.Status404NotFound,
                "Entity Not Found",
                e.Message,
                "entity-not-found",
                baseDetails),

            DomainException e => CreateProblemDetails(
                StatusCodes.Status400BadRequest,
                "Domain Rule Violation",
                e.Message,
                "domain-error",
                baseDetails),

            CustomException e => CreateProblemDetails(
                StatusCodes.Status400BadRequest,
                "Business Rule Violation",
                e.Message,
                "business-error",
                baseDetails),

            // System exceptions
            UnauthorizedAccessException => CreateProblemDetails(
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                "You do not have permission to access this resource",
                "unauthorized",
                baseDetails),

            ArgumentException e => CreateProblemDetails(
                StatusCodes.Status400BadRequest,
                "Invalid Argument",
                e.Message,
                "invalid-argument",
                baseDetails),

            InvalidOperationException e => CreateProblemDetails(
                StatusCodes.Status409Conflict,
                "Invalid Operation",
                e.Message,
                "invalid-operation",
                baseDetails),

            TimeoutException => CreateProblemDetails(
                StatusCodes.Status408RequestTimeout,
                "Request Timeout",
                "The request took too long to complete",
                "timeout",
                baseDetails),

            // Default
            _ => CreateProblemDetails(
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                // Temporarily show detailed errors in production - REMOVE BEFORE CUSTOMER RELEASE
                exception.Message, // was: _environment.IsDevelopment() ? exception.Message : "An unexpected error occurred",
                "internal-error",
                baseDetails)
        };
    }

    private ProblemDetails CreateBaseProblemDetails(HttpContext context)
    {
        var problemDetails = new ProblemDetails
        {
            Instance = context.Request.Path,
            Extensions = 
            {
                ["traceId"] = context.TraceIdentifier,
                ["timestamp"] = DateTimeOffset.UtcNow
            }
        };

        // Temporarily show extra details in production - REMOVE BEFORE CUSTOMER RELEASE
        // if (_environment.IsDevelopment())
        // {
            problemDetails.Extensions["requestId"] = context.Connection.Id;
            problemDetails.Extensions["method"] = context.Request.Method;
        // }

        return problemDetails;
    }

    private (int statusCode, ProblemDetails problemDetails) CreateProblemDetails(
        int statusCode,
        string title,
        string detail,
        string errorCode,
        ProblemDetails baseDetails)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = $"https://api.sledzspecke.com/errors/{errorCode}",
            Instance = baseDetails.Instance
        };

        // Copy extensions from base
        foreach (var extension in baseDetails.Extensions)
        {
            problemDetails.Extensions[extension.Key] = extension.Value;
        }

        // Add error code for client-side handling
        problemDetails.Extensions["errorCode"] = errorCode;

        return (statusCode, problemDetails);
    }
}
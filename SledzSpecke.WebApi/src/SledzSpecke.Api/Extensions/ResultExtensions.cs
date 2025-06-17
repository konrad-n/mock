using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application;

namespace SledzSpecke.Api.Extensions;

/// <summary>
/// Extension methods for converting Result patterns to ActionResults
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Converts a Result<T> to an appropriate IActionResult
    /// </summary>
    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        return result.Match(
            onSuccess: value => new OkObjectResult(value),
            onFailure: (error, code) => ToErrorResult(error, code)
        );
    }

    /// <summary>
    /// Converts a Result to an appropriate IActionResult
    /// </summary>
    public static IActionResult ToActionResult(this Result result)
    {
        return result.IsSuccess
            ? new OkResult()
            : ToErrorResult(result.Error, result.ErrorCode);
    }

    /// <summary>
    /// Converts a Result<T> to an appropriate IActionResult with a custom success handler
    /// </summary>
    public static IActionResult ToActionResult<T, TResponse>(
        this Result<T> result,
        Func<T, TResponse> mapper)
    {
        return result.Match(
            onSuccess: value => new OkObjectResult(mapper(value)),
            onFailure: (error, code) => ToErrorResult(error, code)
        );
    }

    /// <summary>
    /// Converts a Result<T> to a CreatedAtActionResult for resource creation endpoints
    /// </summary>
    public static IActionResult ToCreatedResult<T>(
        this Result<T> result,
        string actionName,
        string controllerName,
        Func<T, object> routeValuesFactory)
    {
        return result.Match(
            onSuccess: value => new CreatedAtActionResult(
                actionName,
                controllerName,
                routeValuesFactory(value),
                value),
            onFailure: (error, code) => ToErrorResult(error, code)
        );
    }

    /// <summary>
    /// Converts a Result<T> to a CreatedAtRouteResult
    /// </summary>
    public static IActionResult ToCreatedAtRouteResult<T>(
        this Result<T> result,
        string routeName,
        Func<T, object> routeValuesFactory)
    {
        return result.Match(
            onSuccess: value => new CreatedAtRouteResult(
                routeName,
                routeValuesFactory(value),
                value),
            onFailure: (error, code) => ToErrorResult(error, code)
        );
    }

    /// <summary>
    /// Converts a Result to a NoContentResult for operations that don't return data
    /// </summary>
    public static IActionResult ToNoContentResult(this Result result)
    {
        return result.IsSuccess
            ? new NoContentResult()
            : ToErrorResult(result.Error, result.ErrorCode);
    }

    /// <summary>
    /// Async version of ToActionResult
    /// </summary>
    public static async Task<IActionResult> ToActionResultAsync<T>(this Task<Result<T>> resultTask)
    {
        var result = await resultTask;
        return result.ToActionResult();
    }

    /// <summary>
    /// Async version of ToActionResult with mapper
    /// </summary>
    public static async Task<IActionResult> ToActionResultAsync<T, TResponse>(
        this Task<Result<T>> resultTask,
        Func<T, TResponse> mapper)
    {
        var result = await resultTask;
        return result.ToActionResult(mapper);
    }

    /// <summary>
    /// Converts error information to appropriate HTTP status codes
    /// </summary>
    private static IActionResult ToErrorResult(string error, string? errorCode)
    {
        var errorResponse = new
        {
            error,
            errorCode,
            timestamp = DateTime.UtcNow
        };

        return errorCode switch
        {
            ErrorCodes.NOT_FOUND => new NotFoundObjectResult(errorResponse),
            ErrorCodes.UNAUTHORIZED => new UnauthorizedObjectResult(errorResponse),
            ErrorCodes.FORBIDDEN => new ObjectResult(errorResponse) { StatusCode = 403 },
            ErrorCodes.CONFLICT => new ConflictObjectResult(errorResponse),
            ErrorCodes.VALIDATION_ERROR => new BadRequestObjectResult(errorResponse),
            ErrorCodes.DOMAIN_ERROR => new BadRequestObjectResult(errorResponse),
            ErrorCodes.BUSINESS_RULE_VIOLATION => new BadRequestObjectResult(errorResponse),
            ErrorCodes.EXTERNAL_SERVICE_ERROR => new ObjectResult(errorResponse) { StatusCode = 503 },
            ErrorCodes.TIMEOUT => new ObjectResult(errorResponse) { StatusCode = 504 },
            ErrorCodes.INTERNAL_ERROR => new ObjectResult(errorResponse) { StatusCode = 500 },
            _ => new BadRequestObjectResult(errorResponse)
        };
    }
}

/// <summary>
/// Standard error codes used throughout the application
/// </summary>
public static class ErrorCodes
{
    public const string NOT_FOUND = "NOT_FOUND";
    public const string UNAUTHORIZED = "UNAUTHORIZED";
    public const string FORBIDDEN = "FORBIDDEN";
    public const string CONFLICT = "CONFLICT";
    public const string VALIDATION_ERROR = "VALIDATION_ERROR";
    public const string DOMAIN_ERROR = "DOMAIN_ERROR";
    public const string BUSINESS_RULE_VIOLATION = "BUSINESS_RULE_VIOLATION";
    public const string EXTERNAL_SERVICE_ERROR = "EXTERNAL_SERVICE_ERROR";
    public const string TIMEOUT = "TIMEOUT";
    public const string INTERNAL_ERROR = "INTERNAL_ERROR";
    
    // Domain-specific error codes
    public const string USER_ALREADY_EXISTS = "USER_ALREADY_EXISTS";
    public const string INVALID_CREDENTIALS = "INVALID_CREDENTIALS";
    public const string SHIFT_OUTSIDE_INTERNSHIP = "SHIFT_OUTSIDE_INTERNSHIP";
    public const string WEEKLY_HOURS_EXCEEDED = "WEEKLY_HOURS_EXCEEDED";
    public const string MONTHLY_HOURS_INSUFFICIENT = "MONTHLY_HOURS_INSUFFICIENT";
    public const string DUPLICATE_PROCEDURE = "DUPLICATE_PROCEDURE";
    public const string INVALID_MODULE_PROGRESSION = "INVALID_MODULE_PROGRESSION";
    public const string SPECIALIZATION_NOT_ACTIVE = "SPECIALIZATION_NOT_ACTIVE";
    public const string COURSE_NOT_APPROVED = "COURSE_NOT_APPROVED";
    public const string INVALID_SMK_VERSION = "INVALID_SMK_VERSION";
}
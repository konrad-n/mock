using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Constants;

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

    #region Minimal API Extensions

    /// <summary>
    /// Converts a Result to an appropriate IResult for minimal APIs
    /// </summary>
    public static IResult ToApiResult<T>(this Result<T> result, string? resourceLocation = null)
    {
        if (result.IsSuccess)
        {
            return resourceLocation is not null
                ? Results.Created(resourceLocation, new { value = result.Value })
                : Results.Ok(result.Value);
        }

        return ToApiError(result);
    }
    
    /// <summary>
    /// Converts a Result (without value) to an appropriate IResult
    /// </summary>
    public static IResult ToApiResult(this Result result)
    {
        return result.IsSuccess
            ? Results.NoContent()
            : ToApiError(result);
    }
    
    private static IResult ToApiError<T>(Result<T> result)
    {
        if (result.ValidationErrors is not null)
        {
            return Results.ValidationProblem(result.ValidationErrors);
        }

        var problemDetails = new ProblemDetails
        {
            Title = "Request Failed",
            Detail = result.Error,
            Extensions = { ["errorCode"] = result.ErrorCode },
        };

        return result.ErrorCode switch
        {
            ErrorCodes.NOT_FOUND or 
            ErrorCodes.USER_NOT_FOUND or
            ErrorCodes.INTERNSHIP_NOT_FOUND or
            ErrorCodes.SHIFT_NOT_FOUND => Results.Problem(problemDetails with { Status = 404 }),
            
            ErrorCodes.ALREADY_EXISTS or
            ErrorCodes.SHIFT_ALREADY_EXISTS => Results.Problem(problemDetails with { Status = 409 }),
            
            ErrorCodes.UNAUTHORIZED or
            ErrorCodes.INVALID_CREDENTIALS => Results.Problem(problemDetails with { Status = 401 }),
            
            ErrorCodes.FORBIDDEN => Results.Problem(problemDetails with { Status = 403 }),
            
            ErrorCodes.VALIDATION_ERROR or
            ErrorCodes.INVALID_OPERATION => Results.Problem(problemDetails with { Status = 400 }),
            
            _ => Results.Problem(problemDetails with { Status = 500 }),
        };
    }
    
    private static IResult ToApiError(Result result)
        => ToApiError(Result<object>.Failure(result.Error!, result.ErrorCode!));

    #endregion
}


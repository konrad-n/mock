using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Api.Extensions;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Queries;
using SledzSpecke.Application.DTO;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Api.Endpoints;

public static class ModuleEndpoints
{
    public static void MapModuleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v2/modules")
            .RequireAuthorization()
            .WithOpenApi()
            .WithTags("Modules V2");

        group.MapGet("/specialization/{specializationId:int}", GetModulesBySpecialization)
            .WithName("GetModulesBySpecializationV2")
            .WithSummary("Get all modules for a specialization")
            .Produces<IEnumerable<ModuleDto>>();

        group.MapGet("/{id:int}/progress", GetModuleProgress)
            .WithName("GetModuleProgressV2")
            .WithSummary("Get progress for a specific module")
            .Produces<ModuleProgressDto>();

        group.MapGet("/", GetUserModules)
            .WithName("GetUserModulesV2")
            .WithSummary("Get all modules for current user")
            .Produces<IEnumerable<ModuleWithProgressDto>>();

        group.MapGet("/{id:int}/requirements", GetModuleRequirements)
            .WithName("GetModuleRequirementsV2")
            .WithSummary("Get requirements for a specific module")
            .Produces<ModuleRequirementsDto>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> GetModulesBySpecialization(
        [FromRoute] int specializationId,
        [FromServices] IResultQueryHandler<GetModulesBySpecialization, IEnumerable<ModuleDto>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetModulesBySpecialization(specializationId);
        var result = await handler.HandleAsync(query, cancellationToken);
        return result.ToApiResult();
    }

    private static async Task<IResult> GetModuleProgress(
        [FromRoute] int id,
        [FromServices] IResultQueryHandler<GetModuleProgress, SpecializationStatisticsDto> handler,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId(httpContext);
        var query = new GetModuleProgress(id, userId);
        var result = await handler.HandleAsync(query, cancellationToken);
        return result.ToApiResult();
    }

    private static async Task<IResult> GetUserModules(
        [FromQuery] bool? includeCompleted,
        [FromServices] IResultQueryHandler<GetUserModules, IEnumerable<ModuleWithProgressDto>> handler,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId(httpContext);
        var query = new GetUserModules(userId, includeCompleted ?? true);
        var result = await handler.HandleAsync(query, cancellationToken);
        return result.ToApiResult();
    }

    private static async Task<IResult> GetModuleRequirements(
        [FromRoute] int id,
        [FromServices] IResultQueryHandler<GetModuleRequirements, ModuleRequirementsDto> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetModuleRequirements(id);
        var result = await handler.HandleAsync(query, cancellationToken);
        return result.ToApiResult();
    }

    private static int GetCurrentUserId(HttpContext context)
    {
        var userIdClaim = context.User.FindFirst("UserId")?.Value 
            ?? context.User.FindFirst("sub")?.Value;

        if (int.TryParse(userIdClaim, out var userId))
            return userId;

        throw new UnauthorizedAccessException("User ID not found in claims");
    }
}

public record ModuleProgressDto(
    int ModuleId,
    string ModuleName,
    int CompletedInternships,
    int TotalInternships,
    int CompletedShifts,
    int RequiredShifts,
    int CompletedProcedures,
    int RequiredProcedures,
    double CompletionPercentage,
    DateTime? StartDate,
    DateTime? EndDate,
    bool IsCompleted);



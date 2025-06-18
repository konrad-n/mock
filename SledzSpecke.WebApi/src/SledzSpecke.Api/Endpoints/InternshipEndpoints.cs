using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Api.Extensions;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Api.Endpoints;

public static class InternshipEndpoints
{
    public static void MapInternshipEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v2/internships")
            .RequireAuthorization()
            .WithOpenApi()
            .WithTags("Internships V2");

        group.MapPost("/", CreateInternship)
            .WithName("CreateInternshipV2")
            .WithSummary("Create a new internship")
            .Produces<InternshipCreatedResponse>(StatusCodes.Status201Created);

        group.MapGet("/", GetInternships)
            .WithName("GetInternshipsV2")
            .WithSummary("Get all internships for current user")
            .Produces<IEnumerable<InternshipDto>>();

        group.MapGet("/{id:int}", GetInternship)
            .WithName("GetInternshipV2")
            .WithSummary("Get specific internship")
            .Produces<InternshipDto>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{id:int}/complete", CompleteInternship)
            .WithName("CompleteInternshipV2")
            .WithSummary("Mark internship as completed")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/{id:int}/progress", GetInternshipProgress)
            .WithName("GetInternshipProgressV2")
            .WithSummary("Get internship progress including shifts and procedures")
            .Produces<SledzSpecke.Application.Queries.InternshipProgressDto>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> CreateInternship(
        [FromBody] CreateInternshipRequest request,
        [FromServices] IResultCommandHandler<CreateInternship, int> handler,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId(httpContext);
        // For now, we'll hardcode a specialization ID - in production this would come from the request or user context
        var specializationId = 1; // TODO: Get from user's active specialization
        
        // Example data from SMK PDFs
        var command = new CreateInternship(
            SpecializationId: specializationId,
            Name: request.Name ?? "Staż podstawowy w zakresie chorób wewnętrznych",
            InstitutionName: request.InstitutionName,
            DepartmentName: request.DepartmentName ?? "Oddział Chorób Wewnętrznych",
            StartDate: request.StartDate,
            EndDate: request.EndDate,
            PlannedWeeks: request.PlannedWeeks ?? 67, // From PDF
            PlannedDays: request.PlannedDays ?? 335, // From PDF
            SupervisorName: null,
            SupervisorPwz: null,
            ModuleId: request.ModuleId
        );

        var result = await handler.HandleAsync(command, cancellationToken);
        return result.ToApiResult($"/api/v2/internships/{result.Value}");
    }

    private static async Task<IResult> GetInternships(
        [FromQuery] int? moduleId,
        [FromQuery] bool? includeCompleted,
        [FromServices] IResultQueryHandler<GetUserInternships, IEnumerable<InternshipDto>> handler,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId(httpContext);
        var query = new GetUserInternships(userId, moduleId, includeCompleted ?? true);
        var result = await handler.HandleAsync(query, cancellationToken);
        return result.ToApiResult();
    }

    private static async Task<IResult> GetInternship(
        [FromRoute] int id,
        [FromServices] IResultQueryHandler<GetInternshipById, InternshipDto> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetInternshipById(id);
        var result = await handler.HandleAsync(query, cancellationToken);
        return result.ToApiResult();
    }

    private static async Task<IResult> CompleteInternship(
        [FromRoute] int id,
        [FromServices] IResultCommandHandler<MarkInternshipAsCompleted> handler,
        CancellationToken cancellationToken)
    {
        var command = new MarkInternshipAsCompleted(id);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.ToApiResult();
    }

    private static async Task<IResult> GetInternshipProgress(
        [FromRoute] int id,
        [FromServices] IResultQueryHandler<GetInternshipProgress, SledzSpecke.Application.Queries.InternshipProgressDto> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetInternshipProgress(id);
        var result = await handler.HandleAsync(query, cancellationToken);
        return result.ToApiResult();
    }

    private static int GetCurrentUserId(HttpContext context)
    {
        var userIdClaim = context.User.FindFirst("UserId")?.Value;
        if (int.TryParse(userIdClaim, out var userId))
            return userId;

        throw new UnauthorizedAccessException("User ID not found");
    }
}

public record CreateInternshipRequest(
    int ModuleId,
    string? Name,
    string InstitutionName,
    string? DepartmentName,
    DateTime StartDate,
    DateTime EndDate,
    int? PlannedWeeks,
    int? PlannedDays);

public record InternshipCreatedResponse(int Id);


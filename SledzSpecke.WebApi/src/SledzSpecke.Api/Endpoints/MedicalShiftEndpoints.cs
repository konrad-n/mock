using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Api.Extensions;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Features.MedicalShifts.Commands.AddMedicalShift;
using SledzSpecke.Application.Features.MedicalShifts.Commands.UpdateMedicalShift;
using SledzSpecke.Application.Features.MedicalShifts.Commands.DeleteMedicalShift;
using SledzSpecke.Application.Features.MedicalShifts.DTOs;
using SledzSpecke.Application.Features.MedicalShifts.Queries.GetMedicalShifts;
using SledzSpecke.Application.Features.MedicalShifts.Queries.GetMedicalShiftById;
using SledzSpecke.Application.Features.MedicalShifts.Queries.GetMedicalShiftStatistics;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Api.Endpoints;

public static class MedicalShiftEndpoints
{
    public static void MapMedicalShiftEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v2/medical-shifts")
            .RequireAuthorization()
            .WithOpenApi()
            .WithTags("Medical Shifts V2");

        // POST: Add medical shift
        group.MapPost("/", AddMedicalShift)
            .WithName("AddMedicalShiftV2")
            .WithSummary("Add a new medical shift")
            .Produces<MedicalShiftCreatedResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        // GET: Get medical shifts with filtering
        group.MapGet("/", GetMedicalShifts)
            .WithName("GetMedicalShiftsV2")
            .WithSummary("Get medical shifts with optional filtering")
            .Produces<IEnumerable<MedicalShiftDto>>();

        // GET: Get single medical shift
        group.MapGet("/{id:int}", GetMedicalShift)
            .WithName("GetMedicalShiftV2")
            .WithSummary("Get a specific medical shift")
            .Produces<MedicalShiftDto>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        // PUT: Update medical shift
        group.MapPut("/{id:int}", UpdateMedicalShift)
            .WithName("UpdateMedicalShiftV2")
            .WithSummary("Update a medical shift")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);

        // DELETE: Delete medical shift
        group.MapDelete("/{id:int}", DeleteMedicalShift)
            .WithName("DeleteMedicalShiftV2")
            .WithSummary("Delete a medical shift")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);

        // GET: Statistics
        group.MapGet("/statistics", GetMedicalShiftStatistics)
            .WithName("GetMedicalShiftStatisticsV2")
            .WithSummary("Get medical shift statistics")
            .Produces<MedicalShiftStatisticsDto>();
    }

    private static async Task<IResult> AddMedicalShift(
        [FromBody] AddMedicalShiftRequest request,
        [FromServices] IResultCommandHandler<AddMedicalShift, int> handler,
        CancellationToken cancellationToken)
    {
        var command = new AddMedicalShift(
            request.InternshipId,
            request.Date,
            request.Hours,
            request.Minutes,
            request.Location,
            request.Year);

        var result = await handler.HandleAsync(command, cancellationToken);
        return result.ToApiResult($"/api/v2/medical-shifts/{result.Value}");
    }

    private static async Task<IResult> GetMedicalShifts(
        [FromQuery] int? userId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int? internshipId,
        [FromServices] IResultQueryHandler<GetMedicalShifts, IEnumerable<MedicalShiftDto>> handler,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId(httpContext);
        var targetUserId = userId ?? currentUserId;

        var query = new GetMedicalShifts
        {
            UserId = targetUserId,
            FromDate = fromDate ?? DateTime.Today.AddMonths(-1),
            ToDate = toDate ?? DateTime.Today,
            InternshipId = internshipId,
        };

        var result = await handler.HandleAsync(query, cancellationToken);
        return result.ToApiResult();
    }

    private static async Task<IResult> GetMedicalShift(
        [FromRoute] int id,
        [FromServices] IResultQueryHandler<GetMedicalShiftById, MedicalShiftDto> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetMedicalShiftById(id);
        var result = await handler.HandleAsync(query, cancellationToken);
        return result.ToApiResult();
    }

    private static async Task<IResult> UpdateMedicalShift(
        [FromRoute] int id,
        [FromBody] UpdateMedicalShiftRequest request,
        [FromServices] IResultCommandHandler<UpdateMedicalShift> handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateMedicalShift(
            id,
            request.Date,
            request.Hours,
            request.Minutes,
            request.Location);

        var result = await handler.HandleAsync(command, cancellationToken);
        return result.ToApiResult();
    }

    private static async Task<IResult> DeleteMedicalShift(
        [FromRoute] int id,
        [FromServices] IResultCommandHandler<DeleteMedicalShift> handler,
        CancellationToken cancellationToken)
    {
        var command = new DeleteMedicalShift(id);
        var result = await handler.HandleAsync(command, cancellationToken);
        return result.ToApiResult();
    }

    private static async Task<IResult> GetMedicalShiftStatistics(
        [FromQuery] int? userId,
        [FromQuery] int? year,
        [FromQuery] int? month,
        [FromServices] IResultQueryHandler<GetMedicalShiftStatistics, MedicalShiftStatisticsDto> handler,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId(httpContext);

        var query = new GetMedicalShiftStatistics
        {
            UserId = userId ?? currentUserId,
            Year = year ?? DateTime.Today.Year,
            Month = month
        };

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

// Request/Response DTOs
public record AddMedicalShiftRequest(
    int InternshipId,
    DateTime Date,
    int Hours,
    int Minutes,
    string Location,
    int Year);

public record UpdateMedicalShiftRequest(
    DateTime? Date,
    int? Hours,
    int? Minutes,
    string? Location);

public record MedicalShiftCreatedResponse(int Id);
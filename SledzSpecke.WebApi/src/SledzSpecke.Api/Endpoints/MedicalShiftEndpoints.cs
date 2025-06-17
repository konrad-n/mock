using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Api.Extensions;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;
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

        group.MapPost("/", AddMedicalShift)
            .WithName("AddMedicalShiftV2")
            .WithSummary("Add a new medical shift")
            .WithDescription("Creates a new medical shift for the specified internship")
            .Produces<MedicalShiftCreatedResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        // TODO: Uncomment when query handlers are updated
        // group.MapGet("/", GetMedicalShifts)
        //     .WithName("GetMedicalShiftsV2")
        //     .WithSummary("Get medical shifts")
        //     .WithDescription("Retrieves medical shifts for a user within a date range")
        //     .Produces<IEnumerable<MedicalShiftDto>>();

        // group.MapGet("/{id:int}", GetMedicalShift)
        //     .WithName("GetMedicalShiftV2")
        //     .WithSummary("Get a specific medical shift")
        //     .Produces<MedicalShiftDto>()
        //     .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{id:int}", UpdateMedicalShift)
            .WithName("UpdateMedicalShiftV2")
            .WithSummary("Update a medical shift")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:int}", DeleteMedicalShift)
            .WithName("DeleteMedicalShiftV2")
            .WithSummary("Delete a medical shift")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> AddMedicalShift(
        [FromBody] AddMedicalShiftRequest request,
        [FromServices] IResultCommandHandler<AddMedicalShift, int> handler,
        HttpContext httpContext,
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
        
        if (result.IsSuccess)
        {
            var location = $"{httpContext.Request.Path}/{result.Value}";
            return Results.Created(location, new MedicalShiftCreatedResponse(result.Value));
        }
        
        return result.ToApiResult();
    }

    // TODO: Uncomment when query handlers are updated
    // private static async Task<IResult> GetMedicalShifts(
    //     [FromQuery] int? userId,
    //     [FromQuery] DateTime? fromDate,
    //     [FromQuery] DateTime? toDate,
    //     [FromQuery] int? internshipId,
    //     [FromServices] IResultQueryHandler<GetMedicalShifts, IEnumerable<MedicalShiftDto>> handler,
    //     HttpContext httpContext,
    //     CancellationToken cancellationToken)
    // {
    //     var currentUserId = GetCurrentUserId(httpContext);
    //     var targetUserId = userId ?? currentUserId;
    //     
    //     var query = new GetMedicalShifts
    //     {
    //         UserId = targetUserId,
    //         FromDate = fromDate ?? DateTime.Today.AddMonths(-1),
    //         ToDate = toDate ?? DateTime.Today,
    //         InternshipId = internshipId,
    //     };

    //     var result = await handler.HandleAsync(query, cancellationToken);
    //     return result.ToApiResult();
    // }

    // private static async Task<IResult> GetMedicalShift(
    //     [FromRoute] int id,
    //     [FromServices] IResultQueryHandler<GetMedicalShiftById, MedicalShiftDto> handler,
    //     CancellationToken cancellationToken)
    // {
    //     var query = new GetMedicalShiftById(id);
    //     var result = await handler.HandleAsync(query, cancellationToken);
    //     return result.ToApiResult();
    // }

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
using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.Queries;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AbsencesController : BaseController
{
    private readonly ICommandHandler<CreateAbsence> _createAbsenceHandler;
    private readonly IQueryHandler<GetUserAbsences, IEnumerable<AbsenceDto>> _getUserAbsencesHandler;
    private readonly ICommandHandler<ApproveAbsence> _approveAbsenceHandler;
    private readonly ICommandHandler<UpdateAbsence> _updateAbsenceHandler;
    private readonly ICommandHandler<DeleteAbsence> _deleteAbsenceHandler;

    public AbsencesController(
        ICommandHandler<CreateAbsence> createAbsenceHandler,
        IQueryHandler<GetUserAbsences, IEnumerable<AbsenceDto>> getUserAbsencesHandler,
        ICommandHandler<ApproveAbsence> approveAbsenceHandler,
        ICommandHandler<UpdateAbsence> updateAbsenceHandler,
        ICommandHandler<DeleteAbsence> deleteAbsenceHandler) : base()
    {
        _createAbsenceHandler = createAbsenceHandler;
        _getUserAbsencesHandler = getUserAbsencesHandler;
        _approveAbsenceHandler = approveAbsenceHandler;
        _updateAbsenceHandler = updateAbsenceHandler;
        _deleteAbsenceHandler = deleteAbsenceHandler;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAbsence([FromBody] CreateAbsenceRequest request)
    {
        var command = new CreateAbsence(
            new SpecializationId(request.SpecializationId),
            new UserId(request.UserId),
            (AbsenceType)request.Type,
            request.StartDate,
            request.EndDate,
            request.Description);

        await _createAbsenceHandler.HandleAsync(command);
        return Ok();
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserAbsences(int userId, [FromQuery] int? specializationId = null)
    {
        var query = new GetUserAbsences(userId, specializationId);
        var absences = await _getUserAbsencesHandler.HandleAsync(query);
        return Ok(absences);
    }

    [HttpPut("{id}/approve")]
    public async Task<IActionResult> ApproveAbsence(Guid id, [FromBody] ApproveAbsenceRequest request)
    {
        var command = new ApproveAbsence(new AbsenceId(id), request.ApprovedBy);
        await _approveAbsenceHandler.HandleAsync(command);
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAbsence(Guid id, [FromBody] UpdateAbsenceRequest request)
    {
        var command = new UpdateAbsence(
            new AbsenceId(id),
            (AbsenceType)request.Type,
            request.StartDate,
            request.EndDate,
            request.Description);

        await _updateAbsenceHandler.HandleAsync(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAbsence(Guid id)
    {
        var command = new DeleteAbsence(new AbsenceId(id));
        await _deleteAbsenceHandler.HandleAsync(command);
        return NoContent();
    }
}

public record CreateAbsenceRequest(
    int SpecializationId,
    int UserId,
    int Type,
    DateTime StartDate,
    DateTime EndDate,
    string? Description = null);

public record ApproveAbsenceRequest(int ApprovedBy);

public record UpdateAbsenceRequest(
    int Type,
    DateTime StartDate,
    DateTime EndDate,
    string? Description);
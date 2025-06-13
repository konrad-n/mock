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

    public AbsencesController(
        ICommandHandler<CreateAbsence> createAbsenceHandler,
        IQueryHandler<GetUserAbsences, IEnumerable<AbsenceDto>> getUserAbsencesHandler) : base()
    {
        _createAbsenceHandler = createAbsenceHandler;
        _getUserAbsencesHandler = getUserAbsencesHandler;
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
        // This would need an approve absence command and handler
        return Ok();
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;

namespace SledzSpecke.Api.Controllers;

[Authorize]
public class ProceduresController : BaseController
{
    private readonly ICommandHandler<AddProcedure, int> _addProcedureHandler;
    private readonly ICommandHandler<UpdateProcedure> _updateProcedureHandler;
    private readonly ICommandHandler<DeleteProcedure> _deleteProcedureHandler;
    private readonly IQueryHandler<GetUserProcedures, IEnumerable<ProcedureDto>> _getUserProceduresHandler;
    private readonly IQueryHandler<GetProcedureById, ProcedureDto> _getProcedureByIdHandler;
    private readonly IUserContextService _userContextService;

    public ProceduresController(
        ICommandHandler<AddProcedure, int> addProcedureHandler,
        ICommandHandler<UpdateProcedure> updateProcedureHandler,
        ICommandHandler<DeleteProcedure> deleteProcedureHandler,
        IQueryHandler<GetUserProcedures, IEnumerable<ProcedureDto>> getUserProceduresHandler,
        IQueryHandler<GetProcedureById, ProcedureDto> getProcedureByIdHandler,
        IUserContextService userContextService)
    {
        _addProcedureHandler = addProcedureHandler;
        _updateProcedureHandler = updateProcedureHandler;
        _deleteProcedureHandler = deleteProcedureHandler;
        _getUserProceduresHandler = getUserProceduresHandler;
        _getProcedureByIdHandler = getProcedureByIdHandler;
        _userContextService = userContextService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProcedureDto>>> GetUserProcedures(
        [FromQuery] int? internshipId = null,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var userId = _userContextService.GetUserId();
        var query = new GetUserProcedures(userId, internshipId, status, startDate, endDate);
        return await HandleAsync(query, _getUserProceduresHandler);
    }

    [HttpGet("{procedureId:int}")]
    public async Task<ActionResult<ProcedureDto>> GetProcedureById(int procedureId)
    {
        var query = new GetProcedureById(procedureId);
        return await HandleAsync(query, _getProcedureByIdHandler);
    }

    [HttpPost]
    public async Task<ActionResult<int>> AddProcedure([FromBody] AddProcedureRequest request)
    {
        var command = new AddProcedure(
            request.InternshipId,
            request.Date,
            request.Year,
            request.Code,
            request.Location,
            request.Status,
            request.OperatorCode,
            request.PerformingPerson,
            request.PatientInitials,
            request.PatientGender);

        var procedureId = await _addProcedureHandler.HandleAsync(command);
        return CreatedAtAction(nameof(GetProcedureById), new { procedureId }, procedureId);
    }

    [HttpPut("{procedureId:int}")]
    public async Task<ActionResult> UpdateProcedure(int procedureId, [FromBody] UpdateProcedureRequest request)
    {
        var command = new UpdateProcedure(
            procedureId,
            request.Date,
            request.Code,
            request.Location,
            request.Status,
            request.OperatorCode,
            request.PerformingPerson,
            request.PatientInitials,
            request.PatientGender);
        
        return await HandleAsync(command, _updateProcedureHandler);
    }

    [HttpDelete("{procedureId:int}")]
    public async Task<ActionResult> DeleteProcedure(int procedureId)
    {
        var command = new DeleteProcedure(procedureId);
        return await HandleAsync(command, _deleteProcedureHandler);
    }
}

public class AddProcedureRequest
{
    public int InternshipId { get; set; }
    public DateTime Date { get; set; }
    public int Year { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? OperatorCode { get; set; }
    public string? PerformingPerson { get; set; }
    public string? PatientInitials { get; set; }
    public char? PatientGender { get; set; }
}

public class UpdateProcedureRequest
{
    public DateTime? Date { get; set; }
    public string? Code { get; set; }
    public string? Location { get; set; }
    public string? Status { get; set; }
    public string? OperatorCode { get; set; }
    public string? PerformingPerson { get; set; }
    public string? PatientInitials { get; set; }
    public char? PatientGender { get; set; }
}
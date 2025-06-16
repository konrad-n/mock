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
    private readonly IQueryHandler<GetProcedureStatistics, ProcedureSummaryDto> _getProcedureStatisticsHandler;
    private readonly IUserContextService _userContextService;

    public ProceduresController(
        ICommandHandler<AddProcedure, int> addProcedureHandler,
        ICommandHandler<UpdateProcedure> updateProcedureHandler,
        ICommandHandler<DeleteProcedure> deleteProcedureHandler,
        IQueryHandler<GetUserProcedures, IEnumerable<ProcedureDto>> getUserProceduresHandler,
        IQueryHandler<GetProcedureById, ProcedureDto> getProcedureByIdHandler,
        IQueryHandler<GetProcedureStatistics, ProcedureSummaryDto> getProcedureStatisticsHandler,
        IUserContextService userContextService)
    {
        _addProcedureHandler = addProcedureHandler;
        _updateProcedureHandler = updateProcedureHandler;
        _deleteProcedureHandler = deleteProcedureHandler;
        _getUserProceduresHandler = getUserProceduresHandler;
        _getProcedureByIdHandler = getProcedureByIdHandler;
        _getProcedureStatisticsHandler = getProcedureStatisticsHandler;
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
        try
        {
            // Log the incoming request for debugging
            Console.WriteLine($"[DEBUG] AddProcedure request: InternshipId={request.InternshipId}, Code={request.Code}, Status={request.Status}");

            var command = new AddProcedure(
                request.InternshipId,
                request.Date,
                request.Year,
                request.Code,
                request.Code, // Name (using Code as Name for now)
                request.Location,
                request.Status,
                request.OperatorCode ?? "CodeA", // ExecutionType (default to CodeA)
                request.PerformingPerson ?? "Unknown", // SupervisorName
                null, // SupervisorPwz
                request.PerformingPerson,
                null, // PatientInfo
                request.PatientInitials,
                request.PatientGender,
                // Old SMK specific fields
                request.ProcedureRequirementId,
                request.ProcedureGroup,
                request.AssistantData,
                request.InternshipName,
                // New SMK specific fields
                request.ModuleId,
                request.ProcedureName,
                request.CountA,
                request.CountB,
                request.Supervisor,
                request.Institution,
                request.Comments);

            var procedureId = await _addProcedureHandler.HandleAsync(command);
            Console.WriteLine($"[DEBUG] Procedure created successfully with ID: {procedureId}");
            return CreatedAtAction(nameof(GetProcedureById), new { procedureId }, procedureId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] AddProcedure failed: {ex.GetType().Name}: {ex.Message}");
            Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
            throw; // Re-throw to let the framework handle it
        }
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
            request.OperatorCode, // ExecutionType
            request.PerformingPerson,
            null, // PatientInfo
            request.PatientInitials,
            request.PatientGender,
            // Old SMK specific fields
            request.ProcedureRequirementId,
            request.ProcedureGroup,
            request.AssistantData,
            request.InternshipName,
            // New SMK specific fields
            request.CountA,
            request.CountB,
            request.Supervisor,
            request.Institution,
            request.Comments);

        return await HandleAsync(command, _updateProcedureHandler);
    }

    [HttpDelete("{procedureId:int}")]
    public async Task<ActionResult> DeleteProcedure(int procedureId)
    {
        var command = new DeleteProcedure(procedureId);
        return await HandleAsync(command, _deleteProcedureHandler);
    }

    [HttpGet("statistics")]
    public async Task<ActionResult<ProcedureSummaryDto>> GetProcedureStatistics(
        [FromQuery] int? moduleId = null,
        [FromQuery] int? procedureRequirementId = null)
    {
        var query = new GetProcedureStatistics(moduleId, procedureRequirementId);
        return await HandleAsync(query, _getProcedureStatisticsHandler);
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

    // Old SMK specific fields
    public int? ProcedureRequirementId { get; set; }
    public string? ProcedureGroup { get; set; }
    public string? AssistantData { get; set; }
    public string? InternshipName { get; set; }

    // New SMK specific fields
    public int? ModuleId { get; set; }
    public string? ProcedureName { get; set; }
    public int? CountA { get; set; }
    public int? CountB { get; set; }
    public string? Supervisor { get; set; }
    public string? Institution { get; set; }
    public string? Comments { get; set; }
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

    // Old SMK specific fields
    public int? ProcedureRequirementId { get; set; }
    public string? ProcedureGroup { get; set; }
    public string? AssistantData { get; set; }
    public string? InternshipName { get; set; }

    // New SMK specific fields
    public int? CountA { get; set; }
    public int? CountB { get; set; }
    public string? Supervisor { get; set; }
    public string? Institution { get; set; }
    public string? Comments { get; set; }
}
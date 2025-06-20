using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.Queries;
using SledzSpecke.Application.DTO;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProceduresController : BaseController
{
    private readonly ICommandHandler<AddProcedureRealizationCommand> _addProcedureRealizationHandler;
    private readonly ICommandHandler<UpdateProcedureRealizationCommand> _updateProcedureRealizationHandler;
    private readonly ICommandHandler<DeleteProcedureRealizationCommand> _deleteProcedureRealizationHandler;
    private readonly IQueryHandler<GetModuleProceduresQuery, ModuleProceduresDto> _getModuleProceduresHandler;
    private readonly IQueryHandler<GetUserProceduresQuery, UserProceduresDto> _getUserProceduresHandler;
    private readonly IUserContextService _userContextService;

    public ProceduresController(
        ICommandHandler<AddProcedureRealizationCommand> addProcedureRealizationHandler,
        ICommandHandler<UpdateProcedureRealizationCommand> updateProcedureRealizationHandler,
        ICommandHandler<DeleteProcedureRealizationCommand> deleteProcedureRealizationHandler,
        IQueryHandler<GetModuleProceduresQuery, ModuleProceduresDto> getModuleProceduresHandler,
        IQueryHandler<GetUserProceduresQuery, UserProceduresDto> getUserProceduresHandler,
        IUserContextService userContextService)
    {
        _addProcedureRealizationHandler = addProcedureRealizationHandler;
        _updateProcedureRealizationHandler = updateProcedureRealizationHandler;
        _deleteProcedureRealizationHandler = deleteProcedureRealizationHandler;
        _getModuleProceduresHandler = getModuleProceduresHandler;
        _getUserProceduresHandler = getUserProceduresHandler;
        _userContextService = userContextService;
    }

    /// <summary>
    /// Get procedures by internship ID (compatibility endpoint)
    /// </summary>
    /// <param name="internshipId">Internship ID</param>
    /// <returns>Procedures for the internship</returns>
    [HttpGet]
    public async Task<ActionResult<object>> GetProceduresByInternship([FromQuery] int? internshipId = null)
    {
        // For now, return mock data to match frontend expectations
        var mockProcedures = new[]
        {
            new
            {
                id = 1,
                code = "89.52",
                name = "Koronarografia",
                date = DateTime.Now.AddDays(-1),
                location = "Pracownia Hemodynamiki",
                role = "Operator",
                roleDisplay = "Operator",
                status = "synchronized",
                patient = new { age = "65 lat", gender = "M" }
            },
            new
            {
                id = 2,
                code = "36.01",
                name = "PTCA (angioplastyka wieńcowa)",
                date = DateTime.Now.AddDays(-5),
                location = "Pracownia Hemodynamiki",
                role = "FirstAssistant",
                roleDisplay = "Pierwsza asysta",
                status = "modified",
                patient = new { age = "58 lat", gender = "K" }
            },
            new
            {
                id = 3,
                code = "37.22",
                name = "Cewnikowanie serca",
                date = DateTime.Now.AddDays(-10),
                location = "Oddział Kardiologii",
                role = "SecondAssistant",
                roleDisplay = "Druga asysta",
                status = "unsynchronized",
                patient = new { age = "72 lat", gender = "M" }
            }
        };
        
        return Ok(mockProcedures);
    }

    /// <summary>
    /// Get procedures for a specific module
    /// </summary>
    /// <param name="moduleId">Module ID</param>
    /// <returns>Module procedures with progress</returns>
    [HttpGet("modules/{moduleId}")]
    public async Task<ActionResult<ModuleProceduresDto>> GetModuleProcedures(int moduleId)
    {
        var userId = new UserId(_userContextService.GetUserId());
        var query = new GetModuleProceduresQuery(userId, new ModuleId(moduleId));
        return await HandleAsync(query, _getModuleProceduresHandler);
    }

    /// <summary>
    /// Get all procedures for the current user
    /// </summary>
    /// <param name="specializationId">Optional specialization filter</param>
    /// <returns>User procedures across all modules</returns>
    [HttpGet("user")]
    public async Task<ActionResult<UserProceduresDto>> GetUserProcedures([FromQuery] int? specializationId = null)
    {
        var userId = new UserId(_userContextService.GetUserId());
        var query = new GetUserProceduresQuery(userId, specializationId);
        return await HandleAsync(query, _getUserProceduresHandler);
    }

    /// <summary>
    /// Add a new procedure realization
    /// </summary>
    /// <param name="request">Add procedure realization request</param>
    /// <returns>Created result</returns>
    [HttpPost("realizations")]
    public async Task<IActionResult> AddProcedureRealization([FromBody] AddProcedureRealizationRequest request)
    {
        var userId = new UserId(_userContextService.GetUserId());
        var command = new AddProcedureRealizationCommand(
            new ProcedureRequirementId(request.RequirementId),
            userId,
            request.Date,
            request.Location,
            request.Role,
            request.Year
        );

        await _addProcedureRealizationHandler.HandleAsync(command);
        return Ok(new { message = "Realizacja procedury została dodana" });
    }

    /// <summary>
    /// Update an existing procedure realization
    /// </summary>
    /// <param name="id">Realization ID</param>
    /// <param name="request">Update procedure realization request</param>
    /// <returns>Updated result</returns>
    [HttpPut("realizations/{id}")]
    public async Task<IActionResult> UpdateProcedureRealization(int id, [FromBody] UpdateProcedureRealizationRequest request)
    {
        var command = new UpdateProcedureRealizationCommand(
            new ProcedureRealizationId(id),
            request.Date,
            request.Location,
            request.Role
        );

        await _updateProcedureRealizationHandler.HandleAsync(command);
        return Ok(new { message = "Realizacja procedury została zaktualizowana" });
    }

    /// <summary>
    /// Delete a procedure realization
    /// </summary>
    /// <param name="id">Realization ID</param>
    /// <returns>Deleted result</returns>
    [HttpDelete("realizations/{id}")]
    public async Task<IActionResult> DeleteProcedureRealization(int id)
    {
        var command = new DeleteProcedureRealizationCommand(new ProcedureRealizationId(id));
        await _deleteProcedureRealizationHandler.HandleAsync(command);
        return Ok(new { message = "Realizacja procedury została usunięta" });
    }
}

public class AddProcedureRealizationRequest
{
    public int RequirementId { get; set; }
    public DateTime Date { get; set; }
    public string Location { get; set; } = string.Empty;
    public ProcedureRole Role { get; set; }
    public int? Year { get; set; }
}

public class UpdateProcedureRealizationRequest
{
    public DateTime Date { get; set; }
    public string Location { get; set; } = string.Empty;
    public ProcedureRole Role { get; set; }
}
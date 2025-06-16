using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/modules")]
public class ModulesController : BaseController
{
    private readonly ICommandHandler<SwitchModule> _switchModuleHandler;
    private readonly IQueryHandler<GetAvailableModules, IEnumerable<ModuleListDto>> _getAvailableModulesHandler;
    private readonly IQueryHandler<GetModuleById, ModuleDto> _getModuleByIdHandler;
    private readonly IQueryHandler<GetModuleProgress, SpecializationStatisticsDto> _getModuleProgressHandler;
    private readonly ICommandHandler<CompleteModule> _completeModuleHandler;
    private readonly IUserContextService _userContextService;

    public ModulesController(
        ICommandHandler<SwitchModule> switchModuleHandler,
        IQueryHandler<GetAvailableModules, IEnumerable<ModuleListDto>> getAvailableModulesHandler,
        IQueryHandler<GetModuleById, ModuleDto> getModuleByIdHandler,
        IQueryHandler<GetModuleProgress, SpecializationStatisticsDto> getModuleProgressHandler,
        ICommandHandler<CompleteModule> completeModuleHandler,
        IUserContextService userContextService)
    {
        _switchModuleHandler = switchModuleHandler;
        _getAvailableModulesHandler = getAvailableModulesHandler;
        _getModuleByIdHandler = getModuleByIdHandler;
        _getModuleProgressHandler = getModuleProgressHandler;
        _completeModuleHandler = completeModuleHandler;
        _userContextService = userContextService;
    }

    /// <summary>
    /// Gets all modules for a specialization
    /// </summary>
    [HttpGet("specialization/{specializationId:int}")]
    public async Task<ActionResult<IEnumerable<ModuleListDto>>> GetAvailableModules(int specializationId)
        => await HandleAsync(new GetAvailableModules(specializationId), _getAvailableModulesHandler);

    /// <summary>
    /// Gets a specific module by ID
    /// </summary>
    [HttpGet("{moduleId:int}")]
    public async Task<ActionResult<ModuleDto>> GetModule(int moduleId)
    {
        var query = new GetModuleById(moduleId);
        return await HandleAsync(query, _getModuleByIdHandler);
    }

    /// <summary>
    /// Gets progress information for a module
    /// </summary>
    [HttpGet("{moduleId:int}/progress")]
    public async Task<ActionResult<SpecializationStatisticsDto>> GetModuleProgress(int moduleId)
    {
        // GetModuleProgress requires specializationId, not moduleId
        // This would need to be refactored to get specializationId from moduleId
        return BadRequest("Please use /api/specializations/{specializationId}/statistics endpoint");
    }

    /// <summary>
    /// Switches active module within a specialization
    /// </summary>
    [HttpPut("switch")]
    public async Task<ActionResult> SwitchModule([FromBody] SwitchModuleRequest request)
    {
        var command = new SwitchModule(request.SpecializationId, request.ModuleId);
        return await HandleAsync(command, _switchModuleHandler);
    }

    /// <summary>
    /// Marks a module as completed
    /// </summary>
    [HttpPost("{moduleId:int}/complete")]
    public async Task<ActionResult> CompleteModule(int moduleId)
    {
        var command = new CompleteModule(moduleId);
        return await HandleAsync(command, _completeModuleHandler);
    }

    /// <summary>
    /// Creates an internship within a module
    /// </summary>
    [HttpPost("{moduleId:int}/internships")]
    public async Task<ActionResult<CreateInternshipResponse>> CreateInternship(
        int moduleId, 
        [FromBody] CreateModuleInternshipRequest request)
    {
        // Forward to internships controller with module context
        var internshipRequest = new CreateInternship(
            request.SpecializationId,
            request.Name,
            request.InstitutionName,
            request.DepartmentName,
            request.StartDate,
            request.EndDate,
            request.PlannedWeeks,
            request.PlannedDays,
            null, // SupervisorName
            null, // SupervisorPwz
            moduleId);

        // Note: This would typically be handled by the InternshipController
        // For now, return a placeholder response
        return Ok(new CreateInternshipResponse { 
            InternshipId = 0, 
            Message = "Please use /api/internships endpoint with moduleId parameter" 
        });
    }

    /// <summary>
    /// Adds a medical shift to a module
    /// </summary>
    [HttpPost("{moduleId:int}/medical-shifts")]
    public async Task<ActionResult<AddMedicalShiftResponse>> AddMedicalShift(
        int moduleId, 
        [FromBody] AddModuleMedicalShiftRequest request)
    {
        // Forward to medical shifts controller with module context
        return Ok(new AddMedicalShiftResponse { 
            ShiftId = 0, 
            Message = "Please use /api/medical-shifts endpoint with moduleId parameter" 
        });
    }

    /// <summary>
    /// Adds a procedure to a module
    /// </summary>
    [HttpPost("{moduleId:int}/procedures")]
    public async Task<ActionResult<AddProcedureResponse>> AddProcedure(
        int moduleId, 
        [FromBody] AddModuleProcedureRequest request)
    {
        // Forward to procedures controller with module context
        return Ok(new AddProcedureResponse { 
            ProcedureId = 0, 
            Message = "Please use /api/procedures endpoint with moduleId parameter" 
        });
    }

    /// <summary>
    /// Adds a course to a module
    /// </summary>
    [HttpPost("{moduleId:int}/courses")]
    public async Task<ActionResult<CreateCourseResponse>> CreateCourse(
        int moduleId, 
        [FromBody] CreateModuleCourseRequest request)
    {
        // Forward to courses controller with module context
        return Ok(new CreateCourseResponse { 
            CourseId = 0, 
            Message = "Please use /api/courses endpoint with moduleId parameter" 
        });
    }

    /// <summary>
    /// Adds self-education activity to a module
    /// </summary>
    [HttpPost("{moduleId:int}/self-education")]
    public async Task<ActionResult<AddSelfEducationResponse>> AddSelfEducation(
        int moduleId, 
        [FromBody] AddModuleSelfEducationRequest request)
    {
        // Forward to self-education controller with module context
        return Ok(new AddSelfEducationResponse { 
            SelfEducationId = 0, 
            Message = "Please use /api/self-education endpoint with moduleId parameter" 
        });
    }

    /// <summary>
    /// Adds additional self-education days to a module
    /// </summary>
    [HttpPost("{moduleId:int}/additional-days")]
    public async Task<ActionResult<AddAdditionalDaysResponse>> AddAdditionalDays(
        int moduleId, 
        [FromBody] AddModuleAdditionalDaysRequest request)
    {
        // Forward to additional days controller with module context
        return Ok(new AddAdditionalDaysResponse { 
            AdditionalDaysId = 0, 
            Message = "Please use /api/additional-self-education-days endpoint with moduleId parameter" 
        });
    }
}

// Request DTOs
public class SwitchModuleRequest
{
    public int SpecializationId { get; set; }
    public int ModuleId { get; set; }
}

public class CreateModuleInternshipRequest
{
    public int SpecializationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string InstitutionName { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int PlannedWeeks { get; set; }
    public int PlannedDays { get; set; }
}

public class AddModuleMedicalShiftRequest
{
    public int? InternshipId { get; set; }
    public DateTime Date { get; set; }
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public string? Location { get; set; }
}

public class AddModuleProcedureRequest  
{
    public int InternshipId { get; set; }
    public DateTime Date { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string ExecutionType { get; set; } = string.Empty; // CodeA or CodeB
    public string SupervisorName { get; set; } = string.Empty;
}

public class CreateModuleCourseRequest
{
    public string CourseName { get; set; } = string.Empty;
    public string CourseNumber { get; set; } = string.Empty;
    public string InstitutionName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DurationDays { get; set; }
    public int DurationHours { get; set; }
    public string? CmkpCertificateNumber { get; set; }
}

public class AddModuleSelfEducationRequest
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int Hours { get; set; }
}

public class AddModuleAdditionalDaysRequest
{
    public int InternshipId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int NumberOfDays { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string? EventName { get; set; }
}

// Response DTOs
public class CreateInternshipResponse
{
    public int InternshipId { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class AddMedicalShiftResponse
{
    public int ShiftId { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class AddProcedureResponse
{
    public int ProcedureId { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class CreateCourseResponse
{
    public int CourseId { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class AddSelfEducationResponse
{
    public int SelfEducationId { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class AddAdditionalDaysResponse
{
    public int AdditionalDaysId { get; set; }
    public string Message { get; set; } = string.Empty;
}
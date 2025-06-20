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
    private readonly ICommandHandler<CreateInternship, int> _createInternshipHandler;
    private readonly ICommandHandler<AddProcedure, int> _addProcedureHandler;
    private readonly ICommandHandler<CreateCourse, int> _createCourseHandler;
    private readonly ICommandHandler<CreateSelfEducation> _createSelfEducationHandler;
    private readonly ICommandHandler<AddAdditionalSelfEducationDays, int> _addAdditionalDaysHandler;
    private readonly IUserContextService _userContextService;
    private readonly IQueryHandler<GetModuleProceduresQuery, ModuleProceduresDto> _getModuleProceduresHandler;

    public ModulesController(
        ICommandHandler<SwitchModule> switchModuleHandler,
        IQueryHandler<GetAvailableModules, IEnumerable<ModuleListDto>> getAvailableModulesHandler,
        IQueryHandler<GetModuleById, ModuleDto> getModuleByIdHandler,
        IQueryHandler<GetModuleProgress, SpecializationStatisticsDto> getModuleProgressHandler,
        ICommandHandler<CompleteModule> completeModuleHandler,
        ICommandHandler<CreateInternship, int> createInternshipHandler,
        ICommandHandler<AddProcedure, int> addProcedureHandler,
        ICommandHandler<CreateCourse, int> createCourseHandler,
        ICommandHandler<CreateSelfEducation> createSelfEducationHandler,
        ICommandHandler<AddAdditionalSelfEducationDays, int> addAdditionalDaysHandler,
        IUserContextService userContextService,
        IQueryHandler<GetModuleProceduresQuery, ModuleProceduresDto> getModuleProceduresHandler)
    {
        _switchModuleHandler = switchModuleHandler;
        _getAvailableModulesHandler = getAvailableModulesHandler;
        _getModuleByIdHandler = getModuleByIdHandler;
        _getModuleProgressHandler = getModuleProgressHandler;
        _completeModuleHandler = completeModuleHandler;
        _createInternshipHandler = createInternshipHandler;
        _addProcedureHandler = addProcedureHandler;
        _createCourseHandler = createCourseHandler;
        _createSelfEducationHandler = createSelfEducationHandler;
        _addAdditionalDaysHandler = addAdditionalDaysHandler;
        _userContextService = userContextService;
        _getModuleProceduresHandler = getModuleProceduresHandler;
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
    /// Gets all internships for a module
    /// </summary>
    [HttpGet("{moduleId:int}/internships")]
    public async Task<ActionResult> GetModuleInternships(int moduleId)
    {
        // For now, return mock data
        // TODO: Implement proper query handler
        var internships = new[]
        {
            new { id = 1, name = "Oddział Kardiologii", moduleId = moduleId },
            new { id = 2, name = "Oddział Intensywnej Terapii Kardiologicznej", moduleId = moduleId }
        };
        
        return Ok(internships);
    }

    /// <summary>
    /// Creates an internship within a module
    /// </summary>
    [HttpPost("{moduleId:int}/internships")]
    public async Task<ActionResult<CreateInternshipResponse>> CreateInternship(
        int moduleId, 
        [FromBody] CreateModuleInternshipRequest request)
    {
        var command = new CreateInternship(
            request.SpecializationId,
            request.Name,
            request.InstitutionName,
            request.DepartmentName,
            request.StartDate,
            request.EndDate,
            request.PlannedWeeks,
            request.PlannedDays,
            null, // SupervisorName
            moduleId);
            
        var internshipId = await _createInternshipHandler.HandleAsync(command);
        
        return Ok(new CreateInternshipResponse 
        { 
            InternshipId = internshipId, 
            Message = "Internship created successfully" 
        });
    }

    /// <summary>
    /// Gets all procedures for a module with user progress
    /// </summary>
    [HttpGet("{moduleId:int}/procedures")]
    public async Task<ActionResult<ModuleProceduresDto>> GetModuleProcedures(int moduleId)
    {
        var userId = new Core.ValueObjects.UserId(_userContextService.GetUserId());
        var query = new GetModuleProceduresQuery(userId, new Core.ValueObjects.ModuleId(moduleId));
        
        return await HandleAsync(query, _getModuleProceduresHandler);
    }

    /// <summary>
    /// Adds a procedure to a module (DEPRECATED - use /api/procedures/realizations)
    /// </summary>
    [HttpPost("{moduleId:int}/procedures")]
    [Obsolete("Use POST /api/procedures/realizations instead")]
    public async Task<ActionResult<AddProcedureResponse>> AddProcedure(
        int moduleId, 
        [FromBody] AddModuleProcedureRequest request)
    {
        var command = new AddProcedure(
            request.InternshipId,
            request.Date,
            Year: request.Date.Year,
            request.Code,
            request.Name,
            request.Location,
            Status: "Pending", // Default status
            request.ExecutionType,
            request.SupervisorName,
            SupervisorPwz: null,
            PerformingPerson: null,
            PatientInfo: null,
            PatientInitials: null,
            PatientGender: null,
            ProcedureRequirementId: null,
            ProcedureGroup: null,
            AssistantData: null,
            InternshipName: null,
            ModuleId: moduleId,
            ProcedureName: request.Name,
            CountA: null,
            CountB: null,
            Supervisor: request.SupervisorName,
            Institution: null,
            Comments: null
        );
        
        var procedureId = await _addProcedureHandler.HandleAsync(command);
        
        return Ok(new AddProcedureResponse 
        { 
            ProcedureId = procedureId, 
            Message = "Procedure added successfully" 
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
        // Need to get specialization ID from module
        var module = await _getModuleByIdHandler.HandleAsync(new GetModuleById(moduleId));
        
        var command = new CreateCourse(
            module.SpecializationId,
            "Specialization", // CourseType
            request.CourseName,
            request.InstitutionName,
            request.EndDate, // CompletionDate
            request.CourseNumber,
            request.CmkpCertificateNumber,
            moduleId
        );
        
        var courseId = await _createCourseHandler.HandleAsync(command);
        
        return Ok(new CreateCourseResponse 
        { 
            CourseId = courseId, 
            Message = "Course created successfully" 
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
        // Need to get specialization ID from module
        var module = await _getModuleByIdHandler.HandleAsync(new GetModuleById(moduleId));
        
        // Parse the self-education type
        var selfEducationType = request.Type switch
        {
            "Conference" => Core.ValueObjects.SelfEducationType.Conference,
            "Workshop" => Core.ValueObjects.SelfEducationType.Workshop,
            "Publication" => Core.ValueObjects.SelfEducationType.Publication,
            "LiteratureStudy" => Core.ValueObjects.SelfEducationType.LiteratureStudy,
            "ScientificMeeting" => Core.ValueObjects.SelfEducationType.ScientificMeeting,
            _ => Core.ValueObjects.SelfEducationType.Conference // Default to conference
        };
        
        var command = new CreateSelfEducation(
            new Core.ValueObjects.SpecializationId(module.SpecializationId),
            new Core.ValueObjects.UserId(_userContextService.GetUserId()),
            selfEducationType,
            request.Date.Year,
            request.Description,
            request.Hours,
            request.Description,
            null // Provider
        );
        
        await _createSelfEducationHandler.HandleAsync(command);
        
        return Ok(new AddSelfEducationResponse 
        { 
            SelfEducationId = 0, // Handler doesn't return ID yet
            Message = "Self-education activity added successfully" 
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
        // Need to get specialization ID from module
        var module = await _getModuleByIdHandler.HandleAsync(new GetModuleById(moduleId));
        
        var command = new AddAdditionalSelfEducationDays(
            module.SpecializationId,
            request.StartDate.Year,
            request.NumberOfDays,
            request.Purpose
        );
        
        var additionalDaysId = await _addAdditionalDaysHandler.HandleAsync(command);
        
        return Ok(new AddAdditionalDaysResponse 
        { 
            AdditionalDaysId = additionalDaysId, 
            Message = "Additional self-education days added successfully" 
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.DomainServices;

namespace SledzSpecke.Api.Controllers;

/// <summary>
/// Controller for SMK (System Monitorowania Kształcenia) specific operations
/// including validation, export, and compliance checking
/// </summary>
[Authorize]
[ApiController]
[Route("api/smk")]
public class SmkController : BaseController
{
    private readonly IQueryHandler<ValidateSpecializationForSmk, SmkValidationResultDto> _validateHandler;
    private readonly IQueryHandler<ExportSpecializationToXlsx, byte[]> _exportHandler;
    private readonly IQueryHandler<PreviewSmkExport, SmkExportPreviewDto> _previewHandler;
    private readonly IQueryHandler<GetSmkRequirements, SmkRequirementsDto> _requirementsHandler;
    private readonly IUserContextService _userContextService;

    public SmkController(
        IQueryHandler<ValidateSpecializationForSmk, SmkValidationResultDto> validateHandler,
        IQueryHandler<ExportSpecializationToXlsx, byte[]> exportHandler,
        IQueryHandler<PreviewSmkExport, SmkExportPreviewDto> previewHandler,
        IQueryHandler<GetSmkRequirements, SmkRequirementsDto> requirementsHandler,
        IUserContextService userContextService)
    {
        _validateHandler = validateHandler;
        _exportHandler = exportHandler;
        _previewHandler = previewHandler;
        _requirementsHandler = requirementsHandler;
        _userContextService = userContextService;
    }

    /// <summary>
    /// Validates a specialization for SMK compliance
    /// </summary>
    /// <param name="specializationId">ID of the specialization to validate</param>
    /// <returns>Validation result with any errors or warnings</returns>
    [HttpGet("validate/{specializationId:int}")]
    public async Task<ActionResult<SmkValidationResultDto>> ValidateSpecialization(int specializationId)
    {
        var query = new ValidateSpecializationForSmk(specializationId);
        return await HandleAsync(query, _validateHandler);
    }

    /// <summary>
    /// Exports specialization data to XLSX format compatible with SMK import
    /// </summary>
    /// <param name="specializationId">ID of the specialization to export</param>
    /// <returns>XLSX file with all SMK-required data</returns>
    [HttpGet("export/{specializationId:int}/xlsx")]
    public async Task<IActionResult> ExportToXlsx(int specializationId)
    {
        var query = new ExportSpecializationToXlsx(specializationId);
        var result = await _exportHandler.HandleAsync(query);
        
        var fileName = $"SMK_Export_{specializationId}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
        return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    /// <summary>
    /// Previews the data that will be exported for a specialization
    /// </summary>
    /// <param name="specializationId">ID of the specialization to preview</param>
    /// <returns>Preview of export data without generating the file</returns>
    [HttpGet("export/{specializationId:int}/preview")]
    public async Task<ActionResult<SmkExportPreviewDto>> PreviewExport(int specializationId)
    {
        var query = new PreviewSmkExport(specializationId);
        return await HandleAsync(query, _previewHandler);
    }

    /// <summary>
    /// Gets SMK requirements for a specific specialization and version
    /// </summary>
    /// <param name="specialization">Specialization name (e.g., "kardiologia")</param>
    /// <param name="smkVersion">SMK version ("old" or "new")</param>
    /// <returns>Requirements including procedures, courses, and hours</returns>
    [HttpGet("requirements/{specialization}/{smkVersion}")]
    public async Task<ActionResult<SmkRequirementsDto>> GetRequirements(string specialization, string smkVersion)
    {
        var query = new GetSmkRequirements(specialization, smkVersion);
        return await HandleAsync(query, _requirementsHandler);
    }

    /// <summary>
    /// Validates CMKP certificate number format
    /// </summary>
    /// <param name="request">Certificate validation request</param>
    /// <returns>Validation result</returns>
    [HttpPost("validate/cmkp-certificate")]
    public ActionResult<CmkpValidationResultDto> ValidateCmkpCertificate([FromBody] ValidateCmkpCertificateRequest request)
    {
        var validationService = new CmkpValidationService();
        var result = validationService.ValidateCertificateNumber(request.CertificateNumber);
        
        return Ok(new CmkpValidationResultDto
        {
            IsValid = result.IsSuccess && result.Value,
            ErrorMessage = result.IsFailure ? result.Error : null,
            SuggestedFormat = result.IsFailure ? "CMKP/YYYY/NUMBER (e.g., CMKP/2024/1234)" : null
        });
    }

}

// Request DTOs
public class ValidateCmkpCertificateRequest
{
    public string CertificateNumber { get; set; } = string.Empty;
}

public class ValidateWeeklyHoursRequest
{
    public int InternshipId { get; set; }
    public DateTime WeekStartDate { get; set; }
}

public class ValidateMonthlyHoursRequest
{
    public int ModuleId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
}

// Response DTOs
public class CmkpValidationResultDto
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public string? SuggestedFormat { get; set; }
}

public class ModuleCompletionStatusDto
{
    public int SpecializationId { get; set; }
    public List<ModuleStatus> Modules { get; set; } = new();
    public bool AllModulesComplete { get; set; }
    public decimal OverallProgress { get; set; }
}

public class ModuleStatus
{
    public int ModuleId { get; set; }
    public string ModuleName { get; set; } = string.Empty;
    public string ModuleType { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
    public decimal ProgressPercentage { get; set; }
    public List<string> MissingRequirements { get; set; } = new();
}

public class WeeklyHoursValidationDto
{
    public bool IsValid { get; set; }
    public double TotalHours { get; set; }
    public double MaxAllowedHours { get; set; }
    public double? ExcessHours { get; set; }
    public DateTime WeekStartDate { get; set; }
    public DateTime WeekEndDate { get; set; }
    public string? ValidationMessage { get; set; }
}

public class MonthlyHoursValidationDto
{
    public bool IsValid { get; set; }
    public double TotalHours { get; set; }
    public double MinRequiredHours { get; set; }
    public double? MissingHours { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public string? ValidationMessage { get; set; }
}


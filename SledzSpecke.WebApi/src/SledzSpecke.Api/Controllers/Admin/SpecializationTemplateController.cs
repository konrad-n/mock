using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application.SpecializationTemplates.DTOs;
using SledzSpecke.Application.SpecializationTemplates.Services;

namespace SledzSpecke.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/specialization-templates")]
[Authorize(Policy = "AdminOnly")]
public class SpecializationTemplateController : ControllerBase
{
    private readonly ISpecializationTemplateImportService _importService;
    private readonly ILogger<SpecializationTemplateController> _logger;

    public SpecializationTemplateController(
        ISpecializationTemplateImportService importService,
        ILogger<SpecializationTemplateController> logger)
    {
        _importService = importService;
        _logger = logger;
    }

    /// <summary>
    /// Get all specialization templates
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<SpecializationTemplateDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTemplates()
    {
        _logger.LogInformation("Fetching all specialization templates");
        var result = await _importService.GetAllTemplatesAsync();
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get specific specialization template
    /// </summary>
    [HttpGet("{code}/{version}")]
    [ProducesResponseType(typeof(SpecializationTemplateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTemplate(string code, string version)
    {
        _logger.LogInformation("Fetching template for {Code} version {Version}", code, version);
        var result = await _importService.GetTemplateAsync(code, version);
        
        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Import single specialization template from JSON
    /// </summary>
    [HttpPost("import")]
    [ProducesResponseType(typeof(ImportResultDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ImportTemplate([FromBody] SpecializationTemplateDto template)
    {
        _logger.LogInformation("Importing template for {Code} version {Version}", template.Code, template.Version);
        
        var validationResult = await _importService.ValidateTemplateAsync(template);
        if (!validationResult.IsSuccess)
        {
            return BadRequest(new { error = "Validation failed", details = validationResult.Error });
        }

        var result = await _importService.ImportTemplateAsync(template);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return CreatedAtAction(
            nameof(GetTemplate), 
            new { code = template.Code, version = template.Version }, 
            new ImportResultDto { TemplateId = result.Value, Message = "Template imported successfully" });
    }

    /// <summary>
    /// Import multiple templates from directory
    /// </summary>
    [HttpPost("import-bulk")]
    [ProducesResponseType(typeof(BulkImportResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ImportBulkFromDirectory([FromBody] BulkImportRequestDto request)
    {
        _logger.LogInformation("Starting bulk import from directory: {Directory}", request.DirectoryPath);
        
        var result = await _importService.ImportFromDirectoryAsync(request.DirectoryPath);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new BulkImportResultDto 
        { 
            ImportedCount = result.Value.Count,
            TemplateIds = result.Value,
            Message = $"Successfully imported {result.Value.Count} templates"
        });
    }

    /// <summary>
    /// Update existing specialization template
    /// </summary>
    [HttpPut("{code}/{version}")]
    [ProducesResponseType(typeof(ImportResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateTemplate(string code, string version, [FromBody] SpecializationTemplateDto template)
    {
        _logger.LogInformation("Updating template for {Code} version {Version}", code, version);
        
        // Ensure consistency
        template.Code = code;
        template.Version = version;

        var validationResult = await _importService.ValidateTemplateAsync(template);
        if (!validationResult.IsSuccess)
        {
            return BadRequest(new { error = "Validation failed", details = validationResult.Error });
        }

        var result = await _importService.UpdateTemplateAsync(code, version, template);
        
        if (!result.IsSuccess)
        {
            if (result.Error?.Contains("not found") == true)
            {
                return NotFound(new { error = result.Error });
            }
            return BadRequest(new { error = result.Error });
        }

        return Ok(new ImportResultDto { TemplateId = result.Value, Message = "Template updated successfully" });
    }

    /// <summary>
    /// Delete specialization template
    /// </summary>
    [HttpDelete("{code}/{version}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTemplate(string code, string version)
    {
        _logger.LogInformation("Deleting template for {Code} version {Version}", code, version);
        
        var result = await _importService.DeleteTemplateAsync(code, version);
        
        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return NoContent();
    }

    /// <summary>
    /// Validate template before import
    /// </summary>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(ValidationResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ValidateTemplate([FromBody] SpecializationTemplateDto template)
    {
        _logger.LogInformation("Validating template for {Code} version {Version}", template.Code, template.Version);
        
        var result = await _importService.ValidateTemplateAsync(template);
        
        return Ok(new ValidationResultDto 
        { 
            IsValid = result.IsSuccess,
            Errors = result.IsSuccess ? new List<string>() : new List<string> { result.Error ?? "Unknown validation error" }
        });
    }

    /// <summary>
    /// Import templates from CMKP website
    /// </summary>
    [HttpPost("import-from-cmkp")]
    [ProducesResponseType(typeof(BulkImportResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ImportFromCmkp([FromBody] CmkpImportRequestDto request)
    {
        _logger.LogInformation("Importing templates from CMKP for version: {Version}", request.SmkVersion);
        
        var result = await _importService.ImportFromCmkpWebsiteAsync(request.SmkVersion);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new BulkImportResultDto 
        { 
            ImportedCount = result.Value.Count,
            TemplateIds = result.Value,
            Message = $"Successfully imported {result.Value.Count} templates from CMKP"
        });
    }
}

// Request/Response DTOs
public class ImportResultDto
{
    public int TemplateId { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class BulkImportResultDto
{
    public int ImportedCount { get; set; }
    public List<int> TemplateIds { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}

public class BulkImportRequestDto
{
    public string DirectoryPath { get; set; } = string.Empty;
}

public class ValidationResultDto
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class CmkpImportRequestDto
{
    public string SmkVersion { get; set; } = string.Empty; // "old" or "new"
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Api.Controllers;

[ApiController]
[Route("api/specialization-templates")]
[Authorize]
public class SpecializationTemplatesController : BaseController
{
    private readonly ISpecializationTemplateService _templateService;

    public SpecializationTemplatesController(ISpecializationTemplateService templateService)
    {
        _templateService = templateService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTemplates()
    {
        var templates = await _templateService.GetAllTemplatesAsync();
        return Ok(templates);
    }

    [HttpGet("{specializationCode}/{smkVersion}")]
    public async Task<IActionResult> GetTemplate(string specializationCode, string smkVersion)
    {
        if (!Enum.TryParse<SmkVersion>(smkVersion, true, out var version))
        {
            return BadRequest($"Invalid SMK version: {smkVersion}");
        }

        var template = await _templateService.GetTemplateAsync(specializationCode, version);
        if (template == null)
        {
            return NotFound($"Template not found for {specializationCode} - {smkVersion}");
        }

        return Ok(template);
    }

    [HttpGet("{specializationCode}/{smkVersion}/modules/{moduleId}")]
    public async Task<IActionResult> GetModuleTemplate(string specializationCode, string smkVersion, int moduleId)
    {
        if (!Enum.TryParse<SmkVersion>(smkVersion, true, out var version))
        {
            return BadRequest($"Invalid SMK version: {smkVersion}");
        }

        var module = await _templateService.GetModuleTemplateAsync(specializationCode, version, moduleId);
        if (module == null)
        {
            return NotFound($"Module template not found");
        }

        return Ok(module);
    }

    [HttpGet("{specializationCode}/{smkVersion}/procedures/{procedureId}")]
    public async Task<IActionResult> GetProcedureTemplate(string specializationCode, string smkVersion, int procedureId)
    {
        if (!Enum.TryParse<SmkVersion>(smkVersion, true, out var version))
        {
            return BadRequest($"Invalid SMK version: {smkVersion}");
        }

        var procedure = await _templateService.GetProcedureTemplateAsync(specializationCode, version, procedureId);
        if (procedure == null)
        {
            return NotFound($"Procedure template not found");
        }

        return Ok(procedure);
    }

    [HttpGet("{specializationCode}/{smkVersion}/internships/{internshipId}")]
    public async Task<IActionResult> GetInternshipTemplate(string specializationCode, string smkVersion, int internshipId)
    {
        if (!Enum.TryParse<SmkVersion>(smkVersion, true, out var version))
        {
            return BadRequest($"Invalid SMK version: {smkVersion}");
        }

        var internship = await _templateService.GetInternshipTemplateAsync(specializationCode, version, internshipId);
        if (internship == null)
        {
            return NotFound($"Internship template not found");
        }

        return Ok(internship);
    }

    [HttpGet("{specializationCode}/{smkVersion}/courses/{courseId}")]
    public async Task<IActionResult> GetCourseTemplate(string specializationCode, string smkVersion, int courseId)
    {
        if (!Enum.TryParse<SmkVersion>(smkVersion, true, out var version))
        {
            return BadRequest($"Invalid SMK version: {smkVersion}");
        }

        var course = await _templateService.GetCourseTemplateAsync(specializationCode, version, courseId);
        if (course == null)
        {
            return NotFound($"Course template not found");
        }

        return Ok(course);
    }

    [HttpPost("{specializationCode}/{smkVersion}/procedures/{procedureId}/validate")]
    public async Task<IActionResult> ValidateProcedureCode(
        string specializationCode, 
        string smkVersion, 
        int procedureId,
        [FromBody] ValidateProcedureRequest request)
    {
        if (!Enum.TryParse<SmkVersion>(smkVersion, true, out var version))
        {
            return BadRequest($"Invalid SMK version: {smkVersion}");
        }

        var isValid = await _templateService.ValidateProcedureRequirementsAsync(
            specializationCode, version, procedureId, request.ProcedureCode);

        return Ok(new { isValid });
    }
}

public class ValidateProcedureRequest
{
    public string ProcedureCode { get; set; } = string.Empty;
}
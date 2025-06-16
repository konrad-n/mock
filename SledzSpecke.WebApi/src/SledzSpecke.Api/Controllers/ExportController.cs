using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Security;
using SledzSpecke.Core.DomainServices;

namespace SledzSpecke.Api.Controllers;

[ApiController]
[Route("api/export")]
[Authorize]
public class ExportController : BaseResultController
{
    private readonly ISpecializationExportService _exportService;
    private readonly IUserContext _userContext;
    private readonly ILogger<ExportController> _logger;

    public ExportController(
        ISpecializationExportService exportService,
        IUserContext userContext,
        ILogger<ExportController> logger)
    {
        _exportService = exportService;
        _userContext = userContext;
        _logger = logger;
    }

    /// <summary>
    /// Export specialization data to XLSX format compatible with SMK import
    /// </summary>
    /// <param name="id">Specialization ID</param>
    /// <returns>XLSX file</returns>
    [HttpGet("specialization/{id}/xlsx")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExportToXlsx(int id)
    {
        _logger.LogInformation("User {UserId} requesting XLSX export for specialization {SpecializationId}", 
            _userContext.UserId, id);

        var result = await _exportService.ExportToXlsxAsync(id);
        
        if (result.IsFailure)
        {
            _logger.LogWarning("Export failed for specialization {SpecializationId}: {Error}", 
                id, result.Error);
                
            if (result.ErrorCode == "SPECIALIZATION_NOT_FOUND")
                return NotFound(result.Error);
                
            return BadRequest(result.Error);
        }

        var fileName = $"EKS_{id}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        
        _logger.LogInformation("Successfully generated export file {FileName} for specialization {SpecializationId}", 
            fileName, id);
        
        return File(
            result.Value, 
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    /// <summary>
    /// Preview export data before generating XLSX file
    /// </summary>
    /// <param name="id">Specialization ID</param>
    /// <returns>Export preview data</returns>
    [HttpGet("specialization/{id}/preview")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PreviewExport(int id)
    {
        _logger.LogInformation("User {UserId} requesting export preview for specialization {SpecializationId}", 
            _userContext.UserId, id);

        var result = await _exportService.PreviewExportAsync(id);
        
        if (result.IsFailure)
        {
            if (result.ErrorCode == "SPECIALIZATION_NOT_FOUND")
                return NotFound(result.Error);
                
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Validate specialization data for SMK export compliance
    /// </summary>
    /// <param name="id">Specialization ID</param>
    /// <returns>Validation report</returns>
    [HttpPost("specialization/{id}/validate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ValidateForExport(int id)
    {
        _logger.LogInformation("User {UserId} requesting export validation for specialization {SpecializationId}", 
            _userContext.UserId, id);

        var result = await _exportService.ValidateForExportAsync(id);
        
        if (result.IsFailure)
        {
            if (result.ErrorCode == "SPECIALIZATION_NOT_FOUND")
                return NotFound(result.Error);
                
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}
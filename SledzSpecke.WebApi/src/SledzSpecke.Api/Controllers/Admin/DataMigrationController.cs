using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Entities;
using SledzSpecke.Infrastructure.DAL;
using SledzSpecke.Infrastructure.Migrations;

namespace SledzSpecke.Api.Controllers.Admin;

/// <summary>
/// Admin controller for running data migrations
/// </summary>
[ApiController]
[Route("api/admin/data-migration")]
[Authorize(Policy = "AdminOnly")]
public class DataMigrationController : ControllerBase
{
    private readonly SledzSpeckeDbContext _context;
    private readonly ILogger<DataMigrationController> _logger;
    private readonly ILogger<ProcedureDataMigration> _migrationLogger;
    
    public DataMigrationController(
        SledzSpeckeDbContext context,
        ILogger<DataMigrationController> logger,
        ILogger<ProcedureDataMigration> migrationLogger)
    {
        _context = context;
        _logger = logger;
        _migrationLogger = migrationLogger;
    }
    
    /// <summary>
    /// Run the procedure data migration from old structure to new
    /// </summary>
    /// <returns>Migration result with statistics</returns>
    [HttpPost("procedures")]
    public async Task<IActionResult> MigrateProcedures()
    {
        _logger.LogInformation("Starting procedure migration requested by user {User}", User.Identity?.Name);
        
        try
        {
            var migration = new ProcedureDataMigration(_context, _migrationLogger);
            var result = await migration.MigrateAsync();
            
            if (result.Success)
            {
                _logger.LogInformation("Procedure migration completed successfully");
                return Ok(new
                {
                    success = true,
                    message = result.Message,
                    statistics = new
                    {
                        oldProcedures = result.OldProcedureCount,
                        requirementsCreated = result.RequirementsCreated,
                        realizationsCreated = result.RealizationsCreated
                    }
                });
            }
            else
            {
                _logger.LogError("Procedure migration failed: {Message}", result.Message);
                return BadRequest(new
                {
                    success = false,
                    message = result.Message,
                    error = result.Error
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during procedure migration");
            return StatusCode(500, new
            {
                success = false,
                message = "An unexpected error occurred during migration",
                error = ex.Message
            });
        }
    }
    
    /// <summary>
    /// Check the status of procedure migration
    /// </summary>
    /// <returns>Current migration status</returns>
    [HttpGet("procedures/status")]
    public async Task<IActionResult> GetMigrationStatus()
    {
        try
        {
            var oldProcedureCount = await _context.Procedures.CountAsync();
            var requirementCount = await _context.Set<ProcedureRequirement>().CountAsync();
            var realizationCount = await _context.Set<ProcedureRealization>().CountAsync();
            
            var migrationNeeded = requirementCount == 0 && realizationCount == 0 && oldProcedureCount > 0;
            
            return Ok(new
            {
                migrationNeeded,
                oldProcedureCount,
                requirementCount,
                realizationCount,
                status = migrationNeeded ? "Not migrated" : "Already migrated"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking migration status");
            return StatusCode(500, new
            {
                success = false,
                message = "Error checking migration status",
                error = ex.Message
            });
        }
    }
    
    /// <summary>
    /// Validate the procedure migration without making changes
    /// </summary>
    /// <returns>Validation results</returns>
    [HttpPost("procedures/validate")]
    public async Task<IActionResult> ValidateMigration()
    {
        try
        {
            var oldProcedures = await _context.Procedures
                .ToListAsync();
                
            var validationIssues = new List<string>();
            var statistics = new
            {
                totalProcedures = oldProcedures.Count(),
                proceduresWithoutCode = oldProcedures.Count(p => string.IsNullOrEmpty(p.Code)),
                proceduresWithoutModule = oldProcedures.Count(p => p.ModuleId == null),
                proceduresWithoutInternship = oldProcedures.Count(p => p.InternshipId == null),
                proceduresWithoutLocation = oldProcedures.Count(p => string.IsNullOrEmpty(p.Location)),
                uniqueProcedureCodes = oldProcedures
                    .Where(p => !string.IsNullOrEmpty(p.Code))
                    .Select(p => p.Code)
                    .Distinct()
                    .Count()
            };
            
            if (statistics.proceduresWithoutCode > 0)
            {
                validationIssues.Add($"{statistics.proceduresWithoutCode} procedures have no code");
            }
            
            if (statistics.proceduresWithoutModule > 0)
            {
                validationIssues.Add($"{statistics.proceduresWithoutModule} procedures have no module");
            }
            
            if (statistics.proceduresWithoutLocation > 0)
            {
                validationIssues.Add($"{statistics.proceduresWithoutLocation} procedures have no location");
            }
            
            return Ok(new
            {
                isValid = validationIssues.Count == 0,
                statistics,
                issues = validationIssues
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating migration");
            return StatusCode(500, new
            {
                success = false,
                message = "Error validating migration",
                error = ex.Message
            });
        }
    }
}
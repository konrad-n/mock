using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Infrastructure.DAL;
using System.Text.Json;

namespace SledzSpecke.Infrastructure.Migrations;

/// <summary>
/// Populates ProcedureRequirements from specialization templates
/// This ensures all modules have their required procedures defined
/// </summary>
public class PopulateProcedureRequirements
{
    private readonly SledzSpeckeDbContext _context;
    private readonly ISpecializationTemplateService _templateService;
    private readonly ILogger<PopulateProcedureRequirements> _logger;
    
    public PopulateProcedureRequirements(
        SledzSpeckeDbContext context,
        ISpecializationTemplateService templateService,
        ILogger<PopulateProcedureRequirements> logger)
    {
        _context = context;
        _templateService = templateService;
        _logger = logger;
    }
    
    public async Task<PopulationResult> PopulateAsync(CancellationToken cancellationToken = default)
    {
        var result = new PopulationResult();
        
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            _logger.LogInformation("Starting procedure requirements population from templates");
            
            // Get all specializations with their modules
            var specializations = await _context.Specializations
                .Include(s => s.Modules)
                .ToListAsync(cancellationToken);
                
            result.TotalSpecializations = specializations.Count;
            
            foreach (var specialization in specializations)
            {
                try
                {
                    // Get the template for this specialization
                    var template = await _templateService.GetTemplateAsync(
                        specialization.ProgramCode, 
                        specialization.SmkVersion);
                        
                    if (template == null)
                    {
                        _logger.LogWarning("No template found for specialization {Code} {Version}", 
                            specialization.ProgramCode, specialization.SmkVersion);
                        result.SpecializationsWithoutTemplate++;
                        continue;
                    }
                    
                    // Process each module
                    foreach (var module in specialization.Modules)
                    {
                        var moduleTemplate = template.Modules
                            .FirstOrDefault(mt => mt.Name == module.Name || mt.ModuleType == module.Type.ToString());
                            
                        if (moduleTemplate == null)
                        {
                            _logger.LogWarning("No template found for module {Name} in specialization {Code}", 
                                module.Name, specialization.ProgramCode);
                            continue;
                        }
                        
                        // Process procedures in the module template
                        foreach (var procedureTemplate in moduleTemplate.Procedures)
                        {
                            // Check if requirement already exists
                            var existingRequirement = await _context.Set<ProcedureRequirement>()
                                .FirstOrDefaultAsync(pr => 
                                    pr.ModuleId == module.Id &&
                                    pr.Code == procedureTemplate.Type,
                                    cancellationToken);
                                    
                            if (existingRequirement != null)
                            {
                                _logger.LogDebug("Requirement already exists for {Code} in module {Module}", 
                                    procedureTemplate.Type, module.Name);
                                continue;
                            }
                            
                            try 
                            {
                                // Create new requirement
                                var requirement = new ProcedureRequirement(
                                    module.Id,
                                    procedureTemplate.Type, // Use Type as Code since ProcedureTemplate doesn't have Code
                                    procedureTemplate.Name,
                                    procedureTemplate.RequiredCountA,
                                    procedureTemplate.RequiredCountB,
                                    0 // displayOrder
                                );
                                
                                await _context.Set<ProcedureRequirement>().AddAsync(
                                    requirement, 
                                    cancellationToken);
                                result.RequirementsCreated++;
                                
                                _logger.LogDebug("Created requirement for {Code} in module {Module}", 
                                    procedureTemplate.Type, module.Name);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning("Failed to create requirement for {Code}: {Error}", 
                                    procedureTemplate.Type, ex.Message);
                                result.FailedRequirements++;
                            }
                        }
                        
                        result.ModulesProcessed++;
                    }
                    
                    result.SpecializationsProcessed++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing specialization {Id}", specialization.Id);
                    result.Errors.Add($"Specialization {specialization.Name}: {ex.Message}");
                }
            }
            
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            
            result.Success = true;
            result.Message = $"Successfully created {result.RequirementsCreated} procedure requirements";
            
            _logger.LogInformation("Procedure requirements population completed: {Result}", 
                JsonSerializer.Serialize(result));
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during procedure requirements population");
            await transaction.RollbackAsync(cancellationToken);
            
            result.Success = false;
            result.Message = $"Population failed: {ex.Message}";
            
            return result;
        }
    }
    
    /// <summary>
    /// Populate requirements for a specific specialization template
    /// </summary>
    public async Task<PopulationResult> PopulateForTemplateAsync(
        string templateCode, 
        SmkVersion smkVersion,
        CancellationToken cancellationToken = default)
    {
        var result = new PopulationResult();
        
        try
        {
            _logger.LogInformation("Populating requirements for template {Code} {Version}", 
                templateCode, smkVersion);
                
            // Get the template
            var template = await _templateService.GetTemplateAsync(templateCode, smkVersion);
            if (template == null)
            {
                result.Success = false;
                result.Message = $"Template not found: {templateCode} {smkVersion}";
                return result;
            }
            
            // Get all modules in the system that match this template
            // First get specializations, then their modules
            var specializations = await _context.Specializations
                .Include(s => s.Modules)
                .Where(s => s.ProgramCode == templateCode &&
                           s.SmkVersion == smkVersion)
                .ToListAsync(cancellationToken);
                
            var modules = specializations.SelectMany(s => s.Modules).ToList();
                
            foreach (var module in modules)
            {
                var moduleTemplate = template.Modules
                    .FirstOrDefault(mt => mt.Name == module.Name || mt.ModuleType == module.Type.ToString());
                    
                if (moduleTemplate == null) continue;
                
                foreach (var procedureTemplate in moduleTemplate.Procedures)
                {
                    var existingRequirement = await _context.Set<ProcedureRequirement>()
                        .AnyAsync(pr => pr.ModuleId == module.Id && pr.Code == procedureTemplate.Type,
                            cancellationToken);
                            
                    if (!existingRequirement)
                    {
                        try
                        {
                            var requirement = new ProcedureRequirement(
                                module.Id,
                                procedureTemplate.Type, // Use Type as Code
                                procedureTemplate.Name,
                                procedureTemplate.RequiredCountA,
                                procedureTemplate.RequiredCountB,
                                0 // displayOrder
                            );
                            
                            await _context.Set<ProcedureRequirement>().AddAsync(
                                requirement, 
                                cancellationToken);
                            result.RequirementsCreated++;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning("Failed to create requirement: {Error}", ex.Message);
                        }
                    }
                }
                
                result.ModulesProcessed++;
            }
            
            await _context.SaveChangesAsync(cancellationToken);
            
            result.Success = true;
            result.Message = $"Created {result.RequirementsCreated} requirements for {result.ModulesProcessed} modules";
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error populating requirements for template {Code}", templateCode);
            
            result.Success = false;
            result.Message = $"Population failed: {ex.Message}";
            
            return result;
        }
    }
}

public class PopulationResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int TotalSpecializations { get; set; }
    public int SpecializationsProcessed { get; set; }
    public int SpecializationsWithoutTemplate { get; set; }
    public int ModulesProcessed { get; set; }
    public int RequirementsCreated { get; set; }
    public int FailedRequirements { get; set; }
    public List<string> Errors { get; set; } = new();
}
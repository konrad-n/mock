using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Infrastructure.DAL;

namespace SledzSpecke.Infrastructure.Migrations;

/// <summary>
/// Migrates procedure data from old structure (ProcedureBase/ProcedureOldSmk/ProcedureNewSmk)
/// to new structure (ProcedureRequirement/ProcedureRealization)
/// </summary>
public class ProcedureDataMigration
{
    private readonly SledzSpeckeDbContext _context;
    private readonly ILogger<ProcedureDataMigration> _logger;
    
    public ProcedureDataMigration(SledzSpeckeDbContext context, ILogger<ProcedureDataMigration> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<MigrationResult> MigrateAsync(CancellationToken cancellationToken = default)
    {
        var result = new MigrationResult();
        
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            _logger.LogInformation("Starting procedure data migration");
            
            // Step 1: Check if migration is needed
            var existingRequirements = await _context.Set<ProcedureRequirement>().AnyAsync(cancellationToken);
            var existingRealizations = await _context.Set<ProcedureRealization>().AnyAsync(cancellationToken);
            
            if (existingRequirements || existingRealizations)
            {
                _logger.LogWarning("Procedure migration already performed. Skipping.");
                result.Success = true;
                result.Message = "Migration already completed";
                return result;
            }
            
            // Step 2: Load old procedures
            var oldProcedures = await _context.Procedures
                .ToListAsync(cancellationToken);
                
            result.OldProcedureCount = oldProcedures.Count();
            _logger.LogInformation("Found {Count} old procedures to migrate", oldProcedures.Count());
            
            // Step 3: Create requirements from unique procedure codes per module
            var requirementMap = new Dictionary<string, ProcedureRequirement>();
            var requirementGroups = oldProcedures
                .Where(p => !string.IsNullOrEmpty(p.Code) && p.ModuleId != null)
                .GroupBy(p => new { p.Code, p.ModuleId, p.SmkVersion });
                
            foreach (var group in requirementGroups)
            {
                var firstProcedure = group.First();
                var key = $"{group.Key.Code}_{group.Key.ModuleId}_{group.Key.SmkVersion}";
                
                // Check if this is an old or new SMK procedure
                int requiredAsOperator = 1;
                int requiredAsAssistant = 0;
                int? year = null;
                
                if (firstProcedure is ProcedureOldSmk oldSmk)
                {
                    year = oldSmk.Year;
                    requiredAsOperator = 1; // Old SMK typically requires 1
                }
                else if (firstProcedure is ProcedureNewSmk newSmk)
                {
                    requiredAsOperator = newSmk.RequiredCountCodeA;
                    requiredAsAssistant = newSmk.RequiredCountCodeB;
                }
                
                try
                {
                    var requirement = new ProcedureRequirement(
                        new ModuleId(group.Key.ModuleId),
                        group.Key.Code,
                        firstProcedure.ProcedureGroup ?? group.Key.Code,
                        requiredAsOperator,
                        requiredAsAssistant,
                        0 // displayOrder - will be set later if needed
                    );
                    
                    await _context.Set<ProcedureRequirement>().AddAsync(requirement, cancellationToken);
                    requirementMap[key] = requirement;
                    result.RequirementsCreated++;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Failed to create requirement for {Code}: {Error}", 
                        group.Key.Code, ex.Message);
                }
            }
            
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Created {Count} procedure requirements", result.RequirementsCreated);
            
            // Step 4: Create realizations from procedure instances
            foreach (var oldProcedure in oldProcedures)
            {
                if (string.IsNullOrEmpty(oldProcedure.Code) || oldProcedure.ModuleId == null)
                {
                    _logger.LogWarning("Skipping procedure {Id} with missing code or module", oldProcedure.Id);
                    continue;
                }
                
                var key = $"{oldProcedure.Code}_{oldProcedure.ModuleId}_{oldProcedure.SmkVersion}";
                if (!requirementMap.TryGetValue(key, out var requirement))
                {
                    _logger.LogWarning("No requirement found for procedure {Id}", oldProcedure.Id);
                    continue;
                }
                
                // Determine role based on execution type
                var role = oldProcedure.ExecutionType == ProcedureExecutionType.CodeA 
                    ? ProcedureRole.Operator 
                    : ProcedureRole.Assistant;
                    
                // Get year for old SMK procedures
                int? year = null;
                if (oldProcedure is ProcedureOldSmk oldSmk2)
                {
                    year = oldSmk2.Year;
                }
                
                // Get UserId from the internship's specialization
                var internshipWithSpec = await _context.Internships
                    .Where(i => i.InternshipId == oldProcedure.InternshipId)
                    .Select(i => new { i.SpecializationId })
                    .FirstOrDefaultAsync(cancellationToken);
                    
                if (internshipWithSpec == null)
                {
                    _logger.LogWarning("No internship found for procedure {Id}", oldProcedure.Id);
                    continue;
                }
                
                var specialization = await _context.Specializations
                    .Where(s => s.SpecializationId == internshipWithSpec.SpecializationId)
                    .Select(s => new { s.UserId })
                    .FirstOrDefaultAsync(cancellationToken);
                    
                if (specialization == null)
                {
                    _logger.LogWarning("No specialization found for procedure {Id}", oldProcedure.Id);
                    continue;
                }
                
                var realization = ProcedureRealization.Create(
                    requirement.Id,
                    specialization.UserId,
                    oldProcedure.Date,
                    oldProcedure.Location ?? "Unknown",
                    role,
                    year
                );
                
                if (realization.IsSuccess)
                {
                    await _context.Set<ProcedureRealization>().AddAsync(realization.Value, cancellationToken);
                    result.RealizationsCreated++;
                    
                    // Track mapping for audit
                    result.Mappings.Add(new ProcedureMigrationMapping
                    {
                        OldProcedureId = oldProcedure.Id,
                        NewRequirementId = requirement.Id.Value,
                        NewRealizationId = realization.Value.Id.Value
                    });
                }
                else
                {
                    _logger.LogWarning("Failed to create realization for procedure {Id}: {Error}", 
                        oldProcedure.Id, realization.Error);
                }
            }
            
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Created {Count} procedure realizations", result.RealizationsCreated);
            
            // Step 5: Verify migration
            if (result.OldProcedureCount != result.RealizationsCreated)
            {
                _logger.LogWarning("Migration count mismatch: {Old} old procedures, {New} new realizations",
                    result.OldProcedureCount, result.RealizationsCreated);
            }
            
            await transaction.CommitAsync(cancellationToken);
            
            result.Success = true;
            result.Message = $"Successfully migrated {result.RealizationsCreated} procedures with {result.RequirementsCreated} requirements";
            
            _logger.LogInformation("Procedure migration completed successfully");
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during procedure migration");
            await transaction.RollbackAsync(cancellationToken);
            
            result.Success = false;
            result.Message = $"Migration failed: {ex.Message}";
            result.Error = ex.ToString();
            
            return result;
        }
    }
}

public class MigrationResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Error { get; set; }
    public int OldProcedureCount { get; set; }
    public int RequirementsCreated { get; set; }
    public int RealizationsCreated { get; set; }
    public List<ProcedureMigrationMapping> Mappings { get; set; } = new();
}

public class ProcedureMigrationMapping
{
    public int OldProcedureId { get; set; }
    public int NewRequirementId { get; set; }
    public int NewRealizationId { get; set; }
}
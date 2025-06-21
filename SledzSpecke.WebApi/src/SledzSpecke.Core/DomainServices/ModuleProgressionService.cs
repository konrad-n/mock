using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Enums;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.DomainServices;

/// <summary>
/// Service responsible for managing module progression rules in specializations.
/// Ensures that modules follow the correct progression path (Basic -> Specialist).
/// </summary>
public interface IModuleProgressionService
{
    Task<Result<Module>> CanProgressToModuleAsync(
        Specialization specialization, 
        ValueObjects.ModuleType targetType,
        CancellationToken cancellationToken = default);
        
    Task<Result<bool>> ValidateModuleCompletionAsync(
        Module module,
        CancellationToken cancellationToken = default);
        
    Task<Result<ModuleProgressionStatus>> GetProgressionStatusAsync(
        Specialization specialization,
        CancellationToken cancellationToken = default);
}

public class ModuleProgressionService : IModuleProgressionService
{
    private readonly IInternshipRepository _internshipRepository;
    private readonly IMedicalShiftRepository _medicalShiftRepository;
    private readonly ICourseRepository _courseRepository;

    public ModuleProgressionService(
        IInternshipRepository internshipRepository,
        IMedicalShiftRepository medicalShiftRepository,
        ICourseRepository courseRepository)
    {
        _internshipRepository = internshipRepository;
        _medicalShiftRepository = medicalShiftRepository;
        _courseRepository = courseRepository;
    }

    public async Task<Result<Module>> CanProgressToModuleAsync(
        Specialization specialization, 
        ValueObjects.ModuleType targetType,
        CancellationToken cancellationToken = default)
    {
        // Validate specialization has modules
        if (specialization.Modules == null || !specialization.Modules.Any())
        {
            return Result<Module>.Failure(
                "Specialization has no modules defined",
                "NO_MODULES");
        }

        // Check if target module already exists
        var existingTargetModule = specialization.Modules
            .FirstOrDefault(m => (m.Type == Enums.ModuleType.Basic && targetType == ValueObjects.ModuleType.Basic) || 
                             (m.Type == Enums.ModuleType.Specialist && targetType == ValueObjects.ModuleType.Specialist));
            
        if (existingTargetModule != null)
        {
            return Result<Module>.Failure(
                $"{targetType} module already exists in this specialization",
                "MODULE_EXISTS");
        }

        // Validate progression rules
        if (targetType == ValueObjects.ModuleType.Specialist)
        {
            // Must complete Basic module before Specialist
            var basicModule = specialization.Modules
                .FirstOrDefault(m => m.Type == Enums.ModuleType.Basic);
                
            if (basicModule == null)
            {
                return Result<Module>.Failure(
                    "Basic module must be created before Specialist module",
                    "BASIC_MODULE_MISSING");
            }

            // Validate basic module completion
            var completionResult = await ValidateModuleCompletionAsync(basicModule, cancellationToken);
            if (completionResult.IsFailure || !completionResult.Value)
            {
                return Result<Module>.Failure(
                    "Basic module must be completed before progressing to Specialistic module",
                    "BASIC_MODULE_NOT_COMPLETED");
            }
        }

        // For SMK "new" version, additional validation may be required
        if (specialization.SmkVersion == Enums.SmkVersion.New)
        {
            // New SMK may have additional requirements
            // This is where we'd add them based on SMK documentation
        }

        // All validations passed - progression is allowed
        // Note: We don't create the module here, just validate if it's allowed
        return Result<Module>.Success(null!); // Null indicates validation passed
    }

    public async Task<Result<bool>> ValidateModuleCompletionAsync(
        Module module,
        CancellationToken cancellationToken = default)
    {
        // Check if all required internships are completed
        var internships = await _internshipRepository.GetByModuleIdAsync(new ModuleId(module.ModuleId));
            
        if (!internships.Any())
        {
            return Result<bool>.Success(false);
        }

        var incompleteInternships = internships
            .Where(i => !i.IsCompleted)
            .ToList();
            
        if (incompleteInternships.Any())
        {
            return Result<bool>.Success(false);
        }

        // Check if minimum hours are met
        var totalHours = 0;
        foreach (var internship in internships)
        {
            var shifts = await _medicalShiftRepository.GetByInternshipIdAsync(internship.InternshipId);
                
            totalHours += shifts
                .Where(s => s.IsApproved)
                .Sum(s => s.Hours);
        }

        // Module is considered complete if it has the expected progress
        var expectedInternships = module.TotalInternships;
        var completedInternships = module.CompletedInternships;
        
        if (completedInternships < expectedInternships)
        {
            return Result<bool>.Success(false);
        }

        // Check course completion
        var expectedCourses = module.TotalCourses;
        var completedCourses = module.CompletedCourses;
        
        if (completedCourses < expectedCourses)
        {
            return Result<bool>.Success(false);
        }

        return Result<bool>.Success(true);
    }

    public async Task<Result<ModuleProgressionStatus>> GetProgressionStatusAsync(
        Specialization specialization,
        CancellationToken cancellationToken = default)
    {
        var status = new ModuleProgressionStatus
        {
            SpecializationId = new SpecializationId(specialization.SpecializationId),
            SmkVersion = specialization.SmkVersion == Enums.SmkVersion.Old ? ValueObjects.SmkVersion.Old : ValueObjects.SmkVersion.New
        };

        if (specialization.Modules == null || !specialization.Modules.Any())
        {
            status.CurrentStage = ModuleProgressionStage.NotStarted;
            return Result<ModuleProgressionStatus>.Success(status);
        }

        // Check for Basic module
        var basicModule = specialization.Modules
            .FirstOrDefault(m => m.Type == Enums.ModuleType.Basic);
            
        if (basicModule == null)
        {
            status.CurrentStage = ModuleProgressionStage.NotStarted;
            return Result<ModuleProgressionStatus>.Success(status);
        }

        status.BasicModuleId = new ModuleId(basicModule.ModuleId);
        
        // Check Basic module completion
        var basicCompletionResult = await ValidateModuleCompletionAsync(
            basicModule, cancellationToken);
            
        if (basicCompletionResult.IsFailure)
        {
            return Result<ModuleProgressionStatus>.Failure(
                basicCompletionResult.Error,
                basicCompletionResult.ErrorCode);
        }

        status.BasicModuleCompleted = basicCompletionResult.Value;

        // Check for Specialist module
        var specialistModule = specialization.Modules
            .FirstOrDefault(m => m.Type == Enums.ModuleType.Specialist);
            
        if (specialistModule != null)
        {
            status.SpecialistModuleId = new ModuleId(specialistModule.ModuleId);
            
            var specialistCompletionResult = await ValidateModuleCompletionAsync(
                specialistModule, cancellationToken);
                
            if (specialistCompletionResult.IsSuccess)
            {
                status.SpecialistModuleCompleted = specialistCompletionResult.Value;
            }
        }

        // Determine current stage
        if (status.SpecialistModuleCompleted)
        {
            status.CurrentStage = ModuleProgressionStage.Completed;
        }
        else if (specialistModule != null)
        {
            status.CurrentStage = ModuleProgressionStage.InSpecialisticModule;
        }
        else if (status.BasicModuleCompleted)
        {
            status.CurrentStage = ModuleProgressionStage.BasicCompleted;
            status.CanProgressToSpecialistic = true;
        }
        else
        {
            status.CurrentStage = ModuleProgressionStage.InBasicModule;
        }

        // Calculate overall progress
        status.CalculateOverallProgress();

        return Result<ModuleProgressionStatus>.Success(status);
    }
}

/// <summary>
/// Represents the current status of module progression in a specialization.
/// </summary>
public class ModuleProgressionStatus
{
    public SpecializationId SpecializationId { get; set; } = null!;
    public ValueObjects.SmkVersion SmkVersion { get; set; }
    public ModuleProgressionStage CurrentStage { get; set; }
    public ModuleId? BasicModuleId { get; set; }
    public ModuleId? SpecialistModuleId { get; set; }
    public bool BasicModuleCompleted { get; set; }
    public bool SpecialistModuleCompleted { get; set; }
    public bool CanProgressToSpecialistic { get; set; }
    public decimal OverallProgressPercentage { get; set; }
    
    public void CalculateOverallProgress()
    {
        if (CurrentStage == ModuleProgressionStage.NotStarted)
        {
            OverallProgressPercentage = 0;
        }
        else if (CurrentStage == ModuleProgressionStage.InBasicModule)
        {
            OverallProgressPercentage = 25;
        }
        else if (CurrentStage == ModuleProgressionStage.BasicCompleted)
        {
            OverallProgressPercentage = 50;
        }
        else if (CurrentStage == ModuleProgressionStage.InSpecialisticModule)
        {
            OverallProgressPercentage = 75;
        }
        else if (CurrentStage == ModuleProgressionStage.Completed)
        {
            OverallProgressPercentage = 100;
        }
    }
}

/// <summary>
/// Represents the stages of module progression.
/// </summary>
public enum ModuleProgressionStage
{
    NotStarted,
    InBasicModule,
    BasicCompleted,
    InSpecialisticModule,
    Completed
}
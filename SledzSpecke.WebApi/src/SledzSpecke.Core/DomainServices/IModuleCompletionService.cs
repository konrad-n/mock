using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.DomainServices;

/// <summary>
/// Domain service for managing module-based progression in the New SMK system
/// </summary>
public interface IModuleCompletionService
{
    /// <summary>
    /// Validates if all requirements for a module are met
    /// </summary>
    Task<Result<ModuleCompletionStatus>> ValidateModuleCompletionAsync(
        InternshipId internshipId,
        ModuleId moduleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates weighted progress across all requirement types
    /// </summary>
    Task<Result<ModuleProgress>> CalculateWeightedProgressAsync(
        InternshipId internshipId,
        ModuleId moduleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Orchestrates module transition ensuring data integrity
    /// </summary>
    Task<Result> TransitionToNextModuleAsync(
        InternshipId internshipId,
        ModuleId currentModuleId,
        ModuleId nextModuleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Allocates procedures to specific internship requirements
    /// </summary>
    Task<Result<ProcedureAllocation>> AllocateProcedureToRequirementAsync(
        ProcedureId procedureId,
        InternshipId internshipId,
        ModuleRequirementId requirementId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if switching between basic and specialistic modules is allowed
    /// </summary>
    Task<Result<bool>> CanSwitchModuleTypeAsync(
        InternshipId internshipId,
        ModuleType targetType,
        CancellationToken cancellationToken = default);
}

public record ModuleCompletionStatus
{
    public ModuleId ModuleId { get; init; }
    public bool IsComplete { get; init; }
    public Dictionary<string, RequirementStatus> Requirements { get; init; } = new();
    public double OverallProgress { get; init; }
    public DateTime? EstimatedCompletionDate { get; init; }
}

public record RequirementStatus
{
    public string RequirementType { get; init; } = string.Empty;
    public int Required { get; init; }
    public int Completed { get; init; }
    public double PercentageComplete { get; init; }
}

public record ModuleProgress
{
    public double InternshipsProgress { get; init; } // 35% weight
    public double CoursesProgress { get; init; }     // 25% weight
    public double ProceduresProgress { get; init; }  // 30% weight
    public double MedicalShiftsProgress { get; init; } // 10% weight
    public double WeightedTotal { get; init; }
    
    public static ModuleProgress Calculate(
        double internships, 
        double courses, 
        double procedures, 
        double shifts)
    {
        return new ModuleProgress
        {
            InternshipsProgress = internships,
            CoursesProgress = courses,
            ProceduresProgress = procedures,
            MedicalShiftsProgress = shifts,
            WeightedTotal = (internships * 0.35) + (courses * 0.25) + 
                          (procedures * 0.30) + (shifts * 0.10)
        };
    }
}

public record ProcedureAllocation
{
    public ProcedureId ProcedureId { get; init; }
    public ModuleRequirementId AllocatedTo { get; init; }
    public string RequirementType { get; init; } = string.Empty;
    public int RemainingForRequirement { get; init; }
}

public record ModuleRequirementId(int Value);

public enum ModuleType
{
    Basic,
    Specialistic
}
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.DomainServices;

/// <summary>
/// Domain service for managing complex procedure requirements and allocations
/// </summary>
public interface IProcedureAllocationService
{
    /// <summary>
    /// Validates procedure code against specialization template
    /// </summary>
    Task<Result<ProcedureValidation>> ValidateProcedureAsync(
        string procedureCode,
        SpecializationId specializationId,
        SmkVersion smkVersion,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Allocates a procedure to the appropriate requirement (Type A vs Type B)
    /// </summary>
    Task<Result<AllocationResult>> AllocateProcedureAsync(
        ProcedureId procedureId,
        InternshipId internshipId,
        string operatorRole,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates that procedure is performed within valid internship for New SMK
    /// </summary>
    Task<Result<bool>> ValidateInternshipContextAsync(
        ProcedureId procedureId,
        InternshipId internshipId,
        ModuleId moduleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Tracks procedure completion against module/year requirements
    /// </summary>
    Task<Result<RequirementProgress>> TrackProcedureProgressAsync(
        InternshipId internshipId,
        string procedureCode,
        int count,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates operator roles and procedure groups
    /// </summary>
    Task<Result<OperatorValidation>> ValidateOperatorRoleAsync(
        string procedureCode,
        string operatorRole,
        SpecializationId specializationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets recommended procedures based on current progress
    /// </summary>
    Task<Result<List<ProcedureRecommendation>>> GetRecommendedProceduresAsync(
        InternshipId internshipId,
        CancellationToken cancellationToken = default);
}

public record ProcedureValidation
{
    public bool IsValid { get; init; }
    public string ProcedureCode { get; init; } = string.Empty;
    public string ProcedureName { get; init; } = string.Empty;
    public ProcedureCategory Category { get; init; }
    public List<string> ValidationErrors { get; init; } = new();
    public List<string> AllowedOperatorRoles { get; init; } = new();
    public int? MinimumRequired { get; init; }
    public int? MaximumAllowed { get; init; }
}

public record AllocationResult
{
    public ProcedureId ProcedureId { get; init; }
    public ProcedureType AllocatedType { get; init; }
    public string RequirementCode { get; init; } = string.Empty;
    public int RemainingRequired { get; init; }
    public bool IsOverLimit { get; init; }
    public string AllocationNote { get; init; } = string.Empty;
}

public record RequirementProgress
{
    public string RequirementCode { get; init; } = string.Empty;
    public string RequirementName { get; init; } = string.Empty;
    public int Required { get; init; }
    public int Completed { get; init; }
    public double PercentageComplete { get; init; }
    public List<ProcedureContribution> Contributions { get; init; } = new();
}

public record ProcedureContribution
{
    public string ProcedureCode { get; init; } = string.Empty;
    public int Count { get; init; }
    public DateTime LastPerformed { get; init; }
    public string OperatorRole { get; init; } = string.Empty;
}

public record OperatorValidation
{
    public bool IsValid { get; init; }
    public string OperatorRole { get; init; } = string.Empty;
    public bool CanPerformIndependently { get; init; }
    public bool RequiresSupervisor { get; init; }
    public int MinimumYearRequired { get; init; }
    public string ValidationMessage { get; init; } = string.Empty;
}

public record ProcedureRecommendation
{
    public string ProcedureCode { get; init; } = string.Empty;
    public string ProcedureName { get; init; } = string.Empty;
    public int Priority { get; init; }
    public int RemainingRequired { get; init; }
    public string RecommendationReason { get; init; } = string.Empty;
    public List<string> PreferredLocations { get; init; } = new();
}

public enum ProcedureType
{
    TypeA,  // Performed independently
    TypeB,  // Assisted
    TypeC   // Observed
}

public enum ProcedureCategory
{
    Basic,
    Advanced,
    Specialized,
    Emergency
}
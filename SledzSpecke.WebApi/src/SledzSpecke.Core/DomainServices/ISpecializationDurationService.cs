using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.DomainServices;

/// <summary>
/// Domain service for calculating and managing specialization duration adjustments
/// </summary>
public interface ISpecializationDurationService
{
    /// <summary>
    /// Calculates total duration extension based on all absences
    /// </summary>
    Task<Result<DurationExtension>> CalculateTotalExtensionAsync(
        UserId userId,
        SpecializationId specializationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates total duration reduction based on achievements
    /// </summary>
    Task<Result<DurationReduction>> CalculateTotalReductionAsync(
        UserId userId,
        SpecializationId specializationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates net duration adjustment and projected completion date
    /// </summary>
    Task<Result<DurationAdjustment>> CalculateNetAdjustmentAsync(
        UserId userId,
        SpecializationId specializationId,
        DateTime originalEndDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates that a proposed adjustment complies with SMK regulations
    /// </summary>
    Task<Result<ComplianceCheck>> ValidateAdjustmentComplianceAsync(
        DurationAdjustment adjustment,
        SmkVersion smkVersion,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates credit hours from completed courses for duration reduction
    /// </summary>
    Task<Result<CreditHoursSummary>> CalculateCreditHoursAsync(
        UserId userId,
        SpecializationId specializationId,
        CancellationToken cancellationToken = default);
}

public record DurationExtension
{
    public int TotalDays { get; init; }
    public List<ExtensionDetail> Details { get; init; } = new();
    public Dictionary<AbsenceType, int> DaysByType { get; init; } = new();
}

public record ExtensionDetail
{
    public AbsenceType Type { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public int Days { get; init; }
    public string Reason { get; init; } = string.Empty;
    public bool RequiresDocumentation { get; init; }
}

public record DurationReduction
{
    public int TotalDays { get; init; }
    public List<ReductionDetail> Details { get; init; } = new();
    public int PublicationDays { get; init; }
    public int RecognitionDays { get; init; }
    public int CourseCreditDays { get; init; }
}

public record ReductionDetail
{
    public string Type { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int Days { get; init; }
    public DateTime AchievementDate { get; init; }
    public double ImpactFactor { get; init; } // For publications
}

public record DurationAdjustment
{
    public UserId UserId { get; init; }
    public SpecializationId SpecializationId { get; init; }
    public DateTime OriginalEndDate { get; init; }
    public DateTime AdjustedEndDate { get; init; }
    public int NetAdjustmentDays { get; init; }
    public int ExtensionDays { get; init; }
    public int ReductionDays { get; init; }
    public string CalculationSummary { get; init; } = string.Empty;
}

public record ComplianceCheck
{
    public bool IsCompliant { get; init; }
    public List<ComplianceViolation> Violations { get; init; } = new();
    public DateTime MaxAllowedEndDate { get; init; }
    public int MaxAllowedExtensionDays { get; init; }
}

public record ComplianceViolation
{
    public string RuleCode { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Severity { get; init; } = "Warning";
}

public record CreditHoursSummary
{
    public int TotalCreditHours { get; init; }
    public int EligibleForReduction { get; init; }
    public int DaysReduction { get; init; }
    public List<CourseCredit> CourseDetails { get; init; } = new();
}

public record CourseCredit
{
    public string CourseName { get; init; } = string.Empty;
    public int CreditHours { get; init; }
    public DateTime CompletionDate { get; init; }
    public bool IsEligible { get; init; }
    public string IneligibilityReason { get; init; } = string.Empty;
}

public enum AbsenceType
{
    MaternityLeave,
    SickLeave,
    ChildcareLeave,
    UnpaidLeave,
    MilitaryService,
    Other
}
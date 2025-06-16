using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.DomainServices;

/// <summary>
/// Domain service for ensuring overall compliance with SMK regulations
/// </summary>
public interface IMedicalEducationComplianceService
{
    /// <summary>
    /// Validates that medical shifts don't exceed weekly/monthly limits
    /// </summary>
    Task<Result<ShiftComplianceReport>> ValidateShiftLimitsAsync(
        InternshipId internshipId,
        DateTime periodStart,
        DateTime periodEnd,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Ensures internship sequences follow SMK rules
    /// </summary>
    Task<Result<InternshipSequenceValidation>> ValidateInternshipSequenceAsync(
        UserId userId,
        SpecializationId specializationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates compliance reports for regulatory submissions
    /// </summary>
    Task<Result<ComplianceReport>> GenerateComplianceReportAsync(
        UserId userId,
        SpecializationId specializationId,
        ReportPeriod period,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Tracks quality metrics for specialization assessment
    /// </summary>
    Task<Result<QualityMetrics>> CalculateQualityMetricsAsync(
        UserId userId,
        SpecializationId specializationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates that all activities fall within specialization period
    /// </summary>
    Task<Result<PeriodValidation>> ValidateActivityPeriodsAsync(
        UserId userId,
        SpecializationId specializationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks readiness for final specialization examination
    /// </summary>
    Task<Result<ExaminationReadiness>> CheckExaminationReadinessAsync(
        UserId userId,
        SpecializationId specializationId,
        CancellationToken cancellationToken = default);
}

public record ShiftComplianceReport
{
    public bool IsCompliant { get; init; }
    public Dictionary<DateTime, WeeklyShiftSummary> WeeklySummaries { get; init; } = new();
    public Dictionary<DateTime, MonthlyShiftSummary> MonthlySummaries { get; init; } = new();
    public List<ShiftViolation> Violations { get; init; } = new();
}

public record WeeklyShiftSummary
{
    public DateTime WeekStart { get; init; }
    public int TotalHours { get; init; }
    public int MaxAllowedHours { get; init; } = 48;
    public int ShiftCount { get; init; }
    public bool ExceedsLimit { get; init; }
}

public record MonthlyShiftSummary
{
    public DateTime Month { get; init; }
    public int TotalHours { get; init; }
    public int RequiredHours { get; init; } = 160;
    public int ApprovedHours { get; init; }
    public double CompletionPercentage { get; init; }
}

public record ShiftViolation
{
    public DateTime Date { get; init; }
    public string ViolationType { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int ExcessHours { get; init; }
}

public record InternshipSequenceValidation
{
    public bool IsValid { get; init; }
    public List<InternshipSequenceItem> Sequence { get; init; } = new();
    public List<SequenceViolation> Violations { get; init; } = new();
    public DateTime? NextAllowedStartDate { get; init; }
}

public record InternshipSequenceItem
{
    public InternshipId InternshipId { get; init; }
    public string InstitutionName { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public bool HasOverlap { get; init; }
}

public record SequenceViolation
{
    public string ViolationType { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public InternshipId? InternshipId1 { get; init; }
    public InternshipId? InternshipId2 { get; init; }
}

public record ComplianceReport
{
    public UserId UserId { get; init; }
    public SpecializationId SpecializationId { get; init; }
    public ReportPeriod Period { get; init; }
    public DateTime GeneratedAt { get; init; }
    public ComplianceStatus OverallStatus { get; init; }
    public Dictionary<string, ComponentCompliance> Components { get; init; } = new();
    public List<ComplianceRecommendation> Recommendations { get; init; } = new();
}

public record ComponentCompliance
{
    public string ComponentName { get; init; } = string.Empty;
    public bool IsCompliant { get; init; }
    public double CompletionPercentage { get; init; }
    public List<string> Issues { get; init; } = new();
}

public record ComplianceRecommendation
{
    public string Area { get; init; } = string.Empty;
    public string Recommendation { get; init; } = string.Empty;
    public string Priority { get; init; } = "Medium";
    public DateTime? Deadline { get; init; }
}

public record QualityMetrics
{
    public double OverallScore { get; init; }
    public int PublicationPoints { get; init; }
    public int RecognitionPoints { get; init; }
    public int CoursePoints { get; init; }
    public int ProcedureComplexityScore { get; init; }
    public Dictionary<string, double> CategoryScores { get; init; } = new();
    public QualityLevel Level { get; init; }
}

public record PeriodValidation
{
    public bool AllActivitiesValid { get; init; }
    public DateTime SpecializationStart { get; init; }
    public DateTime SpecializationEnd { get; init; }
    public List<OutOfPeriodActivity> InvalidActivities { get; init; } = new();
}

public record OutOfPeriodActivity
{
    public string ActivityType { get; init; } = string.Empty;
    public DateTime ActivityDate { get; init; }
    public string Description { get; init; } = string.Empty;
    public int DaysOutOfBounds { get; init; }
}

public record ExaminationReadiness
{
    public bool IsReady { get; init; }
    public DateTime? EarliestExamDate { get; init; }
    public List<ExamRequirement> Requirements { get; init; } = new();
    public double OverallProgress { get; init; }
}

public record ExamRequirement
{
    public string RequirementName { get; init; } = string.Empty;
    public bool IsMet { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Details { get; init; } = string.Empty;
}

public record ReportPeriod
{
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public string PeriodType { get; init; } = "Annual";
}

public enum ComplianceStatus
{
    Compliant,
    PartiallyCompliant,
    NonCompliant,
    UnderReview
}

public enum QualityLevel
{
    Exceptional,
    AboveAverage,
    Average,
    BelowAverage,
    Insufficient
}
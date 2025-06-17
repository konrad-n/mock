using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Models.Statistics;

/// <summary>
/// Represents module progress statistics for medical education tracking
/// </summary>
public class ModuleProgressStatistics
{
    public InternshipId InternshipId { get; init; }
    public ModuleId ModuleId { get; init; }
    public string ModuleName { get; init; } = string.Empty;
    public ModuleType ModuleType { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime? CompletionDate { get; init; }
    public int DurationInMonths { get; init; }
    public int ElapsedMonths { get; init; }
    public decimal ProgressPercentage { get; init; }
    
    // Procedure progress
    public int TotalProceduresRequired { get; init; }
    public int TotalProceduresCompleted { get; init; }
    public decimal ProcedureCompletionRate { get; init; }
    
    // Shift hours progress
    public int RequiredHours { get; init; }
    public int CompletedHours { get; init; }
    public int ApprovedHours { get; init; }
    
    // Course progress
    public int RequiredCourses { get; init; }
    public int CompletedCourses { get; init; }
    public List<string> PendingCourses { get; init; } = new();
    
    // Overall status
    public ModuleStatus Status { get; init; }
    public bool IsOnTrack { get; init; }
    public int EstimatedDaysToCompletion { get; init; }
}

public enum ModuleStatus
{
    NotStarted,
    InProgress,
    Delayed,
    NearCompletion,
    Completed,
    Overdue
}

public class YearProgressStatistics
{
    public InternshipId InternshipId { get; init; }
    public int Year { get; init; }
    public DateTime YearStartDate { get; init; }
    public DateTime YearEndDate { get; init; }
    
    // Overall progress
    public decimal OverallProgressPercentage { get; init; }
    public List<ModuleProgressStatistics> ModulesProgress { get; init; } = new();
    
    // Aggregated metrics
    public int TotalHoursCompleted { get; init; }
    public int TotalHoursRequired { get; init; }
    public int TotalProceduresCompleted { get; init; }
    public int TotalProceduresRequired { get; init; }
    public int CoursesCompleted { get; init; }
    public int CoursesRequired { get; init; }
    
    // Compliance
    public bool MeetsYearRequirements { get; init; }
    public List<string> DeficientAreas { get; init; } = new();
    public DateTime? ProjectedCompletionDate { get; init; }
}
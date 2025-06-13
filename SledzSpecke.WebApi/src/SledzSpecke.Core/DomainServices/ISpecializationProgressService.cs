using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.DomainServices;

public interface ISpecializationProgressService
{
    Task<SpecializationProgressSummary> CalculateProgressAsync(UserId userId, SpecializationId specializationId);
    Task<DateTime> EstimateCompletionDateAsync(UserId userId, SpecializationId specializationId);
    Task<int> CalculateAdjustedDurationDaysAsync(UserId userId, SpecializationId specializationId);
    Task<WeightedProgressStatistics> CalculateWeightedProgressAsync(UserId userId, SpecializationId specializationId);
    Task<IEnumerable<ModuleProgressSummary>> GetModuleProgressAsync(UserId userId, SpecializationId specializationId);
    Task<CompletionProjection> ProjectCompletionAsync(UserId userId, SpecializationId specializationId);
}

public class SpecializationProgressSummary
{
    public double OverallProgressPercentage { get; set; }
    public int CompletedInternships { get; set; }
    public int TotalInternships { get; set; }
    public int CompletedCourses { get; set; }
    public int TotalCourses { get; set; }
    public int CompletedProcedures { get; set; }
    public int TotalProcedures { get; set; }
    public int TotalMedicalShiftHours { get; set; }
    public int RequiredMedicalShiftHours { get; set; }
    public int DaysExtended { get; set; }
    public int DaysReduced { get; set; }
    public int NetDurationAdjustment { get; set; }
    public DateTime EstimatedCompletionDate { get; set; }
    public DateTime OriginalCompletionDate { get; set; }
    public bool IsOnTrack { get; set; }
}

public class WeightedProgressStatistics
{
    public double WeightedProgressPercentage { get; set; }
    public double InternshipWeight { get; set; } = 0.4;
    public double CourseWeight { get; set; } = 0.3;
    public double ProcedureWeight { get; set; } = 0.2;
    public double SelfEducationWeight { get; set; } = 0.1;
    public double InternshipProgress { get; set; }
    public double CourseProgress { get; set; }
    public double ProcedureProgress { get; set; }
    public double SelfEducationProgress { get; set; }
    public int QualityScore { get; set; }
    public int TotalPublications { get; set; }
    public int TotalAbsences { get; set; }
    public int TotalRecognitions { get; set; }
}

public class ModuleProgressSummary
{
    public ModuleId ModuleId { get; set; }
    public string ModuleName { get; set; } = string.Empty;
    public double ProgressPercentage { get; set; }
    public int CompletedInternships { get; set; }
    public int RequiredInternships { get; set; }
    public int CompletedCourses { get; set; }
    public int RequiredCourses { get; set; }
    public int CompletedProcedures { get; set; }
    public int RequiredProcedures { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class CompletionProjection
{
    public DateTime EstimatedCompletionDate { get; set; }
    public int RemainingDays { get; set; }
    public double CompletionProbability { get; set; }
    public IEnumerable<string> RiskFactors { get; set; } = new List<string>();
    public IEnumerable<string> Recommendations { get; set; } = new List<string>();
    public bool RequiresIntervention { get; set; }
}
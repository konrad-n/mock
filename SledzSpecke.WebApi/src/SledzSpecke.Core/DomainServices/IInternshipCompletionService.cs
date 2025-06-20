using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.DomainServices;

public interface IInternshipCompletionService
{
    Task<Result<InternshipProgress>> CalculateProgressAsync(
        Internship internship,
        IEnumerable<MedicalShift> shifts,
        IEnumerable<ProcedureRealization> procedures);
    
    Task<Result<bool>> CanCompleteAsync(
        Internship internship,
        IEnumerable<MedicalShift> shifts,
        IEnumerable<ProcedureRealization> procedures);
    
    Task<Result> CompleteInternshipAsync(
        InternshipId internshipId,
        UserId completedBy,
        DateTime completionDate);
    
    Task<Result<InternshipCompletionRequirements>> GetCompletionRequirementsAsync(
        InternshipId internshipId);
    
    Task<Result<IEnumerable<InternshipMilestone>>> GetMilestonesAsync(
        InternshipId internshipId);
}

public class InternshipProgress
{
    public InternshipId InternshipId { get; set; }
    public string InternshipName { get; set; } = string.Empty;
    public double OverallProgressPercentage { get; set; }
    
    // Days
    public int CompletedDays { get; set; }
    public int RequiredDays { get; set; }
    public double DaysProgressPercentage { get; set; }
    
    // Medical Shifts
    public int CompletedShiftHours { get; set; }
    public int RequiredShiftHours { get; set; }
    public double ShiftProgressPercentage { get; set; }
    
    // Procedures
    public int CompletedProcedures { get; set; }
    public int RequiredProcedures { get; set; }
    public double ProcedureProgressPercentage { get; set; }
    
    // Status
    public bool MeetsAllRequirements { get; set; }
    public List<string> UnmetRequirements { get; set; } = new();
    public DateTime? EstimatedCompletionDate { get; set; }
}

public class InternshipCompletionRequirements
{
    public int MinimumDays { get; set; }
    public int MinimumShiftHours { get; set; }
    public int MinimumProcedures { get; set; }
    public bool RequiresSupervisorApproval { get; set; }
    public SmkVersion SmkVersion { get; set; }
    public List<string> SpecificRequirements { get; set; } = new();
}

public class InternshipMilestone
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double TargetPercentage { get; set; }
    public bool IsAchieved { get; set; }
    public DateTime? AchievedDate { get; set; }
    public MilestoneType Type { get; set; }
}

public enum MilestoneType
{
    Days25Percent,
    Days50Percent,
    Days75Percent,
    Days100Percent,
    FirstProcedure,
    HalfProcedures,
    AllProcedures,
    FirstWeekCompleted,
    FirstMonthCompleted,
    SupervisorApproval
}
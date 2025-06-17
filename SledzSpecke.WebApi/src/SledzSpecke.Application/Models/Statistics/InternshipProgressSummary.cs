using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Models.Statistics;

public class InternshipProgressSummary
{
    public InternshipId InternshipId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public InternshipStatus Status { get; set; }
    public decimal ProgressPercentage { get; set; }
    public int TotalHoursCompleted { get; set; }
    public int ApprovedHours { get; set; }
    public int RequiredHours { get; set; }
    public int TotalProcedures { get; set; }
    public int TotalShifts { get; set; }
    public int ModulesCompleted { get; set; }
    public int TotalModules { get; set; }
    public bool IsOnTrack { get; set; }
    public DateTime EstimatedCompletionDate { get; set; }
}
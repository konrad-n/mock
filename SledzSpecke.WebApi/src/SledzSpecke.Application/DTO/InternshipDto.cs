namespace SledzSpecke.Application.DTO;

public class InternshipDto
{
    public int Id { get; set; }
    public int SpecializationId { get; set; }
    public int? ModuleId { get; set; }
    public string InstitutionName { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string? SupervisorName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DaysCount { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsApproved { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string? ApproverName { get; set; }
    public string SyncStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int TotalShiftHours { get; set; }
    public int ApprovedProceduresCount { get; set; }
}
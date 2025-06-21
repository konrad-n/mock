using SledzSpecke.Core.Enums;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Core.Entities;

public class Internship
{
    public int InternshipId { get; set; }
    public int SpecializationId { get; set; }
    public int? ModuleId { get; set; }
    public string Name { get; set; }
    public string InstitutionName { get; set; }
    public string DepartmentName { get; set; }
    public string? SupervisorName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DaysCount { get; set; }
    public int PlannedWeeks { get; set; }
    public int PlannedDays { get; set; }
    public int CompletedDays { get; set; }
    public string Status { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsApproved { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string? ApproverName { get; set; }
    public SyncStatus SyncStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<MedicalShift> MedicalShifts { get; set; } = new List<MedicalShift>();
    public ICollection<ProcedureBase> Procedures { get; set; } = new List<ProcedureBase>();

    // Parameterless constructor for EF Core
    private Internship() { }

    // Factory method for creating new internship
    public static Internship Create(int specializationId, string name,
        string institutionName, string departmentName, DateTime startDate, DateTime endDate,
        int plannedWeeks, int plannedDays)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));
        if (string.IsNullOrEmpty(institutionName))
            throw new ArgumentNullException(nameof(institutionName));
        if (string.IsNullOrEmpty(departmentName))
            throw new ArgumentNullException(nameof(departmentName));
        if (endDate <= startDate)
            throw new ArgumentException("End date must be after start date.");
        if (plannedWeeks < 0)
            throw new ArgumentException("Planned weeks cannot be negative.", nameof(plannedWeeks));
        if (plannedDays < 0)
            throw new ArgumentException("Planned days cannot be negative.", nameof(plannedDays));

        return new Internship
        {
            SpecializationId = specializationId,
            Name = name,
            InstitutionName = institutionName,
            DepartmentName = departmentName,
            StartDate = startDate,
            EndDate = endDate,
            DaysCount = (int)(endDate - startDate).TotalDays,
            PlannedWeeks = plannedWeeks,
            PlannedDays = plannedDays,
            CompletedDays = 0,
            Status = "Planned",
            SyncStatus = SyncStatus.Unsynced,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsCompleted = false,
            IsApproved = false
        };
    }

    public void AssignToModule(int moduleId)
    {
        EnsureCanModify();
        ModuleId = moduleId;
        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public void SetSupervisor(string supervisorName)
    {
        EnsureCanModify();
        if (string.IsNullOrWhiteSpace(supervisorName))
            throw new ArgumentException("Supervisor name cannot be empty.", nameof(supervisorName));

        SupervisorName = supervisorName;
        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public void UpdateInstitution(string institutionName, string departmentName)
    {
        EnsureCanModify();

        if (string.IsNullOrWhiteSpace(institutionName))
            throw new ArgumentException("Institution name cannot be empty.", nameof(institutionName));

        if (string.IsNullOrWhiteSpace(departmentName))
            throw new ArgumentException("Department name cannot be empty.", nameof(departmentName));

        InstitutionName = institutionName;
        DepartmentName = departmentName;
        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public Result UpdateDates(DateTime startDate, DateTime endDate)
    {
        EnsureCanModify();

        if (endDate <= startDate)
            return Result.Failure("End date must be after start date.", "INVALID_DATE_RANGE");

        StartDate = startDate.ToUniversalTime();
        EndDate = endDate.ToUniversalTime();
        DaysCount = (int)(endDate - startDate).TotalDays;
        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
        
        return Result.Success();
    }

    public Result MarkAsCompleted()
    {
        EnsureCanModify();
        
        // Business rule: Cannot mark as completed before the end date
        if (DateTime.UtcNow.Date < EndDate.Date)
        {
            return Result.Failure("Cannot mark internship as completed before the end date.", "INTERN_NOT_ENDED");
        }
        
        IsCompleted = true;
        Status = "Completed";
        CompletedDays = DaysCount; // When completed, all planned days are considered completed
        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
        
        return Result.Success();
    }
    
    public void UpdateStatus(string status)
    {
        EnsureCanModify();
        Status = status;
        UpdatedAt = DateTime.UtcNow;
        
        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }
    
    public void UpdateCompletedDays(int completedDays)
    {
        EnsureCanModify();
        
        if (completedDays < 0)
            throw new ArgumentException("Completed days cannot be negative.", nameof(completedDays));
            
        if (completedDays > DaysCount)
            throw new ArgumentException("Completed days cannot exceed total days count.", nameof(completedDays));
            
        CompletedDays = completedDays;
        UpdatedAt = DateTime.UtcNow;
        
        // Update status based on completed days
        if (completedDays > 0 && Status == "Planned")
        {
            Status = "InProgress";
        }
        
        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public void Approve(string approverName)
    {
        if (string.IsNullOrWhiteSpace(approverName))
            throw new ArgumentException("Approver name cannot be empty.", nameof(approverName));

        IsApproved = true;
        ApprovalDate = DateTime.UtcNow;
        ApproverName = approverName;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        IsApproved = false;
        ApprovalDate = null;
        ApproverName = null;
        UpdatedAt = DateTime.UtcNow;
    }

    private void EnsureCanModify()
    {
        if (IsApproved)
            throw new InvalidOperationException("Cannot modify approved internship.");
    }
}

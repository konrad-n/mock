using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

public class Internship
{
    public InternshipId Id { get; private set; }
    public SpecializationId SpecializationId { get; private set; }
    public ModuleId? ModuleId { get; private set; }
    public string InstitutionName { get; private set; }
    public string DepartmentName { get; private set; }
    public string? SupervisorName { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int DaysCount { get; private set; }
    public bool IsCompleted { get; private set; }
    public bool IsApproved { get; private set; }
    public DateTime? ApprovalDate { get; private set; }
    public string? ApproverName { get; private set; }
    public SyncStatus SyncStatus { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<MedicalShift> _medicalShifts = new();
    public IReadOnlyList<MedicalShift> MedicalShifts => _medicalShifts.AsReadOnly();

    private readonly List<Procedure> _procedures = new();
    public IReadOnlyList<Procedure> Procedures => _procedures.AsReadOnly();

    private Internship(InternshipId id, SpecializationId specializationId, string institutionName,
        string departmentName, DateTime startDate, DateTime endDate)
    {
        Id = id;
        SpecializationId = specializationId;
        InstitutionName = institutionName;
        DepartmentName = departmentName;
        StartDate = startDate;
        EndDate = endDate;
        DaysCount = CalculateDaysCount(startDate, endDate);
        SyncStatus = SyncStatus.NotSynced;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Internship Create(InternshipId id, SpecializationId specializationId,
        string institutionName, string departmentName, DateTime startDate, DateTime endDate)
    {
        if (string.IsNullOrWhiteSpace(institutionName))
            throw new ArgumentException("Institution name cannot be empty.", nameof(institutionName));
        
        if (string.IsNullOrWhiteSpace(departmentName))
            throw new ArgumentException("Department name cannot be empty.", nameof(departmentName));
        
        if (endDate <= startDate)
            throw new InvalidDateRangeException();

        return new Internship(id, specializationId, institutionName, departmentName, startDate, endDate);
    }

    public void AssignToModule(ModuleId moduleId)
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
    
    public void UpdateDates(DateTime startDate, DateTime endDate)
    {
        EnsureCanModify();
        
        if (endDate <= startDate)
            throw new InvalidDateRangeException();
            
        StartDate = startDate;
        EndDate = endDate;
        DaysCount = CalculateDaysCount(startDate, endDate);
        UpdatedAt = DateTime.UtcNow;
        
        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public void MarkAsCompleted()
    {
        EnsureCanModify();
        IsCompleted = true;
        UpdatedAt = DateTime.UtcNow;
        
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

    public void AddMedicalShift(MedicalShift medicalShift)
    {
        EnsureCanModify();
        if (medicalShift.InternshipId != Id)
            throw new InvalidOperationException("Medical shift must belong to this internship.");
        
        _medicalShifts.Add(medicalShift);
        UpdatedAt = DateTime.UtcNow;
        
        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public void AddProcedure(Procedure procedure)
    {
        EnsureCanModify();
        if (procedure.InternshipId != Id)
            throw new InvalidOperationException("Procedure must belong to this internship.");
        
        _procedures.Add(procedure);
        UpdatedAt = DateTime.UtcNow;
        
        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public void UpdateSyncStatus(SyncStatus syncStatus)
    {
        SyncStatus = syncStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool CanBeDeleted()
    {
        return SyncStatus == SyncStatus.NotSynced || SyncStatus == SyncStatus.SyncFailed;
    }

    public int GetTotalShiftHours()
    {
        return _medicalShifts.Where(s => s.IsApproved).Sum(s => s.TotalMinutes) / 60;
    }

    public int GetApprovedProceduresCount()
    {
        return _procedures.Count(p => p.Status == ProcedureStatus.Approved);
    }

    /// <summary>
    /// Ensures the internship can be modified.
    /// Only approved internships cannot be modified (they are locked).
    /// Synced internships CAN be modified - they will automatically transition to Modified status.
    /// This is a key change from the original design where synced items were read-only.
    /// </summary>
    private void EnsureCanModify()
    {
        if (IsApproved)
            throw new InvalidOperationException("Cannot modify approved internship.");
        
        // IMPORTANT: Design Decision
        // Previously, synced items could not be modified at all (threw CannotModifySyncedDataException).
        // Now, synced items CAN be modified - they automatically transition to Modified status.
        // This allows users to correct/update synced data while maintaining the audit trail.
        // Only APPROVED items are truly read-only.
    }

    private static int CalculateDaysCount(DateTime startDate, DateTime endDate)
    {
        return (endDate - startDate).Days + 1;
    }
}
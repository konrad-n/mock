using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

/// <summary>
/// Enhanced Internship entity using value objects to eliminate primitive obsession
/// </summary>
public class InternshipEnhanced
{
    public InternshipId Id { get; private set; }
    public SpecializationId SpecializationId { get; private set; }
    public ModuleId? ModuleId { get; private set; }
    public InstitutionName InstitutionName { get; private set; }
    public DepartmentName DepartmentName { get; private set; }
    public PersonName? SupervisorName { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int DaysCount { get; private set; }
    public bool IsCompleted { get; private set; }
    public bool IsApproved { get; private set; }
    public DateTime? ApprovalDate { get; private set; }
    public PersonName? ApproverName { get; private set; }
    public SyncStatus SyncStatus { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<MedicalShift> _medicalShifts = new();
    public IReadOnlyList<MedicalShift> MedicalShifts => _medicalShifts.AsReadOnly();

    private readonly List<Procedure> _procedures = new();
    public IReadOnlyList<Procedure> Procedures => _procedures.AsReadOnly();

    private InternshipEnhanced(
        InternshipId id, 
        SpecializationId specializationId, 
        InstitutionName institutionName,
        DepartmentName departmentName, 
        DateTime startDate, 
        DateTime endDate)
    {
        Id = id;
        SpecializationId = specializationId;
        InstitutionName = institutionName;
        DepartmentName = departmentName;
        StartDate = EnsureUtc(startDate);
        EndDate = EnsureUtc(endDate);
        DaysCount = CalculateDaysCount(startDate, endDate);
        SyncStatus = SyncStatus.NotSynced;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static InternshipEnhanced Create(
        InternshipId id, 
        SpecializationId specializationId,
        string institutionName, 
        string departmentName, 
        DateTime startDate, 
        DateTime endDate)
    {
        if (endDate <= startDate)
            throw new InvalidDateRangeException();

        // Value objects handle their own validation
        var institution = new InstitutionName(institutionName);
        var department = new DepartmentName(departmentName);

        return new InternshipEnhanced(id, specializationId, institution, department, startDate, endDate);
    }

    public static InternshipEnhanced Create(
        InternshipId id, 
        SpecializationId specializationId,
        InstitutionName institutionName, 
        DepartmentName departmentName, 
        DateTime startDate, 
        DateTime endDate)
    {
        if (endDate <= startDate)
            throw new InvalidDateRangeException();

        return new InternshipEnhanced(id, specializationId, institutionName, departmentName, startDate, endDate);
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
        
        // Value object handles validation
        SupervisorName = new PersonName(supervisorName);
        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public void SetSupervisor(PersonName supervisorName)
    {
        EnsureCanModify();
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

        // Value objects handle validation
        InstitutionName = new InstitutionName(institutionName);
        DepartmentName = new DepartmentName(departmentName);
        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public void UpdateInstitution(InstitutionName institutionName, DepartmentName departmentName)
    {
        EnsureCanModify();
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

        StartDate = EnsureUtc(startDate);
        EndDate = EnsureUtc(endDate);
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
        // Value object handles validation
        var approverNameVO = new PersonName(approverName);
        
        IsApproved = true;
        ApprovalDate = DateTime.UtcNow;
        ApproverName = approverNameVO;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve(PersonName approverName)
    {
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
    /// Only approved internships cannot be modified.
    /// </summary>
    private void EnsureCanModify()
    {
        if (IsApproved)
            throw new InvalidOperationException("Cannot modify approved internship.");
    }

    private static int CalculateDaysCount(DateTime startDate, DateTime endDate)
    {
        return (endDate - startDate).Days + 1;
    }

    private static DateTime EnsureUtc(DateTime dateTime)
    {
        return dateTime.Kind switch
        {
            DateTimeKind.Utc => dateTime,
            DateTimeKind.Local => dateTime.ToUniversalTime(),
            DateTimeKind.Unspecified => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc),
            _ => dateTime
        };
    }
}
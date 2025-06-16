using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Events;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;
using System.Linq;

namespace SledzSpecke.Core.Entities;

public class Internship : AggregateRoot
{
    public override int Id { get => InternshipId.Value; protected set => InternshipId = new InternshipId(value); }
    public InternshipId InternshipId { get; private set; }
    public SpecializationId SpecializationId { get; private set; }
    public ModuleId? ModuleId { get; private set; }
    public string Name { get; private set; }
    public string InstitutionName { get; private set; }
    public string DepartmentName { get; private set; }
    public string? SupervisorName { get; private set; }
    public string? SupervisorPwz { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int DaysCount { get; private set; }
    public int PlannedWeeks { get; private set; }
    public int PlannedDays { get; private set; }
    public int CompletedDays { get; private set; }
    public InternshipStatus Status { get; private set; }
    public bool IsCompleted { get; private set; }
    public bool IsApproved { get; private set; }
    public DateTime? ApprovalDate { get; private set; }
    public string? ApproverName { get; private set; }
    public SyncStatus SyncStatus { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<MedicalShift> _medicalShifts = new();
    public IReadOnlyList<MedicalShift> MedicalShifts => _medicalShifts.AsReadOnly();

    private readonly List<ProcedureBase> _procedures = new();
    public IReadOnlyList<ProcedureBase> Procedures => _procedures.AsReadOnly();

    // Parameterless constructor for EF Core
    private Internship() { }

    private Internship(InternshipId id, SpecializationId specializationId, string name,
        string institutionName, string departmentName, DateTime startDate, DateTime endDate,
        int plannedWeeks, int plannedDays)
    {
        InternshipId = id;
        SpecializationId = specializationId;
        Name = name;
        InstitutionName = institutionName;
        DepartmentName = departmentName;
        StartDate = EnsureUtc(startDate);
        EndDate = EnsureUtc(endDate);
        DaysCount = CalculateDaysCount(startDate, endDate);
        PlannedWeeks = plannedWeeks;
        PlannedDays = plannedDays;
        CompletedDays = 0;
        Status = InternshipStatus.Planned;
        SyncStatus = SyncStatus.NotSynced;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Internship Create(InternshipId id, SpecializationId specializationId,
        string name, string institutionName, string departmentName, DateTime startDate, 
        DateTime endDate, int plannedWeeks, int plannedDays)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Internship name cannot be empty.", nameof(name));

        if (string.IsNullOrWhiteSpace(institutionName))
            throw new ArgumentException("Institution name cannot be empty.", nameof(institutionName));

        if (string.IsNullOrWhiteSpace(departmentName))
            throw new ArgumentException("Department name cannot be empty.", nameof(departmentName));

        if (endDate <= startDate)
            throw new InvalidDateRangeException();

        if (plannedWeeks < 0)
            throw new ArgumentException("Planned weeks cannot be negative.", nameof(plannedWeeks));

        if (plannedDays < 0)
            throw new ArgumentException("Planned days cannot be negative.", nameof(plannedDays));

        return new Internship(id, specializationId, name, institutionName, departmentName, 
            startDate, endDate, plannedWeeks, plannedDays);
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

    public void SetSupervisor(string supervisorName, string? supervisorPwz = null)
    {
        EnsureCanModify();
        if (string.IsNullOrWhiteSpace(supervisorName))
            throw new ArgumentException("Supervisor name cannot be empty.", nameof(supervisorName));

        SupervisorName = supervisorName;
        SupervisorPwz = supervisorPwz;
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
        DaysCount = CalculateDaysCount(startDate, endDate);
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
        Status = InternshipStatus.Completed;
        CompletedDays = DaysCount; // When completed, all planned days are considered completed
        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
        
        return Result.Success();
    }
    
    public void UpdateStatus(InternshipStatus status)
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
        if (completedDays > 0 && Status == InternshipStatus.Planned)
        {
            Status = InternshipStatus.InProgress;
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

    public Result<MedicalShift> AddMedicalShift(
        DateTime date,
        int hours,
        int minutes,
        string location,
        int year,
        SmkVersion smkVersion,
        int[] availableYears)
    {
        // Check if internship can be modified
        if (IsApproved)
            return Result.Failure<MedicalShift>("Cannot modify approved internship.", "INTERN_CANNOT_MODIFY");

        // Validate shift duration
        if (hours < 0)
            return Result.Failure<MedicalShift>("Hours cannot be negative.", "SHIFT_INVALID_DURATION");
        
        if (minutes < 0)
            return Result.Failure<MedicalShift>("Minutes cannot be negative.", "SHIFT_INVALID_DURATION");
        
        if (hours == 0 && minutes == 0)
            return Result.Failure<MedicalShift>("Shift duration must be greater than zero.", "SHIFT_INVALID_DURATION");

        // Validate location
        if (string.IsNullOrWhiteSpace(location))
            return Result.Failure<MedicalShift>("Location is required.", "VAL_REQUIRED_FIELD");
        
        if (location.Length > 100)
            return Result.Failure<MedicalShift>("Location name cannot exceed 100 characters.", "VAL_OUT_OF_RANGE");

        // Date range validation for New SMK
        if (smkVersion == SmkVersion.New)
        {
            if (date < StartDate || date > EndDate)
                return Result.Failure<MedicalShift>("Medical shift date must be within the internship period for New SMK.", "SHIFT_OUTSIDE_INTERNSHIP");
        }

        // Year validation based on SMK version
        if (smkVersion.IsOld)
        {
            // Old SMK: Allow year 0 (unassigned) or valid years from specialization
            if (year != 0 && (availableYears == null || !availableYears.Contains(year)))
            {
                var minYear = availableYears?.Min() ?? 1;
                var maxYear = availableYears?.Max() ?? 6;
                return Result.Failure<MedicalShift>($"Year must be 0 (unassigned) or between {minYear} and {maxYear} for this specialization.", "VAL_OUT_OF_RANGE");
            }
        }
        else if (smkVersion.IsNew)
        {
            // New SMK: Year must be provided (> 0)
            if (year <= 0)
                return Result.Failure<MedicalShift>("Year must be provided for New SMK.", "VAL_REQUIRED_FIELD");
        }

        // Create the medical shift
        var medicalShiftId = MedicalShiftId.New();
        var medicalShift = MedicalShift.Create(
            medicalShiftId,
            InternshipId,
            ModuleId,
            date,
            hours,
            minutes,
            ShiftType.Accompanying, // Default to accompanying shift
            location,
            SupervisorName, // Use internship's supervisor by default
            year
        );

        _medicalShifts.Add(medicalShift);
        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }

        // Add domain event
        AddDomainEvent(new MedicalShiftCreatedEvent(
            medicalShiftId,
            InternshipId,
            date,
            hours,
            minutes,
            location,
            year
        ));

        return Result.Success(medicalShift);
    }
    
    public Result<ProcedureOldSmk> AddProcedureOldSmk(
        ProcedureId procedureId,
        DateTime date,
        int year,
        string code,
        string name,
        string location,
        ProcedureExecutionType executionType,
        string supervisorName)
    {
        if (IsCompleted || IsApproved)
        {
            return Result<ProcedureOldSmk>.Failure("Cannot add procedure to a completed or approved internship.", "INTERNSHIP_NOT_MODIFIABLE");
        }

        // Note: SMK version validation should be done at the handler level since Internship doesn't track SMK version

        try
        {
            // Assuming ModuleId comes from a Module navigation property
            var moduleId = new ModuleId(1); // TODO: Get actual module ID from Module property
            var procedure = ProcedureOldSmk.Create(procedureId, moduleId, InternshipId, date, year, code, name, location, executionType, supervisorName);
            _procedures.Add(procedure);
            UpdatedAt = DateTime.UtcNow;

            // Automatically transition from Synced to Modified
            if (SyncStatus == SyncStatus.Synced)
            {
                SyncStatus = SyncStatus.Modified;
            }

            return Result<ProcedureOldSmk>.Success(procedure);
        }
        catch (Exception ex)
        {
            return Result<ProcedureOldSmk>.Failure($"Failed to add procedure: {ex.Message}", "PROCEDURE_CREATION_FAILED");
        }
    }

    public Result<ProcedureNewSmk> AddProcedureNewSmk(
        ProcedureId procedureId,
        DateTime date,
        string code,
        string location,
        ModuleId moduleId,
        int procedureRequirementId,
        string procedureName,
        ProcedureExecutionType executionType,
        string supervisorName)
    {
        if (IsCompleted || IsApproved)
        {
            return Result<ProcedureNewSmk>.Failure("Cannot add procedure to a completed or approved internship.", "INTERNSHIP_NOT_MODIFIABLE");
        }

        // Note: SMK version validation should be done at the handler level since Internship doesn't track SMK version

        try
        {
            var procedure = ProcedureNewSmk.Create(procedureId, moduleId, InternshipId, date, code, procedureName, location, executionType, supervisorName, procedureRequirementId);
            _procedures.Add(procedure);
            UpdatedAt = DateTime.UtcNow;

            // Automatically transition from Synced to Modified
            if (SyncStatus == SyncStatus.Synced)
            {
                SyncStatus = SyncStatus.Modified;
            }

            return Result<ProcedureNewSmk>.Success(procedure);
        }
        catch (Exception ex)
        {
            return Result<ProcedureNewSmk>.Failure($"Failed to add procedure: {ex.Message}", "PROCEDURE_CREATION_FAILED");
        }
    }

    // Keep the old method for backward compatibility temporarily
    [Obsolete("Use AddMedicalShift with Result pattern instead")]
    public void AddMedicalShiftLegacy(MedicalShift medicalShift)
    {
        EnsureCanModify();
        if (medicalShift.InternshipId != InternshipId)
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
        if (procedure.InternshipId != InternshipId)
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
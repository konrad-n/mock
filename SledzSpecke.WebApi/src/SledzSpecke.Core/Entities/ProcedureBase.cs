using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

public abstract class ProcedureBase
{
    public ProcedureId Id { get; protected set; }
    public int ModuleId { get; protected set; }
    public Module Module { get; protected set; }
    public int InternshipId { get; protected set; }
    public DateTime Date { get; protected set; }
    public int Year { get; protected set; }
    public string Code { get; protected set; }
    public string Name { get; protected set; }
    public DateTime PerformedDate { get; protected set; }
    public string Location { get; protected set; }
    public string? PatientInfo { get; protected set; }
    public ProcedureExecutionType ExecutionType { get; protected set; }
    public string SupervisorName { get; protected set; }
    public string? PerformingPerson { get; protected set; }
    public string? PatientInitials { get; protected set; }
    public char? PatientGender { get; protected set; }
    public string? AssistantData { get; protected set; }
    public string? ProcedureGroup { get; protected set; }
    public ProcedureStatus Status { get; protected set; }
    public SyncStatus SyncStatus { get; protected set; }
    public string? AdditionalFields { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }
    public SmkVersion SmkVersion { get; protected set; }

    public bool IsCompleted => Status == ProcedureStatus.Completed;
    public bool IsApproved => Status == ProcedureStatus.Approved;
    public bool CanBeModified => SyncStatus != SyncStatus.Synced || !IsApproved;
    public bool IsCodeA => ExecutionType == ProcedureExecutionType.CodeA;
    public bool IsCodeB => ExecutionType == ProcedureExecutionType.CodeB;

    protected ProcedureBase(ProcedureId id, int moduleId, int internshipId, DateTime date, int year,
        string code, string name, string location, ProcedureExecutionType executionType, 
        string supervisorName, ProcedureStatus status, SmkVersion smkVersion)
    {
        Id = id;
        ModuleId = moduleId;
        InternshipId = internshipId;
        Date = EnsureUtc(date);
        PerformedDate = EnsureUtc(date);
        Year = year;
        Code = code;
        Name = name;
        Location = location;
        ExecutionType = executionType;
        SupervisorName = supervisorName;
        Status = status;
        SyncStatus = SyncStatus.NotSynced;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        SmkVersion = smkVersion;
    }

    /// <summary>
    /// Updates procedure details. If the procedure is currently synced with SMK,
    /// it will automatically transition to Modified status to track the change.
    /// This allows synced data to be edited while maintaining audit trail.
    /// </summary>
    public virtual void UpdateProcedureDetails(ProcedureExecutionType executionType, string? performingPerson,
        string? patientInfo, string? patientInitials, char? patientGender)
    {
        EnsureCanModify();

        ExecutionType = executionType;
        PerformingPerson = performingPerson;
        PatientInfo = patientInfo;
        PatientInitials = patientInitials;
        PatientGender = patientGender;
        UpdatedAt = DateTime.UtcNow;

        // IMPORTANT: Sync Status Management
        // When a synced item is modified, we automatically transition it to Modified status.
        // This allows users to edit synced data while maintaining traceability.
        // The item remains linked to SMK but is marked as having local changes.
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public virtual void SetAssistantData(string? assistantData)
    {
        EnsureCanModify();
        AssistantData = assistantData;
        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public virtual void SetProcedureGroup(string? procedureGroup)
    {
        EnsureCanModify();
        ProcedureGroup = procedureGroup;
        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public virtual void ChangeStatus(ProcedureStatus newStatus)
    {
        EnsureCanModify();
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public virtual void Complete()
    {
        EnsureCanModify();
        Status = ProcedureStatus.Completed;
        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public virtual void Approve()
    {
        if (Status != ProcedureStatus.Completed)
            throw new InvalidOperationException("Cannot approve procedure that is not completed.");

        Status = ProcedureStatus.Approved;
        UpdatedAt = DateTime.UtcNow;
    }

    public virtual void Reject()
    {
        Status = ProcedureStatus.NotApproved;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetSyncStatus(SyncStatus status)
    {
        SyncStatus = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public virtual void SetAdditionalFields(string? additionalFields)
    {
        EnsureCanModify();
        AdditionalFields = additionalFields;
        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public virtual void UpdateSupervisor(string supervisorName)
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

    /// <summary>
    /// Ensures the procedure can be modified.
    /// Only approved procedures cannot be modified (they are locked).
    /// Synced procedures CAN be modified - they will automatically transition to Modified status.
    /// This is a key change from the original design where synced items were read-only.
    /// </summary>
    protected void EnsureCanModify()
    {
        if (IsApproved)
            throw new InvalidOperationException("Cannot modify approved procedure.");

        // IMPORTANT: Design Decision
        // Previously, synced items could not be modified at all (threw CannotModifySyncedDataException).
        // Now, synced items CAN be modified - they automatically transition to Modified status.
        // This allows users to correct/update synced data while maintaining the audit trail.
        // Only APPROVED items are truly read-only.
    }

    public abstract bool IsValidForSmkVersion();
    public abstract void ValidateSmkSpecificRules();

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
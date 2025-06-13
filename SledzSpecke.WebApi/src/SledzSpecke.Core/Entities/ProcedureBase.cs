using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

public abstract class ProcedureBase
{
    public ProcedureId Id { get; protected set; }
    public InternshipId InternshipId { get; protected set; }
    public DateTime Date { get; protected set; }
    public int Year { get; protected set; }
    public string Code { get; protected set; }
    public string? OperatorCode { get; protected set; }
    public string? PerformingPerson { get; protected set; }
    public string Location { get; protected set; }
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
    public bool CanBeModified => SyncStatus != SyncStatus.Synced;
    public bool IsTypeA => !string.IsNullOrEmpty(OperatorCode);
    public bool IsTypeB => string.IsNullOrEmpty(OperatorCode);

    protected ProcedureBase(ProcedureId id, InternshipId internshipId, DateTime date, int year, 
        string code, string location, ProcedureStatus status, SmkVersion smkVersion)
    {
        Id = id;
        InternshipId = internshipId;
        Date = date;
        Year = year;
        Code = code;
        Location = location;
        Status = status;
        SyncStatus = SyncStatus.NotSynced;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        SmkVersion = smkVersion;
    }

    public virtual void UpdateProcedureDetails(string? operatorCode, string? performingPerson,
        string? patientInitials, char? patientGender)
    {
        EnsureCanModify();

        OperatorCode = operatorCode;
        PerformingPerson = performingPerson;
        PatientInitials = patientInitials;
        PatientGender = patientGender;
        UpdatedAt = DateTime.UtcNow;
    }

    public virtual void SetAssistantData(string? assistantData)
    {
        EnsureCanModify();
        AssistantData = assistantData;
        UpdatedAt = DateTime.UtcNow;
    }

    public virtual void SetProcedureGroup(string? procedureGroup)
    {
        EnsureCanModify();
        ProcedureGroup = procedureGroup;
        UpdatedAt = DateTime.UtcNow;
    }

    public virtual void ChangeStatus(ProcedureStatus newStatus)
    {
        EnsureCanModify();
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    public virtual void Complete()
    {
        EnsureCanModify();
        Status = ProcedureStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
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
    }

    protected void EnsureCanModify()
    {
        if (SyncStatus == SyncStatus.Synced)
            throw new CannotModifySyncedDataException();
    }

    public abstract bool IsValidForSmkVersion();
    public abstract void ValidateSmkSpecificRules();
}
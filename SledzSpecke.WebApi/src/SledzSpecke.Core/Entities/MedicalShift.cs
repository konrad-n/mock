using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

public class MedicalShift
{
    public MedicalShiftId Id { get; private set; }
    public InternshipId InternshipId { get; private set; }
    public ModuleId? ModuleId { get; private set; }
    public DateTime Date { get; private set; }
    public Duration Duration { get; private set; }
    public ShiftType Type { get; private set; }
    public string Location { get; private set; }
    public string? SupervisorName { get; private set; }
    public int Year { get; private set; }
    public bool IsExempt { get; private set; }
    public string? ExemptionReason { get; private set; }
    public DateTime? ExemptionStartDate { get; private set; }
    public DateTime? ExemptionEndDate { get; private set; }
    public SyncStatus SyncStatus { get; private set; }
    public string? AdditionalFields { get; private set; }
    public DateTime? ApprovalDate { get; private set; }
    public string? ApproverName { get; private set; }
    public string? ApproverRole { get; private set; }

    public bool IsApproved => SyncStatus == SyncStatus.Synced && ApprovalDate.HasValue;
    public bool CanBeDeleted => !IsApproved;
    public int Hours => Duration.Hours;
    public int Minutes => Duration.Minutes;
    public int TotalMinutes => Duration.TotalMinutes;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Parameterless constructor for EF Core
    private MedicalShift() { }

    private MedicalShift(MedicalShiftId id, InternshipId internshipId, ModuleId? moduleId,
        DateTime date, Duration duration, ShiftType type, string location, string? supervisorName, int year)
    {
        Id = id;
        InternshipId = internshipId;
        ModuleId = moduleId;
        Date = EnsureUtc(date);
        Duration = duration ?? throw new ArgumentNullException(nameof(duration));
        Type = type;
        Location = location ?? throw new ArgumentNullException(nameof(location));
        SupervisorName = supervisorName;
        Year = year;
        IsExempt = false;
        SyncStatus = SyncStatus.NotSynced;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static MedicalShift Create(MedicalShiftId id, InternshipId internshipId, ModuleId? moduleId,
        DateTime date, int hours, int minutes, ShiftType type, string location, 
        string? supervisorName, int year)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be empty.", nameof(location));

        // No future date validation - MAUI app allows future dates
        var duration = new Duration(hours, minutes);

        return new MedicalShift(id, internshipId, moduleId, date, duration, type, 
            location, supervisorName, year);
    }

    /// <summary>
    /// Updates the medical shift details. If the shift is currently synced with SMK,
    /// it will automatically transition to Modified status to track the change.
    /// This allows synced data to be edited while maintaining audit trail.
    /// </summary>
    /// <param name="hours">Hours worked (0-23)</param>
    /// <param name="minutes">Minutes worked (0-59)</param>
    /// <param name="location">Location where the shift was performed</param>
    public void UpdateShiftDetails(int hours, int minutes, string location)
    {
        EnsureCanModify();

        Duration = new Duration(hours, minutes);
        Location = location ?? throw new ArgumentNullException(nameof(location));
        UpdatedAt = DateTime.UtcNow;

        // AI HINT: This is a KEY CHANGE from original MAUI implementation!
        // Original: Synced items were read-only (CanBeModified => SyncStatus != SyncStatus.Synced)
        // New: Synced items CAN be modified and auto-transition to Modified status
        // This change was specifically requested to allow editing of previously synced data
        // IMPORTANT: Sync Status Management
        // When a synced item is modified, we automatically transition it to Modified status.
        // This allows users to edit synced data while maintaining traceability.
        // The item remains linked to SMK but is marked as having local changes.
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public void SetSyncStatus(SyncStatus status)
    {
        SyncStatus = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve(string approverName, string approverRole)
    {
        if (SyncStatus != SyncStatus.Synced)
        {
            throw new InvalidOperationException("Medical shift must be synced before approval.");
        }

        ApprovalDate = DateTime.UtcNow;
        ApproverName = approverName ?? throw new ArgumentNullException(nameof(approverName));
        ApproverRole = approverRole ?? throw new ArgumentNullException(nameof(approverRole));
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAdditionalFields(string? additionalFields)
    {
        AdditionalFields = additionalFields;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void SetExemption(string reason, DateTime startDate, DateTime endDate)
    {
        EnsureCanModify();
        
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Exemption reason cannot be empty.", nameof(reason));
            
        if (endDate < startDate)
            throw new ArgumentException("Exemption end date must be after start date.");
            
        IsExempt = true;
        ExemptionReason = reason;
        ExemptionStartDate = EnsureUtc(startDate);
        ExemptionEndDate = EnsureUtc(endDate);
        UpdatedAt = DateTime.UtcNow;
        
        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }
    
    public void RemoveExemption()
    {
        EnsureCanModify();
        
        IsExempt = false;
        ExemptionReason = null;
        ExemptionStartDate = null;
        ExemptionEndDate = null;
        UpdatedAt = DateTime.UtcNow;
        
        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
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
    
    public void SetSupervisor(string? supervisorName)
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
    /// Ensures the medical shift can be modified. 
    /// Only approved shifts cannot be modified (they are locked).
    /// Synced shifts CAN be modified - they will automatically transition to Modified status.
    /// This is a key change from the original design where synced items were read-only.
    /// </summary>
    private void EnsureCanModify()
    {
        if (IsApproved)
            throw new InvalidOperationException("Cannot modify approved medical shift.");

        // IMPORTANT: Design Decision
        // Previously, synced items could not be modified at all (threw CannotModifySyncedDataException).
        // Now, synced items CAN be modified - they automatically transition to Modified status.
        // This allows users to correct/update synced data while maintaining the audit trail.
        // Only APPROVED items are truly read-only.
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
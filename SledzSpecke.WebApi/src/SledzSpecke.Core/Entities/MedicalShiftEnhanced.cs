using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

/// <summary>
/// Enhanced MedicalShift entity using value objects to eliminate primitive obsession
/// </summary>
public class MedicalShiftEnhanced
{
    public MedicalShiftId Id { get; private set; }
    public InternshipId InternshipId { get; private set; }
    public DateTime Date { get; private set; }
    public ShiftDuration Duration { get; private set; }
    public ShiftLocation Location { get; private set; }
    public Year Year { get; private set; }
    public SyncStatus SyncStatus { get; private set; }
    public string? AdditionalFields { get; private set; }
    public DateTime? ApprovalDate { get; private set; }
    public PersonName? ApproverName { get; private set; }
    public string? ApproverRole { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public bool IsApproved => SyncStatus == SyncStatus.Synced && ApprovalDate.HasValue;
    public bool CanBeDeleted => !IsApproved;

    private MedicalShiftEnhanced(
        MedicalShiftId id, 
        InternshipId internshipId, 
        DateTime date, 
        ShiftDuration duration,
        ShiftLocation location, 
        Year year)
    {
        Id = id;
        InternshipId = internshipId;
        Date = EnsureUtc(date);
        Duration = duration;
        Location = location;
        Year = year;
        SyncStatus = SyncStatus.NotSynced;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static MedicalShiftEnhanced Create(
        MedicalShiftId id, 
        InternshipId internshipId,
        DateTime date, 
        int hours, 
        int minutes, 
        string location, 
        int year)
    {
        // Value objects handle their own validation
        var duration = new ShiftDuration(hours, minutes);
        var shiftLocation = new ShiftLocation(location);
        var yearValue = new Year(year);

        return new MedicalShiftEnhanced(id, internshipId, date, duration, shiftLocation, yearValue);
    }

    public static MedicalShiftEnhanced Create(
        MedicalShiftId id, 
        InternshipId internshipId,
        DateTime date, 
        ShiftDuration duration, 
        ShiftLocation location, 
        Year year)
    {
        return new MedicalShiftEnhanced(id, internshipId, date, duration, location, year);
    }

    /// <summary>
    /// Updates the medical shift details. If the shift is currently synced with SMK,
    /// it will automatically transition to Modified status to track the change.
    /// </summary>
    public void UpdateShiftDetails(int hours, int minutes, string location)
    {
        EnsureCanModify();

        // Value objects handle validation
        Duration = new ShiftDuration(hours, minutes);
        Location = new ShiftLocation(location);
        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    /// <summary>
    /// Updates the medical shift details using value objects
    /// </summary>
    public void UpdateShiftDetails(ShiftDuration duration, ShiftLocation location)
    {
        EnsureCanModify();

        Duration = duration;
        Location = location;
        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public void UpdateDate(DateTime date)
    {
        EnsureCanModify();
        Date = EnsureUtc(date);
        UpdatedAt = DateTime.UtcNow;

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

        // Value object handles validation
        ApprovalDate = DateTime.UtcNow;
        ApproverName = new PersonName(approverName);
        ApproverRole = approverRole ?? throw new ArgumentNullException(nameof(approverRole));
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAdditionalFields(string? additionalFields)
    {
        AdditionalFields = additionalFields;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Ensures the medical shift can be modified. 
    /// Only approved shifts cannot be modified.
    /// </summary>
    private void EnsureCanModify()
    {
        if (IsApproved)
            throw new InvalidOperationException("Cannot modify approved medical shift.");
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
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

public class MedicalShift
{
    public MedicalShiftId Id { get; private set; }
    public InternshipId InternshipId { get; private set; }
    public DateTime Date { get; private set; }
    public int Hours { get; private set; }
    public int Minutes { get; private set; }
    public string Location { get; private set; }
    public int Year { get; private set; }
    public SyncStatus SyncStatus { get; private set; }
    public string? AdditionalFields { get; private set; }
    public DateTime? ApprovalDate { get; private set; }
    public string? ApproverName { get; private set; }
    public string? ApproverRole { get; private set; }

    public bool IsApproved => SyncStatus == SyncStatus.Synced && ApprovalDate.HasValue;
    public bool CanBeDeleted => !IsApproved;
    public TimeSpan Duration => new(Hours, Minutes, 0);
    public int TotalMinutes => Hours * 60 + Minutes;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private MedicalShift(MedicalShiftId id, InternshipId internshipId, DateTime date, int hours, int minutes,
        string location, int year)
    {
        Id = id;
        InternshipId = internshipId;
        Date = date;
        Hours = Math.Max(0, hours);
        Minutes = Math.Max(0, Math.Min(59, minutes));
        Location = location ?? throw new ArgumentNullException(nameof(location));
        Year = year;
        SyncStatus = SyncStatus.NotSynced;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static MedicalShift Create(MedicalShiftId id, InternshipId internshipId,
        DateTime date, int hours, int minutes, string location, int year)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be empty.", nameof(location));
        
        if (date > DateTime.UtcNow.Date)
            throw new ArgumentException("Shift date cannot be in the future.", nameof(date));

        return new MedicalShift(id, internshipId, date, hours, minutes, location, year);
    }

    public void UpdateShiftDetails(int hours, int minutes, string location)
    {
        EnsureCanModify();

        Hours = Math.Max(0, hours);
        Minutes = Math.Max(0, Math.Min(59, minutes));
        Location = location ?? throw new ArgumentNullException(nameof(location));
        UpdatedAt = DateTime.UtcNow;
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

    private void EnsureCanModify()
    {
        if (IsApproved)
            throw new InvalidOperationException("Cannot modify approved medical shift.");
        
        if (SyncStatus == SyncStatus.Synced)
            throw new CannotModifySyncedDataException();
    }
}
using SledzSpecke.Core.Enums;

namespace SledzSpecke.Core.Entities;

public class MedicalShift
{
    public int ShiftId { get; set; }
    public int InternshipId { get; set; }
    public int? ModuleId { get; set; }
    public DateTime Date { get; set; }
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public string Type { get; set; }
    public string Location { get; set; }
    public string? SupervisorName { get; set; }
    public int Year { get; set; } // For Stary SMK (1-6)
    public SmkVersion SmkVersion { get; set; }
    public SyncStatus SyncStatus { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string? ApproverName { get; set; }
    public string? ApproverRole { get; set; }
    public string? AdditionalFields { get; set; }
    
    // Properties for compatibility
    public bool IsApproved => SyncStatus == SyncStatus.Synced && ApprovalDate.HasValue;
    public bool CanBeDeleted => !IsApproved;
    public int TotalMinutes => Hours * 60 + Minutes;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Parameterless constructor for EF Core
    private MedicalShift() { }

    // Factory method for creating new shifts
    public static MedicalShift Create(int internshipId, int? moduleId,
        DateTime date, int hours, int minutes, string type, string location, 
        string? supervisorName, int year)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be empty.", nameof(location));
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Type cannot be empty.", nameof(type));
        if (hours < 0 || hours > 24)
            throw new ArgumentException("Hours must be between 0 and 24.", nameof(hours));
        if (minutes < 0)
            throw new ArgumentException("Minutes cannot be negative.", nameof(minutes));

        return new MedicalShift
        {
            InternshipId = internshipId,
            ModuleId = moduleId,
            Date = date.ToUniversalTime(),
            Hours = hours,
            Minutes = minutes,
            Type = type,
            Location = location,
            SupervisorName = supervisorName,
            Year = year,
            SyncStatus = SyncStatus.Unsynced,
            SmkVersion = SmkVersion.New, // Default to new
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void UpdateShiftDetails(int hours, int minutes, string location)
    {
        EnsureCanModify();

        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentNullException(nameof(location));
        if (hours < 0 || hours > 24)
            throw new ArgumentException("Hours must be between 0 and 24.", nameof(hours));
        if (minutes < 0)
            throw new ArgumentException("Minutes cannot be negative.", nameof(minutes));

        Hours = hours;
        Minutes = minutes;
        Location = location;
        UpdatedAt = DateTime.UtcNow;

        // When a synced item is modified, we automatically transition it to Modified status.
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
    
    private void EnsureCanModify()
    {
        if (IsApproved)
            throw new InvalidOperationException("Cannot modify approved medical shift.");
    }

}
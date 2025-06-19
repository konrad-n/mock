using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Core.Entities.Base;
using SledzSpecke.Core.DomainEvents;
using SledzSpecke.Core.DomainEvents.Base;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Core.Entities;

public class MedicalShift : Entity
{
    public MedicalShiftId Id { get; private set; }
    public InternshipId InternshipId { get; private set; }
    public ModuleId? ModuleId { get; private set; }
    public DateTime Date { get; private set; }
    public ShiftDuration Duration { get; private set; }
    public ShiftType Type { get; private set; }
    public string Location { get; private set; }
    public string? SupervisorName { get; private set; }
    public int Year { get; private set; } // For Stary SMK (1-6)
    public SmkVersion SmkVersion { get; private set; }
    public SyncStatus SyncStatus { get; private set; }
    public DateTime? ApprovalDate { get; private set; }
    public string? ApproverName { get; private set; }
    public string? ApproverRole { get; private set; }
    public string? AdditionalFields { get; private set; }
    
    // Properties for compatibility
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
        DateTime date, ShiftDuration duration, ShiftType type, string location, string? supervisorName, int year)
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
        SyncStatus = SyncStatus.NotSynced;
        SmkVersion = SmkVersion.New; // Default to new
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
        var duration = new ShiftDuration(hours, minutes);

        var shift = new MedicalShift(id, internshipId, moduleId, date, duration, type, 
            location, supervisorName, year);
        
        // Raise domain event
        shift.AddDomainEvent(new MedicalShiftCreated(
            shift.Id,
            shift.InternshipId,
            shift.Date,
            shift.Duration,
            shift.Type,
            shift.Location));
            
        return shift;
    }

    public void UpdateShiftDetails(int hours, int minutes, string location)
    {
        EnsureCanModify();

        var oldDuration = Duration;
        Duration = new ShiftDuration(hours, minutes);
        Location = location ?? throw new ArgumentNullException(nameof(location));
        UpdatedAt = DateTime.UtcNow;
        
        // Raise domain event if duration changed
        if (oldDuration.TotalMinutes != Duration.TotalMinutes)
        {
            AddDomainEvent(new MedicalShiftDurationChanged(
                Id,
                oldDuration,
                Duration));
        }

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
        
        // Raise domain event
        AddDomainEvent(new MedicalShiftCompleted(
            Id,
            InternshipId,
            Date,
            Duration,
            Type,
            ApprovalDate.Value));
    }
    
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
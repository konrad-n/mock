using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

public class EducationalActivity
{
    public EducationalActivityId Id { get; private set; }
    public SpecializationId SpecializationId { get; private set; }
    public ModuleId? ModuleId { get; private set; }
    public EducationalActivityType Type { get; private set; }
    public ActivityTitle Title { get; private set; }
    public Description? Description { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public SyncStatus SyncStatus { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private EducationalActivity(
        EducationalActivityId id,
        SpecializationId specializationId,
        EducationalActivityType type,
        ActivityTitle title,
        DateTime startDate,
        DateTime endDate)
    {
        Id = id;
        SpecializationId = specializationId;
        Type = type;
        Title = title;
        StartDate = EnsureUtc(startDate);
        EndDate = EnsureUtc(endDate);
        SyncStatus = SyncStatus.NotSynced;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static EducationalActivity Create(
        EducationalActivityId id,
        SpecializationId specializationId,
        EducationalActivityType type,
        string title,
        DateTime startDate,
        DateTime endDate)
    {
        if (endDate < startDate)
            throw new InvalidDateRangeException("End date cannot be before start date.");

        var titleValue = new ActivityTitle(title);
        return new EducationalActivity(id, specializationId, type, titleValue, startDate, endDate);
    }

    public void AssignToModule(ModuleId moduleId)
    {
        EnsureCanModify();
        ModuleId = moduleId;
        UpdatedAt = DateTime.UtcNow;

        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public void UpdateDetails(
        EducationalActivityType type,
        string title,
        string? description,
        DateTime startDate,
        DateTime endDate)
    {
        EnsureCanModify();

        if (endDate < startDate)
            throw new InvalidDateRangeException("End date cannot be before start date.");

        Type = type;
        Title = new ActivityTitle(title);
        Description = description != null ? new Description(description) : null;
        StartDate = EnsureUtc(startDate);
        EndDate = EnsureUtc(endDate);
        UpdatedAt = DateTime.UtcNow;

        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public void SetDescription(string? description)
    {
        EnsureCanModify();
        Description = description != null ? new Description(description) : null;
        UpdatedAt = DateTime.UtcNow;

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

    public TimeSpan GetDuration()
    {
        return EndDate - StartDate;
    }

    public bool IsOngoing()
    {
        var now = DateTime.UtcNow;
        return StartDate <= now && EndDate >= now;
    }

    public bool IsCompleted()
    {
        return EndDate < DateTime.UtcNow;
    }

    public bool IsUpcoming()
    {
        return StartDate > DateTime.UtcNow;
    }

    private void EnsureCanModify()
    {
        if (SyncStatus == SyncStatus.Synced)
        {
            throw new CannotModifySyncedDataException();
        }
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
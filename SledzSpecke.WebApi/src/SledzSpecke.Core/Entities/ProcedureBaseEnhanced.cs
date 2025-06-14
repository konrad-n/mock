using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Events;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

/// <summary>
/// Enhanced ProcedureBase following MySpot patterns
/// This is an example of how to refactor existing entities
/// </summary>
public abstract class ProcedureBaseEnhanced : AggregateRoot
{
    // Private fields for encapsulation
    private ProcedureId _id = default!;
    private InternshipId _internshipId = default!;
    private DateTime _date;
    private int _year;
    private string _code = string.Empty;
    private string? _operatorCode;
    private string? _performingPerson;
    private string _location = string.Empty;
    private string? _patientInitials;
    private char? _patientGender;
    private string? _assistantData;
    private string? _procedureGroup;
    private ProcedureStatus _status;
    private SyncStatus _syncStatus;
    private string? _additionalFields;
    private DateTime _createdAt;
    private DateTime _updatedAt;
    private SmkVersion _smkVersion;

    // Public properties with private setters
    public override int Id 
    { 
        get => _id.Value; 
        protected set => _id = value; 
    }
    
    public ProcedureId ProcedureId => _id;
    public InternshipId InternshipId => _internshipId;
    public DateTime Date => _date;
    public int Year => _year;
    public string Code => _code;
    public string? OperatorCode => _operatorCode;
    public string? PerformingPerson => _performingPerson;
    public string Location => _location;
    public string? PatientInitials => _patientInitials;
    public char? PatientGender => _patientGender;
    public string? AssistantData => _assistantData;
    public string? ProcedureGroup => _procedureGroup;
    public ProcedureStatus Status => _status;
    public SyncStatus SyncStatus => _syncStatus;
    public string? AdditionalFields => _additionalFields;
    public DateTime CreatedAt => _createdAt;
    public DateTime UpdatedAt => _updatedAt;
    public SmkVersion SmkVersion => _smkVersion;

    // Computed properties
    public bool IsCompleted => _status == ProcedureStatus.Completed;
    public bool IsApproved => _status == ProcedureStatus.Approved;
    public bool CanBeModified => _syncStatus != SyncStatus.Synced || !IsApproved;
    public bool IsTypeA => !string.IsNullOrEmpty(_operatorCode);
    public bool IsTypeB => string.IsNullOrEmpty(_operatorCode);

    protected ProcedureBaseEnhanced()
    {
        // Required for EF Core
    }

    protected ProcedureBaseEnhanced(
        ProcedureId id, 
        InternshipId internshipId, 
        DateTime date, 
        int year,
        string code, 
        string location, 
        ProcedureStatus status, 
        SmkVersion smkVersion)
    {
        // Guard clauses
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Procedure code cannot be empty");
        
        if (string.IsNullOrWhiteSpace(location))
            throw new DomainException("Procedure location cannot be empty");
        
        if (year < 1 || year > 6)
            throw new DomainException("Year must be between 1 and 6");

        _id = id ?? throw new ArgumentNullException(nameof(id));
        _internshipId = internshipId ?? throw new ArgumentNullException(nameof(internshipId));
        _date = EnsureUtc(date);
        _year = year;
        _code = code;
        _location = location;
        _status = status;
        _syncStatus = SyncStatus.NotSynced;
        _createdAt = DateTime.UtcNow;
        _updatedAt = DateTime.UtcNow;
        _smkVersion = smkVersion;

        // Raise domain event
        AddDomainEvent(new ProcedureCreatedEvent(
            id.Value, 
            code, 
            status.ToString(), 
            internshipId.Value));
    }

    /// <summary>
    /// Updates procedure details with rich validation and domain events
    /// </summary>
    public Result UpdateDetails(
        DateTime? date = null,
        string? code = null,
        string? location = null,
        string? performingPerson = null,
        string? patientInitials = null,
        char? patientGender = null)
    {
        // Business rule: Cannot modify synced and approved procedures
        if (!CanBeModified)
        {
            return Result.Failure("Cannot modify a synced and approved procedure");
        }

        // Business rule: Cannot change to past date if already completed
        if (date.HasValue && IsCompleted && date.Value < _date)
        {
            return Result.Failure("Cannot change completed procedure to an earlier date");
        }

        // Apply changes
        var hasChanges = false;

        if (date.HasValue && date.Value != _date)
        {
            _date = EnsureUtc(date.Value);
            hasChanges = true;
        }

        if (!string.IsNullOrWhiteSpace(code) && code != _code)
        {
            _code = code;
            hasChanges = true;
        }

        if (!string.IsNullOrWhiteSpace(location) && location != _location)
        {
            _location = location;
            hasChanges = true;
        }

        if (performingPerson != _performingPerson)
        {
            _performingPerson = performingPerson;
            hasChanges = true;
        }

        if (patientInitials != _patientInitials)
        {
            _patientInitials = patientInitials;
            hasChanges = true;
        }

        if (patientGender != _patientGender)
        {
            _patientGender = patientGender;
            hasChanges = true;
        }

        if (hasChanges)
        {
            _updatedAt = DateTime.UtcNow;
            
            // Update sync status if needed
            if (_syncStatus == SyncStatus.Synced)
            {
                _syncStatus = SyncStatus.Modified;
            }

            // Raise domain event
            AddDomainEvent(new ProcedureUpdatedEvent(
                _id.Value,
                _code,
                _updatedAt));
        }

        return Result.Success();
    }

    /// <summary>
    /// Changes procedure status with validation
    /// </summary>
    public Result ChangeStatus(ProcedureStatus newStatus)
    {
        if (_status == newStatus)
        {
            return Result.Success(); // No change needed
        }

        // Business rules for status transitions
        var transition = (_status, newStatus) switch
        {
            (ProcedureStatus.Pending, ProcedureStatus.Completed) => Result.Success(),
            (ProcedureStatus.Pending, ProcedureStatus.Approved) => Result.Failure("Cannot approve a pending procedure directly"),
            (ProcedureStatus.Completed, ProcedureStatus.Approved) => Result.Success(),
            (ProcedureStatus.Approved, _) => Result.Failure("Cannot change status of an approved procedure"),
            _ => Result.Failure($"Invalid status transition from {_status} to {newStatus}")
        };

        if (transition.IsFailure)
        {
            return transition;
        }

        var previousStatus = _status;
        _status = newStatus;
        _updatedAt = DateTime.UtcNow;

        // Raise appropriate domain event
        if (newStatus == ProcedureStatus.Completed)
        {
            AddDomainEvent(new ProcedureCompletedEvent(_id.Value, _code));
        }
        else if (newStatus == ProcedureStatus.Approved)
        {
            AddDomainEvent(new ProcedureApprovedEvent(_id.Value, _code));
        }

        return Result.Success();
    }

    /// <summary>
    /// Marks procedure as synced with external system
    /// </summary>
    public Result MarkAsSynced()
    {
        if (_syncStatus == SyncStatus.Synced)
        {
            return Result.Success(); // Already synced
        }

        _syncStatus = SyncStatus.Synced;
        _updatedAt = DateTime.UtcNow;

        AddDomainEvent(new ProcedureSyncedEvent(_id.Value, _code));

        return Result.Success();
    }

    private static DateTime EnsureUtc(DateTime dateTime)
    {
        return dateTime.Kind == DateTimeKind.Utc 
            ? dateTime 
            : DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
    }

    // Abstract methods for derived classes
    protected abstract void ValidateSpecificRules();
}

// Domain Events
public record ProcedureUpdatedEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
    public int ProcedureId { get; }
    public string Code { get; }
    public DateTime UpdatedAt { get; }

    public ProcedureUpdatedEvent(int procedureId, string code, DateTime updatedAt)
    {
        ProcedureId = procedureId;
        Code = code;
        UpdatedAt = updatedAt;
    }
}

public record ProcedureCompletedEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
    public int ProcedureId { get; }
    public string Code { get; }

    public ProcedureCompletedEvent(int procedureId, string code)
    {
        ProcedureId = procedureId;
        Code = code;
    }
}

public record ProcedureApprovedEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
    public int ProcedureId { get; }
    public string Code { get; }

    public ProcedureApprovedEvent(int procedureId, string code)
    {
        ProcedureId = procedureId;
        Code = code;
    }
}

public record ProcedureSyncedEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
    public int ProcedureId { get; }
    public string Code { get; }

    public ProcedureSyncedEvent(int procedureId, string code)
    {
        ProcedureId = procedureId;
        Code = code;
    }
}
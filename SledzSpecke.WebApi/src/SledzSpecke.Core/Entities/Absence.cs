using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

public class Absence
{
    public AbsenceId Id { get; private set; }
    public SpecializationId SpecializationId { get; private set; }
    public UserId UserId { get; private set; }
    public AbsenceType Type { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int DurationInDays { get; private set; }
    public string? Description { get; private set; }
    public string? DocumentPath { get; private set; }
    public bool IsApproved { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public UserId? ApprovedBy { get; private set; }
    public SyncStatus SyncStatus { get; private set; }
    public string? AdditionalFields { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public bool CanBeModified => SyncStatus != SyncStatus.Synced && !IsApproved;
    public bool ExtendsDuration => Type == AbsenceType.Sick || Type == AbsenceType.Maternity ||
                                   Type == AbsenceType.Paternity || Type == AbsenceType.Unpaid;
    public bool IsActive => DateTime.UtcNow.Date >= StartDate.Date && DateTime.UtcNow.Date <= EndDate.Date;

    private Absence(AbsenceId id, SpecializationId specializationId, UserId userId, AbsenceType type,
        DateTime startDate, DateTime endDate, string? description)
    {
        Id = id;
        SpecializationId = specializationId;
        UserId = userId;
        Type = type;
        StartDate = startDate;
        EndDate = endDate;
        DurationInDays = CalculateDurationInDays(startDate, endDate);
        Description = description;
        IsApproved = false;
        SyncStatus = SyncStatus.NotSynced;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Result<Absence> Create(AbsenceId id, SpecializationId specializationId, UserId userId,
        AbsenceType type, DateTime startDate, DateTime endDate, string? description = null)
    {
        var validationResult = ValidateInput(startDate, endDate);
        if (!validationResult.IsSuccess)
            return Result.Failure<Absence>(validationResult.Error, validationResult.ErrorCode);
            
        var absence = new Absence(id, specializationId, userId, type, startDate, endDate, description);
        return Result.Success(absence);
    }
    
    public static Result<Absence> CreateWithOverlapCheck(
        AbsenceId id, 
        SpecializationId specializationId, 
        UserId userId,
        AbsenceType type, 
        DateTime startDate, 
        DateTime endDate, 
        IEnumerable<Absence> existingAbsences,
        string? description = null)
    {
        var validationResult = ValidateInput(startDate, endDate);
        if (!validationResult.IsSuccess)
            return Result.Failure<Absence>(validationResult.Error, validationResult.ErrorCode);
            
        // Check for overlapping absences
        var hasOverlap = existingAbsences.Any(a => a.OverlapsWith(startDate, endDate));
        if (hasOverlap)
            return Result.Failure<Absence>("The specified date range overlaps with an existing absence.", "ABSENCE_OVERLAP");
            
        var absence = new Absence(id, specializationId, userId, type, startDate, endDate, description);
        return Result.Success(absence);
    }

    public Result UpdateDetails(AbsenceType type, DateTime startDate, DateTime endDate, string? description)
    {
        EnsureCanModify();
        
        var validationResult = ValidateInput(startDate, endDate);
        if (!validationResult.IsSuccess)
            return validationResult;

        Type = type;
        StartDate = startDate;
        EndDate = endDate;
        DurationInDays = CalculateDurationInDays(startDate, endDate);
        Description = description;
        UpdatedAt = DateTime.UtcNow;
        
        return Result.Success();
    }

    public void SetDocumentPath(string? documentPath)
    {
        EnsureCanModify();
        DocumentPath = documentPath;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve(UserId approvedBy)
    {
        if (IsApproved)
            throw new InvalidOperationException("Absence is already approved.");

        IsApproved = true;
        ApprovedAt = DateTime.UtcNow;
        ApprovedBy = approvedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Revoke()
    {
        if (!IsApproved)
            throw new InvalidOperationException("Cannot revoke approval for non-approved absence.");

        IsApproved = false;
        ApprovedAt = null;
        ApprovedBy = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetSyncStatus(SyncStatus status)
    {
        SyncStatus = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAdditionalFields(string? additionalFields)
    {
        EnsureCanModify();
        AdditionalFields = additionalFields;
        UpdatedAt = DateTime.UtcNow;
    }

    public int CalculateSpecializationExtensionDays()
    {
        if (!ExtendsDuration || !IsApproved)
            return 0;

        return Type switch
        {
            AbsenceType.Sick => Math.Min(DurationInDays, 90), // Max 90 days extension for sick leave
            AbsenceType.Maternity => DurationInDays, // Full maternity leave extends specialization
            AbsenceType.Paternity => DurationInDays, // Full paternity leave extends specialization
            AbsenceType.Unpaid => DurationInDays, // Unpaid leave extends specialization
            _ => 0
        };
    }

    public bool OverlapsWith(DateTime startDate, DateTime endDate)
    {
        return StartDate <= endDate && EndDate >= startDate;
    }

    public bool OverlapsWith(Absence other)
    {
        return OverlapsWith(other.StartDate, other.EndDate);
    }

    private void EnsureCanModify()
    {
        if (SyncStatus == SyncStatus.Synced)
            throw new CannotModifySyncedDataException();

        if (IsApproved)
            throw new InvalidOperationException("Cannot modify approved absence.");
    }

    private static int CalculateDurationInDays(DateTime startDate, DateTime endDate)
    {
        return (int)(endDate - startDate).TotalDays + 1;
    }

    private static Result ValidateInput(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            return Result.Failure("Start date cannot be after end date.", "INVALID_DATE_RANGE");

        if (endDate < DateTime.UtcNow.Date.AddDays(-365))
            return Result.Failure("End date cannot be more than a year in the past.", "DATE_TOO_OLD");

        if (startDate > DateTime.UtcNow.Date.AddDays(365))
            return Result.Failure("Start date cannot be more than a year in the future.", "DATE_TOO_FUTURE");
            
        return Result.Success();
    }
}
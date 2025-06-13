using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

public class Recognition
{
    public RecognitionId Id { get; private set; }
    public SpecializationId SpecializationId { get; private set; }
    public UserId UserId { get; private set; }
    public RecognitionType Type { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public string? Institution { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int DaysReduction { get; private set; }
    public bool IsApproved { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public UserId? ApprovedBy { get; private set; }
    public string? DocumentPath { get; private set; }
    public SyncStatus SyncStatus { get; private set; }
    public string? AdditionalFields { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public bool CanBeModified => SyncStatus != SyncStatus.Synced && !IsApproved;
    public bool ReducesSpecializationTime => IsApproved && DaysReduction > 0;
    public int DurationInDays => (int)(EndDate - StartDate).TotalDays + 1;

    private Recognition(RecognitionId id, SpecializationId specializationId, UserId userId,
        RecognitionType type, string title, DateTime startDate, DateTime endDate, int daysReduction)
    {
        Id = id;
        SpecializationId = specializationId;
        UserId = userId;
        Type = type;
        Title = title;
        StartDate = startDate;
        EndDate = endDate;
        DaysReduction = daysReduction;
        IsApproved = false;
        SyncStatus = SyncStatus.NotSynced;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Recognition Create(RecognitionId id, SpecializationId specializationId, UserId userId,
        RecognitionType type, string title, DateTime startDate, DateTime endDate, int daysReduction)
    {
        ValidateInput(title, startDate, endDate, daysReduction);
        return new Recognition(id, specializationId, userId, type, title, startDate, endDate, daysReduction);
    }

    public void UpdateDetails(RecognitionType type, string title, string? description, string? institution,
        DateTime startDate, DateTime endDate, int daysReduction)
    {
        EnsureCanModify();
        ValidateInput(title, startDate, endDate, daysReduction);

        Type = type;
        Title = title;
        Description = description;
        Institution = institution;
        StartDate = startDate;
        EndDate = endDate;
        DaysReduction = daysReduction;
        UpdatedAt = DateTime.UtcNow;
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
            throw new InvalidOperationException("Recognition is already approved.");

        ValidateForApproval();

        IsApproved = true;
        ApprovedAt = DateTime.UtcNow;
        ApprovedBy = approvedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Revoke()
    {
        if (!IsApproved)
            throw new InvalidOperationException("Cannot revoke approval for non-approved recognition.");

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

    public int CalculateSpecializationReductionDays()
    {
        if (!IsApproved)
            return 0;

        return Type switch
        {
            RecognitionType.PreviousEducation => Math.Min(DaysReduction, 365), // Max 1 year reduction
            RecognitionType.WorkExperience => Math.Min(DaysReduction, 730), // Max 2 years reduction
            RecognitionType.ForeignQualifications => Math.Min(DaysReduction, 1095), // Max 3 years reduction
            RecognitionType.Research => Math.Min(DaysReduction, 180), // Max 6 months reduction
            RecognitionType.Other => Math.Min(DaysReduction, 90), // Max 3 months reduction for other types
            _ => 0
        };
    }

    public bool OverlapsWith(DateTime startDate, DateTime endDate)
    {
        return StartDate <= endDate && EndDate >= startDate;
    }

    public bool OverlapsWith(Recognition other)
    {
        return OverlapsWith(other.StartDate, other.EndDate);
    }

    private void EnsureCanModify()
    {
        if (SyncStatus == SyncStatus.Synced)
            throw new CannotModifySyncedDataException();

        if (IsApproved)
            throw new InvalidOperationException("Cannot modify approved recognition.");
    }

    private void ValidateForApproval()
    {
        if (string.IsNullOrEmpty(Title))
            throw new InvalidOperationException("Title is required for approval.");

        if (DaysReduction <= 0)
            throw new InvalidOperationException("Days reduction must be greater than zero for approval.");

        if (string.IsNullOrEmpty(DocumentPath))
            throw new InvalidOperationException("Supporting documentation is required for approval.");
    }

    private static void ValidateInput(string title, DateTime startDate, DateTime endDate, int daysReduction)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));

        if (startDate > endDate)
            throw new InvalidDateRangeException("Start date cannot be after end date.");

        if (daysReduction < 0)
            throw new ArgumentException("Days reduction cannot be negative.", nameof(daysReduction));

        if (daysReduction > 1095) // 3 years max
            throw new ArgumentException("Days reduction cannot exceed 3 years (1095 days).", nameof(daysReduction));

        if (endDate < DateTime.UtcNow.Date.AddYears(-10))
            throw new ArgumentException("End date cannot be more than 10 years in the past.");
    }
}
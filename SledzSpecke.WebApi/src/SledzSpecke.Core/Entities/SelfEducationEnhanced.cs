using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

/// <summary>
/// Enhanced SelfEducation entity using value objects to eliminate primitive obsession
/// </summary>
public class SelfEducationEnhanced
{
    public SelfEducationId Id { get; private set; }
    public SpecializationId SpecializationId { get; private set; }
    public UserId UserId { get; private set; }
    public SelfEducationType Type { get; private set; }
    public Year Year { get; private set; }
    public SelfEducationTitle Title { get; private set; }
    public Description? Description { get; private set; }
    public ProviderName? Provider { get; private set; }
    public string? Publisher { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public int? DurationHours { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public FilePath? CertificatePath { get; private set; }
    public WebUrl? URL { get; private set; }
    public ISBN? ISBN { get; private set; }
    public DOI? DOI { get; private set; }
    public CreditHours CreditHours { get; private set; }
    public decimal? QualityScore { get; private set; }
    public SyncStatus SyncStatus { get; private set; }
    public string? AdditionalFields { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public bool CanBeModified => SyncStatus != SyncStatus.Synced;
    public bool HasCertificate => CertificatePath != null;
    public bool IsRecentlyCompleted => CompletedAt.HasValue && CompletedAt.Value >= DateTime.UtcNow.AddYears(-2);
    public CreditHours EffectiveCreditHours => IsCompleted ? CreditHours : CreditHours.Zero;

    private SelfEducationEnhanced(
        SelfEducationId id, 
        SpecializationId specializationId, 
        UserId userId,
        SelfEducationType type, 
        Year year, 
        SelfEducationTitle title, 
        CreditHours creditHours)
    {
        Id = id;
        SpecializationId = specializationId;
        UserId = userId;
        Type = type;
        Year = year;
        Title = title;
        CreditHours = creditHours;
        IsCompleted = false;
        SyncStatus = SyncStatus.NotSynced;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static SelfEducationEnhanced Create(
        SelfEducationId id, 
        SpecializationId specializationId, 
        UserId userId,
        SelfEducationType type, 
        int year, 
        string title, 
        int creditHours)
    {
        // Value objects handle their own validation
        var yearVO = new Year(year);
        var titleVO = new SelfEducationTitle(title);
        var creditHoursVO = new CreditHours(creditHours);

        return new SelfEducationEnhanced(id, specializationId, userId, type, yearVO, titleVO, creditHoursVO);
    }

    public void UpdateBasicDetails(
        SelfEducationType type, 
        int year, 
        string title, 
        string? description,
        string? provider, 
        string? publisher)
    {
        EnsureCanModify();

        // Value objects handle validation
        Type = type;
        Year = new Year(year);
        Title = new SelfEducationTitle(title);
        Description = description != null ? new Description(description) : null;
        Provider = provider != null ? new ProviderName(provider) : null;
        Publisher = publisher;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDuration(DateTime? startDate, DateTime? endDate, int? durationHours)
    {
        EnsureCanModify();

        if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            throw new InvalidDateRangeException("Start date cannot be after end date.");

        StartDate = startDate;
        EndDate = endDate;
        DurationHours = durationHours;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetCreditHours(int creditHours)
    {
        EnsureCanModify();
        
        // Value object handles validation
        CreditHours = new CreditHours(creditHours);
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDigitalReferences(string? url, string? isbn, string? doi)
    {
        EnsureCanModify();

        // Value objects handle validation
        URL = url != null ? new WebUrl(url) : null;
        ISBN = isbn != null ? new ISBN(isbn) : null;
        DOI = doi != null ? new DOI(doi) : null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete(DateTime? completedAt = null)
    {
        EnsureCanModify();

        if (IsCompleted)
            throw new InvalidOperationException("Self-education activity is already completed.");

        IsCompleted = true;
        CompletedAt = completedAt ?? DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkIncomplete()
    {
        EnsureCanModify();

        if (!IsCompleted)
            throw new InvalidOperationException("Self-education activity is not completed.");

        IsCompleted = false;
        CompletedAt = null;
        CertificatePath = null; // Remove certificate if marking as incomplete
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetCertificatePath(string? certificatePath)
    {
        EnsureCanModify();

        if (!IsCompleted && !string.IsNullOrEmpty(certificatePath))
            throw new InvalidOperationException("Cannot set certificate for incomplete activity.");

        CertificatePath = certificatePath != null ? new FilePath(certificatePath) : null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(
        string title, 
        string? description, 
        string? provider, 
        int creditHours, 
        DateTime? startDate, 
        DateTime? endDate, 
        int? durationHours)
    {
        EnsureCanModify();

        // Value objects handle validation
        Title = new SelfEducationTitle(title);
        Description = description != null ? new Description(description) : null;
        Provider = provider != null ? new ProviderName(provider) : null;
        CreditHours = new CreditHours(creditHours);
        StartDate = startDate;
        EndDate = endDate;
        DurationHours = durationHours;
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

    public int CalculateQualityScore()
    {
        int score = 0;

        // Base score by type
        score += Type switch
        {
            SelfEducationType.OnlineCourse => 8,
            SelfEducationType.Conference => 10,
            SelfEducationType.Workshop => 7,
            SelfEducationType.Webinar => 5,
            SelfEducationType.MedicalJournal => 9,
            SelfEducationType.ClinicalGuideline => 8,
            SelfEducationType.Research => 10,
            SelfEducationType.Book => 6,
            SelfEducationType.Article => 4,
            SelfEducationType.Literature => 5,
            SelfEducationType.Video => 3,
            SelfEducationType.Podcast => 3,
            _ => 2
        };

        // Completion bonus
        if (IsCompleted) score += 5;

        // Certificate bonus
        if (HasCertificate) score += 3;

        // Provider bonus (recognized institutions)
        if (Provider != null && Provider.IsRecognizedProvider())
            score += 2;

        // Digital identifier bonus
        if (DOI != null || ISBN != null)
            score += 1;

        // Duration bonus for substantial activities
        if (DurationHours.HasValue && DurationHours.Value >= 8)
            score += 2;

        return score;
    }

    public bool MeetsModuleRequirements(int requiredCreditHours)
    {
        return IsCompleted && CreditHours.MeetsRequirement(requiredCreditHours);
    }

    private void EnsureCanModify()
    {
        if (SyncStatus == SyncStatus.Synced)
            throw new CannotModifySyncedDataException();
    }
}
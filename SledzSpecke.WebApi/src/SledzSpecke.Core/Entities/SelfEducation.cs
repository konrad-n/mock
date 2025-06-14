using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

public class SelfEducation
{
    public SelfEducationId Id { get; private set; }
    public SpecializationId SpecializationId { get; private set; }
    public UserId UserId { get; private set; }
    public SelfEducationType Type { get; private set; }
    public int Year { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public string? Provider { get; private set; }
    public string? Publisher { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public int? DurationHours { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? CertificatePath { get; private set; }
    public string? URL { get; private set; }
    public string? ISBN { get; private set; }
    public string? DOI { get; private set; }
    public int CreditHours { get; private set; }
    public decimal? QualityScore { get; private set; }
    public SyncStatus SyncStatus { get; private set; }
    public string? AdditionalFields { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public bool CanBeModified => SyncStatus != SyncStatus.Synced;
    public bool HasCertificate => !string.IsNullOrEmpty(CertificatePath);
    public bool IsRecentlyCompleted => CompletedAt.HasValue && CompletedAt.Value >= DateTime.UtcNow.AddYears(-2);
    public int EffectiveCreditHours => IsCompleted ? CreditHours : 0;

    private SelfEducation(SelfEducationId id, SpecializationId specializationId, UserId userId,
        SelfEducationType type, int year, string title, int creditHours)
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

    public static SelfEducation Create(SelfEducationId id, SpecializationId specializationId, UserId userId,
        SelfEducationType type, int year, string title, int creditHours)
    {
        ValidateInput(title, year, creditHours);
        return new SelfEducation(id, specializationId, userId, type, year, title, creditHours);
    }

    public void UpdateBasicDetails(SelfEducationType type, int year, string title, string? description,
        string? provider, string? publisher)
    {
        EnsureCanModify();
        ValidateInput(title, year, CreditHours);

        Type = type;
        Year = year;
        Title = title;
        Description = description;
        Provider = provider;
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

        if (creditHours < 0)
            throw new ArgumentException("Credit hours cannot be negative.", nameof(creditHours));

        if (creditHours > 200)
            throw new ArgumentException("Credit hours cannot exceed 200 for a single activity.", nameof(creditHours));

        CreditHours = creditHours;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDigitalReferences(string? url, string? isbn, string? doi)
    {
        EnsureCanModify();

        URL = url;
        ISBN = isbn;
        DOI = doi;
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

        CertificatePath = certificatePath;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string title, string? description, string? provider, 
        int creditHours, DateTime? startDate, DateTime? endDate, int? durationHours)
    {
        EnsureCanModify();
        ValidateInput(title, Year, creditHours);

        Title = title;
        Description = description;
        Provider = provider;
        CreditHours = creditHours;
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
        if (!string.IsNullOrEmpty(Provider) && IsRecognizedProvider(Provider))
            score += 2;

        // Digital identifier bonus
        if (!string.IsNullOrEmpty(DOI) || !string.IsNullOrEmpty(ISBN))
            score += 1;

        // Duration bonus for substantial activities
        if (DurationHours.HasValue && DurationHours.Value >= 8)
            score += 2;

        return score;
    }

    public bool MeetsModuleRequirements(int requiredCreditHours)
    {
        return IsCompleted && CreditHours >= requiredCreditHours;
    }

    private void EnsureCanModify()
    {
        if (SyncStatus == SyncStatus.Synced)
            throw new CannotModifySyncedDataException();
    }

    private static bool IsRecognizedProvider(string provider)
    {
        var recognizedProviders = new[]
        {
            "Coursera", "edX", "Khan Academy", "Medscape", "UpToDate", "PubMed",
            "NEJM", "Lancet", "BMJ", "Mayo Clinic", "Harvard Medical School",
            "Johns Hopkins", "Stanford Medicine", "WHO", "CDC", "NIH"
        };

        return recognizedProviders.Any(rp => provider.Contains(rp, StringComparison.OrdinalIgnoreCase));
    }

    private static void ValidateInput(string title, int year, int creditHours)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));

        if (year < 1900 || year > DateTime.UtcNow.Year + 1)
            throw new ArgumentException("Year must be between 1900 and next year.", nameof(year));

        if (creditHours < 0)
            throw new ArgumentException("Credit hours cannot be negative.", nameof(creditHours));

        if (creditHours > 200)
            throw new ArgumentException("Credit hours cannot exceed 200 for a single activity.", nameof(creditHours));
    }
}
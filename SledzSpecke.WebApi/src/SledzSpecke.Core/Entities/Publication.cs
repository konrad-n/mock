using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

public class Publication
{
    public PublicationId Id { get; private set; }
    public SpecializationId SpecializationId { get; private set; }
    public UserId UserId { get; private set; }
    public PublicationType Type { get; private set; }
    public string Title { get; private set; }
    public string? Authors { get; private set; }
    public string? Journal { get; private set; }
    public string? Publisher { get; private set; }
    public DateTime PublicationDate { get; private set; }
    public string? Volume { get; private set; }
    public string? Issue { get; private set; }
    public string? Pages { get; private set; }
    public string? DOI { get; private set; }
    public string? PMID { get; private set; }
    public string? ISBN { get; private set; }
    public string? URL { get; private set; }
    public string? Abstract { get; private set; }
    public string? Keywords { get; private set; }
    public string? FilePath { get; private set; }
    public bool IsFirstAuthor { get; private set; }
    public bool IsCorrespondingAuthor { get; private set; }
    public bool IsPeerReviewed { get; private set; }
    public SyncStatus SyncStatus { get; private set; }
    public string? AdditionalFields { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public bool CanBeModified => SyncStatus != SyncStatus.Synced;
    public bool IsRecent => PublicationDate >= DateTime.UtcNow.AddYears(-5);
    public bool HasDigitalIdentifier => !string.IsNullOrEmpty(DOI) || !string.IsNullOrEmpty(PMID);

    private Publication(PublicationId id, SpecializationId specializationId, UserId userId,
        PublicationType type, string title, DateTime publicationDate)
    {
        Id = id;
        SpecializationId = specializationId;
        UserId = userId;
        Type = type;
        Title = title;
        PublicationDate = publicationDate;
        SyncStatus = SyncStatus.NotSynced;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Publication Create(PublicationId id, SpecializationId specializationId, UserId userId,
        PublicationType type, string title, DateTime publicationDate)
    {
        ValidateInput(title, publicationDate);
        return new Publication(id, specializationId, userId, type, title, publicationDate);
    }

    public void UpdateBasicDetails(PublicationType type, string title, DateTime publicationDate,
        string? authors = null, string? journal = null, string? publisher = null)
    {
        EnsureCanModify();
        ValidateInput(title, publicationDate);

        Type = type;
        Title = title;
        PublicationDate = publicationDate;
        Authors = authors;
        Journal = journal;
        Publisher = publisher;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePublicationDetails(string? volume, string? issue, string? pages, 
        string? doi, string? pmid, string? isbn, string? url)
    {
        EnsureCanModify();

        Volume = volume;
        Issue = issue;
        Pages = pages;
        DOI = doi;
        PMID = pmid;
        ISBN = isbn;
        URL = url;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateContent(string? abstractText, string? keywords)
    {
        EnsureCanModify();

        Abstract = abstractText;
        Keywords = keywords;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAuthorshipDetails(bool isFirstAuthor, bool isCorrespondingAuthor)
    {
        EnsureCanModify();

        IsFirstAuthor = isFirstAuthor;
        IsCorrespondingAuthor = isCorrespondingAuthor;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPeerReviewStatus(bool isPeerReviewed)
    {
        EnsureCanModify();

        IsPeerReviewed = isPeerReviewed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetFilePath(string? filePath)
    {
        EnsureCanModify();

        FilePath = filePath;
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

    public int CalculateImpactScore()
    {
        int score = 0;

        // Base score by type
        score += Type switch
        {
            PublicationType.Journal => IsPeerReviewed ? 10 : 5,
            PublicationType.Conference => IsPeerReviewed ? 7 : 4,
            PublicationType.Book => 8,
            PublicationType.Chapter => 6,
            PublicationType.Thesis => 5,
            PublicationType.Poster => 3,
            PublicationType.Abstract => 2,
            PublicationType.CaseReport => 4,
            PublicationType.Review => IsPeerReviewed ? 12 : 6,
            _ => 1
        };

        // Authorship bonus
        if (IsFirstAuthor) score += 3;
        if (IsCorrespondingAuthor) score += 2;

        // Digital identifier bonus
        if (HasDigitalIdentifier) score += 2;

        // Recency bonus
        if (IsRecent) score += 1;

        return score;
    }

    public string GetCitationFormat()
    {
        var citation = new List<string>();

        if (!string.IsNullOrEmpty(Authors))
            citation.Add(Authors);

        citation.Add($"\"{Title}\"");

        if (!string.IsNullOrEmpty(Journal))
            citation.Add(Journal);

        if (!string.IsNullOrEmpty(Volume))
            citation.Add($"Vol. {Volume}");

        if (!string.IsNullOrEmpty(Issue))
            citation.Add($"No. {Issue}");

        if (!string.IsNullOrEmpty(Pages))
            citation.Add($"pp. {Pages}");

        citation.Add(PublicationDate.ToString("yyyy"));

        return string.Join(", ", citation);
    }

    private void EnsureCanModify()
    {
        if (SyncStatus == SyncStatus.Synced)
            throw new CannotModifySyncedDataException();
    }

    private static void ValidateInput(string title, DateTime publicationDate)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));

        if (publicationDate > DateTime.UtcNow.Date)
            throw new ArgumentException("Publication date cannot be in the future.", nameof(publicationDate));

        if (publicationDate < DateTime.UtcNow.AddYears(-50))
            throw new ArgumentException("Publication date cannot be more than 50 years in the past.", nameof(publicationDate));
    }
}
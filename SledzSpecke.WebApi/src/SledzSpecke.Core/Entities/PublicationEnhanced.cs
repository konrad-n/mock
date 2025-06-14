using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

/// <summary>
/// Enhanced Publication entity using value objects to eliminate primitive obsession
/// </summary>
public class PublicationEnhanced
{
    public PublicationId Id { get; private set; }
    public SpecializationId SpecializationId { get; private set; }
    public UserId UserId { get; private set; }
    public PublicationType Type { get; private set; }
    public PublicationTitle Title { get; private set; }
    public string? Authors { get; private set; } // TODO: Create AuthorsList value object
    public JournalName? Journal { get; private set; }
    public InstitutionName? Publisher { get; private set; }
    public DateTime PublicationDate { get; private set; }
    public string? Volume { get; private set; } // TODO: Create JournalVolume value object
    public string? Issue { get; private set; } // TODO: Create JournalIssue value object
    public string? Pages { get; private set; } // TODO: Create PageRange value object
    public DOI? DOI { get; private set; }
    public PMID? PMID { get; private set; }
    public ISBN? ISBN { get; private set; }
    public WebUrl? URL { get; private set; }
    public Description Abstract { get; private set; }
    public string? Keywords { get; private set; } // TODO: Create KeywordsList value object
    public FilePath? FilePath { get; private set; }
    public bool IsFirstAuthor { get; private set; }
    public bool IsCorrespondingAuthor { get; private set; }
    public bool IsPeerReviewed { get; private set; }
    public decimal? ImpactFactor { get; private set; } // TODO: Create ImpactFactor value object
    public SyncStatus SyncStatus { get; private set; }
    public string? AdditionalFields { get; private set; } // TODO: Create JsonData value object
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public bool CanBeModified => SyncStatus != SyncStatus.Synced;
    public bool IsRecent => PublicationDate >= DateTime.UtcNow.AddYears(-5);
    public bool HasDigitalIdentifier => DOI != null || PMID != null;

    private PublicationEnhanced(
        PublicationId id, 
        SpecializationId specializationId, 
        UserId userId,
        PublicationType type, 
        PublicationTitle title, 
        DateTime publicationDate)
    {
        Id = id;
        SpecializationId = specializationId;
        UserId = userId;
        Type = type;
        Title = title;
        PublicationDate = publicationDate;
        Abstract = new Description(null); // Empty description by default
        SyncStatus = SyncStatus.NotSynced;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static PublicationEnhanced Create(
        PublicationId id, 
        SpecializationId specializationId, 
        UserId userId,
        PublicationType type, 
        string title, 
        DateTime publicationDate)
    {
        ValidatePublicationDate(publicationDate);
        
        var titleVO = new PublicationTitle(title);
        return new PublicationEnhanced(id, specializationId, userId, type, titleVO, publicationDate);
    }

    public void UpdateBasicDetails(
        PublicationType type, 
        string title, 
        DateTime publicationDate,
        string? authors = null, 
        string? journal = null, 
        string? publisher = null)
    {
        EnsureCanModify();
        ValidatePublicationDate(publicationDate);

        Type = type;
        Title = new PublicationTitle(title);
        PublicationDate = publicationDate;
        Authors = authors;
        Journal = journal != null ? new JournalName(journal) : null;
        Publisher = publisher != null ? new InstitutionName(publisher) : null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePublicationDetails(
        string? volume, 
        string? issue, 
        string? pages,
        string? doi, 
        string? pmid, 
        string? isbn, 
        string? url)
    {
        EnsureCanModify();

        Volume = volume;
        Issue = issue;
        Pages = pages;
        DOI = doi != null ? new DOI(doi) : null;
        PMID = pmid != null ? new PMID(pmid) : null;
        ISBN = isbn != null ? new ISBN(isbn) : null;
        URL = url != null ? new WebUrl(url) : null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateContent(string? abstractText, string? keywords)
    {
        EnsureCanModify();

        Abstract = new Description(abstractText);
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

    public void SetImpactFactor(decimal? impactFactor)
    {
        EnsureCanModify();

        if (impactFactor.HasValue && (impactFactor.Value < 0 || impactFactor.Value > 100))
        {
            throw new ArgumentException("Impact factor must be between 0 and 100.", nameof(impactFactor));
        }

        ImpactFactor = impactFactor;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetFilePath(string? filePath)
    {
        EnsureCanModify();

        FilePath = filePath != null ? new FilePath(filePath) : null;
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

        // Impact factor bonus
        if (ImpactFactor.HasValue)
        {
            score += ImpactFactor.Value switch
            {
                >= 10 => 5,
                >= 5 => 3,
                >= 2 => 2,
                _ => 1
            };
        }

        return score;
    }

    public string GetCitationFormat()
    {
        var citation = new List<string>();

        if (!string.IsNullOrEmpty(Authors))
            citation.Add(Authors);

        citation.Add($"\"{Title.Value}\"");

        if (Journal != null)
            citation.Add(Journal.Value);

        if (!string.IsNullOrEmpty(Volume))
            citation.Add($"Vol. {Volume}");

        if (!string.IsNullOrEmpty(Issue))
            citation.Add($"No. {Issue}");

        if (!string.IsNullOrEmpty(Pages))
            citation.Add($"pp. {Pages}");

        citation.Add(PublicationDate.ToString("yyyy"));

        if (DOI != null)
            citation.Add($"DOI: {DOI.Value}");

        return string.Join(", ", citation);
    }

    private void EnsureCanModify()
    {
        if (SyncStatus == SyncStatus.Synced)
            throw new CannotModifySyncedDataException();
    }

    private static void ValidatePublicationDate(DateTime publicationDate)
    {
        if (publicationDate > DateTime.UtcNow.Date)
            throw new ArgumentException("Publication date cannot be in the future.", nameof(publicationDate));

        if (publicationDate < DateTime.UtcNow.AddYears(-50))
            throw new ArgumentException("Publication date cannot be more than 50 years in the past.", nameof(publicationDate));
    }
}
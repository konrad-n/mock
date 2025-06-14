namespace SledzSpecke.Application.DTO;

public class PublicationDto
{
    public Guid Id { get; set; }
    public int SpecializationId { get; set; }
    public int UserId { get; set; }
    public string Type { get; set; }
    public string Title { get; set; }
    public string? Authors { get; set; }
    public string? Journal { get; set; }
    public string? Publisher { get; set; }
    public DateTime PublicationDate { get; set; }
    public string? Volume { get; set; }
    public string? Issue { get; set; }
    public string? Pages { get; set; }
    public string? DOI { get; set; }
    public string? PMID { get; set; }
    public string? ISBN { get; set; }
    public string? URL { get; set; }
    public string? Abstract { get; set; }
    public string? Keywords { get; set; }
    public string? FilePath { get; set; }
    public bool IsFirstAuthor { get; set; }
    public bool IsCorrespondingAuthor { get; set; }
    public bool IsPeerReviewed { get; set; }
    public decimal? ImpactFactor { get; set; }
    public double ImpactScore { get; set; }
    public string SyncStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
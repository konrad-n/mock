namespace SledzSpecke.Application.DTO;

public class SelfEducationDto
{
    public Guid Id { get; set; }
    public int SpecializationId { get; set; }
    public int UserId { get; set; }
    public string Type { get; set; }
    public int Year { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string? Provider { get; set; }
    public string? Publisher { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? DurationHours { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? CertificatePath { get; set; }
    public string? URL { get; set; }
    public string? ISBN { get; set; }
    public string? DOI { get; set; }
    public int CreditHours { get; set; }
    public double QualityScore { get; set; }
    public string SyncStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
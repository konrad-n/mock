namespace SledzSpecke.Application.DTO;

public class RecognitionDto
{
    public Guid Id { get; set; }
    public int SpecializationId { get; set; }
    public int UserId { get; set; }
    public string Type { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string? Institution { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DaysReduction { get; set; }
    public bool IsApproved { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public int? ApprovedBy { get; set; }
    public string? DocumentPath { get; set; }
    public string SyncStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
namespace SledzSpecke.Application.DTO;

public class EducationalActivityDto
{
    public int Id { get; set; }
    public int SpecializationId { get; set; }
    public int? ModuleId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string SyncStatus { get; set; } = string.Empty;
    public bool IsOngoing { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsUpcoming { get; set; }
    public double DurationDays { get; set; }
}
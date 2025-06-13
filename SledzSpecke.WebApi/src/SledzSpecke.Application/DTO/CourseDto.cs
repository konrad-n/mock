namespace SledzSpecke.Application.DTO;

public class CourseDto
{
    public int Id { get; set; }
    public int SpecializationId { get; set; }
    public int? ModuleId { get; set; }
    public string CourseType { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string? CourseNumber { get; set; }
    public string InstitutionName { get; set; } = string.Empty;
    public DateTime CompletionDate { get; set; }
    public bool HasCertificate { get; set; }
    public string? CertificateNumber { get; set; }
    public bool IsApproved { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string? ApproverName { get; set; }
    public string SyncStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsMandatory { get; set; }
}
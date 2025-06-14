namespace SledzSpecke.Application.DTO;

public class ModuleListDto
{
    public int Id { get; set; }
    public int SpecializationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string SmkVersion { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
}
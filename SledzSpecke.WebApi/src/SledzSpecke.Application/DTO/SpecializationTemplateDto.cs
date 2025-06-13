namespace SledzSpecke.Application.DTO;

public class SpecializationTemplateDto
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public int DurationYears { get; set; }
    public int TotalWorkingDays { get; set; }
    public string TargetGroup { get; set; } = string.Empty;
    public string SmkVersion { get; set; } = string.Empty;
    public int ModulesCount { get; set; }
    public int TotalCourses { get; set; }
    public int TotalInternships { get; set; }
    public int TotalProcedures { get; set; }
}
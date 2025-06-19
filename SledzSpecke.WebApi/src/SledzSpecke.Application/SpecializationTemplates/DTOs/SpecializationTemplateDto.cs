namespace SledzSpecke.Application.SpecializationTemplates.DTOs;

public sealed class SpecializationTemplateDto
{
    public int? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty; // "CMKP 2014" or "CMKP 2023"
    public TotalDurationDto TotalDuration { get; set; } = new();
    public int TotalWorkingDays { get; set; }
    public BasicInfoDto BasicInfo { get; set; } = new();
    public List<ModuleTemplateDto> Modules { get; set; } = new();
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed class TotalDurationDto
{
    public int Years { get; set; }
    public int Months { get; set; }
    public int Days { get; set; }
}

public sealed class BasicInfoDto
{
    public string TargetGroup { get; set; } = string.Empty;
    public string QualificationProcedure { get; set; } = string.Empty;
}

public sealed class ModuleTemplateDto
{
    public int ModuleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string ModuleType { get; set; } = string.Empty; // "Basic" or "Specialist"
    public string Version { get; set; } = string.Empty;
    public DurationDto Duration { get; set; } = new();
    public int WorkingDays { get; set; }
    public List<CourseTemplateDto> Courses { get; set; } = new();
    public List<InternshipTemplateDto> Internships { get; set; } = new();
    public List<ProcedureTemplateDto> Procedures { get; set; } = new();
}

public sealed class DurationDto
{
    public int Years { get; set; }
    public int Months { get; set; }
    public int Days { get; set; }
}

public sealed class CourseTemplateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Weeks { get; set; }
    public int WorkingDays { get; set; }
    public bool Required { get; set; }
}

public sealed class InternshipTemplateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Weeks { get; set; }
    public int WorkingDays { get; set; }
    public bool IsBasic { get; set; }
    public string? Location { get; set; }
}

public sealed class ProcedureTemplateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MinimumCount { get; set; }
    public string? Code { get; set; }
    public string? Category { get; set; }
    public bool IsRequired { get; set; }
}
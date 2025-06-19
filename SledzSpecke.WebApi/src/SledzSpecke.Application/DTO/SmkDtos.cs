namespace SledzSpecke.Application.DTO;

public class SmkValidationResultDto
{
    public int SpecializationId { get; set; }
    public string SmkVersion { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public DateTime ValidationDate { get; set; }
    public int TotalErrors { get; set; }
    public List<string> UserDataErrors { get; set; } = new();
    public List<string> MedicalShiftErrors { get; set; } = new();
    public List<string> ProcedureErrors { get; set; } = new();
    public List<string> ModuleErrors { get; set; } = new();
    public List<ModuleValidationInfo> ModuleValidations { get; set; } = new();
}

public class ModuleValidationInfo
{
    public int ModuleId { get; set; }
    public string ModuleName { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

public class SmkExportPreviewDto
{
    public int SpecializationId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string SpecializationName { get; set; } = string.Empty;
    public string SmkVersion { get; set; } = string.Empty;
    public int TotalInternships { get; set; }
    public int TotalCourses { get; set; }
    public int TotalMedicalShifts { get; set; }
    public int TotalProcedures { get; set; }
    public int TotalSelfEducationDays { get; set; }
    public string ValidationStatus { get; set; } = string.Empty;
    public List<string> ValidationWarnings { get; set; } = new();
}

public class SmkRequirementsDto
{
    public string SpecializationName { get; set; } = string.Empty;
    public string SmkVersion { get; set; } = string.Empty;
    public int DurationYears { get; set; }
    public List<ModuleRequirement> Modules { get; set; } = new();
    public List<ProcedureRequirement> RequiredProcedures { get; set; } = new();
    public List<CourseRequirement> RequiredCourses { get; set; } = new();
    public WeeklyHoursRequirement WeeklyHours { get; set; } = new();
    public int MonthlyHoursMinimum { get; set; }
}

public class ModuleRequirement
{
    public string ModuleType { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;
    public int DurationMonths { get; set; }
    public int RequiredInternships { get; set; }
    public int RequiredCourses { get; set; }
}

public class ProcedureRequirement
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int RequiredCountCodeA { get; set; }
    public int RequiredCountCodeB { get; set; }
}

public class CourseRequirement
{
    public string CourseType { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public bool RequiresCmkpCertificate { get; set; }
    public int MinimumHours { get; set; }
}

public class WeeklyHoursRequirement
{
    public int MinimumHours { get; set; }
    public int MaximumHours { get; set; }
    public int AverageHours { get; set; }
    public int AverageMinutes { get; set; }
}
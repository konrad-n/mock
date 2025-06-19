namespace SledzSpecke.Application.DTO;

public class DashboardOverviewDto
{
    public decimal OverallProgress { get; set; }
    public int CurrentModuleId { get; set; }
    public string CurrentModuleName { get; set; } = string.Empty;
    public DashboardModuleType ModuleType { get; set; }
    public SpecializationInfoDto Specialization { get; set; } = new();
    public ModuleProgressDto ModuleProgress { get; set; } = new();
    public int SelfEducationCount { get; set; }
    public int PublicationsCount { get; set; }
}

public class SpecializationInfoDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ProgramCode { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public int DurationYears { get; set; }
    public string SmkVersion { get; set; } = string.Empty;
}

public class ModuleProgressDto
{
    public DashboardInternshipProgressDto Internships { get; set; } = new();
    public DashboardCourseProgressDto Courses { get; set; } = new();
    public DashboardProcedureProgressDto Procedures { get; set; } = new();
    public DashboardMedicalShiftProgressDto MedicalShifts { get; set; } = new();
}

public enum DashboardModuleType
{
    Basic = 0,
    Specialist = 1
}
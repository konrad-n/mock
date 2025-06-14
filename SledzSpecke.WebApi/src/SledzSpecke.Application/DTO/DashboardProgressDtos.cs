namespace SledzSpecke.Application.DTO;

public class DashboardInternshipProgressDto
{
    public int Completed { get; set; }
    public int Required { get; set; }
    public int CompletedDays { get; set; }
    public int RequiredDays { get; set; }
    public decimal ProgressPercentage { get; set; }
}

public class DashboardCourseProgressDto
{
    public int Completed { get; set; }
    public int Required { get; set; }
    public decimal ProgressPercentage { get; set; }
}

public class DashboardProcedureProgressDto
{
    public int CompletedTypeA { get; set; }
    public int RequiredTypeA { get; set; }
    public int CompletedTypeB { get; set; }
    public int RequiredTypeB { get; set; }
    public decimal ProgressPercentage { get; set; }
}

public class DashboardMedicalShiftProgressDto
{
    public int CompletedHours { get; set; }
    public int RequiredHours { get; set; }
    public decimal ProgressPercentage { get; set; }
}
namespace SledzSpecke.Application.DTO;

public class SpecializationStatisticsDto
{
    public double OverallProgress { get; set; }
    public InternshipProgressDto InternshipProgress { get; set; } = new();
    public CourseProgressDto CourseProgress { get; set; } = new();
    public ProcedureProgressDto ProcedureProgress { get; set; } = new();
    public MedicalShiftProgressDto MedicalShiftProgress { get; set; } = new();
    public DateTime CalculatedAt { get; set; }
}

public class InternshipProgressDto
{
    public int Completed { get; set; }
    public int Total { get; set; }
    public double PercentageComplete { get; set; }
}

public class CourseProgressDto
{
    public int Completed { get; set; }
    public int Total { get; set; }
    public double PercentageComplete { get; set; }
}

public class ProcedureProgressDto
{
    public int CompletedTypeA { get; set; }
    public int TotalTypeA { get; set; }
    public int CompletedTypeB { get; set; }
    public int TotalTypeB { get; set; }
    public double PercentageComplete { get; set; }
}

public class MedicalShiftProgressDto
{
    public int CompletedHours { get; set; }
    public int RequiredHours { get; set; }
    public double PercentageComplete { get; set; }
}
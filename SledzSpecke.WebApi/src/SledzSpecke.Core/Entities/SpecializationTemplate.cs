namespace SledzSpecke.Core.Entities;

public class SpecializationTemplate
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public TotalDuration TotalDuration { get; set; } = new();
    public int TotalWorkingDays { get; set; }
    public BasicInfo BasicInfo { get; set; } = new();
    public List<ModuleTemplate> Modules { get; set; } = new();
    public SelfEducationInfo SelfEducation { get; set; } = new();
    public HolidaysInfo Holidays { get; set; } = new();
    public MedicalShiftsInfo MedicalShifts { get; set; } = new();
    public ProcedureCodeDescription ProcedureCodeDescription { get; set; } = new();
    public ExaminationInfo ExaminationInfo { get; set; } = new();
}

public class TotalDuration
{
    public int Years { get; set; }
    public int Months { get; set; }
    public int Days { get; set; }
}

public class BasicInfo
{
    public string TargetGroup { get; set; } = string.Empty;
    public string QualificationProcedure { get; set; } = string.Empty;
}

public class ModuleTemplate
{
    public int ModuleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string ModuleType { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public TotalDuration Duration { get; set; } = new();
    public int WorkingDays { get; set; }
    public List<CourseTemplate> Courses { get; set; } = new();
    public List<InternshipTemplate> Internships { get; set; } = new();
    public List<ProcedureTemplate> Procedures { get; set; } = new();
    public SelfEducationInfo SelfEducation { get; set; } = new();
    public HolidaysInfo Holidays { get; set; } = new();
    public MedicalShiftsInfo MedicalShifts { get; set; } = new();
    public ProcedureCodeDescription ProcedureCodeDescription { get; set; } = new();
    public ExaminationInfo? ExaminationInfo { get; set; }
}

public class CourseTemplate
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Weeks { get; set; }
    public int WorkingDays { get; set; }
    public bool Required { get; set; }
}

public class InternshipTemplate
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Weeks { get; set; }
    public int WorkingDays { get; set; }
    public bool IsBasic { get; set; }
    public string? Location { get; set; }
}

public class ProcedureTemplate
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int RequiredCountA { get; set; }
    public int RequiredCountB { get; set; }
    public int InternshipId { get; set; }
}

public class SelfEducationInfo
{
    public int DaysPerYear { get; set; }
    public int TotalDays { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class HolidaysInfo
{
    public int VacationDays { get; set; }
    public int NationalHolidays { get; set; }
    public int? ExamPreparationDays { get; set; }
}

public class MedicalShiftsInfo
{
    public double HoursPerWeek { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class ProcedureCodeDescription
{
    public string CodeA { get; set; } = string.Empty;
    public string CodeB { get; set; } = string.Empty;
}

public class ExaminationInfo
{
    public string ExamType { get; set; } = string.Empty;
    public List<ExaminationComponent> Components { get; set; } = new();
}

public class ExaminationComponent
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
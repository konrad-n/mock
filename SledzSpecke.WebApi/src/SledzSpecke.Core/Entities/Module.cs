using SledzSpecke.Core.Enums;

namespace SledzSpecke.Core.Entities;

public class Module
{
    public int ModuleId { get; set; }
    public int SpecializationId { get; set; }
    public ModuleType Type { get; set; }
    public SmkVersion SmkVersion { get; set; }
    public string Version { get; set; }
    public string Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Structure { get; set; }

    public int CompletedInternships { get; set; }
    public int TotalInternships { get; set; }
    public int CompletedCourses { get; set; }
    public int TotalCourses { get; set; }
    public int CompletedProceduresA { get; set; }
    public int TotalProceduresA { get; set; }
    public int CompletedProceduresB { get; set; }
    public int TotalProceduresB { get; set; }
    public int CompletedShiftHours { get; set; }
    public int RequiredShiftHours { get; set; }
    public double WeeklyShiftHours { get; set; }
    public int CompletedSelfEducationDays { get; set; }
    public int TotalSelfEducationDays { get; set; }

    // Navigation properties
    public ICollection<Internship> Internships { get; set; } = new List<Internship>();
    public ICollection<Course> Courses { get; set; } = new List<Course>();
    public ICollection<MedicalShift> MedicalShifts { get; set; } = new List<MedicalShift>();
    public ICollection<ProcedureBase> Procedures { get; set; } = new List<ProcedureBase>();
    public ICollection<SelfEducation> SelfEducations { get; set; } = new List<SelfEducation>();

    // Parameterless constructor for EF Core
    private Module() { }

    // Factory method for creating new module
    public static Module Create(int specializationId, ModuleType type, SmkVersion smkVersion,
        string version, string name, DateTime startDate, DateTime endDate, string structure)
    {
        if (string.IsNullOrEmpty(version))
            throw new ArgumentNullException(nameof(version));
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));
        if (string.IsNullOrEmpty(structure))
            throw new ArgumentNullException(nameof(structure));

        return new Module
        {
            SpecializationId = specializationId,
            Type = type,
            SmkVersion = smkVersion,
            Version = version,
            Name = name,
            StartDate = startDate,
            EndDate = endDate,
            Structure = structure,
            CompletedInternships = 0,
            TotalInternships = 0,
            CompletedCourses = 0,
            TotalCourses = 0,
            CompletedProceduresA = 0,
            TotalProceduresA = 0,
            CompletedProceduresB = 0,
            TotalProceduresB = 0,
            CompletedShiftHours = 0,
            RequiredShiftHours = 0,
            WeeklyShiftHours = 0,
            CompletedSelfEducationDays = 0,
            TotalSelfEducationDays = 0
        };
    }

    public void UpdateProgress(int completedInternships, int totalInternships, int completedCourses, int totalCourses)
    {
        CompletedInternships = Math.Max(0, completedInternships);
        TotalInternships = Math.Max(0, totalInternships);
        CompletedCourses = Math.Max(0, completedCourses);
        TotalCourses = Math.Max(0, totalCourses);
    }

    public void UpdateProceduresProgress(int completedA, int totalA, int completedB, int totalB)
    {
        CompletedProceduresA = Math.Max(0, completedA);
        TotalProceduresA = Math.Max(0, totalA);
        CompletedProceduresB = Math.Max(0, completedB);
        TotalProceduresB = Math.Max(0, totalB);
    }

    public void UpdateShiftHours(int completed, int required, double weekly)
    {
        CompletedShiftHours = Math.Max(0, completed);
        RequiredShiftHours = Math.Max(0, required);
        WeeklyShiftHours = Math.Max(0, weekly);
    }

    public void UpdateSelfEducation(int completed, int total)
    {
        CompletedSelfEducationDays = Math.Max(0, completed);
        TotalSelfEducationDays = Math.Max(0, total);
    }

    public bool IsCompleted()
    {
        return CompletedInternships >= TotalInternships &&
               CompletedCourses >= TotalCourses &&
               CompletedProceduresA >= TotalProceduresA &&
               CompletedProceduresB >= TotalProceduresB &&
               CompletedShiftHours >= RequiredShiftHours &&
               CompletedSelfEducationDays >= TotalSelfEducationDays;
    }

    public double GetOverallProgress()
    {
        var metrics = new[]
        {
            TotalInternships > 0 ? (double)CompletedInternships / TotalInternships : 1.0,
            TotalCourses > 0 ? (double)CompletedCourses / TotalCourses : 1.0,
            TotalProceduresA > 0 ? (double)CompletedProceduresA / TotalProceduresA : 1.0,
            TotalProceduresB > 0 ? (double)CompletedProceduresB / TotalProceduresB : 1.0,
            RequiredShiftHours > 0 ? (double)CompletedShiftHours / RequiredShiftHours : 1.0,
            TotalSelfEducationDays > 0 ? (double)CompletedSelfEducationDays / TotalSelfEducationDays : 1.0
        };

        return metrics.Average();
    }
}
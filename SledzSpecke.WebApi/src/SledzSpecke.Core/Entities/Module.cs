using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

public class Module
{
    public ModuleId Id { get; private set; }
    public SpecializationId SpecializationId { get; private set; }
    public ModuleType Type { get; private set; }
    public SmkVersion SmkVersion { get; private set; }
    public string Version { get; private set; }
    public string Name { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public string Structure { get; private set; }

    public int CompletedInternships { get; private set; }
    public int TotalInternships { get; private set; }
    public int CompletedCourses { get; private set; }
    public int TotalCourses { get; private set; }
    public int CompletedProceduresA { get; private set; }
    public int TotalProceduresA { get; private set; }
    public int CompletedProceduresB { get; private set; }
    public int TotalProceduresB { get; private set; }
    public int CompletedShiftHours { get; private set; }
    public int RequiredShiftHours { get; private set; }
    public double WeeklyShiftHours { get; private set; }
    public int CompletedSelfEducationDays { get; private set; }
    public int TotalSelfEducationDays { get; private set; }

    // Navigation properties
    public ICollection<Internship> Internships { get; private set; } = new List<Internship>();
    public ICollection<Course> Courses { get; private set; } = new List<Course>();
    public ICollection<MedicalShift> MedicalShifts { get; private set; } = new List<MedicalShift>();
    public ICollection<ProcedureBase> Procedures { get; private set; } = new List<ProcedureBase>();
    public ICollection<SelfEducation> SelfEducations { get; private set; } = new List<SelfEducation>();

    public Module(ModuleId id, SpecializationId specializationId, ModuleType type, SmkVersion smkVersion,
        string version, string name, DateTime startDate, DateTime endDate, string structure)
    {
        Id = id;
        SpecializationId = specializationId;
        Type = type;
        SmkVersion = smkVersion;
        Version = version ?? throw new ArgumentNullException(nameof(version));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        StartDate = startDate;
        EndDate = endDate;
        Structure = structure ?? throw new ArgumentNullException(nameof(structure));
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
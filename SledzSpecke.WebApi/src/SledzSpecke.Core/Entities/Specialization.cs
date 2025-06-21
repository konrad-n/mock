using SledzSpecke.Core.Enums;

namespace SledzSpecke.Core.Entities;

public class Specialization
{
    public int SpecializationId { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }
    public string ProgramCode { get; set; }
    public SmkVersion SmkVersion { get; set; }
    public string ProgramVariant { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public DateTime CalculatedEndDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public int PlannedPesYear { get; set; }
    public string Status { get; set; }
    public string ProgramStructure { get; set; }
    public int? CurrentModuleId { get; set; }
    public int DurationYears { get; set; }
    public int DurationInDays => DurationYears * 365;
    
    // Module tracking
    public bool HasBasicModule { get; set; }
    public bool HasSpecializedModule { get; set; }
    public DateTime? BasicModuleCompletionDate { get; set; }

    public ICollection<Module> Modules { get; set; } = new List<Module>();

    // Parameterless constructor for EF Core
    private Specialization() { }

    // Factory method for creating new specialization
    public static Specialization Create(
        int userId,
        string name, 
        string programCode, 
        SmkVersion smkVersion,
        string programVariant,
        DateTime startDate, 
        DateTime plannedEndDate, 
        int plannedPesYear,
        string programStructure, 
        int durationYears)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));
        if (string.IsNullOrEmpty(programCode))
            throw new ArgumentNullException(nameof(programCode));
        if (string.IsNullOrEmpty(programVariant))
            throw new ArgumentNullException(nameof(programVariant));
        if (string.IsNullOrEmpty(programStructure))
            throw new ArgumentNullException(nameof(programStructure));

        return new Specialization
        {
            UserId = userId,
            Name = name,
            ProgramCode = programCode,
            SmkVersion = smkVersion,
            ProgramVariant = programVariant,
            StartDate = startDate,
            PlannedEndDate = plannedEndDate,
            PlannedPesYear = plannedPesYear,
            ProgramStructure = programStructure,
            DurationYears = durationYears,
            CalculatedEndDate = startDate.AddYears(durationYears),
            Status = "Active"
        };
    }

    public void AddModule(Module module)
    {
        if (Modules.Any(m => m.ModuleId == module.ModuleId))
        {
            throw new InvalidOperationException($"Module with ID {module.ModuleId} already exists.");
        }

        Modules.Add(module);
    }

    public void SetCurrentModule(int moduleId)
    {
        if (!Modules.Any(m => m.ModuleId == moduleId))
        {
            throw new InvalidOperationException($"Module with ID {moduleId} does not exist in this specialization.");
        }

        CurrentModuleId = moduleId;
    }

    public (int Completed, int Total) GetInternshipProgress()
    {
        var completed = Modules.Sum(m => m.CompletedInternships);
        var total = Modules.Sum(m => m.TotalInternships);
        return (completed, total);
    }

    public (int Completed, int Total) GetCoursesProgress()
    {
        var completed = Modules.Sum(m => m.CompletedCourses);
        var total = Modules.Sum(m => m.TotalCourses);
        return (completed, total);
    }

    public void Complete(DateTime completionDate)
    {
        if (Status == "Completed")
        {
            throw new InvalidOperationException("Specialization is already completed.");
        }

        if (completionDate < StartDate)
        {
            throw new ArgumentException("Completion date cannot be before start date.");
        }

        ActualEndDate = completionDate;
        Status = "Completed";
    }

    public void Suspend()
    {
        if (Status == "Completed")
        {
            throw new InvalidOperationException("Cannot suspend a completed specialization.");
        }

        if (Status == "Cancelled")
        {
            throw new InvalidOperationException("Cannot suspend a cancelled specialization.");
        }

        Status = "Suspended";
    }

    public void Resume()
    {
        if (Status != "Suspended")
        {
            throw new InvalidOperationException("Can only resume a suspended specialization.");
        }

        Status = "Active";
    }

    public void Cancel()
    {
        if (Status == "Completed")
        {
            throw new InvalidOperationException("Cannot cancel a completed specialization.");
        }

        Status = "Cancelled";
        ActualEndDate = DateTime.UtcNow;
    }
    
    public void MarkBasicModuleCompleted(DateTime completionDate)
    {
        if (HasBasicModule)
        {
            throw new InvalidOperationException("Basic module is already marked as completed.");
        }
        
        HasBasicModule = true;
        BasicModuleCompletionDate = completionDate;
    }
    
    public void MarkSpecializedModuleCompleted()
    {
        if (!HasBasicModule)
        {
            throw new InvalidOperationException("Cannot complete specialized module before basic module.");
        }
        
        if (HasSpecializedModule)
        {
            throw new InvalidOperationException("Specialized module is already marked as completed.");
        }
        
        HasSpecializedModule = true;
    }
    
    public bool IsReadyForSpecializedModule()
    {
        return HasBasicModule && !HasSpecializedModule;
    }
    
    public bool AreAllModulesCompleted()
    {
        return HasBasicModule && HasSpecializedModule;
    }
}
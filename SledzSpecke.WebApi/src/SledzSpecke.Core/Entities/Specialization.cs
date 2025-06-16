using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

public class Specialization
{
    public SpecializationId Id { get; private set; }
    public UserId UserId { get; private set; }
    public string Name { get; private set; }
    public string ProgramCode { get; private set; }
    public SmkVersion SmkVersion { get; private set; }
    public string ProgramVariant { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime PlannedEndDate { get; private set; }
    public DateTime CalculatedEndDate { get; private set; }
    public DateTime? ActualEndDate { get; private set; }
    public int PlannedPesYear { get; private set; }
    public SpecializationStatus Status { get; private set; }
    public string ProgramStructure { get; private set; }
    public ModuleId? CurrentModuleId { get; private set; }
    public int DurationYears { get; private set; }
    public int DurationInDays => DurationYears * 365;

    private readonly List<Module> _modules = new();
    public IReadOnlyList<Module> Modules => _modules.AsReadOnly();

    private Specialization() { }

    public Specialization(
        SpecializationId id, 
        UserId userId,
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
        Id = id;
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ProgramCode = programCode ?? throw new ArgumentNullException(nameof(programCode));
        SmkVersion = smkVersion ?? throw new ArgumentNullException(nameof(smkVersion));
        ProgramVariant = programVariant ?? throw new ArgumentNullException(nameof(programVariant));
        StartDate = startDate;
        PlannedEndDate = plannedEndDate;
        PlannedPesYear = plannedPesYear;
        ProgramStructure = programStructure ?? throw new ArgumentNullException(nameof(programStructure));
        DurationYears = durationYears;
        CalculatedEndDate = startDate.AddYears(durationYears);
        Status = SpecializationStatus.Active;
    }

    public Specialization(
        UserId userId,
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
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ProgramCode = programCode ?? throw new ArgumentNullException(nameof(programCode));
        SmkVersion = smkVersion ?? throw new ArgumentNullException(nameof(smkVersion));
        ProgramVariant = programVariant ?? throw new ArgumentNullException(nameof(programVariant));
        StartDate = startDate;
        PlannedEndDate = plannedEndDate;
        PlannedPesYear = plannedPesYear;
        ProgramStructure = programStructure ?? throw new ArgumentNullException(nameof(programStructure));
        DurationYears = durationYears;
        CalculatedEndDate = startDate.AddYears(durationYears);
        Status = SpecializationStatus.Active;
    }

    public void AddModule(Module module)
    {
        if (_modules.Any(m => m.Id == module.Id))
        {
            throw new InvalidOperationException($"Module with ID {module.Id} already exists.");
        }

        _modules.Add(module);
    }

    public void SetCurrentModule(ModuleId moduleId)
    {
        if (!_modules.Any(m => m.Id == moduleId))
        {
            throw new InvalidOperationException($"Module with ID {moduleId} does not exist in this specialization.");
        }

        CurrentModuleId = moduleId;
    }

    public (int Completed, int Total) GetInternshipProgress()
    {
        var completed = _modules.Sum(m => m.CompletedInternships);
        var total = _modules.Sum(m => m.TotalInternships);
        return (completed, total);
    }

    public (int Completed, int Total) GetCoursesProgress()
    {
        var completed = _modules.Sum(m => m.CompletedCourses);
        var total = _modules.Sum(m => m.TotalCourses);
        return (completed, total);
    }

    public void Complete(DateTime completionDate)
    {
        if (Status == SpecializationStatus.Completed)
        {
            throw new InvalidOperationException("Specialization is already completed.");
        }

        if (completionDate < StartDate)
        {
            throw new ArgumentException("Completion date cannot be before start date.");
        }

        ActualEndDate = completionDate;
        Status = SpecializationStatus.Completed;
    }

    public void Suspend()
    {
        if (Status == SpecializationStatus.Completed)
        {
            throw new InvalidOperationException("Cannot suspend a completed specialization.");
        }

        if (Status == SpecializationStatus.Cancelled)
        {
            throw new InvalidOperationException("Cannot suspend a cancelled specialization.");
        }

        Status = SpecializationStatus.Suspended;
    }

    public void Resume()
    {
        if (Status != SpecializationStatus.Suspended)
        {
            throw new InvalidOperationException("Can only resume a suspended specialization.");
        }

        Status = SpecializationStatus.Active;
    }

    public void Cancel()
    {
        if (Status == SpecializationStatus.Completed)
        {
            throw new InvalidOperationException("Cannot cancel a completed specialization.");
        }

        Status = SpecializationStatus.Cancelled;
        ActualEndDate = DateTime.UtcNow;
    }
}
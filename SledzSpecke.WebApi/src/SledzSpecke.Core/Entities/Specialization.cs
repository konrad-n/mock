using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

public class Specialization
{
    public SpecializationId Id { get; private set; }
    public string Name { get; private set; }
    public string ProgramCode { get; private set; }
    public SmkVersion SmkVersion { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime PlannedEndDate { get; private set; }
    public DateTime CalculatedEndDate { get; private set; }
    public string ProgramStructure { get; private set; }
    public ModuleId? CurrentModuleId { get; private set; }
    public int DurationYears { get; private set; }
    public int DurationInDays => DurationYears * 365;

    private readonly List<Module> _modules = new();
    public IReadOnlyList<Module> Modules => _modules.AsReadOnly();

    public Specialization(SpecializationId id, string name, string programCode, SmkVersion smkVersion,
        DateTime startDate, DateTime plannedEndDate, string programStructure, int durationYears)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ProgramCode = programCode ?? throw new ArgumentNullException(nameof(programCode));
        SmkVersion = smkVersion;
        StartDate = startDate;
        PlannedEndDate = plannedEndDate;
        ProgramStructure = programStructure ?? throw new ArgumentNullException(nameof(programStructure));
        DurationYears = durationYears;
        CalculatedEndDate = startDate.AddYears(durationYears);
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
}
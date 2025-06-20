using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.DomainServices;

public interface IProcedureValidationService
{
    Task<Result> ValidateProcedureRealizationAsync(
        ProcedureRealization realization,
        ProcedureRequirement requirement,
        UserId userId,
        Module currentModule);
    
    Task<Result<ProcedureProgress>> CalculateProcedureProgressAsync(
        ProcedureRequirement requirement,
        IEnumerable<ProcedureRealization> realizations);
    
    Task<Result<bool>> IsProcedureRequirementMetAsync(
        ProcedureRequirement requirement,
        IEnumerable<ProcedureRealization> realizations);
    
    Task<Result<ProcedureValidationSummary>> GetValidationSummaryAsync(
        UserId userId,
        ModuleId moduleId);
    
    Task<Result<IEnumerable<ProcedureRequirement>>> GetUnmetRequirementsAsync(
        UserId userId,
        ModuleId moduleId);
}

public class ProcedureProgress
{
    public ProcedureRequirement Requirement { get; set; } = null!;
    public int CompletedCountA { get; set; }
    public int CompletedCountB { get; set; }
    public int TotalCompleted { get; set; }
    public double ProgressPercentage { get; set; }
    public bool IsRequirementMet { get; set; }
    public List<string> ValidationMessages { get; set; } = new();
}

public class ProcedureValidationSummary
{
    public int TotalRequirements { get; set; }
    public int CompletedRequirements { get; set; }
    public int TotalProceduresCompleted { get; set; }
    public double OverallProgressPercentage { get; set; }
    public SmkVersion SmkVersion { get; set; }
    public List<ProcedureProgress> RequirementProgress { get; set; } = new();
    public List<string> ValidationErrors { get; set; } = new();
    public List<string> ValidationWarnings { get; set; } = new();
}
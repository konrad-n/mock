using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.DomainServices;

public interface IProcedureValidationService
{
    Task<Result> ValidateProcedureAsync(
        ProcedureBase procedure,
        UserId userId,
        Specialization specialization,
        Module? currentModule = null);
    
    Task<Result<ProcedureProgress>> CalculateProcedureProgressAsync(
        ProcedureRequirement requirement,
        IEnumerable<ProcedureBase> completedProcedures);
    
    Task<Result<bool>> IsProcedureRequirementMetAsync(
        ProcedureRequirement requirement,
        IEnumerable<ProcedureBase> procedures);
    
    Task<Result<ProcedureValidationSummary>> GetValidationSummaryAsync(
        UserId userId,
        SpecializationId specializationId,
        ModuleId? moduleId = null);
    
    Task<Result<IEnumerable<ProcedureRequirement>>> GetUnmetRequirementsAsync(
        UserId userId,
        SpecializationId specializationId,
        ModuleId? moduleId = null);
}

public class ProcedureRequirement
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int MinimumCountA { get; set; }
    public int MinimumCountB { get; set; }
    public int MinimumTotal { get; set; }
    public ModuleId? ModuleId { get; set; }
    public int? Year { get; set; }
    public SmkVersion SmkVersion { get; set; }
    public bool IsOptional { get; set; }
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
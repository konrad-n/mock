using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Policies;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.DomainServices;

public class ProcedureValidationService : IProcedureValidationService
{
    private readonly IProcedureRepository _procedureRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly ISmkPolicyFactory _policyFactory;
    
    // Mock procedure requirements for demonstration
    // In real implementation, these would come from a repository
    private readonly List<ProcedureRequirement> _mockRequirements = new()
    {
        // Old SMK requirements (year-based)
        new ProcedureRequirement 
        { 
            Id = 1, 
            Code = "PROC001", 
            Name = "Badanie fizykalne", 
            MinimumCountA = 10, 
            MinimumCountB = 5, 
            MinimumTotal = 15, 
            Year = 1, 
            SmkVersion = SmkVersion.Old 
        },
        new ProcedureRequirement 
        { 
            Id = 2, 
            Code = "PROC002", 
            Name = "EKG", 
            MinimumCountA = 5, 
            MinimumCountB = 10, 
            MinimumTotal = 15, 
            Year = 2, 
            SmkVersion = SmkVersion.Old 
        },
        // New SMK requirements (module-based)
        new ProcedureRequirement 
        { 
            Id = 3, 
            Code = "PROC001", 
            Name = "Badanie fizykalne", 
            MinimumCountA = 20, 
            MinimumCountB = 10, 
            MinimumTotal = 30, 
            ModuleId = new ModuleId(1), 
            SmkVersion = SmkVersion.New 
        },
        new ProcedureRequirement 
        { 
            Id = 4, 
            Code = "PROC002", 
            Name = "EKG", 
            MinimumCountA = 10, 
            MinimumCountB = 20, 
            MinimumTotal = 30, 
            ModuleId = new ModuleId(1), 
            SmkVersion = SmkVersion.New 
        }
    };

    public ProcedureValidationService(
        IProcedureRepository procedureRepository,
        ISpecializationRepository specializationRepository,
        ISmkPolicyFactory policyFactory)
    {
        _procedureRepository = procedureRepository;
        _specializationRepository = specializationRepository;
        _policyFactory = policyFactory;
    }

    public Result ValidateProcedure(
        ProcedureBase procedure,
        UserId userId,
        Specialization specialization,
        Module? currentModule = null)
    {
        // Create context for policy validation
        var context = new SpecializationContext(
            specialization.Id,
            userId,
            specialization.SmkVersion,
            currentModule?.Id,
            procedure.Date);

        // Apply version-specific policy
        var policy = _policyFactory.GetPolicy<ProcedureBase>(specialization.SmkVersion);
        var policyResult = policy.Validate(procedure, context);
        
        if (!policyResult.IsSuccess)
        {
            return policyResult;
        }

        // Additional validation for procedure code
        var requirement = GetRequirementByCode(procedure.Code, specialization.SmkVersion, currentModule?.Id);
        if (requirement == null)
        {
            return Result.Failure($"Kod procedury '{procedure.Code}' nie jest zdefiniowany w szablonie specjalizacji");
        }

        // Validate procedure is performed within valid internship
        if (procedure.InternshipId == null)
        {
            return Result.Failure("Procedura musi być przypisana do stażu");
        }

        return Result.Success();
    }

    public Result<ProcedureProgress> CalculateProcedureProgress(
        ProcedureRequirement requirement,
        IEnumerable<ProcedureBase> completedProcedures)
    {
        var progress = new ProcedureProgress
        {
            Requirement = requirement,
            CompletedCountA = 0,
            CompletedCountB = 0,
            TotalCompleted = 0
        };

        foreach (var procedure in completedProcedures.Where(p => p.Code == requirement.Code && p.IsCompleted))
        {
            if (requirement.SmkVersion == SmkVersion.Old)
            {
                // Old SMK: count individual procedures by operator code
                if (procedure is ProcedureOldSmk oldSmkProc)
                {
                    if (oldSmkProc.ExecutionType == ProcedureExecutionType.CodeA)
                        progress.CompletedCountA++;
                    else if (oldSmkProc.ExecutionType == ProcedureExecutionType.CodeB)
                        progress.CompletedCountB++;
                    else
                        progress.TotalCompleted++; // No execution type specified
                }
            }
            else if (requirement.SmkVersion == SmkVersion.New)
            {
                // New SMK: aggregate counts
                if (procedure is ProcedureNewSmk newSmkProc)
                {
                    progress.CompletedCountA += newSmkProc.CountA;
                    progress.CompletedCountB += newSmkProc.CountB;
                }
            }
        }

        progress.TotalCompleted = progress.CompletedCountA + progress.CompletedCountB;

        // Calculate progress percentage
        var requiredTotal = Math.Max(requirement.MinimumTotal, requirement.MinimumCountA + requirement.MinimumCountB);
        progress.ProgressPercentage = requiredTotal > 0 
            ? Math.Min(100, (progress.TotalCompleted * 100.0) / requiredTotal)
            : 100;

        // Check if requirement is met
        progress.IsRequirementMet = 
            progress.CompletedCountA >= requirement.MinimumCountA &&
            progress.CompletedCountB >= requirement.MinimumCountB &&
            progress.TotalCompleted >= requirement.MinimumTotal;

        // Add validation messages
        if (progress.CompletedCountA < requirement.MinimumCountA)
        {
            progress.ValidationMessages.Add($"Brakuje {requirement.MinimumCountA - progress.CompletedCountA} procedur jako operator (A)");
        }
        
        if (progress.CompletedCountB < requirement.MinimumCountB)
        {
            progress.ValidationMessages.Add($"Brakuje {requirement.MinimumCountB - progress.CompletedCountB} procedur jako asystent (B)");
        }

        return Result<ProcedureProgress>.Success(progress);
    }

    public async Task<Result<bool>> IsProcedureRequirementMetAsync(
        ProcedureRequirement requirement,
        IEnumerable<ProcedureBase> procedures)
    {
        var progressResult = await CalculateProcedureProgressAsync(requirement, procedures);
        return progressResult.IsSuccess 
            ? Result<bool>.Success(progressResult.Value.IsRequirementMet)
            : Result<bool>.Failure(progressResult.Error);
    }

    public async Task<Result<ProcedureValidationSummary>> GetValidationSummaryAsync(
        UserId userId,
        SpecializationId specializationId,
        ModuleId? moduleId = null)
    {
        var summary = new ProcedureValidationSummary();

        // Get user's specialization
        var specialization = await _specializationRepository.GetByIdAsync(specializationId);
        if (specialization == null)
        {
            return Result<ProcedureValidationSummary>.Failure("Nie znaleziono specjalizacji");
        }

        summary.SmkVersion = specialization.SmkVersion;

        // Get all user's procedures
        var userProcedures = await _procedureRepository.GetByUserAsync(userId);

        // Get requirements based on SMK version and module
        var requirements = GetRequirementsForContext(specialization.SmkVersion, moduleId);
        summary.TotalRequirements = requirements.Count;

        // Calculate progress for each requirement
        foreach (var requirement in requirements)
        {
            var progressResult = await CalculateProcedureProgressAsync(requirement, userProcedures);
            if (progressResult.IsSuccess)
            {
                summary.RequirementProgress.Add(progressResult.Value);
                if (progressResult.Value.IsRequirementMet)
                {
                    summary.CompletedRequirements++;
                }
            }
        }

        // Calculate totals
        summary.TotalProceduresCompleted = userProcedures.Count(p => p.IsCompleted);
        summary.OverallProgressPercentage = summary.TotalRequirements > 0
            ? (summary.CompletedRequirements * 100.0) / summary.TotalRequirements
            : 0;

        // Add warnings for low progress
        if (summary.OverallProgressPercentage < 25)
        {
            summary.ValidationWarnings.Add("Postęp w procedurach jest poniżej 25%");
        }

        // Check for expired procedures (Old SMK)
        if (specialization.SmkVersion == SmkVersion.Old)
        {
            var expiredCount = userProcedures.Count(p => 
                p.Date < DateTime.UtcNow.AddYears(-2) && !p.IsApproved);
            if (expiredCount > 0)
            {
                summary.ValidationWarnings.Add($"{expiredCount} procedur może wymagać ponownego wykonania (starsze niż 2 lata)");
            }
        }

        return Result<ProcedureValidationSummary>.Success(summary);
    }

    public async Task<Result<IEnumerable<ProcedureRequirement>>> GetUnmetRequirementsAsync(
        UserId userId,
        SpecializationId specializationId,
        ModuleId? moduleId = null)
    {
        var summaryResult = await GetValidationSummaryAsync(userId, specializationId, moduleId);
        if (!summaryResult.IsSuccess)
        {
            return Result<IEnumerable<ProcedureRequirement>>.Failure(summaryResult.Error);
        }

        var unmetRequirements = summaryResult.Value.RequirementProgress
            .Where(p => !p.IsRequirementMet)
            .Select(p => p.Requirement);

        return Result<IEnumerable<ProcedureRequirement>>.Success(unmetRequirements);
    }

    private ProcedureRequirement? GetRequirementByCode(string code, SmkVersion smkVersion, ModuleId? moduleId)
    {
        return _mockRequirements.FirstOrDefault(r => 
            r.Code == code && 
            r.SmkVersion == smkVersion &&
            (moduleId == null || r.ModuleId == moduleId));
    }

    private List<ProcedureRequirement> GetRequirementsForContext(SmkVersion smkVersion, ModuleId? moduleId)
    {
        if (moduleId != null)
        {
            // New SMK - get requirements for specific module
            return _mockRequirements.Where(r => 
                r.SmkVersion == smkVersion && 
                r.ModuleId == moduleId).ToList();
        }
        else
        {
            // Old SMK - get all year-based requirements
            return _mockRequirements.Where(r => 
                r.SmkVersion == smkVersion && 
                r.Year != null).ToList();
        }
    }
}
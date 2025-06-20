using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.DomainServices;

public class ProcedureValidationService : IProcedureValidationService
{
    private readonly IProcedureRealizationRepository _procedureRealizationRepository;
    private readonly IProcedureRequirementRepository _procedureRequirementRepository;
    private readonly IModuleRepository _moduleRepository;
    public ProcedureValidationService(
        IProcedureRealizationRepository procedureRealizationRepository,
        IProcedureRequirementRepository procedureRequirementRepository,
        IModuleRepository moduleRepository)
    {
        _procedureRealizationRepository = procedureRealizationRepository;
        _procedureRequirementRepository = procedureRequirementRepository;
        _moduleRepository = moduleRepository;
    }

    public async Task<Result> ValidateProcedureRealizationAsync(
        ProcedureRealization realization,
        ProcedureRequirement requirement,
        UserId userId,
        Module currentModule)
    {
        // Validate that realization belongs to the user
        if (realization.UserId != userId)
        {
            return Result.Failure("Realizacja procedury nie należy do użytkownika");
        }

        // Validate that requirement belongs to the module
        if (requirement.ModuleId != currentModule.Id)
        {
            return Result.Failure("Wymaganie procedury nie należy do tego modułu");
        }

        // Validate date is not in the future
        if (realization.Date > DateTime.UtcNow)
        {
            return Result.Failure("Data realizacji procedury nie może być w przyszłości");
        }

        // Validate location is provided
        if (string.IsNullOrWhiteSpace(realization.Location))
        {
            return Result.Failure("Lokalizacja realizacji procedury jest wymagana");
        }

        // Additional validation can be added here based on business rules

        return await Task.FromResult(Result.Success());
    }

    public async Task<Result<ProcedureProgress>> CalculateProcedureProgressAsync(
        ProcedureRequirement requirement,
        IEnumerable<ProcedureRealization> realizations)
    {
        var progress = new ProcedureProgress
        {
            Requirement = requirement,
            CompletedCountA = 0,
            CompletedCountB = 0,
            TotalCompleted = 0
        };

        // Count realizations by role
        foreach (var realization in realizations.Where(r => r.RequirementId == requirement.Id))
        {
            if (realization.Role == ProcedureRole.Operator)
            {
                progress.CompletedCountA++;
            }
            else if (realization.Role == ProcedureRole.Assistant)
            {
                progress.CompletedCountB++;
            }
        }

        progress.TotalCompleted = progress.CompletedCountA + progress.CompletedCountB;

        // Calculate progress percentage
        var requiredTotal = Math.Max(
            requirement.RequiredAsOperator + requirement.RequiredAsAssistant,
            requirement.RequiredAsOperator + requirement.RequiredAsAssistant);
        
        progress.ProgressPercentage = requiredTotal > 0 
            ? Math.Min(100, (progress.TotalCompleted * 100.0) / requiredTotal)
            : 100;

        // Check if requirement is met
        progress.IsRequirementMet = 
            progress.CompletedCountA >= requirement.RequiredAsOperator &&
            progress.CompletedCountB >= requirement.RequiredAsAssistant;

        // Add validation messages
        if (progress.CompletedCountA < requirement.RequiredAsOperator)
        {
            progress.ValidationMessages.Add($"Brakuje {requirement.RequiredAsOperator - progress.CompletedCountA} procedur jako operator");
        }
        
        if (progress.CompletedCountB < requirement.RequiredAsAssistant)
        {
            progress.ValidationMessages.Add($"Brakuje {requirement.RequiredAsAssistant - progress.CompletedCountB} procedur jako asystent");
        }

        return await Task.FromResult(Result<ProcedureProgress>.Success(progress));
    }

    public async Task<Result<bool>> IsProcedureRequirementMetAsync(
        ProcedureRequirement requirement,
        IEnumerable<ProcedureRealization> realizations)
    {
        var progressResult = await CalculateProcedureProgressAsync(requirement, realizations);
        return progressResult.IsSuccess 
            ? Result<bool>.Success(progressResult.Value.IsRequirementMet)
            : Result<bool>.Failure(progressResult.Error);
    }

    public async Task<Result<ProcedureValidationSummary>> GetValidationSummaryAsync(
        UserId userId,
        ModuleId moduleId)
    {
        var summary = new ProcedureValidationSummary();

        // Get module to determine SMK version
        var module = await _moduleRepository.GetByIdAsync(moduleId);
        if (module == null)
        {
            return Result<ProcedureValidationSummary>.Failure("Nie znaleziono modułu");
        }

        // For now, use New SMK as default (will be determined from specialization later)
        summary.SmkVersion = SmkVersion.New;

        // Get all requirements for this module
        var requirements = await _procedureRequirementRepository.GetByModuleIdAsync(moduleId);
        summary.TotalRequirements = requirements.Count();

        // Get all user's realizations
        var userRealizations = await _procedureRealizationRepository.GetByUserIdAsync(userId);

        // Calculate progress for each requirement
        foreach (var requirement in requirements)
        {
            var progressResult = await CalculateProcedureProgressAsync(requirement, userRealizations);
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
        summary.TotalProceduresCompleted = userRealizations.Count();
        summary.OverallProgressPercentage = summary.TotalRequirements > 0
            ? (summary.CompletedRequirements * 100.0) / summary.TotalRequirements
            : 0;

        // Add warnings for low progress
        if (summary.OverallProgressPercentage < 25)
        {
            summary.ValidationWarnings.Add("Postęp w procedurach jest poniżej 25%");
        }

        return Result<ProcedureValidationSummary>.Success(summary);
    }

    public async Task<Result<IEnumerable<ProcedureRequirement>>> GetUnmetRequirementsAsync(
        UserId userId,
        ModuleId moduleId)
    {
        var summaryResult = await GetValidationSummaryAsync(userId, moduleId);
        if (!summaryResult.IsSuccess)
        {
            return Result<IEnumerable<ProcedureRequirement>>.Failure(summaryResult.Error);
        }

        var unmetRequirements = summaryResult.Value.RequirementProgress
            .Where(p => !p.IsRequirementMet)
            .Select(p => p.Requirement);

        return Result<IEnumerable<ProcedureRequirement>>.Success(unmetRequirements);
    }
}
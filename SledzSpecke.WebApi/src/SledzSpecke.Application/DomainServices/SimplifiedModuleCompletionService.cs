using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.DomainServices;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.DomainServices;

/// <summary>
/// Simplified implementation that matches the actual domain model
/// </summary>
public sealed class SimplifiedModuleCompletionService : IModuleCompletionService
{
    private readonly ILogger<SimplifiedModuleCompletionService> _logger;
    private readonly IInternshipRepository _internshipRepository;
    private readonly IModuleRepository _moduleRepository;

    public SimplifiedModuleCompletionService(
        ILogger<SimplifiedModuleCompletionService> logger,
        IInternshipRepository internshipRepository,
        IModuleRepository moduleRepository)
    {
        _logger = logger;
        _internshipRepository = internshipRepository;
        _moduleRepository = moduleRepository;
    }

    public async Task<Result<ModuleCompletionStatus>> ValidateModuleCompletionAsync(
        InternshipId internshipId,
        ModuleId moduleId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var module = await _moduleRepository.GetByIdAsync(moduleId.Value);
            if (module == null)
            {
                return Result<ModuleCompletionStatus>.Failure("Module not found");
            }

            // Simple completion check based on available data
            var requirements = new Dictionary<string, RequirementStatus>
            {
                ["Internships"] = new RequirementStatus
                {
                    RequirementType = "Internships",
                    Required = module.TotalInternships,
                    Completed = module.CompletedInternships,
                    PercentageComplete = module.TotalInternships > 0 
                        ? (module.CompletedInternships * 100.0) / module.TotalInternships 
                        : 0
                },
                ["Medical Shifts"] = new RequirementStatus
                {
                    RequirementType = "Medical Shifts",
                    Required = module.RequiredShiftHours,
                    Completed = module.CompletedShiftHours,
                    PercentageComplete = module.RequiredShiftHours > 0 
                        ? (module.CompletedShiftHours * 100.0) / module.RequiredShiftHours 
                        : 0
                }
            };

            var overallProgress = requirements.Values.Average(r => r.PercentageComplete);
            var isComplete = requirements.Values.All(r => r.PercentageComplete >= 100);

            var status = new ModuleCompletionStatus
            {
                ModuleId = moduleId,
                IsComplete = isComplete,
                Requirements = requirements,
                OverallProgress = overallProgress,
                EstimatedCompletionDate = isComplete ? DateTime.UtcNow : null
            };

            return Result<ModuleCompletionStatus>.Success(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating module completion");
            return Result<ModuleCompletionStatus>.Failure("Failed to validate module completion");
        }
    }

    public async Task<Result<Core.DomainServices.ModuleProgress>> CalculateWeightedProgressAsync(
        InternshipId internshipId,
        ModuleId moduleId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var status = await ValidateModuleCompletionAsync(internshipId, moduleId, cancellationToken);
            if (!status.IsSuccess)
            {
                return Result<Core.DomainServices.ModuleProgress>.Failure(status.Error);
            }

            // Simplified calculation with available data
            var progress = Core.DomainServices.ModuleProgress.Calculate(
                internships: status.Value.Requirements.GetValueOrDefault("Internships")?.PercentageComplete ?? 0,
                courses: 0, // Not implemented
                procedures: 0, // Not implemented  
                shifts: status.Value.Requirements.GetValueOrDefault("Medical Shifts")?.PercentageComplete ?? 0
            );

            return Result<Core.DomainServices.ModuleProgress>.Success(progress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating weighted progress");
            return Result<Core.DomainServices.ModuleProgress>.Failure("Failed to calculate progress");
        }
    }

    public async Task<Result> TransitionToNextModuleAsync(
        InternshipId internshipId,
        ModuleId currentModuleId,
        ModuleId nextModuleId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var internship = await _internshipRepository.GetByIdAsync(internshipId.Value);
            if (internship == null)
            {
                return Result.Failure("Internship not found");
            }

            // Simple transition - just update the module assignment
            internship.AssignToModule(nextModuleId);
            await _internshipRepository.UpdateAsync(internship);

            _logger.LogInformation(
                "Transitioned internship {InternshipId} to module {ModuleId}",
                internshipId.Value,
                nextModuleId.Value);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error transitioning to next module");
            return Result.Failure("Failed to transition module");
        }
    }

    public Task<Result<ProcedureAllocation>> AllocateProcedureToRequirementAsync(
        ProcedureId procedureId,
        InternshipId internshipId,
        ModuleRequirementId requirementId,
        CancellationToken cancellationToken = default)
    {
        // Simplified implementation
        var allocation = new ProcedureAllocation
        {
            ProcedureId = procedureId,
            AllocatedTo = requirementId,
            RequirementType = "General",
            RemainingForRequirement = 0
        };

        return Task.FromResult(Result<ProcedureAllocation>.Success(allocation));
    }

    public async Task<Result<bool>> CanSwitchModuleTypeAsync(
        InternshipId internshipId,
        Core.DomainServices.ModuleType targetType,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var internship = await _internshipRepository.GetByIdAsync(internshipId.Value);
            if (internship == null || internship.ModuleId == null)
            {
                return Result<bool>.Success(false);
            }

            // Simple rule: can always switch for now
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking module type switch");
            return Result<bool>.Failure("Failed to check module type switch");
        }
    }
}
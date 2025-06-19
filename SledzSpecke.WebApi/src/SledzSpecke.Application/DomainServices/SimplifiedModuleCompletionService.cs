using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.DomainServices;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.Specifications;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.DomainServices;

/// <summary>
/// Production implementation of module completion service with full SMK business logic
/// </summary>
public sealed class SimplifiedModuleCompletionService : IModuleCompletionService
{
    private readonly ILogger<SimplifiedModuleCompletionService> _logger;
    private readonly IInternshipRepository _internshipRepository;
    private readonly IModuleRepository _moduleRepository;
    private readonly IProcedureRepository _procedureRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IMedicalShiftRepository _medicalShiftRepository;

    public SimplifiedModuleCompletionService(
        ILogger<SimplifiedModuleCompletionService> logger,
        IInternshipRepository internshipRepository,
        IModuleRepository moduleRepository,
        IProcedureRepository procedureRepository,
        ICourseRepository courseRepository,
        IMedicalShiftRepository medicalShiftRepository)
    {
        _logger = logger;
        _internshipRepository = internshipRepository;
        _moduleRepository = moduleRepository;
        _procedureRepository = procedureRepository;
        _courseRepository = courseRepository;
        _medicalShiftRepository = medicalShiftRepository;
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

            var internship = await _internshipRepository.GetByIdAsync(internshipId.Value);
            if (internship == null)
            {
                return Result<ModuleCompletionStatus>.Failure("Internship not found");
            }

            // Get all related internships for this module
            var moduleInternships = await _internshipRepository.GetByModuleIdAsync(moduleId.Value);
            var completedInternships = moduleInternships.Count(i => i.IsCompleted);

            // Get completed procedures for this internship
            var procedures = await _procedureRepository.GetByInternshipIdAsync(internshipId.Value);
            var completedProceduresA = procedures.Count(p => 
                p.Status == ProcedureStatus.Completed && 
                p.ExecutionType == ProcedureExecutionType.CodeA);
            var completedProceduresB = procedures.Count(p => 
                p.Status == ProcedureStatus.Completed && 
                p.ExecutionType == ProcedureExecutionType.CodeB);

            // Get valid courses for this module
            var courses = await _courseRepository.GetByModuleIdAsync(moduleId);
            var completedCourses = courses.Count(c => c.IsApproved && c.IsVerifiedByCmkp);

            // Calculate medical shift hours for this internship's module
            var shifts = await _medicalShiftRepository.GetByInternshipIdAsync(internshipId.Value);
            var approvedShifts = shifts.Where(s => s.IsApproved);
            var completedShiftHours = approvedShifts.Sum(s => s.Hours);

            // Build comprehensive requirement status
            var requirements = new Dictionary<string, RequirementStatus>
            {
                ["Internships"] = new RequirementStatus
                {
                    RequirementType = "Internships",
                    Required = module.TotalInternships,
                    Completed = completedInternships,
                    PercentageComplete = module.TotalInternships > 0 
                        ? (completedInternships * 100.0) / module.TotalInternships 
                        : 100
                },
                ["Courses"] = new RequirementStatus
                {
                    RequirementType = "Courses",
                    Required = module.TotalCourses,
                    Completed = completedCourses,
                    PercentageComplete = module.TotalCourses > 0 
                        ? (completedCourses * 100.0) / module.TotalCourses 
                        : 100
                },
                ["Procedures Type A"] = new RequirementStatus
                {
                    RequirementType = "Procedures Type A",
                    Required = module.TotalProceduresA,
                    Completed = completedProceduresA,
                    PercentageComplete = module.TotalProceduresA > 0 
                        ? (completedProceduresA * 100.0) / module.TotalProceduresA 
                        : 100
                },
                ["Procedures Type B"] = new RequirementStatus
                {
                    RequirementType = "Procedures Type B",
                    Required = module.TotalProceduresB,
                    Completed = completedProceduresB,
                    PercentageComplete = module.TotalProceduresB > 0 
                        ? (completedProceduresB * 100.0) / module.TotalProceduresB 
                        : 100
                },
                ["Medical Shifts"] = new RequirementStatus
                {
                    RequirementType = "Medical Shifts",
                    Required = module.RequiredShiftHours,
                    Completed = completedShiftHours,
                    PercentageComplete = module.RequiredShiftHours > 0 
                        ? (completedShiftHours * 100.0) / module.RequiredShiftHours 
                        : 100
                }
            };

            // Calculate weighted overall progress using SMK weights
            var weightedProgress = CalculateWeightedProgress(requirements);
            var isComplete = requirements.Values.All(r => r.PercentageComplete >= 100);

            // Estimate completion date based on current progress rate
            DateTime? estimatedCompletionDate = null;
            if (!isComplete && weightedProgress > 0)
            {
                var daysSinceStart = (DateTime.UtcNow - module.StartDate).TotalDays;
                var daysToComplete = daysSinceStart / weightedProgress * 100;
                estimatedCompletionDate = module.StartDate.AddDays(daysToComplete);
            }

            var status = new ModuleCompletionStatus
            {
                ModuleId = moduleId,
                IsComplete = isComplete,
                Requirements = requirements,
                OverallProgress = weightedProgress,
                EstimatedCompletionDate = isComplete ? DateTime.UtcNow : estimatedCompletionDate
            };

            _logger.LogInformation(
                "Module {ModuleId} completion validated: {Progress:F1}% complete", 
                moduleId.Value, 
                weightedProgress);

            return Result<ModuleCompletionStatus>.Success(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating module completion for internship {InternshipId}", internshipId.Value);
            return Result<ModuleCompletionStatus>.Failure("Failed to validate module completion");
        }
    }

    private static double CalculateWeightedProgress(Dictionary<string, RequirementStatus> requirements)
    {
        // SMK-defined weights for different requirement types
        var weights = new Dictionary<string, double>
        {
            ["Internships"] = 0.35,
            ["Courses"] = 0.25,
            ["Procedures Type A"] = 0.20,
            ["Procedures Type B"] = 0.10,
            ["Medical Shifts"] = 0.10
        };

        var weightedSum = 0.0;
        var totalWeight = 0.0;

        foreach (var requirement in requirements)
        {
            if (weights.TryGetValue(requirement.Key, out var weight))
            {
                weightedSum += requirement.Value.PercentageComplete * weight;
                totalWeight += weight;
            }
        }

        return totalWeight > 0 ? weightedSum / totalWeight : 0;
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

            // Extract progress values from the validated status
            var internshipsProgress = status.Value.Requirements.GetValueOrDefault("Internships")?.PercentageComplete ?? 0;
            var coursesProgress = status.Value.Requirements.GetValueOrDefault("Courses")?.PercentageComplete ?? 0;
            
            // Combine procedures A and B progress with their respective weights
            var proceduresAProgress = status.Value.Requirements.GetValueOrDefault("Procedures Type A")?.PercentageComplete ?? 0;
            var proceduresBProgress = status.Value.Requirements.GetValueOrDefault("Procedures Type B")?.PercentageComplete ?? 0;
            var combinedProceduresProgress = (proceduresAProgress * 0.67) + (proceduresBProgress * 0.33); // Type A weighted more heavily
            
            var shiftsProgress = status.Value.Requirements.GetValueOrDefault("Medical Shifts")?.PercentageComplete ?? 0;

            // Calculate using the ModuleProgress Calculate method with SMK-defined weights
            var progress = Core.DomainServices.ModuleProgress.Calculate(
                internships: internshipsProgress,
                courses: coursesProgress,
                procedures: combinedProceduresProgress,
                shifts: shiftsProgress
            );

            _logger.LogInformation(
                "Weighted progress calculated for module {ModuleId}: {WeightedTotal:F1}%",
                moduleId.Value,
                progress.WeightedTotal);

            return Result<Core.DomainServices.ModuleProgress>.Success(progress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating weighted progress for internship {InternshipId}", internshipId.Value);
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
            // Validate current module is complete
            var completionStatus = await ValidateModuleCompletionAsync(internshipId, currentModuleId, cancellationToken);
            if (!completionStatus.IsSuccess)
            {
                return Result.Failure($"Cannot validate current module completion: {completionStatus.Error}");
            }

            if (!completionStatus.Value.IsComplete)
            {
                var incompleteRequirements = completionStatus.Value.Requirements
                    .Where(r => r.Value.PercentageComplete < 100)
                    .Select(r => $"{r.Key}: {r.Value.Completed}/{r.Value.Required}")
                    .ToList();
                
                return Result.Failure(
                    $"Current module is not complete. Incomplete requirements: {string.Join(", ", incompleteRequirements)}");
            }

            // Get the internship and validate it exists
            var internship = await _internshipRepository.GetByIdAsync(internshipId.Value);
            if (internship == null)
            {
                return Result.Failure("Internship not found");
            }

            // Validate the next module exists and is valid for transition
            var nextModule = await _moduleRepository.GetByIdAsync(nextModuleId.Value);
            if (nextModule == null)
            {
                return Result.Failure("Next module not found");
            }

            var currentModule = await _moduleRepository.GetByIdAsync(currentModuleId.Value);
            if (currentModule == null)
            {
                return Result.Failure("Current module not found");
            }

            // Validate module transition rules according to SMK
            if (currentModule.SpecializationId != nextModule.SpecializationId)
            {
                return Result.Failure("Cannot transition to a module from a different specialization");
            }

            if (currentModule.SmkVersion != nextModule.SmkVersion)
            {
                return Result.Failure("Cannot transition between different SMK versions");
            }

            // Check module type transition rules
            if (currentModule.Type == Core.ValueObjects.ModuleType.Basic && nextModule.Type == Core.ValueObjects.ModuleType.Basic)
            {
                return Result.Failure("Cannot transition from basic module to another basic module");
            }

            if (currentModule.Type == Core.ValueObjects.ModuleType.Specialist && nextModule.Type == Core.ValueObjects.ModuleType.Basic)
            {
                return Result.Failure("Cannot transition from specialist module back to basic module");
            }

            // Validate dates
            if (nextModule.StartDate < currentModule.EndDate)
            {
                return Result.Failure("Next module cannot start before current module ends");
            }

            // Perform the transition
            internship.AssignToModule(nextModuleId);
            internship.UpdateDates(nextModule.StartDate, nextModule.EndDate);
            
            await _internshipRepository.UpdateAsync(internship);

            _logger.LogInformation(
                "Successfully transitioned internship {InternshipId} from module {CurrentModuleId} to {NextModuleId}",
                internshipId.Value,
                currentModuleId.Value,
                nextModuleId.Value);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Error transitioning internship {InternshipId} from module {CurrentModuleId} to {NextModuleId}",
                internshipId.Value,
                currentModuleId.Value,
                nextModuleId.Value);
            return Result.Failure("Failed to transition module");
        }
    }

    public async Task<Result<ProcedureAllocation>> AllocateProcedureToRequirementAsync(
        ProcedureId procedureId,
        InternshipId internshipId,
        ModuleRequirementId requirementId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Get the procedure
            var procedure = await _procedureRepository.GetByIdAsync(procedureId);
            if (procedure == null)
            {
                return Result<ProcedureAllocation>.Failure("Procedure not found");
            }

            // Validate procedure belongs to the internship
            if (procedure.InternshipId != internshipId)
            {
                return Result<ProcedureAllocation>.Failure("Procedure does not belong to the specified internship");
            }

            // Validate procedure is completed
            if (procedure.Status != ProcedureStatus.Completed)
            {
                return Result<ProcedureAllocation>.Failure("Only completed procedures can be allocated to requirements");
            }

            // Get the internship to find the module
            var internship = await _internshipRepository.GetByIdAsync(internshipId.Value);
            if (internship?.ModuleId == null)
            {
                return Result<ProcedureAllocation>.Failure("Internship or module not found");
            }

            var module = await _moduleRepository.GetByIdAsync(internship.ModuleId.Value);
            if (module == null)
            {
                return Result<ProcedureAllocation>.Failure("Module not found");
            }

            // Determine requirement type based on procedure execution type
            var requirementType = procedure.ExecutionType == ProcedureExecutionType.CodeA
                ? "Procedures Type A"
                : "Procedures Type B";

            // Calculate remaining procedures for this requirement
            var procedures = await _procedureRepository.GetByInternshipIdAsync(internshipId.Value);
            var completedOfSameType = procedures.Count(p => 
                p.Status == ProcedureStatus.Completed && 
                p.ExecutionType == procedure.ExecutionType);

            var requiredCount = procedure.ExecutionType == ProcedureExecutionType.CodeA
                ? module.TotalProceduresA
                : module.TotalProceduresB;

            var remaining = Math.Max(0, requiredCount - completedOfSameType);

            var allocation = new ProcedureAllocation
            {
                ProcedureId = procedureId,
                AllocatedTo = requirementId,
                RequirementType = requirementType,
                RemainingForRequirement = remaining
            };

            _logger.LogInformation(
                "Allocated procedure {ProcedureId} to requirement {RequirementId}. {Remaining} procedures remaining",
                procedureId.Value,
                requirementId.Value,
                remaining);

            return Result<ProcedureAllocation>.Success(allocation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error allocating procedure {ProcedureId} to requirement", procedureId.Value);
            return Result<ProcedureAllocation>.Failure("Failed to allocate procedure to requirement");
        }
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

            var currentModule = await _moduleRepository.GetByIdAsync(internship.ModuleId.Value);
            if (currentModule == null)
            {
                return Result<bool>.Success(false);
            }

            // SMK rules for module type switching
            switch (currentModule.Type)
            {
                case Core.ValueObjects.ModuleType.Basic:
                    // Can only switch from Basic to Specialist
                    if (targetType == Core.DomainServices.ModuleType.Specialist)
                    {
                        // Must have completed at least 50% of basic module
                        var progress = await CalculateWeightedProgressAsync(internshipId, currentModule.Id, cancellationToken);
                        if (!progress.IsSuccess)
                        {
                            return Result<bool>.Success(false);
                        }
                        
                        var canSwitch = progress.Value.WeightedTotal >= 50.0;
                        
                        if (canSwitch)
                        {
                            _logger.LogInformation(
                                "Module type switch allowed for internship {InternshipId}: Basic -> Specialist (Progress: {Progress:F1}%)",
                                internshipId.Value,
                                progress.Value.WeightedTotal);
                        }
                        else
                        {
                            _logger.LogWarning(
                                "Module type switch denied for internship {InternshipId}: Insufficient progress ({Progress:F1}% < 50%)",
                                internshipId.Value,
                                progress.Value.WeightedTotal);
                        }
                        
                        return Result<bool>.Success(canSwitch);
                    }
                    return Result<bool>.Success(false);

                case Core.ValueObjects.ModuleType.Specialist:
                    // Cannot switch back to Basic from Specialist
                    if (targetType == Core.DomainServices.ModuleType.Basic)
                    {
                        _logger.LogWarning(
                            "Module type switch denied for internship {InternshipId}: Cannot revert from Specialist to Basic",
                            internshipId.Value);
                        return Result<bool>.Success(false);
                    }
                    // Can switch between different specialist modules if completed
                    if (targetType == Core.DomainServices.ModuleType.Specialist)
                    {
                        var completionStatus = await ValidateModuleCompletionAsync(internshipId, currentModule.Id, cancellationToken);
                        if (!completionStatus.IsSuccess)
                        {
                            return Result<bool>.Success(false);
                        }
                        
                        var canSwitch = completionStatus.Value.IsComplete;
                        
                        if (canSwitch)
                        {
                            _logger.LogInformation(
                                "Module type switch allowed for internship {InternshipId}: Specialist -> Specialist (Module completed)",
                                internshipId.Value);
                        }
                        else
                        {
                            _logger.LogWarning(
                                "Module type switch denied for internship {InternshipId}: Current specialist module not completed",
                                internshipId.Value);
                        }
                        
                        return Result<bool>.Success(canSwitch);
                    }
                    return Result<bool>.Success(false);

                default:
                    _logger.LogWarning(
                        "Unknown module type {ModuleType} for internship {InternshipId}",
                        currentModule.Type,
                        internshipId.Value);
                    return Result<bool>.Success(false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking module type switch for internship {InternshipId}", internshipId.Value);
            return Result<bool>.Failure("Failed to check module type switch");
        }
    }
}
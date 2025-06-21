using FluentValidation;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Application.Validators;

/// <summary>
/// Validator for SwitchModule command ensuring module switching is allowed
/// </summary>
public class SwitchModuleValidator : AbstractValidator<SwitchModule>
{
    public SwitchModuleValidator(
        IModuleRepository moduleRepository, 
        ISpecializationRepository specializationRepository)
    {
        RuleFor(x => x.SpecializationId)
            .GreaterThan(0)
            .WithMessage("Specialization ID must be a positive number")
            .WithErrorCode("INVALID_SPECIALIZATION_ID");
            
        RuleFor(x => x.ModuleId)
            .GreaterThan(0)
            .WithMessage("Module ID must be a positive number")
            .WithErrorCode("INVALID_MODULE_ID");
            
        // Validate specialization exists
        RuleFor(x => x.SpecializationId)
            .MustAsync(async (specializationId, cancellation) =>
            {
                var specialization = await specializationRepository.GetByIdAsync(specializationId);
                return specialization != null;
            })
            .WithMessage("Specialization not found")
            .WithErrorCode("SPECIALIZATION_NOT_FOUND");
            
        // Validate module exists
        RuleFor(x => x.ModuleId)
            .MustAsync(async (moduleId, cancellation) =>
            {
                var module = await moduleRepository.GetByIdAsync(moduleId);
                return module != null;
            })
            .WithMessage("Module not found")
            .WithErrorCode("MODULE_NOT_FOUND");
            
        // Validate module belongs to the specialization
        RuleFor(x => x)
            .MustAsync(async (command, cancellation) =>
            {
                var module = await moduleRepository.GetByIdAsync(command.ModuleId);
                if (module == null) return true; // Will be caught by previous rule
                
                return module.SpecializationId == command.SpecializationId;
            })
            .WithMessage("Module does not belong to the specified specialization")
            .WithErrorCode("MODULE_SPECIALIZATION_MISMATCH");
            
        // Validate current module can be switched from
        RuleFor(x => x)
            .MustAsync(async (command, cancellation) =>
            {
                // Get user's current active module in this specialization
                var currentModule = await moduleRepository.GetActiveModuleForSpecializationAsync(command.SpecializationId);
                
                if (currentModule == null)
                {
                    // No active module, switching is allowed
                    return true;
                }
                
                // Check if current module is already completed
                return !currentModule.IsCompleted();
            })
            .WithMessage("Cannot switch from a completed module")
            .WithErrorCode("CURRENT_MODULE_COMPLETED");
            
        // Validate target module is not already completed
        RuleFor(x => x.ModuleId)
            .MustAsync(async (moduleId, cancellation) =>
            {
                var module = await moduleRepository.GetByIdAsync(moduleId);
                if (module == null) return true; // Will be caught by previous rule
                
                return !module.IsCompleted();
            })
            .WithMessage("Cannot switch to a completed module")
            .WithErrorCode("TARGET_MODULE_COMPLETED");
            
        // Check if target module hasn't started yet (based on start date)
        RuleFor(x => x.ModuleId)
            .MustAsync(async (moduleId, cancellation) =>
            {
                var module = await moduleRepository.GetByIdAsync(moduleId);
                if (module == null) return true; // Will be caught by previous rule
                
                return DateTime.UtcNow >= module.StartDate;
            })
            .WithMessage("Cannot switch to a module that hasn't started yet")
            .WithErrorCode("MODULE_NOT_STARTED");
            
        // Check if target module hasn't expired
        RuleFor(x => x.ModuleId)
            .MustAsync(async (moduleId, cancellation) =>
            {
                var module = await moduleRepository.GetByIdAsync(moduleId);
                if (module == null) return true; // Will be caught by previous rule
                
                return DateTime.UtcNow <= module.EndDate;
            })
            .WithMessage("Cannot switch to an expired module")
            .WithErrorCode("MODULE_EXPIRED");
            
        // Warning if switching will lose significant progress
        RuleFor(x => x)
            .MustAsync(async (command, cancellation) =>
            {
                var currentModule = await moduleRepository.GetActiveModuleForSpecializationAsync(command.SpecializationId);
                if (currentModule == null) return true;
                
                var progress = currentModule.GetOverallProgress();
                return progress < 0.25; // Warn if more than 25% complete
            })
            .WithMessage("Switching modules will lose significant progress")
            .WithSeverity(Severity.Warning);
            
        // Check if modules are of the same type (basic/specialistic)
        RuleFor(x => x)
            .MustAsync(async (command, cancellation) =>
            {
                var currentModule = await moduleRepository.GetActiveModuleForSpecializationAsync(command.SpecializationId);
                if (currentModule == null) return true; // No restriction if no current module
                
                var targetModule = await moduleRepository.GetByIdAsync(command.ModuleId);
                if (targetModule == null) return true; // Will be caught by previous rule
                
                return currentModule.Type == targetModule.Type;
            })
            .WithMessage("Can only switch between modules of the same type")
            .WithErrorCode("MODULE_TYPE_MISMATCH");
            
        // Check if both modules are the same SMK version
        RuleFor(x => x)
            .MustAsync(async (command, cancellation) =>
            {
                var currentModule = await moduleRepository.GetActiveModuleForSpecializationAsync(command.SpecializationId);
                if (currentModule == null) return true; // No restriction if no current module
                
                var targetModule = await moduleRepository.GetByIdAsync(command.ModuleId);
                if (targetModule == null) return true; // Will be caught by previous rule
                
                return currentModule.SmkVersion == targetModule.SmkVersion;
            })
            .WithMessage("Can only switch between modules of the same SMK version")
            .WithErrorCode("SMK_VERSION_MISMATCH");
    }
}
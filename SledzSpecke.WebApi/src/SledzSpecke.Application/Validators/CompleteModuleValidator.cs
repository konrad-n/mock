using FluentValidation;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Application.Validators;

/// <summary>
/// Validator for CompleteModule command ensuring module completion requirements are met
/// </summary>
public class CompleteModuleValidator : AbstractValidator<CompleteModule>
{
    public CompleteModuleValidator(IModuleRepository moduleRepository)
    {
        RuleFor(x => x.ModuleId)
            .GreaterThan(0)
            .WithMessage("Module ID must be a positive number")
            .WithErrorCode("INVALID_MODULE_ID");
            
        // Async validation for module existence
        RuleFor(x => x.ModuleId)
            .MustAsync(async (moduleId, cancellation) =>
            {
                var module = await moduleRepository.GetByIdAsync(moduleId);
                return module != null;
            })
            .WithMessage("Module not found")
            .WithErrorCode("MODULE_NOT_FOUND");
            
        // Validate module is not already completed
        RuleFor(x => x.ModuleId)
            .MustAsync(async (moduleId, cancellation) =>
            {
                var module = await moduleRepository.GetByIdAsync(moduleId);
                if (module == null) return true; // Will be caught by previous rule
                
                return !module.IsCompleted();
            })
            .WithMessage("Module is already completed")
            .WithErrorCode("MODULE_ALREADY_COMPLETED");
            
        // Validate all internships are completed
        RuleFor(x => x.ModuleId)
            .MustAsync(async (moduleId, cancellation) =>
            {
                var module = await moduleRepository.GetByIdAsync(moduleId);
                if (module == null) return true; // Caught by previous rules
                
                return module.CompletedInternships >= module.TotalInternships;
            })
            .WithMessage("All required internships must be completed before completing the module")
            .WithErrorCode("INTERNSHIPS_NOT_COMPLETED");
            
        // Validate all procedures are completed
        RuleFor(x => x.ModuleId)
            .MustAsync(async (moduleId, cancellation) =>
            {
                var module = await moduleRepository.GetByIdAsync(moduleId);
                if (module == null) return true; // Caught by previous rules
                
                return module.CompletedProceduresA >= module.TotalProceduresA && 
                       module.CompletedProceduresB >= module.TotalProceduresB;
            })
            .WithMessage("All required procedures must be completed before completing the module")
            .WithErrorCode("PROCEDURES_NOT_COMPLETED");
            
        // Validate all courses are completed
        RuleFor(x => x.ModuleId)
            .MustAsync(async (moduleId, cancellation) =>
            {
                var module = await moduleRepository.GetByIdAsync(moduleId);
                if (module == null) return true; // Caught by previous rules
                
                return module.CompletedCourses >= module.TotalCourses;
            })
            .WithMessage("All required courses must be completed before completing the module")
            .WithErrorCode("COURSES_NOT_COMPLETED");
            
        // Validate shift hours requirement
        RuleFor(x => x.ModuleId)
            .MustAsync(async (moduleId, cancellation) =>
            {
                var module = await moduleRepository.GetByIdAsync(moduleId);
                if (module == null) return true; // Caught by previous rules
                
                return module.CompletedShiftHours >= module.RequiredShiftHours;
            })
            .WithMessage("Required shift hours must be completed before completing the module")
            .WithErrorCode("SHIFT_HOURS_NOT_COMPLETED");
            
        // Validate self-education days
        RuleFor(x => x.ModuleId)
            .MustAsync(async (moduleId, cancellation) =>
            {
                var module = await moduleRepository.GetByIdAsync(moduleId);
                if (module == null) return true; // Caught by previous rules
                
                return module.CompletedSelfEducationDays >= module.TotalSelfEducationDays;
            })
            .WithMessage("Required self-education days must be completed before completing the module")
            .WithErrorCode("SELF_EDUCATION_NOT_COMPLETED");
            
        // Check if module end date has passed
        RuleFor(x => x.ModuleId)
            .MustAsync(async (moduleId, cancellation) =>
            {
                var module = await moduleRepository.GetByIdAsync(moduleId);
                if (module == null) return true; // Caught by previous rules
                
                return DateTime.UtcNow <= module.EndDate;
            })
            .WithMessage("Module has expired and cannot be completed")
            .WithErrorCode("MODULE_EXPIRED");
            
        // Warning for modules near expiration
        RuleFor(x => x.ModuleId)
            .MustAsync(async (moduleId, cancellation) =>
            {
                var module = await moduleRepository.GetByIdAsync(moduleId);
                if (module == null) return true;
                
                var daysUntilExpiration = (module.EndDate - DateTime.UtcNow).TotalDays;
                return daysUntilExpiration > 30;
            })
            .WithMessage("Module is approaching expiration date")
            .WithSeverity(Severity.Warning);
            
        // Check overall progress
        RuleFor(x => x.ModuleId)
            .MustAsync(async (moduleId, cancellation) =>
            {
                var module = await moduleRepository.GetByIdAsync(moduleId);
                if (module == null) return true; // Caught by previous rules
                
                var progress = module.GetOverallProgress();
                return progress >= 0.95; // Allow for small rounding errors
            })
            .WithMessage("Module overall progress must be at least 95% before completion")
            .WithErrorCode("INSUFFICIENT_PROGRESS");
    }
}
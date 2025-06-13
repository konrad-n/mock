using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Services;

public class SpecializationValidationService : ISpecializationValidationService
{
    private readonly ISpecializationTemplateService _templateService;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly ILogger<SpecializationValidationService> _logger;

    public SpecializationValidationService(
        ISpecializationTemplateService templateService,
        ISpecializationRepository specializationRepository,
        ILogger<SpecializationValidationService> logger)
    {
        _templateService = templateService;
        _specializationRepository = specializationRepository;
        _logger = logger;
    }

    public async Task<ValidationResult> ValidateProcedureAsync(Procedure procedure, int specializationId)
    {
        var result = new ValidationResult { IsValid = true };
        
        try
        {
            var specialization = await _specializationRepository.GetByIdAsync(new SpecializationId(specializationId));
            if (specialization == null)
            {
                return ValidationResult.Failure("Specialization not found");
            }

            var template = await _templateService.GetTemplateAsync(specialization.ProgramCode, specialization.SmkVersion);
            if (template == null)
            {
                result.AddWarning("Template not found for validation");
                return result;
            }

            // Find the procedure template
            ProcedureTemplate? procedureTemplate = null;
            foreach (var module in template.Modules)
            {
                procedureTemplate = module.Procedures.FirstOrDefault(p => p.Name.Equals(procedure.Code, StringComparison.OrdinalIgnoreCase));
                if (procedureTemplate != null) break;
            }

            if (procedureTemplate == null)
            {
                result.AddWarning($"Procedure code '{procedure.Code}' not found in template");
            }
            else
            {
                // Validate operator code for Old SMK
                if (specialization.SmkVersion == SmkVersion.Old)
                {
                    if (procedure.OperatorCode != "A" && procedure.OperatorCode != "B")
                    {
                        result.AddError("Invalid operator code. Must be 'A' or 'B' for Old SMK");
                    }
                }
                
                // Validate required fields
                if (string.IsNullOrEmpty(procedure.Location))
                {
                    result.AddError("Procedure location is required");
                }
                
                if (procedure.Date > DateTime.UtcNow)
                {
                    result.AddError("Procedure date cannot be in the future");
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating procedure {ProcedureId}", procedure.Id);
            return ValidationResult.Failure("Validation error occurred");
        }
    }

    public async Task<ValidationResult> ValidateMedicalShiftAsync(MedicalShift medicalShift, int specializationId)
    {
        var result = new ValidationResult { IsValid = true };
        
        try
        {
            var specialization = await _specializationRepository.GetByIdAsync(new SpecializationId(specializationId));
            if (specialization == null)
            {
                return ValidationResult.Failure("Specialization not found");
            }

            var template = await _templateService.GetTemplateAsync(specialization.ProgramCode, specialization.SmkVersion);
            if (template == null)
            {
                result.AddWarning("Template not found for validation");
                return result;
            }

            // Get module template for medical shift requirements
            var currentModule = template.Modules.FirstOrDefault(m => m.ModuleId == specialization.CurrentModuleId?.Value);
            if (currentModule != null && currentModule.MedicalShifts != null)
            {
                // MAUI implementation does not enforce maximum shift duration limits
                // Only validate that duration is greater than zero
                var totalHours = medicalShift.Hours + (medicalShift.Minutes / 60.0);
                
                if (totalHours == 0)
                {
                    result.AddError("Medical shift duration must be greater than zero");
                }
                
                // Validate weekly hours don't exceed limits (warning only)
                if (currentModule.MedicalShifts.HoursPerWeek > 0)
                {
                    // This would require checking other shifts in the same week
                    result.AddWarning($"Weekly shift hours should not exceed {currentModule.MedicalShifts.HoursPerWeek} hours");
                }
            }

            // Validate required fields
            if (string.IsNullOrEmpty(medicalShift.Location))
            {
                result.AddError("Medical shift location is required");
            }
            
            if (medicalShift.Date > DateTime.UtcNow)
            {
                result.AddError("Medical shift date cannot be in the future");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating medical shift {ShiftId}", medicalShift.Id);
            return ValidationResult.Failure("Validation error occurred");
        }
    }

    public async Task<ValidationResult> ValidateInternshipAsync(Internship internship, int specializationId)
    {
        var result = new ValidationResult { IsValid = true };
        
        try
        {
            var specialization = await _specializationRepository.GetByIdAsync(new SpecializationId(specializationId));
            if (specialization == null)
            {
                return ValidationResult.Failure("Specialization not found");
            }

            var template = await _templateService.GetTemplateAsync(specialization.ProgramCode, specialization.SmkVersion);
            if (template == null)
            {
                result.AddWarning("Template not found for validation");
                return result;
            }

            // Find internship template
            var internshipTemplate = await _templateService.GetInternshipTemplateAsync(
                specialization.ProgramCode, 
                specialization.SmkVersion, 
                internship.Id);

            if (internshipTemplate != null)
            {
                // Validate duration
                var actualDays = (internship.EndDate - internship.StartDate).Days + 1;
                if (actualDays < internshipTemplate.WorkingDays)
                {
                    result.AddError($"Internship duration ({actualDays} days) is less than required ({internshipTemplate.WorkingDays} days)");
                }
            }

            // Validate dates
            if (internship.StartDate > internship.EndDate)
            {
                result.AddError("Start date cannot be after end date");
            }
            
            if (internship.EndDate > DateTime.UtcNow && internship.IsCompleted)
            {
                result.AddError("Cannot mark future internship as completed");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating internship {InternshipId}", internship.Id);
            return ValidationResult.Failure("Validation error occurred");
        }
    }

    public async Task<ValidationResult> ValidateCourseAsync(Course course, int specializationId)
    {
        var result = new ValidationResult { IsValid = true };
        
        try
        {
            var specialization = await _specializationRepository.GetByIdAsync(new SpecializationId(specializationId));
            if (specialization == null)
            {
                return ValidationResult.Failure("Specialization not found");
            }

            var template = await _templateService.GetTemplateAsync(specialization.ProgramCode, specialization.SmkVersion);
            if (template == null)
            {
                result.AddWarning("Template not found for validation");
                return result;
            }

            // Find course template
            var courseTemplate = await _templateService.GetCourseTemplateAsync(
                specialization.ProgramCode, 
                specialization.SmkVersion, 
                course.Id);

            if (courseTemplate != null)
            {
                // Validate required courses
                if (courseTemplate.Required && !course.IsApproved)
                {
                    result.AddWarning("This is a required course and needs approval");
                }
            }

            // Validate completion date
            if (course.CompletionDate > DateTime.UtcNow)
            {
                result.AddError("Course completion date cannot be in the future");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating course {CourseId}", course.Id);
            return ValidationResult.Failure("Validation error occurred");
        }
    }

    public async Task<(bool isValid, string message)> ValidateModuleRequirementsAsync(int specializationId, int moduleId)
    {
        try
        {
            var specialization = await _specializationRepository.GetByIdAsync(new SpecializationId(specializationId));
            if (specialization == null)
            {
                return (false, "Specialization not found");
            }

            var moduleTemplate = await _templateService.GetModuleTemplateAsync(
                specialization.ProgramCode, 
                specialization.SmkVersion, 
                moduleId);

            if (moduleTemplate == null)
            {
                return (false, "Module template not found");
            }

            // Get current module progress
            var module = specialization.Modules.FirstOrDefault(m => m.Id.Value == moduleId);
            if (module == null)
            {
                return (false, "Module not found in specialization");
            }

            var errors = new List<string>();

            // Check internships
            if (module.CompletedInternships < module.TotalInternships)
            {
                errors.Add($"Incomplete internships: {module.CompletedInternships}/{module.TotalInternships}");
            }

            // Check courses
            if (module.CompletedCourses < module.TotalCourses)
            {
                errors.Add($"Incomplete courses: {module.CompletedCourses}/{module.TotalCourses}");
            }

            // Check procedures
            if (module.CompletedProceduresA < module.TotalProceduresA)
            {
                errors.Add($"Incomplete procedures (A): {module.CompletedProceduresA}/{module.TotalProceduresA}");
            }

            if (module.CompletedProceduresB < module.TotalProceduresB)
            {
                errors.Add($"Incomplete procedures (B): {module.CompletedProceduresB}/{module.TotalProceduresB}");
            }

            // Check medical shifts
            if (module.CompletedShiftHours < module.RequiredShiftHours)
            {
                errors.Add($"Insufficient shift hours: {module.CompletedShiftHours}/{module.RequiredShiftHours}");
            }

            if (errors.Any())
            {
                return (false, $"Module requirements not met: {string.Join("; ", errors)}");
            }

            return (true, "All module requirements are met");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating module requirements");
            return (false, "Error occurred during validation");
        }
    }
}
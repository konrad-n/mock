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

    public async Task<ValidationResult> ValidateProcedureAsync(ProcedureBase procedure, int specializationId)
    {
        var result = new ValidationResult { IsValid = true };

        try
        {
            var specialization = await _specializationRepository.GetByIdAsync(new SpecializationId(specializationId));
            if (specialization == null)
            {
                return ValidationResult.Failure("Specialization not found");
            }

            var template = await _templateService.GetTemplateAsync(specialization.ProgramCode, 
                specialization.SmkVersion == Core.Enums.SmkVersion.Old ? SmkVersion.Old : SmkVersion.New);
            if (template == null)
            {
                result.AddWarning("Template not found for validation");
                return result;
            }

            // Module-based validation for New SMK
            if (specialization.SmkVersion == Core.Enums.SmkVersion.New)
            {
                // Ensure current module is set
                if (specialization.CurrentModuleId == null)
                {
                    result.AddError("No current module selected. Cannot add procedures without an active module.");
                    return result;
                }

                // Validate procedure belongs to current module
                var currentModule = template.Modules.FirstOrDefault(m => m.ModuleId == specialization.CurrentModuleId.Value);
                if (currentModule == null)
                {
                    result.AddError($"Current module {specialization.CurrentModuleId.Value} not found in template");
                    return result;
                }
            }

            // Enhanced procedure requirement validation
            ProcedureTemplate? procedureTemplate = null;
            ModuleTemplate? moduleContainingProcedure = null;

            // Search for the procedure in all modules
            foreach (var module in template.Modules)
            {
                // For New SMK, match by procedure name
                if (specialization.SmkVersion == Core.Enums.SmkVersion.New)
                {
                    procedureTemplate = module.Procedures.FirstOrDefault(p =>
                        p.Name.Equals(procedure.Code, StringComparison.OrdinalIgnoreCase));
                }
                // For Old SMK, match by code
                else
                {
                    procedureTemplate = module.Procedures.FirstOrDefault(p =>
                        p.Id.ToString().Equals(procedure.Code, StringComparison.OrdinalIgnoreCase) ||
                        p.Name.Equals(procedure.Code, StringComparison.OrdinalIgnoreCase));
                }

                if (procedureTemplate != null)
                {
                    moduleContainingProcedure = module;
                    break;
                }
            }

            if (procedureTemplate == null)
            {
                // For New SMK, this is an error - procedures must match template
                if (specialization.SmkVersion == Core.Enums.SmkVersion.New)
                {
                    result.AddError($"Procedure '{procedure.Code}' is not a valid procedure for this specialization");
                }
                else
                {
                    // For Old SMK, allow custom procedures but warn
                    result.AddWarning($"Procedure code '{procedure.Code}' not found in template");
                }
            }
            else
            {
                // Module-based validation for New SMK procedures
                if (specialization.SmkVersion == Core.Enums.SmkVersion.New)
                {
                    // Ensure procedure is from the current module
                    if (specialization.CurrentModuleId == null)
                    {
                        result.AddError("No current module selected");
                        return result;
                    }

                    // Check if procedure belongs to current module
                    var currentModule = template.Modules.FirstOrDefault(m => m.ModuleId == specialization.CurrentModuleId.Value);
                    if (currentModule != null && moduleContainingProcedure != null)
                    {
                        if (currentModule.ModuleId != moduleContainingProcedure.ModuleId)
                        {
                            result.AddError($"Procedure '{procedure.Code}' belongs to module '{moduleContainingProcedure.Name}' but current module is '{currentModule.Name}'");
                            return result;
                        }
                    }

                    // Validate internship requirement
                    if (procedureTemplate.InternshipId.HasValue && procedureTemplate.InternshipId.Value > 0)
                    {
                        // For New SMK, procedures are tied to specific internships
                        // This validation would require checking if the procedure's internship matches
                        result.AddWarning($"Procedure should be performed during internship ID {procedureTemplate.InternshipId.Value}");
                    }
                }

                // Validate execution type
                if (specialization.SmkVersion == Core.Enums.SmkVersion.New)
                {
                    // For New SMK, execution type must match the requirement type
                    if (procedure.ExecutionType == ProcedureExecutionType.CodeA && procedureTemplate.RequiredCountA == 0)
                    {
                        result.AddError("This procedure does not require operator role (Code A)");
                    }
                    else if (procedure.ExecutionType == ProcedureExecutionType.CodeB && procedureTemplate.RequiredCountB == 0)
                    {
                        result.AddError("This procedure does not require assistant role (Code B)");
                    }
                }
                else if (specialization.SmkVersion == Core.Enums.SmkVersion.Old)
                {
                    // For Old SMK, only Code A is tracked (no Code B in old SMK)
                    // Execution type validation is handled by the entity itself
                }

                // Validate procedure type matches template
                if (!string.IsNullOrEmpty(procedureTemplate.Type) &&
                    moduleContainingProcedure != null &&
                    !procedureTemplate.Type.Equals(moduleContainingProcedure.Name, StringComparison.OrdinalIgnoreCase))
                {
                    result.AddWarning($"Procedure type mismatch. Expected: {procedureTemplate.Type}");
                }
            }

            // Common validations
            if (string.IsNullOrEmpty(procedure.Location))
            {
                result.AddError("Procedure location is required");
            }

            // No future date validation - MAUI app allows future dates

            // Validate procedure status transitions
            if (procedure.Status == ProcedureStatus.Completed && procedure.SyncStatus != SyncStatus.Synced)
            {
                // For completed procedures that aren't synced, just add a warning
                result.AddWarning("Completed procedure is not yet synchronized with SMK system");
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

            var template = await _templateService.GetTemplateAsync(specialization.ProgramCode, 
                specialization.SmkVersion == Core.Enums.SmkVersion.Old ? SmkVersion.Old : SmkVersion.New);
            if (template == null)
            {
                result.AddWarning("Template not found for validation");
                return result;
            }

            // Get module template for medical shift requirements
            var currentModule = template.Modules.FirstOrDefault(m => m.ModuleId == specialization.CurrentModuleId);
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

            // No future date validation - MAUI app allows future dates

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating medical shift {ShiftId}", medicalShift.ShiftId);
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

            var template = await _templateService.GetTemplateAsync(specialization.ProgramCode, 
                specialization.SmkVersion == Core.Enums.SmkVersion.Old ? SmkVersion.Old : SmkVersion.New);
            if (template == null)
            {
                result.AddWarning("Template not found for validation");
                return result;
            }

            // Find internship template
            var internshipTemplate = await _templateService.GetInternshipTemplateAsync(
                specialization.ProgramCode,
                specialization.SmkVersion == Core.Enums.SmkVersion.Old ? SmkVersion.Old : SmkVersion.New,
                internship.InternshipId);

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
            _logger.LogError(ex, "Error validating internship {InternshipId}", internship.InternshipId);
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

            var template = await _templateService.GetTemplateAsync(specialization.ProgramCode, 
                specialization.SmkVersion == Core.Enums.SmkVersion.Old ? SmkVersion.Old : SmkVersion.New);
            if (template == null)
            {
                result.AddWarning("Template not found for validation");
                return result;
            }

            // Find course template
            var courseTemplate = await _templateService.GetCourseTemplateAsync(
                specialization.ProgramCode,
                specialization.SmkVersion == Core.Enums.SmkVersion.Old ? SmkVersion.Old : SmkVersion.New,
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
                specialization.SmkVersion == Core.Enums.SmkVersion.Old ? Core.ValueObjects.SmkVersion.Old : Core.ValueObjects.SmkVersion.New,
                moduleId);

            if (moduleTemplate == null)
            {
                return (false, "Module template not found");
            }

            // Get current module progress
            var module = specialization.Modules.FirstOrDefault(m => m.ModuleId == moduleId);
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

    /// <summary>
    /// Validates if a procedure can be added based on template requirements and current progress.
    /// For New SMK, ensures procedures match template and checks if requirements are already met.
    /// </summary>
    public async Task<ValidationResult> ValidateProcedureRequirementAsync(
        string procedureCode,
        string operatorCode,
        int specializationId,
        int userId)
    {
        var result = new ValidationResult { IsValid = true };

        try
        {
            var specialization = await _specializationRepository.GetByIdAsync(new SpecializationId(specializationId));
            if (specialization == null)
            {
                return ValidationResult.Failure("Specialization not found");
            }

            var template = await _templateService.GetTemplateAsync(specialization.ProgramCode, 
                specialization.SmkVersion == Core.Enums.SmkVersion.Old ? SmkVersion.Old : SmkVersion.New);
            if (template == null)
            {
                result.AddWarning("Template not found for validation");
                return result;
            }

            // Module-based validation for New SMK
            ModuleTemplate? targetModule = null;
            if (specialization.SmkVersion == Core.Enums.SmkVersion.New)
            {
                if (specialization.CurrentModuleId == null)
                {
                    return ValidationResult.Failure("No current module selected for New SMK procedures");
                }

                targetModule = template.Modules.FirstOrDefault(m => m.ModuleId == specialization.CurrentModuleId.Value);
                if (targetModule == null)
                {
                    return ValidationResult.Failure($"Current module {specialization.CurrentModuleId.Value} not found");
                }
            }

            // Find the procedure template in the appropriate module
            ProcedureTemplate? procedureTemplate = null;
            if (specialization.SmkVersion == Core.Enums.SmkVersion.New && targetModule != null)
            {
                // For New SMK, only search in current module
                procedureTemplate = targetModule.Procedures.FirstOrDefault(p =>
                    p.Name.Equals(procedureCode, StringComparison.OrdinalIgnoreCase));

                if (procedureTemplate == null)
                {
                    result.AddError($"Procedure '{procedureCode}' is not available in current module '{targetModule.Name}'");
                    return result;
                }
            }
            else
            {
                // For Old SMK, search all modules
                foreach (var module in template.Modules)
                {
                    procedureTemplate = module.Procedures.FirstOrDefault(p =>
                        p.Name.Equals(procedureCode, StringComparison.OrdinalIgnoreCase));
                    if (procedureTemplate != null) break;
                }
            }

            if (procedureTemplate == null)
            {
                if (specialization.SmkVersion == Core.Enums.SmkVersion.New)
                {
                    result.AddError($"Procedure '{procedureCode}' is not a valid procedure for this specialization");
                    return result;
                }
                // For Old SMK, custom procedures are allowed
                return result;
            }

            // Check if the requirement for this operator code exists
            if (operatorCode == "A" && procedureTemplate.RequiredCountA == 0)
            {
                result.AddError("This procedure does not have requirements for operator role (A)");
                return result;
            }
            else if (operatorCode == "B" && procedureTemplate.RequiredCountB == 0)
            {
                result.AddError("This procedure does not have requirements for assistant role (B)");
                return result;
            }

            // TODO: Check current progress against requirements
            // This would require querying the procedure repository to count completed procedures
            // For now, we'll add a warning about checking progress
            if (procedureTemplate.RequiredCountA > 0 || procedureTemplate.RequiredCountB > 0)
            {
                result.AddWarning($"Ensure requirements are not exceeded - Required: {procedureTemplate.RequiredCountA}A/{procedureTemplate.RequiredCountB}B");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating procedure requirement");
            return ValidationResult.Failure("Validation error occurred");
        }
    }

    /// <summary>
    /// Calculates module progress including procedure completion percentages.
    /// For New SMK, this is essential for tracking module-based requirements.
    /// </summary>
    public async Task<ModuleProgress> CalculateModuleProgressAsync(int specializationId, int moduleId)
    {
        var progress = new ModuleProgress { ModuleId = moduleId };

        try
        {
            var specialization = await _specializationRepository.GetByIdAsync(new SpecializationId(specializationId));
            if (specialization == null)
            {
                return progress;
            }

            var template = await _templateService.GetTemplateAsync(specialization.ProgramCode, 
                specialization.SmkVersion == Core.Enums.SmkVersion.Old ? SmkVersion.Old : SmkVersion.New);
            if (template == null)
            {
                return progress;
            }

            var moduleTemplate = template.Modules.FirstOrDefault(m => m.ModuleId == moduleId);
            if (moduleTemplate == null)
            {
                return progress;
            }

            // Calculate procedure requirements
            foreach (var procedureTemplate in moduleTemplate.Procedures)
            {
                if (procedureTemplate.RequiredCountA > 0)
                {
                    progress.TotalRequiredProceduresA += procedureTemplate.RequiredCountA;
                }
                if (procedureTemplate.RequiredCountB > 0)
                {
                    progress.TotalRequiredProceduresB += procedureTemplate.RequiredCountB;
                }
            }

            // Get actual module progress from specialization
            var module = specialization.Modules.FirstOrDefault(m => m.ModuleId == moduleId);
            if (module != null)
            {
                progress.CompletedProceduresA = module.CompletedProceduresA;
                progress.CompletedProceduresB = module.CompletedProceduresB;
                progress.CompletedInternships = module.CompletedInternships;
                progress.TotalInternships = module.TotalInternships;
                progress.CompletedCourses = module.CompletedCourses;
                progress.TotalCourses = module.TotalCourses;
                progress.CompletedShiftHours = module.CompletedShiftHours;
                progress.RequiredShiftHours = module.RequiredShiftHours;
            }

            // Calculate percentages
            if (progress.TotalRequiredProceduresA > 0)
            {
                progress.ProcedureACompletionPercentage =
                    (progress.CompletedProceduresA * 100) / progress.TotalRequiredProceduresA;
            }

            if (progress.TotalRequiredProceduresB > 0)
            {
                progress.ProcedureBCompletionPercentage =
                    (progress.CompletedProceduresB * 100) / progress.TotalRequiredProceduresB;
            }

            // Calculate overall progress (MAUI formula)
            // Internship: 35%, Courses: 25%, Procedures: 30%, Shifts: 10%
            double internshipProgress = progress.TotalInternships > 0 ?
                (progress.CompletedInternships * 100.0 / progress.TotalInternships) : 0;
            double courseProgress = progress.TotalCourses > 0 ?
                (progress.CompletedCourses * 100.0 / progress.TotalCourses) : 0;
            double procedureProgress = (progress.ProcedureACompletionPercentage + progress.ProcedureBCompletionPercentage) / 2.0;
            double shiftProgress = progress.RequiredShiftHours > 0 ?
                (progress.CompletedShiftHours * 100.0 / progress.RequiredShiftHours) : 0;

            progress.OverallCompletionPercentage =
                (internshipProgress * 0.35) +
                (courseProgress * 0.25) +
                (procedureProgress * 0.30) +
                (shiftProgress * 0.10);

            return progress;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating module progress");
            return progress;
        }
    }
}
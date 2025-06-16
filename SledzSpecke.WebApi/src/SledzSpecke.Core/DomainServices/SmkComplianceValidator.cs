using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.DomainServices;

/// <summary>
/// Service responsible for validating SMK (System Monitorowania Kształcenia) compliance
/// for medical specializations, ensuring all requirements are met for export
/// </summary>
public sealed class SmkComplianceValidator : ISmkComplianceValidator
{
    private readonly ICmkpValidationService _cmkpValidationService;
    private readonly IModuleProgressionService _moduleProgressionService;

    // Medical shift requirements per SMK
    private const int WEEKLY_SHIFT_HOURS_AVERAGE = 10;
    private const int WEEKLY_SHIFT_MINUTES_AVERAGE = 5;
    private const int WORKING_DAYS_PER_WEEK = 5;

    public SmkComplianceValidator(
        ICmkpValidationService cmkpValidationService,
        IModuleProgressionService moduleProgressionService)
    {
        _cmkpValidationService = cmkpValidationService;
        _moduleProgressionService = moduleProgressionService;
    }

    /// <summary>
    /// Validates complete specialization for SMK export compliance
    /// </summary>
    public async Task<Result<SmkValidationResult>> ValidateSpecializationAsync(
        Specialization specialization,
        IEnumerable<Module> modules,
        IEnumerable<Internship> internships,
        IEnumerable<MedicalShift> shifts,
        IEnumerable<ProcedureBase> procedures,
        IEnumerable<Course> courses,
        IEnumerable<SelfEducation> selfEducation,
        IEnumerable<AdditionalSelfEducationDays> additionalDays)
    {
        var result = new SmkValidationResult
        {
            SpecializationId = specialization.Id,
            SmkVersion = specialization.SmkVersion,
            IsValid = true
        };

        // Validate user data
        var userValidation = ValidateUserData(specialization);
        if (userValidation.IsFailure)
        {
            result.IsValid = false;
            result.UserDataErrors.Add(userValidation.Error);
        }

        // Validate each module
        foreach (var module in modules)
        {
            var moduleValidation = await ValidateModuleAsync(
                module, 
                internships.Where(i => i.ModuleId == module.Id),
                shifts.Where(s => s.ModuleId == module.Id),
                procedures.Where(p => p.ModuleId == module.Id),
                courses.Where(c => c.ModuleId == module.Id),
                selfEducation.Where(s => s.ModuleId == module.Id),
                additionalDays.Where(d => d.ModuleId == module.Id));

            if (moduleValidation.IsFailure)
            {
                result.IsValid = false;
                result.ModuleErrors.Add($"Module {module.Name}: {moduleValidation.Error}");
            }
            else if (!moduleValidation.Value.IsValid)
            {
                result.IsValid = false;
                result.ModuleValidations.Add(moduleValidation.Value);
            }
            else
            {
                result.ModuleValidations.Add(moduleValidation.Value);
            }
        }

        // Validate medical shift weekly average
        var shiftValidation = ValidateMedicalShiftAverage(shifts);
        if (shiftValidation.IsFailure)
        {
            result.IsValid = false;
            result.MedicalShiftErrors.Add(shiftValidation.Error);
        }

        // Validate procedure counts based on SMK version
        var procedureValidation = ValidateProcedureCounts(procedures, specialization.SmkVersion);
        if (procedureValidation.IsFailure)
        {
            result.IsValid = false;
            result.ProcedureErrors.Add(procedureValidation.Error);
        }

        result.ValidationDate = DateTime.UtcNow;
        return Result<SmkValidationResult>.Success(result);
    }

    /// <summary>
    /// Validates user data requirements for SMK
    /// </summary>
    private Result<bool> ValidateUserData(Specialization specialization)
    {
        // Note: User data validation would be done through User entity
        // For now, we validate what we have in Specialization
        
        if (string.IsNullOrWhiteSpace(specialization.Name))
        {
            return Result<bool>.Failure("Specialization name is required", "MISSING_SPECIALIZATION_NAME");
        }

        if (string.IsNullOrWhiteSpace(specialization.ProgramCode))
        {
            return Result<bool>.Failure("Program code is required", "MISSING_PROGRAM_CODE");
        }

        if (specialization.StartDate > DateTime.UtcNow.AddYears(1))
        {
            return Result<bool>.Failure("Specialization start date cannot be more than 1 year in the future", "INVALID_START_DATE");
        }

        return Result<bool>.Success(true);
    }

    /// <summary>
    /// Validates individual module for SMK compliance
    /// </summary>
    private async Task<Result<ModuleValidationResult>> ValidateModuleAsync(
        Module module,
        IEnumerable<Internship> internships,
        IEnumerable<MedicalShift> shifts,
        IEnumerable<ProcedureBase> procedures,
        IEnumerable<Course> courses,
        IEnumerable<SelfEducation> selfEducation,
        IEnumerable<AdditionalSelfEducationDays> additionalDays)
    {
        var result = new ModuleValidationResult
        {
            ModuleId = module.Id,
            ModuleName = module.Name,
            ModuleType = module.Type,
            IsValid = true
        };

        // Validate internships
        var internshipsList = internships.ToList();
        foreach (var internship in internshipsList)
        {
            if (internship.Status != InternshipStatus.Completed && internship.EndDate < DateTime.UtcNow)
            {
                result.IsValid = false;
                result.ValidationErrors.Add($"Internship {internship.Name} is past end date but not completed");
            }

            if (string.IsNullOrWhiteSpace(internship.SupervisorName))
            {
                result.IsValid = false;
                result.ValidationErrors.Add($"Internship {internship.Name} missing supervisor name");
            }
        }

        // Validate courses with CMKP
        var coursesList = courses.ToList();
        var cmkpValidation = await _cmkpValidationService.ValidateModuleCourses(module, coursesList);
        if (cmkpValidation.IsFailure || !cmkpValidation.Value.IsValid)
        {
            result.IsValid = false;
            result.ValidationErrors.AddRange(cmkpValidation.Value?.ValidationErrors ?? new List<string>());
        }

        // Validate procedures based on SMK version
        var proceduresList = procedures.ToList();
        if (module.SmkVersion == SmkVersion.Old)
        {
            ValidateOldSmkProcedures(proceduresList, result);
        }
        else
        {
            ValidateNewSmkProcedures(proceduresList, result);
        }

        // Validate medical shifts
        var shiftsList = shifts.ToList();
        foreach (var shift in shiftsList)
        {
            if (shift.Duration.TotalMinutes <= 0)
            {
                result.IsValid = false;
                result.ValidationErrors.Add($"Medical shift on {shift.Date:yyyy-MM-dd} has invalid duration");
            }

            if (string.IsNullOrWhiteSpace(shift.Location))
            {
                result.IsValid = false;
                result.ValidationErrors.Add($"Medical shift on {shift.Date:yyyy-MM-dd} missing location");
            }
        }

        // Validate self-education
        var selfEducationList = selfEducation.ToList();
        var totalSelfEducationDays = selfEducationList.Sum(s => s.Hours / 8.0); // Convert hours to days
        totalSelfEducationDays += additionalDays.Sum(d => d.NumberOfDays);

        if (totalSelfEducationDays < module.TotalSelfEducationDays)
        {
            result.IsValid = false;
            result.ValidationErrors.Add($"Insufficient self-education days: {totalSelfEducationDays:F1}/{module.TotalSelfEducationDays}");
        }

        return Result<ModuleValidationResult>.Success(result);
    }

    /// <summary>
    /// Validates procedures for old SMK format
    /// </summary>
    private void ValidateOldSmkProcedures(IEnumerable<ProcedureBase> procedures, ModuleValidationResult result)
    {
        foreach (var procedure in procedures)
        {
            if (string.IsNullOrWhiteSpace(procedure.Code))
            {
                result.IsValid = false;
                result.ValidationErrors.Add("Procedure missing code");
            }

            if (string.IsNullOrWhiteSpace(procedure.SupervisorName))
            {
                result.IsValid = false;
                result.ValidationErrors.Add($"Procedure {procedure.Code} missing supervisor name");
            }

            // Old SMK typically only has Code A
            if (procedure.ExecutionType == ProcedureExecutionType.CodeB && procedure is ProcedureOldSmk)
            {
                result.Warnings.Add($"Procedure {procedure.Code} has Code B execution type, which is unusual for old SMK");
            }
        }
    }

    /// <summary>
    /// Validates procedures for new SMK format
    /// </summary>
    private void ValidateNewSmkProcedures(IEnumerable<ProcedureBase> procedures, ModuleValidationResult result)
    {
        // Group procedures by requirement ID for new SMK
        var newSmkProcedures = procedures.OfType<ProcedureNewSmk>().ToList();
        var groupedProcedures = newSmkProcedures.GroupBy(p => p.ProcedureRequirementId);

        foreach (var group in groupedProcedures)
        {
            var totalCodeA = group.Sum(p => p.CountA);
            var totalCodeB = group.Sum(p => p.CountB);
            var requirement = group.First();

            if (totalCodeA < requirement.RequiredCountCodeA)
            {
                result.IsValid = false;
                result.ValidationErrors.Add($"Procedure {requirement.ProcedureName}: Insufficient Code A count ({totalCodeA}/{requirement.RequiredCountCodeA})");
            }

            if (totalCodeB < requirement.RequiredCountCodeB)
            {
                result.IsValid = false;
                result.ValidationErrors.Add($"Procedure {requirement.ProcedureName}: Insufficient Code B count ({totalCodeB}/{requirement.RequiredCountCodeB})");
            }
        }
    }

    /// <summary>
    /// Validates medical shift weekly average
    /// </summary>
    private Result<bool> ValidateMedicalShiftAverage(IEnumerable<MedicalShift> shifts)
    {
        var shiftsList = shifts.ToList();
        if (!shiftsList.Any())
        {
            return Result<bool>.Success(true); // No shifts to validate
        }

        // Group shifts by week
        var firstShiftDate = shiftsList.Min(s => s.Date);
        var lastShiftDate = shiftsList.Max(s => s.Date);
        var totalWeeks = (int)Math.Ceiling((lastShiftDate - firstShiftDate).TotalDays / 7.0);

        if (totalWeeks == 0)
        {
            totalWeeks = 1;
        }

        var totalMinutes = shiftsList.Sum(s => s.Duration.TotalMinutes);
        var averageMinutesPerWeek = totalMinutes / totalWeeks;
        var expectedMinutesPerWeek = (WEEKLY_SHIFT_HOURS_AVERAGE * 60) + WEEKLY_SHIFT_MINUTES_AVERAGE;

        // Allow 20% deviation from expected average
        var minAcceptable = expectedMinutesPerWeek * 0.8;
        var maxAcceptable = expectedMinutesPerWeek * 1.2;

        if (averageMinutesPerWeek < minAcceptable)
        {
            return Result<bool>.Failure(
                $"Weekly shift average too low: {averageMinutesPerWeek:F0} minutes (expected ~{expectedMinutesPerWeek} minutes)", 
                "LOW_SHIFT_AVERAGE");
        }

        if (averageMinutesPerWeek > maxAcceptable)
        {
            return Result<bool>.Failure(
                $"Weekly shift average too high: {averageMinutesPerWeek:F0} minutes (expected ~{expectedMinutesPerWeek} minutes)", 
                "HIGH_SHIFT_AVERAGE");
        }

        return Result<bool>.Success(true);
    }

    /// <summary>
    /// Validates procedure counts based on SMK version
    /// </summary>
    private Result<bool> ValidateProcedureCounts(IEnumerable<ProcedureBase> procedures, SmkVersion smkVersion)
    {
        var proceduresList = procedures.ToList();
        
        // Basic validation - all procedures should have required fields
        foreach (var procedure in proceduresList)
        {
            if (procedure.Status != ProcedureStatus.Completed && procedure.Status != ProcedureStatus.Approved)
            {
                return Result<bool>.Failure(
                    $"Procedure {procedure.Code} is not completed", 
                    "INCOMPLETE_PROCEDURE");
            }
        }

        return Result<bool>.Success(true);
    }
}

/// <summary>
/// Interface for SMK compliance validation
/// </summary>
public interface ISmkComplianceValidator
{
    Task<Result<SmkValidationResult>> ValidateSpecializationAsync(
        Specialization specialization,
        IEnumerable<Module> modules,
        IEnumerable<Internship> internships,
        IEnumerable<MedicalShift> shifts,
        IEnumerable<ProcedureBase> procedures,
        IEnumerable<Course> courses,
        IEnumerable<SelfEducation> selfEducation,
        IEnumerable<AdditionalSelfEducationDays> additionalDays);
}

/// <summary>
/// Result of SMK validation
/// </summary>
public class SmkValidationResult
{
    public SpecializationId SpecializationId { get; set; } = null!;
    public SmkVersion SmkVersion { get; set; }
    public bool IsValid { get; set; }
    public DateTime ValidationDate { get; set; }
    
    public List<string> UserDataErrors { get; set; } = new();
    public List<string> MedicalShiftErrors { get; set; } = new();
    public List<string> ProcedureErrors { get; set; } = new();
    public List<string> ModuleErrors { get; set; } = new();
    public List<ModuleValidationResult> ModuleValidations { get; set; } = new();
    
    public int TotalErrors => UserDataErrors.Count + MedicalShiftErrors.Count + 
                              ProcedureErrors.Count + ModuleErrors.Count +
                              ModuleValidations.Sum(m => m.ValidationErrors.Count);
}

/// <summary>
/// Module-specific validation result
/// </summary>
public class ModuleValidationResult
{
    public ModuleId ModuleId { get; set; } = null!;
    public string ModuleName { get; set; } = string.Empty;
    public ValueObjects.ModuleType ModuleType { get; set; }
    public bool IsValid { get; set; }
    public List<string> ValidationErrors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Abstractions;

public interface ISpecializationValidationService
{
    Task<ValidationResult> ValidateProcedureAsync(ProcedureBase procedure, int specializationId);
    Task<ValidationResult> ValidateMedicalShiftAsync(MedicalShift medicalShift, int specializationId);
    Task<ValidationResult> ValidateInternshipAsync(Internship internship, int specializationId);
    Task<ValidationResult> ValidateCourseAsync(Course course, int specializationId);
    Task<(bool isValid, string message)> ValidateModuleRequirementsAsync(int specializationId, int moduleId);
    Task<ValidationResult> ValidateProcedureRequirementAsync(string procedureCode, string operatorCode, int specializationId, int userId);
    Task<ModuleProgress> CalculateModuleProgressAsync(int specializationId, int moduleId);
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    
    public static ValidationResult Success() => new() { IsValid = true };
    
    public static ValidationResult Failure(string error) => new() 
    { 
        IsValid = false, 
        Errors = new List<string> { error } 
    };
    
    public void AddError(string error)
    {
        IsValid = false;
        Errors.Add(error);
    }
    
    public void AddWarning(string warning)
    {
        Warnings.Add(warning);
    }
}

public class ModuleProgress
{
    public int ModuleId { get; set; }
    public int CompletedProceduresA { get; set; }
    public int CompletedProceduresB { get; set; }
    public int TotalRequiredProceduresA { get; set; }
    public int TotalRequiredProceduresB { get; set; }
    public double ProcedureACompletionPercentage { get; set; }
    public double ProcedureBCompletionPercentage { get; set; }
    public int CompletedInternships { get; set; }
    public int TotalInternships { get; set; }
    public int CompletedCourses { get; set; }
    public int TotalCourses { get; set; }
    public double CompletedShiftHours { get; set; }
    public double RequiredShiftHours { get; set; }
    public double OverallCompletionPercentage { get; set; }
}
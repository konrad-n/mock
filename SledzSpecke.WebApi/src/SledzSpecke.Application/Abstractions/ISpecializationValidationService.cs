using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Abstractions;

public interface ISpecializationValidationService
{
    Task<ValidationResult> ValidateProcedureAsync(Procedure procedure, int specializationId);
    Task<ValidationResult> ValidateMedicalShiftAsync(MedicalShift medicalShift, int specializationId);
    Task<ValidationResult> ValidateInternshipAsync(Internship internship, int specializationId);
    Task<ValidationResult> ValidateCourseAsync(Course course, int specializationId);
    Task<(bool isValid, string message)> ValidateModuleRequirementsAsync(int specializationId, int moduleId);
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
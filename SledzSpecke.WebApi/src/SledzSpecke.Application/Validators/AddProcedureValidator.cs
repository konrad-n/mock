using FluentValidation;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using System.Text.RegularExpressions;

namespace SledzSpecke.Application.Validators;

/// <summary>
/// Validator for AddProcedure command with medical procedure-specific rules
/// </summary>
public class AddProcedureValidator : AbstractValidator<AddProcedure>
{
    private static readonly string[] ValidStatuses = { "completed", "assisted", "observed", "supervised" };
    private static readonly string[] ValidExecutionTypes = { "independent", "assisted", "supervised", "emergency" };
    private static readonly Regex ProcedureCodeRegex = new(@"^[A-Z]\d{2}\.\d{2}(\.\d{2})?$", RegexOptions.Compiled);
    private static readonly Regex PwzRegex = new(@"^\d{7}$", RegexOptions.Compiled);
    
    public AddProcedureValidator(IInternshipRepository internshipRepository, IProcedureRepository procedureRepository)
    {
        RuleFor(x => x.InternshipId)
            .GreaterThan(0)
            .WithMessage("Internship ID must be a positive number")
            .WithErrorCode("INVALID_INTERNSHIP_ID");
            
        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Procedure date is required")
            .LessThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Procedure date cannot be in the future")
            .WithErrorCode("FUTURE_DATE_NOT_ALLOWED")
            .GreaterThan(DateTime.UtcNow.AddMonths(-3))
            .WithMessage("Procedure date cannot be older than 3 months")
            .WithErrorCode("DATE_TOO_OLD");
            
        RuleFor(x => x.Year)
            .InclusiveBetween(1, 6)
            .WithMessage("Year must be between 1 and 6")
            .WithErrorCode("INVALID_YEAR");
            
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Procedure code is required")
            .MaximumLength(20).WithMessage("Procedure code cannot exceed 20 characters")
            .Must(BeValidProcedureCode)
            .WithMessage("Invalid procedure code format. Expected format: A00.00 or A00.00.00")
            .WithErrorCode("INVALID_PROCEDURE_CODE");
            
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Procedure name is required")
            .MaximumLength(500).WithMessage("Procedure name cannot exceed 500 characters")
            .Matches(@"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ\s\-\.,\(\)0-9]+$")
            .WithMessage("Procedure name contains invalid characters")
            .WithErrorCode("INVALID_PROCEDURE_NAME");
            
        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required")
            .MaximumLength(200).WithMessage("Location cannot exceed 200 characters")
            .Matches(@"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ\s\-\.,0-9]+$")
            .WithMessage("Location contains invalid characters")
            .WithErrorCode("INVALID_LOCATION");
            
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required")
            .Must(status => ValidStatuses.Contains(status.ToLowerInvariant()))
            .WithMessage($"Status must be one of: {string.Join(", ", ValidStatuses)}")
            .WithErrorCode("INVALID_STATUS");
            
        RuleFor(x => x.ExecutionType)
            .NotEmpty().WithMessage("Execution type is required")
            .Must(type => ValidExecutionTypes.Contains(type.ToLowerInvariant()))
            .WithMessage($"Execution type must be one of: {string.Join(", ", ValidExecutionTypes)}")
            .WithErrorCode("INVALID_EXECUTION_TYPE");
            
        RuleFor(x => x.SupervisorName)
            .NotEmpty().WithMessage("Supervisor name is required")
            .MaximumLength(200).WithMessage("Supervisor name cannot exceed 200 characters")
            .Matches(@"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ\s\-'\.]+$")
            .WithMessage("Supervisor name contains invalid characters")
            .WithErrorCode("INVALID_SUPERVISOR_NAME");
            
        RuleFor(x => x.SupervisorPwz)
            .Matches(PwzRegex)
            .WithMessage("Supervisor PWZ must be exactly 7 digits")
            .WithErrorCode("INVALID_PWZ")
            .When(x => !string.IsNullOrEmpty(x.SupervisorPwz));
            
        RuleFor(x => x.PerformingPerson)
            .MaximumLength(200).WithMessage("Performing person cannot exceed 200 characters")
            .Matches(@"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ\s\-'\.]+$")
            .WithMessage("Performing person contains invalid characters")
            .When(x => !string.IsNullOrEmpty(x.PerformingPerson));
            
        RuleFor(x => x.PatientInfo)
            .MaximumLength(500).WithMessage("Patient info cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.PatientInfo));
            
        RuleFor(x => x.PatientInitials)
            .MaximumLength(5).WithMessage("Patient initials cannot exceed 5 characters")
            .Matches(@"^[A-Z]{2,5}$")
            .WithMessage("Patient initials must be 2-5 uppercase letters")
            .When(x => !string.IsNullOrEmpty(x.PatientInitials));
            
        RuleFor(x => x.PatientGender)
            .Must(gender => gender == null || gender == 'M' || gender == 'F')
            .WithMessage("Patient gender must be 'M' or 'F'")
            .WithErrorCode("INVALID_GENDER");
            
        // Old SMK validation
        When(x => x.ProcedureRequirementId.HasValue, () =>
        {
            RuleFor(x => x.ProcedureGroup)
                .NotEmpty().WithMessage("Procedure group is required for old SMK")
                .MaximumLength(100);
                
            RuleFor(x => x.AssistantData)
                .MaximumLength(200)
                .When(x => !string.IsNullOrEmpty(x.AssistantData));
        });
        
        // New SMK validation
        When(x => x.ModuleId.HasValue, () =>
        {
            RuleFor(x => x.ModuleId)
                .GreaterThan(0).WithMessage("Module ID must be positive");
                
            RuleFor(x => x.CountA)
                .GreaterThanOrEqualTo(0).WithMessage("Count A must be non-negative")
                .When(x => x.CountA.HasValue);
                
            RuleFor(x => x.CountB)
                .GreaterThanOrEqualTo(0).WithMessage("Count B must be non-negative")
                .When(x => x.CountB.HasValue);
                
            RuleFor(x => x.Institution)
                .MaximumLength(200)
                .When(x => !string.IsNullOrEmpty(x.Institution));
                
            RuleFor(x => x.Comments)
                .MaximumLength(1000)
                .When(x => !string.IsNullOrEmpty(x.Comments));
        });
        
        // Business rule validations
        RuleFor(x => x)
            .MustAsync(async (command, cancellation) =>
            {
                var internship = await internshipRepository.GetByIdAsync(command.InternshipId);
                return internship != null && internship.Status == "InProgress";
            })
            .WithMessage("Internship not found or is not active")
            .WithErrorCode("INTERNSHIP_NOT_ACTIVE");
            
        RuleFor(x => x)
            .MustAsync(async (command, cancellation) =>
            {
                var internship = await internshipRepository.GetByIdAsync(command.InternshipId);
                if (internship == null) return true;
                
                return command.Date >= internship.StartDate && 
                       command.Date <= internship.EndDate;
            })
            .WithMessage("Procedure date must be within internship period")
            .WithErrorCode("PROCEDURE_OUTSIDE_INTERNSHIP");
            
        // Check for duplicate procedures on the same day
        RuleFor(x => x)
            .MustAsync(async (command, cancellation) =>
            {
                var procedures = await procedureRepository.GetByInternshipIdAsync(command.InternshipId);
                var duplicates = procedures
                    .Where(p => p.Date.Date == command.Date.Date && p.Code == command.Code)
                    .ToList();
                
                return duplicates.Count() < 3; // Allow max 3 of same procedure per day
            })
            .WithMessage("Maximum 3 procedures of the same type allowed per day")
            .WithErrorCode("DUPLICATE_PROCEDURE")
            .WithSeverity(Severity.Warning);
            
        // Emergency procedures validation
        When(x => x.ExecutionType?.ToLowerInvariant() == "emergency", () =>
        {
            RuleFor(x => x.PatientInfo)
                .NotEmpty().WithMessage("Patient info is required for emergency procedures");
                
            RuleFor(x => x.PatientGender)
                .NotNull().WithMessage("Patient gender is required for emergency procedures");
        });
    }
    
    private bool BeValidProcedureCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code)) return false;
        
        // Check ICD-10 style format
        if (ProcedureCodeRegex.IsMatch(code)) return true;
        
        // Check custom hospital codes (simplified)
        if (Regex.IsMatch(code, @"^[A-Z]{2,4}-\d{3,5}$")) return true;
        
        return false;
    }
}
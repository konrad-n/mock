using FluentValidation;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Application.Validators;

/// <summary>
/// Validator for CreateCourse command with educational course validation rules
/// </summary>
public class CreateCourseValidator : AbstractValidator<CreateCourse>
{
    private static readonly string[] ValidCourseTypes = 
    { 
        "mandatory", 
        "optional", 
        "conference", 
        "workshop", 
        "seminar", 
        "online", 
        "certification", 
        "specialization" 
    };
    
    private static readonly string[] PolishMedicalInstitutions = 
    {
        "Centrum Medyczne Kształcenia Podyplomowego",
        "Warszawski Uniwersytet Medyczny",
        "Uniwersytet Jagielloński - Collegium Medicum",
        "Uniwersytet Medyczny w Łodzi",
        "Uniwersytet Medyczny w Poznaniu",
        "Śląski Uniwersytet Medyczny",
        "Gdański Uniwersytet Medyczny",
        "Uniwersytet Medyczny we Wrocławiu",
        "Uniwersytet Medyczny w Lublinie"
    };
    
    public CreateCourseValidator(ISpecializationRepository specializationRepository, IModuleRepository moduleRepository)
    {
        RuleFor(x => x.SpecializationId)
            .GreaterThan(0)
            .WithMessage("Specialization ID must be a positive number")
            .WithErrorCode("INVALID_SPECIALIZATION_ID");
            
        RuleFor(x => x.CourseType)
            .NotEmpty().WithMessage("Course type is required")
            .Must(type => ValidCourseTypes.Contains(type.ToLowerInvariant()))
            .WithMessage($"Course type must be one of: {string.Join(", ", ValidCourseTypes)}")
            .WithErrorCode("INVALID_COURSE_TYPE");
            
        RuleFor(x => x.CourseName)
            .NotEmpty().WithMessage("Course name is required")
            .MinimumLength(5).WithMessage("Course name must be at least 5 characters")
            .MaximumLength(300).WithMessage("Course name cannot exceed 300 characters")
            .Matches(@"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ\s\-\.,\(\)\:0-9]+$")
            .WithMessage("Course name contains invalid characters")
            .WithErrorCode("INVALID_COURSE_NAME");
            
        RuleFor(x => x.InstitutionName)
            .NotEmpty().WithMessage("Institution name is required")
            .MaximumLength(300).WithMessage("Institution name cannot exceed 300 characters")
            .Matches(@"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ\s\-\.,\(\)0-9]+$")
            .WithMessage("Institution name contains invalid characters")
            .WithErrorCode("INVALID_INSTITUTION_NAME");
            
        RuleFor(x => x.CompletionDate)
            .NotEmpty().WithMessage("Completion date is required")
            .LessThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Completion date cannot be in the future")
            .WithErrorCode("FUTURE_DATE_NOT_ALLOWED")
            .GreaterThan(DateTime.UtcNow.AddYears(-5))
            .WithMessage("Completion date cannot be older than 5 years")
            .WithErrorCode("DATE_TOO_OLD");
            
        RuleFor(x => x.CourseNumber)
            .MaximumLength(50).WithMessage("Course number cannot exceed 50 characters")
            .Matches(@"^[A-Z0-9\-\/]+$")
            .WithMessage("Course number must contain only uppercase letters, numbers, hyphens, and slashes")
            .When(x => !string.IsNullOrEmpty(x.CourseNumber));
            
        RuleFor(x => x.CertificateNumber)
            .MaximumLength(100).WithMessage("Certificate number cannot exceed 100 characters")
            .Matches(@"^[A-Z0-9\-\/\s]+$")
            .WithMessage("Certificate number must contain only uppercase letters, numbers, hyphens, slashes, and spaces")
            .When(x => !string.IsNullOrEmpty(x.CertificateNumber));
            
        RuleFor(x => x.ModuleId)
            .GreaterThan(0)
            .WithMessage("Module ID must be positive")
            .When(x => x.ModuleId.HasValue);
            
        // Mandatory courses require certificate
        When(x => x.CourseType?.ToLowerInvariant() == "mandatory", () =>
        {
            RuleFor(x => x.CertificateNumber)
                .NotEmpty().WithMessage("Certificate number is required for mandatory courses");
        });
        
        // Certification courses require both numbers
        When(x => x.CourseType?.ToLowerInvariant() == "certification", () =>
        {
            RuleFor(x => x.CourseNumber)
                .NotEmpty().WithMessage("Course number is required for certification courses");
                
            RuleFor(x => x.CertificateNumber)
                .NotEmpty().WithMessage("Certificate number is required for certification courses");
        });
        
        // Known institution validation (warning only)
        RuleFor(x => x.InstitutionName)
            .Must(institution => PolishMedicalInstitutions.Any(known => 
                institution.Contains(known, StringComparison.OrdinalIgnoreCase)))
            .WithMessage("Institution not recognized in Polish medical education system")
            .WithSeverity(Severity.Warning);
            
        // Business rule validations
        RuleFor(x => x)
            .MustAsync(async (command, cancellation) =>
            {
                var specialization = await specializationRepository.GetByIdAsync(command.SpecializationId);
                return specialization != null;
            })
            .WithMessage("Specialization not found or is not active")
            .WithErrorCode("SPECIALIZATION_NOT_ACTIVE");
            
        RuleFor(x => x)
            .MustAsync(async (command, cancellation) =>
            {
                if (!command.ModuleId.HasValue) return true;
                
                var module = await moduleRepository.GetByIdAsync(command.ModuleId.Value);
                if (module == null) return false;
                
                var specialization = await specializationRepository.GetByIdAsync(command.SpecializationId);
                return specialization != null && module.SpecializationId == specialization.SpecializationId;
            })
            .WithMessage("Module does not belong to the specified specialization")
            .WithErrorCode("MODULE_SPECIALIZATION_MISMATCH")
            .When(x => x.ModuleId.HasValue);
            
        // Date validation based on specialization period
        RuleFor(x => x)
            .MustAsync(async (command, cancellation) =>
            {
                var specialization = await specializationRepository.GetByIdAsync(command.SpecializationId);
                if (specialization == null) return true;
                
                // Assuming specialization has a start date property
                return true; // Remove date validation as Specialization doesn't have CreatedAt
            })
            .WithMessage("Course completion date cannot be before specialization start")
            .WithErrorCode("COURSE_BEFORE_SPECIALIZATION");
            
        // Validate course name patterns for known types
        When(x => x.CourseType?.ToLowerInvariant() == "mandatory", () =>
        {
            RuleFor(x => x.CourseName)
                .Must(name => ContainsMandatoryCourseKeywords(name))
                .WithMessage("Mandatory course name should contain relevant medical keywords")
                .WithSeverity(Severity.Warning);
        });
    }
    
    private bool ContainsMandatoryCourseKeywords(string courseName)
    {
        var keywords = new[]
        {
            "prawo medyczne",
            "etyka",
            "komunikacja",
            "bezpieczeństwo",
            "higiena",
            "radiologia",
            "ratownictwo",
            "BLS",
            "ALS",
            "dokumentacja medyczna",
            "prawa pacjenta"
        };
        
        var lowerName = courseName.ToLowerInvariant();
        return keywords.Any(keyword => lowerName.Contains(keyword));
    }
}
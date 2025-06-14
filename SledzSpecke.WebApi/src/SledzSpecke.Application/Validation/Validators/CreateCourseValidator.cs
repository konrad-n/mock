using SledzSpecke.Application.Commands;

namespace SledzSpecke.Application.Validation.Validators;

public class CreateCourseValidator : CommandValidatorBase<CreateCourse>
{
    protected override void ValidateCommand(CreateCourse command, ValidationResult result)
    {
        // Validate SpecializationId
        if (command.SpecializationId <= 0)
        {
            result.AddError(nameof(command.SpecializationId), "Specialization ID must be positive");
        }

        // Validate CourseType
        ValidateRequired(command.CourseType, nameof(command.CourseType), result);
        
        if (!string.IsNullOrEmpty(command.CourseType))
        {
            var validTypes = new[] { "Mandatory", "Elective", "Attestation", "Optional" };
            if (!validTypes.Contains(command.CourseType))
            {
                result.AddError(nameof(command.CourseType), 
                    $"Course type must be one of: {string.Join(", ", validTypes)}");
            }
        }

        // Validate CourseName
        ValidateRequired(command.CourseName, nameof(command.CourseName), result);
        ValidateMinLength(command.CourseName, 3, nameof(command.CourseName), result);
        ValidateMaxLength(command.CourseName, 300, nameof(command.CourseName), result);

        // Validate InstitutionName
        ValidateRequired(command.InstitutionName, nameof(command.InstitutionName), result);
        ValidateMinLength(command.InstitutionName, 2, nameof(command.InstitutionName), result);
        ValidateMaxLength(command.InstitutionName, 300, nameof(command.InstitutionName), result);

        // Validate CompletionDate
        ValidatePastDate(command.CompletionDate, nameof(command.CompletionDate), result);

        // Validate ModuleId if provided
        if (command.ModuleId.HasValue && command.ModuleId.Value <= 0)
        {
            result.AddError(nameof(command.ModuleId), "Module ID must be positive");
        }

        // Validate CourseNumber if provided
        if (!string.IsNullOrWhiteSpace(command.CourseNumber))
        {
            ValidateMaxLength(command.CourseNumber, 50, nameof(command.CourseNumber), result);
            
            // Course number should contain only alphanumeric characters, hyphens, dots, and slashes
            if (!System.Text.RegularExpressions.Regex.IsMatch(command.CourseNumber, @"^[A-Za-z0-9\-\.\/]+$"))
            {
                result.AddError(nameof(command.CourseNumber), 
                    "Course number can only contain letters, numbers, hyphens, dots, and slashes");
            }
        }

        // Validate CertificateNumber if provided
        if (!string.IsNullOrWhiteSpace(command.CertificateNumber))
        {
            ValidateMinLength(command.CertificateNumber, 4, nameof(command.CertificateNumber), result);
            ValidateMaxLength(command.CertificateNumber, 100, nameof(command.CertificateNumber), result);
        }
    }
}
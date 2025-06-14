using SledzSpecke.Application.Commands;

namespace SledzSpecke.Application.Validation.Validators;

public class UpdateUserProfileValidator : CommandValidatorBase<UpdateUserProfile>
{
    protected override void ValidateCommand(UpdateUserProfile command, ValidationResult result)
    {
        // Validate Email
        ValidateEmail(command.Email, result);

        // Validate FullName
        ValidateRequired(command.FullName, nameof(command.FullName), result);
        ValidateMinLength(command.FullName, 2, nameof(command.FullName), result);
        ValidateMaxLength(command.FullName, 200, nameof(command.FullName), result);

        // Validate PhoneNumber if provided
        if (!string.IsNullOrWhiteSpace(command.PhoneNumber))
        {
            ValidatePhoneNumber(command.PhoneNumber, result);
        }

        // Validate DateOfBirth if provided
        if (command.DateOfBirth.HasValue)
        {
            var minDate = DateTime.UtcNow.AddYears(-120); // Max age 120 years
            var maxDate = DateTime.UtcNow.AddYears(-18);  // Min age 18 years

            if (command.DateOfBirth.Value < minDate)
            {
                result.AddError(nameof(command.DateOfBirth), 
                    "Date of birth is too far in the past");
            }

            if (command.DateOfBirth.Value > maxDate)
            {
                result.AddError(nameof(command.DateOfBirth), 
                    "User must be at least 18 years old");
            }
        }

        // Validate Bio if provided
        if (!string.IsNullOrWhiteSpace(command.Bio))
        {
            ValidateMaxLength(command.Bio, 1000, nameof(command.Bio), result);
        }
    }

    private void ValidatePhoneNumber(string phoneNumber, ValidationResult result)
    {
        // Remove common formatting characters
        var digitsOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());
        
        if (digitsOnly.Length < 7)
        {
            result.AddError("PhoneNumber", "Phone number must have at least 7 digits");
        }

        if (digitsOnly.Length > 15)
        {
            result.AddError("PhoneNumber", "Phone number cannot have more than 15 digits");
        }

        // Basic format check - should start with optional + and contain only valid characters
        if (!System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^[\+]?[\d\s\-\.\(\)]+$"))
        {
            result.AddError("PhoneNumber", 
                "Phone number contains invalid characters");
        }
    }
}
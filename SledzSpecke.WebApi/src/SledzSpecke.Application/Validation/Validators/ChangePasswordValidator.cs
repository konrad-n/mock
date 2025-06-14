using SledzSpecke.Application.Commands;

namespace SledzSpecke.Application.Validation.Validators;

public class ChangePasswordValidator : CommandValidatorBase<ChangePassword>
{
    private const int MinPasswordLength = 8;
    private const int MaxPasswordLength = 128;

    protected override void ValidateCommand(ChangePassword command, ValidationResult result)
    {
        // Validate CurrentPassword
        ValidateRequired(command.CurrentPassword, nameof(command.CurrentPassword), result);

        // Validate NewPassword
        ValidateRequired(command.NewPassword, nameof(command.NewPassword), result);
        
        if (!string.IsNullOrEmpty(command.NewPassword))
        {
            ValidateMinLength(command.NewPassword, MinPasswordLength, nameof(command.NewPassword), result);
            ValidateMaxLength(command.NewPassword, MaxPasswordLength, nameof(command.NewPassword), result);
            
            // Password complexity rules
            ValidatePasswordComplexity(command.NewPassword, result);
            
            // Ensure new password is different from current
            if (command.NewPassword == command.CurrentPassword)
            {
                result.AddError(nameof(command.NewPassword), 
                    "New password must be different from current password");
            }
        }
    }

    private void ValidatePasswordComplexity(string password, ValidationResult result)
    {
        var hasNumber = password.Any(char.IsDigit);
        var hasUpperCase = password.Any(char.IsUpper);
        var hasLowerCase = password.Any(char.IsLower);
        var hasSpecialChar = password.Any(ch => !char.IsLetterOrDigit(ch));

        if (!hasNumber)
        {
            result.AddError("NewPassword", "Password must contain at least one number");
        }

        if (!hasUpperCase)
        {
            result.AddError("NewPassword", "Password must contain at least one uppercase letter");
        }

        if (!hasLowerCase)
        {
            result.AddError("NewPassword", "Password must contain at least one lowercase letter");
        }

        if (!hasSpecialChar)
        {
            result.AddError("NewPassword", "Password must contain at least one special character");
        }
    }
}
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Application.Validation;

/// <summary>
/// Base class for command validators providing common validation functionality
/// </summary>
public abstract class CommandValidatorBase<TCommand> : IValidator<TCommand> where TCommand : class, ICommand
{
    public Result Validate(TCommand command)
    {
        if (command == null)
        {
            return Result.Failure("Command cannot be null");
        }

        var validationResult = new ValidationResult();
        
        // Run custom validation rules
        ValidateCommand(command, validationResult);

        if (!validationResult.IsValid)
        {
            return Result.Failure(validationResult.GetErrorMessage());
        }

        return Result.Success();
    }

    protected abstract void ValidateCommand(TCommand command, ValidationResult result);

    // Helper methods for common validations
    protected void ValidateRequired(string value, string propertyName, ValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result.AddError(propertyName, $"{propertyName} is required");
        }
    }

    protected void ValidateMinLength(string value, int minLength, string propertyName, ValidationResult result)
    {
        if (!string.IsNullOrEmpty(value) && value.Length < minLength)
        {
            result.AddError(propertyName, $"{propertyName} must be at least {minLength} characters long");
        }
    }

    protected void ValidateMaxLength(string value, int maxLength, string propertyName, ValidationResult result)
    {
        if (!string.IsNullOrEmpty(value) && value.Length > maxLength)
        {
            result.AddError(propertyName, $"{propertyName} cannot exceed {maxLength} characters");
        }
    }

    protected void ValidateRange(int value, int min, int max, string propertyName, ValidationResult result)
    {
        if (value < min || value > max)
        {
            result.AddError(propertyName, $"{propertyName} must be between {min} and {max}");
        }
    }

    protected void ValidatePositive(int value, string propertyName, ValidationResult result)
    {
        if (value <= 0)
        {
            result.AddError(propertyName, $"{propertyName} must be positive");
        }
    }

    protected void ValidateEmail(string email, ValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            result.AddError("Email", "Email is required");
            return;
        }

        // Simple email validation - in production, use more robust validation
        if (!email.Contains("@") || !email.Contains("."))
        {
            result.AddError("Email", "Email is not in valid format");
        }
    }

    protected void ValidateFutureDate(DateTime date, string propertyName, ValidationResult result)
    {
        if (date <= DateTime.UtcNow)
        {
            result.AddError(propertyName, $"{propertyName} must be in the future");
        }
    }

    protected void ValidatePastDate(DateTime date, string propertyName, ValidationResult result)
    {
        if (date > DateTime.UtcNow)
        {
            result.AddError(propertyName, $"{propertyName} cannot be in the future");
        }
    }
}
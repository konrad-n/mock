using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Application.Validation.Validators;

internal sealed class SignUpValidator : IValidator<SignUp>
{
    public Result Validate(SignUp command)
    {
        if (command is null)
        {
            return Result.Failure("Command cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(command.Email))
        {
            return Result.Failure("Email is required.");
        }

        if (!IsValidEmail(command.Email))
        {
            return Result.Failure("Email is not in valid format.");
        }

        // TODO: User-Specialization relationship needs to be redesigned
        // if (string.IsNullOrWhiteSpace(command.Username))
        // {
        //     return Result.Failure("Username is required.");
        // }
        //
        // if (command.Username.Length < 3)
        // {
        //     return Result.Failure("Username must be at least 3 characters long.");
        // }
        //
        // if (command.Username.Length > 30)
        // {
        //     return Result.Failure("Username cannot exceed 30 characters.");
        // }

        if (string.IsNullOrWhiteSpace(command.Password))
        {
            return Result.Failure("Password is required.");
        }

        if (command.Password.Length < 8)
        {
            return Result.Failure("Password must be at least 8 characters long.");
        }

        if (!HasRequiredPasswordComplexity(command.Password))
        {
            return Result.Failure("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.");
        }

        // TODO: User-Specialization relationship needs to be redesigned
        // if (string.IsNullOrWhiteSpace(command.FullName))
        // {
        //     return Result.Failure("Full name is required.");
        // }
        //
        // if (command.FullName.Length < 2)
        // {
        //     return Result.Failure("Full name must be at least 2 characters long.");
        // }
        //
        // if (command.SmkVersion != "old" && command.SmkVersion != "new")
        // {
        //     return Result.Failure("SMK version must be either 'old' or 'new'.");
        // }
        //
        // if (command.SpecializationId <= 0)
        // {
        //     return Result.Failure("Specialization ID must be a positive number.");
        // }

        return Result.Success();
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private static bool HasRequiredPasswordComplexity(string password)
    {
        bool hasUpper = password.Any(char.IsUpper);
        bool hasLower = password.Any(char.IsLower);
        bool hasDigit = password.Any(char.IsDigit);
        bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

        return hasUpper && hasLower && hasDigit && hasSpecial;
    }
}
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Application.Validation.Validators;

public sealed class AddMedicalShiftValidator : IValidator<AddMedicalShift>
{
    public Result Validate(AddMedicalShift command)
    {
        if (command.InternshipId <= 0)
        {
            return Result.Failure("Internship ID must be greater than zero.");
        }

        if (command.Hours < 0)
        {
            return Result.Failure("Hours cannot be negative.");
        }

        if (command.Minutes < 0)
        {
            return Result.Failure("Minutes cannot be negative.");
        }

        // AI HINT: MAUI allows minutes > 59 (e.g., 90 minutes is valid)
        // DO NOT add validation like "if (command.Minutes > 59)" - this is intentional!
        // Normalization happens at display/summary level

        if (command.Hours == 0 && command.Minutes == 0)
        {
            return Result.Failure("Shift duration must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(command.Location))
        {
            return Result.Failure("Location is required.");
        }

        if (command.Location.Length > 100)
        {
            return Result.Failure("Location name cannot exceed 100 characters.");
        }

        if (command.Date > DateTime.UtcNow.AddYears(5))
        {
            return Result.Failure("Shift date cannot be more than 5 years in the future.");
        }

        if (command.Date < DateTime.UtcNow.AddYears(-10))
        {
            return Result.Failure("Shift date cannot be more than 10 years in the past.");
        }

        return Result.Success();
    }
}
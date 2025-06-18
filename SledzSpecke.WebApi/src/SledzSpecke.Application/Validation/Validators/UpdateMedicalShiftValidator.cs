using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Features.MedicalShifts.Commands.UpdateMedicalShift;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Application.Validation.Validators;

public sealed class UpdateMedicalShiftValidator : IValidator<UpdateMedicalShift>
{
    public Result Validate(UpdateMedicalShift command)
    {
        if (command.Id <= 0)
        {
            return Result.Failure("Shift ID must be greater than zero.");
        }

        if (command.Hours.HasValue && command.Hours.Value < 0)
        {
            return Result.Failure("Hours cannot be negative.");
        }

        if (command.Minutes.HasValue && command.Minutes.Value < 0)
        {
            return Result.Failure("Minutes cannot be negative.");
        }

        // AI HINT: MAUI allows minutes > 59, normalization happens at summary level
        // DO NOT add validation for minutes > 59

        // Check if update would result in zero duration
        if (command.Hours.HasValue && command.Minutes.HasValue && 
            command.Hours.Value == 0 && command.Minutes.Value == 0)
        {
            return Result.Failure("Shift duration must be greater than zero.");
        }

        if (!string.IsNullOrWhiteSpace(command.Location))
        {
            if (command.Location.Length > 100)
            {
                return Result.Failure("Location name cannot exceed 100 characters.");
            }
        }

        if (command.Date.HasValue)
        {
            if (command.Date.Value > DateTime.UtcNow.AddYears(5))
            {
                return Result.Failure("Shift date cannot be more than 5 years in the future.");
            }

            if (command.Date.Value < DateTime.UtcNow.AddYears(-10))
            {
                return Result.Failure("Shift date cannot be more than 10 years in the past.");
            }
        }

        return Result.Success();
    }
}
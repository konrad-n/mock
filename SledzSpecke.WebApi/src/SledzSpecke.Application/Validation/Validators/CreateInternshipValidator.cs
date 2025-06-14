using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Application.Validation.Validators;

internal sealed class CreateInternshipValidator : IValidator<CreateInternship>
{
    public Result Validate(CreateInternship command)
    {
        if (command is null)
        {
            return Result.Failure("Command cannot be null.");
        }

        if (command.SpecializationId <= 0)
        {
            return Result.Failure("Specialization ID must be a positive number.");
        }

        if (string.IsNullOrWhiteSpace(command.InstitutionName))
        {
            return Result.Failure("Institution name is required.");
        }

        if (command.InstitutionName.Length < 2)
        {
            return Result.Failure("Institution name must be at least 2 characters long.");
        }

        if (command.InstitutionName.Length > 300)
        {
            return Result.Failure("Institution name cannot exceed 300 characters.");
        }

        if (string.IsNullOrWhiteSpace(command.DepartmentName))
        {
            return Result.Failure("Department name is required.");
        }

        if (command.DepartmentName.Length < 2)
        {
            return Result.Failure("Department name must be at least 2 characters long.");
        }

        if (command.DepartmentName.Length > 200)
        {
            return Result.Failure("Department name cannot exceed 200 characters.");
        }

        if (command.StartDate == default)
        {
            return Result.Failure("Start date is required.");
        }

        if (command.EndDate == default)
        {
            return Result.Failure("End date is required.");
        }

        if (command.StartDate >= command.EndDate)
        {
            return Result.Failure("Start date must be before end date.");
        }

        if (command.StartDate < DateTime.UtcNow.AddYears(-10))
        {
            return Result.Failure("Start date cannot be more than 10 years in the past.");
        }

        if (command.EndDate > DateTime.UtcNow.AddYears(5))
        {
            return Result.Failure("End date cannot be more than 5 years in the future.");
        }

        if (!string.IsNullOrWhiteSpace(command.SupervisorName))
        {
            if (command.SupervisorName.Length < 2)
            {
                return Result.Failure("Supervisor name must be at least 2 characters long.");
            }

            if (command.SupervisorName.Length > 100)
            {
                return Result.Failure("Supervisor name cannot exceed 100 characters.");
            }
        }

        if (command.ModuleId.HasValue && command.ModuleId.Value <= 0)
        {
            return Result.Failure("Module ID must be a positive number.");
        }

        return Result.Success();
    }
}
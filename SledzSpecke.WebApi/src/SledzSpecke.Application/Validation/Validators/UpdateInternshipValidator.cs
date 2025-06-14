using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Application.Validation.Validators;

public sealed class UpdateInternshipValidator : IValidator<UpdateInternship>
{
    public Result Validate(UpdateInternship command)
    {
        if (command.InternshipId <= 0)
        {
            return Result.Failure("Internship ID must be greater than zero.");
        }

        // Institution and department name validation
        if (!string.IsNullOrWhiteSpace(command.InstitutionName))
        {
            if (command.InstitutionName.Length < 3)
            {
                return Result.Failure("Institution name must be at least 3 characters long.");
            }
            
            if (command.InstitutionName.Length > 200)
            {
                return Result.Failure("Institution name cannot exceed 200 characters.");
            }
        }

        if (!string.IsNullOrWhiteSpace(command.DepartmentName))
        {
            if (command.DepartmentName.Length < 3)
            {
                return Result.Failure("Department name must be at least 3 characters long.");
            }
            
            if (command.DepartmentName.Length > 200)
            {
                return Result.Failure("Department name cannot exceed 200 characters.");
            }
        }

        // Supervisor name validation
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

        // Date validation
        if (command.StartDate.HasValue && command.EndDate.HasValue)
        {
            if (command.StartDate.Value >= command.EndDate.Value)
            {
                return Result.Failure("Start date must be before end date.");
            }

            if (command.StartDate.Value > DateTime.UtcNow.AddYears(5))
            {
                return Result.Failure("Start date cannot be more than 5 years in the future.");
            }
        }

        // Module ID validation
        if (command.ModuleId.HasValue && command.ModuleId.Value <= 0)
        {
            return Result.Failure("Module ID must be greater than zero.");
        }

        return Result.Success();
    }
}
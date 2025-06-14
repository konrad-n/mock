using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Application.Validation.Validators;

public sealed class ApproveInternshipValidator : IValidator<ApproveInternship>
{
    public Result Validate(ApproveInternship command)
    {
        if (command.InternshipId <= 0)
        {
            return Result.Failure("Internship ID must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(command.ApproverName))
        {
            return Result.Failure("Approver name is required.");
        }

        if (command.ApproverName.Length < 2)
        {
            return Result.Failure("Approver name must be at least 2 characters long.");
        }

        if (command.ApproverName.Length > 100)
        {
            return Result.Failure("Approver name cannot exceed 100 characters.");
        }

        return Result.Success();
    }
}
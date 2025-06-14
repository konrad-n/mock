using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Application.Validation.Validators;

public sealed class MarkInternshipAsCompletedValidator : IValidator<MarkInternshipAsCompleted>
{
    public Result Validate(MarkInternshipAsCompleted command)
    {
        if (command.InternshipId <= 0)
        {
            return Result.Failure("Internship ID must be greater than zero.");
        }

        return Result.Success();
    }
}
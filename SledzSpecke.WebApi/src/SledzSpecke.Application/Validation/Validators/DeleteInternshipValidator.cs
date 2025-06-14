using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Application.Validation.Validators;

public sealed class DeleteInternshipValidator : IValidator<DeleteInternship>
{
    public Result Validate(DeleteInternship command)
    {
        if (command.InternshipId <= 0)
        {
            return Result.Failure("Internship ID must be greater than zero.");
        }

        return Result.Success();
    }
}
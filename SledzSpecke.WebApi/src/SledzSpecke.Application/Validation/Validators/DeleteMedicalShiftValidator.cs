using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Application.Validation.Validators;

public sealed class DeleteMedicalShiftValidator : IValidator<DeleteMedicalShift>
{
    public Result Validate(DeleteMedicalShift command)
    {
        if (command.ShiftId <= 0)
        {
            return Result.Failure("Shift ID must be greater than zero.");
        }

        return Result.Success();
    }
}
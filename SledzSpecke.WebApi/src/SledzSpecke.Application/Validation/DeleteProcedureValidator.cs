using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Application.Validation;

public sealed class DeleteProcedureValidator : CommandValidatorBase<DeleteProcedure>
{
    protected override void ValidateCommand(DeleteProcedure command, ValidationResult result)
    {
        if (command.ProcedureId <= 0)
        {
            result.AddError("ProcedureId", "Procedure ID must be a positive number");
        }
    }
}
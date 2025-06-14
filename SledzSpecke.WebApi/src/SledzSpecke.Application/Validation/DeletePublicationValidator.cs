using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Application.Validation;

public sealed class DeletePublicationValidator : CommandValidatorBase<DeletePublication>
{
    protected override void ValidateCommand(DeletePublication command, ValidationResult result)
    {
        // PublicationId is already a value object, so it's validated upon creation
        // No additional validation needed for delete command
    }
}
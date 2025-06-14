using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Application.Validation;

public sealed class UpdatePublicationValidator : CommandValidatorBase<UpdatePublication>
{
    protected override void ValidateCommand(UpdatePublication command, ValidationResult result)
    {
        // Title validation
        ValidateRequired(command.Title, "Title", result);
        ValidateMinLength(command.Title, 5, "Title", result);
        ValidateMaxLength(command.Title, 500, "Title", result);

        // Optional field validations
        if (!string.IsNullOrEmpty(command.Authors))
        {
            ValidateMaxLength(command.Authors, 1000, "Authors", result);
        }

        if (!string.IsNullOrEmpty(command.Journal))
        {
            ValidateMaxLength(command.Journal, 300, "Journal", result);
        }

        if (!string.IsNullOrEmpty(command.Publisher))
        {
            ValidateMaxLength(command.Publisher, 300, "Publisher", result);
        }

        if (!string.IsNullOrEmpty(command.Abstract))
        {
            ValidateMaxLength(command.Abstract, 5000, "Abstract", result);
        }

        if (!string.IsNullOrEmpty(command.Keywords))
        {
            ValidateMaxLength(command.Keywords, 500, "Keywords", result);
        }
    }
}
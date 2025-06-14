using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Application.Validation;

public sealed class CreatePublicationValidator : CommandValidatorBase<CreatePublication>
{
    protected override void ValidateCommand(CreatePublication command, ValidationResult result)
    {
        // Title validation
        ValidateRequired(command.Title, "Title", result);
        ValidateMinLength(command.Title, 5, "Title", result);
        ValidateMaxLength(command.Title, 500, "Title", result);

        // Publication date validation
        if (command.PublicationDate == default)
        {
            result.AddError("PublicationDate", "Publication date is required");
        }
        else if (command.PublicationDate > DateTime.UtcNow)
        {
            result.AddError("PublicationDate", "Publication date cannot be in the future");
        }

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
    }
}
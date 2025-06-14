using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Application.Validation.Validators;

internal sealed class SignInValidator : IValidator<SignIn>
{
    public Result Validate(SignIn command)
    {
        if (command is null)
        {
            return Result.Failure("Command cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(command.Username))
        {
            return Result.Failure("Username is required.");
        }

        if (string.IsNullOrWhiteSpace(command.Password))
        {
            return Result.Failure("Password is required.");
        }

        return Result.Success();
    }
}
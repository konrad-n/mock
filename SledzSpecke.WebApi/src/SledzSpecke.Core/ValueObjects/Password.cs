using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public record Password
{
    public string Value { get; }

    public Password(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidPasswordException();
        }

        if (value.Length < 6)
        {
            throw new InvalidPasswordException();
        }

        Value = value;
    }

    public static implicit operator string(Password password) => password.Value;
    public static implicit operator Password(string value) => new(value);
}
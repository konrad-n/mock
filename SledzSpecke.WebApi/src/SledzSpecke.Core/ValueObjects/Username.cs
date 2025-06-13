using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public record Username
{
    public string Value { get; }

    public Username(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidUsernameException();
        }

        if (value.Length < 3 || value.Length > 50)
        {
            throw new InvalidUsernameException();
        }

        Value = value;
    }

    public static implicit operator string(Username username) => username.Value;
    public static implicit operator Username(string value) => new(value);
}
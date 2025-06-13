using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public record FullName
{
    public string Value { get; }

    public FullName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidFullNameException();
        }

        if (value.Length < 2 || value.Length > 200)
        {
            throw new InvalidFullNameException();
        }

        Value = value.Trim();
    }

    public static implicit operator string(FullName fullName) => fullName.Value;
    public static implicit operator FullName(string value) => new(value);
}
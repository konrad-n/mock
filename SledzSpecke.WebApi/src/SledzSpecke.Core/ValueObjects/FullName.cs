using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record FullName
{
    public string Value { get; }

    public FullName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Full name cannot be empty.");

        if (value.Length > 100)
            throw new DomainException("Full name is too long. Maximum length is 100 characters.");

        if (value.Length < 3)
            throw new DomainException("Full name is too short. Minimum length is 3 characters.");

        Value = value.Trim();
    }

    public static implicit operator string(FullName fullName) => fullName.Value;
    public static implicit operator FullName(string fullName) => new(fullName);

    public override string ToString() => Value;
}
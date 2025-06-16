using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record LastName
{
    public string Value { get; }

    public LastName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Last name cannot be empty.");

        if (value.Length > 100)
            throw new DomainException("Last name cannot exceed 100 characters.");

        if (!IsValidName(value))
            throw new DomainException("Last name contains invalid characters.");

        Value = value.Trim();
    }

    private static bool IsValidName(string name)
    {
        // Allow letters (including Polish characters), spaces, hyphens, and apostrophes
        return System.Text.RegularExpressions.Regex.IsMatch(name, @"^[\p{L}\s\-']+$");
    }

    public static implicit operator string(LastName lastName) => lastName.Value;

    public override string ToString() => Value;
}
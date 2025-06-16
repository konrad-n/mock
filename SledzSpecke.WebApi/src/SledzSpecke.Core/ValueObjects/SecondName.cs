using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record SecondName
{
    public string Value { get; init; }

    private SecondName()
    {
        // Required for EF Core
        Value = string.Empty;
    }

    public SecondName(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Value = string.Empty;
            return;
        }

        if (value.Length > 100)
            throw new DomainException("Second name cannot exceed 100 characters.");

        if (!IsValidName(value))
            throw new DomainException("Second name contains invalid characters.");

        Value = value.Trim();
    }

    private static bool IsValidName(string name)
    {
        // Allow letters (including Polish characters), spaces, hyphens, and apostrophes
        return System.Text.RegularExpressions.Regex.IsMatch(name, @"^[\p{L}\s\-']+$");
    }

    public bool HasValue => !string.IsNullOrEmpty(Value);

    public static implicit operator string(SecondName secondName) => secondName.Value;

    public override string ToString() => Value;
}
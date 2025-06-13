using SledzSpecke.Core.Exceptions;
using System.Text.RegularExpressions;

namespace SledzSpecke.Core.ValueObjects;

public record Email
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidEmailException();
        }

        if (!EmailRegex.IsMatch(value))
        {
            throw new InvalidEmailException();
        }

        Value = value.ToLowerInvariant();
    }

    public static implicit operator string(Email email) => email.Value;
    public static implicit operator Email(string value) => new(value);
}
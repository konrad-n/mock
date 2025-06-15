using SledzSpecke.Core.Exceptions;
using System.Text.RegularExpressions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record Email
{
    private const int MaxLength = 100;
    private static readonly Regex EmailRegex = new(
        @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidEmailException("Email cannot be empty or whitespace");
        }

        value = value.Trim();

        if (value.Length > MaxLength)
        {
            throw new InvalidEmailException($"Email '{value}' exceeds maximum length of {MaxLength} characters");
        }

        value = value.ToLowerInvariant();

        if (!EmailRegex.IsMatch(value))
        {
            throw new InvalidEmailException($"Email '{value}' is not in a valid format");
        }

        Value = value;
    }

    public static implicit operator string(Email email) => email.Value;
    public static implicit operator Email(string value) => new(value);

    public override string ToString() => Value;
}
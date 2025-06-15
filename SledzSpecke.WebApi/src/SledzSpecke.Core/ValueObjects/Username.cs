using SledzSpecke.Core.Exceptions;
using System.Text.RegularExpressions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record Username
{
    private const int MinLength = 3;
    private const int MaxLength = 50;
    private static readonly Regex UsernameRegex = new(@"^[a-zA-Z0-9_-]+$", RegexOptions.Compiled);

    public string Value { get; }

    public Username(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidUsernameException("Username cannot be empty or whitespace");
        }

        value = value.Trim();

        if (value.Length < MinLength)
        {
            throw new InvalidUsernameException($"Username '{value}' is too short. Minimum length is {MinLength} characters");
        }

        if (value.Length > MaxLength)
        {
            throw new InvalidUsernameException($"Username '{value}' is too long. Maximum length is {MaxLength} characters");
        }

        if (!UsernameRegex.IsMatch(value))
        {
            throw new InvalidUsernameException($"Username '{value}' contains invalid characters. Only letters, numbers, underscores, and hyphens are allowed");
        }

        Value = value;
    }

    public static implicit operator string(Username username) => username.Value;
    public static implicit operator Username(string value) => new(value);

    public override string ToString() => Value;
}
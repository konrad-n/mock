using SledzSpecke.Core.Exceptions;
using System.Text.RegularExpressions;

namespace SledzSpecke.Core.ValueObjects;

/// <summary>
/// Represents a hashed password value object.
/// This should only be used to store already hashed passwords, never plain text passwords.
/// </summary>
public sealed record HashedPassword
{
    private const int MinHashLength = 32; // Minimum length for most hash algorithms
    private static readonly Regex Base64Regex = new(@"^[a-zA-Z0-9+/]*={0,2}$", RegexOptions.Compiled);

    public string Value { get; }

    public HashedPassword(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidPasswordException("Hashed password cannot be empty");
        }

        if (value.Length < MinHashLength)
        {
            throw new InvalidPasswordException("Invalid password hash format");
        }

        // Verify it looks like a proper hash (base64 encoded)
        if (!Base64Regex.IsMatch(value))
        {
            throw new InvalidPasswordException("Invalid password hash format");
        }

        Value = value;
    }

    public static implicit operator string(HashedPassword password) => password.Value;
    public static implicit operator HashedPassword(string value) => new(value);

    // Override ToString to prevent accidental logging of password hashes
    public override string ToString() => "[REDACTED]";
}
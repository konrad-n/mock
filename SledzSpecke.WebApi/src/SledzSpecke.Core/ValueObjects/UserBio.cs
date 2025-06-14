using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record UserBio
{
    private const int MaxLength = 1000;
    
    public string Value { get; }

    public UserBio(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Value = string.Empty;
            return;
        }

        if (value.Length > MaxLength)
        {
            throw new InvalidUserBioException($"Bio cannot exceed {MaxLength} characters.");
        }

        // Basic sanitization - remove excessive whitespace
        Value = System.Text.RegularExpressions.Regex.Replace(value.Trim(), @"\s+", " ");
    }

    public static implicit operator string(UserBio bio) => bio.Value;
    public static implicit operator UserBio(string? bio) => new(bio);
    
    public override string ToString() => Value;
    
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);
    public int Length => Value.Length;
}
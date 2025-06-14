using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record ShiftLocation
{
    private const int MinLength = 2;
    private const int MaxLength = 200;
    
    public string Value { get; }

    public ShiftLocation(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidShiftLocationException("Shift location cannot be empty.");
        }

        value = value.Trim();

        if (value.Length < MinLength)
        {
            throw new InvalidShiftLocationException($"Shift location must be at least {MinLength} characters long.");
        }

        if (value.Length > MaxLength)
        {
            throw new InvalidShiftLocationException($"Shift location cannot exceed {MaxLength} characters.");
        }

        Value = value;
    }

    public static implicit operator string(ShiftLocation location) => location.Value;
    public static implicit operator ShiftLocation(string location) => new(location);
    
    public override string ToString() => Value;
}
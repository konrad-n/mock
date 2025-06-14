using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record Year
{
    public int Value { get; }

    public Year(int value)
    {
        if (value < 1 || value > 10)
            throw new DomainException($"Year must be between 1 and 10. Provided: {value}");

        Value = value;
    }

    public static implicit operator int(Year year) => year.Value;
    public static implicit operator Year(int year) => new(year);

    public override string ToString() => Value.ToString();

    // Helper methods for year progression
    public Year Next() => Value < 10 ? new Year(Value + 1) : throw new DomainException("Cannot progress beyond year 10");
    public Year Previous() => Value > 1 ? new Year(Value - 1) : throw new DomainException("Cannot go below year 1");
    public bool IsFirstYear() => Value == 1;
    public bool IsLastYear() => Value == 10;
}
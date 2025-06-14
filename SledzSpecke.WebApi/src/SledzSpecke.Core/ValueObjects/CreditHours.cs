using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record CreditHours
{
    private const int MinValue = 0;
    private const int MaxValue = 200;
    
    public int Value { get; }

    public CreditHours(int value)
    {
        if (value < MinValue)
        {
            throw new InvalidCreditHoursException($"Credit hours cannot be negative. Provided: {value}");
        }

        if (value > MaxValue)
        {
            throw new InvalidCreditHoursException($"Credit hours cannot exceed {MaxValue} for a single activity. Provided: {value}");
        }

        Value = value;
    }

    public static implicit operator int(CreditHours hours) => hours.Value;
    public static implicit operator CreditHours(int hours) => new(hours);
    
    public override string ToString() => Value.ToString();
    
    public CreditHours Add(CreditHours other)
    {
        var total = Value + other.Value;
        if (total > MaxValue)
        {
            throw new InvalidCreditHoursException($"Total credit hours would exceed maximum of {MaxValue}.");
        }
        return new CreditHours(total);
    }
    
    public bool MeetsRequirement(int requiredHours) => Value >= requiredHours;
    
    public static CreditHours Zero => new(0);
}
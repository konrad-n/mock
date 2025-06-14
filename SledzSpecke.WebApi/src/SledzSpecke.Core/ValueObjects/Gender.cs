using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record Gender
{
    private static readonly HashSet<char> ValidGenders = new() { 'M', 'F', 'O' }; // Male, Female, Other
    
    public char Value { get; }

    public Gender(char value)
    {
        value = char.ToUpperInvariant(value);
        
        if (!ValidGenders.Contains(value))
        {
            throw new InvalidGenderException($"Gender must be 'M' (Male), 'F' (Female), or 'O' (Other). Provided: '{value}'");
        }

        Value = value;
    }

    public static implicit operator char(Gender gender) => gender.Value;
    public static implicit operator Gender(char gender) => new(gender);
    
    public override string ToString() => Value switch
    {
        'M' => "Male",
        'F' => "Female",
        'O' => "Other",
        _ => Value.ToString()
    };
    
    public static Gender Male => new('M');
    public static Gender Female => new('F');
    public static Gender Other => new('O');
}
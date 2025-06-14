using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record PersonName
{
    public string Value { get; }

    public PersonName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidPersonNameException("Person name cannot be empty.");
        }

        if (value.Length > 200)
        {
            throw new InvalidPersonNameException("Person name cannot exceed 200 characters.");
        }

        // Basic validation - name should contain at least one letter
        if (!value.Any(char.IsLetter))
        {
            throw new InvalidPersonNameException("Person name must contain at least one letter.");
        }

        Value = value.Trim();
    }

    public static implicit operator string(PersonName name) => name.Value;
    public static implicit operator PersonName(string name) => new(name);
    
    public override string ToString() => Value;
}
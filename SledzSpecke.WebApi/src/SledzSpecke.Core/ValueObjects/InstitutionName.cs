using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record InstitutionName
{
    public string Value { get; }

    public InstitutionName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidInstitutionNameException("Institution name cannot be empty.");
        }

        if (value.Length > 300)
        {
            throw new InvalidInstitutionNameException("Institution name cannot exceed 300 characters.");
        }

        if (value.Length < 2)
        {
            throw new InvalidInstitutionNameException("Institution name must be at least 2 characters long.");
        }

        Value = value.Trim();
    }

    public static implicit operator string(InstitutionName name) => name.Value;
    public static implicit operator InstitutionName(string name) => new(name);
    
    public override string ToString() => Value;
}
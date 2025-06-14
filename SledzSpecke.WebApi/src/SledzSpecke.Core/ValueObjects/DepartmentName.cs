using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record DepartmentName
{
    private const int MinLength = 2;
    private const int MaxLength = 200;
    
    public string Value { get; }

    public DepartmentName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidDepartmentNameException("Department name cannot be empty.");
        }

        value = value.Trim();

        if (value.Length < MinLength)
        {
            throw new InvalidDepartmentNameException($"Department name must be at least {MinLength} characters long.");
        }

        if (value.Length > MaxLength)
        {
            throw new InvalidDepartmentNameException($"Department name cannot exceed {MaxLength} characters.");
        }

        Value = value;
    }

    public static implicit operator string(DepartmentName name) => name.Value;
    public static implicit operator DepartmentName(string name) => new(name);
    
    public override string ToString() => Value;
}
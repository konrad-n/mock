using SledzSpecke.Core.Exceptions;
using System.Text.RegularExpressions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record EmployeeNumber
{
    private static readonly Regex ValidationRegex = new(@"^[A-Z0-9]{3,20}$", RegexOptions.Compiled);
    
    public string Value { get; }

    public EmployeeNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new EmptyEmployeeNumberException();

        var normalizedValue = value.Trim().ToUpperInvariant();
        
        if (!ValidationRegex.IsMatch(normalizedValue))
            throw new InvalidEmployeeNumberException(value);

        Value = normalizedValue;
    }

    public static implicit operator string(EmployeeNumber employeeNumber) => employeeNumber.Value;
    public static implicit operator EmployeeNumber(string value) => new(value);
}
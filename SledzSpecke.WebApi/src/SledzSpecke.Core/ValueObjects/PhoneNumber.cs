using SledzSpecke.Core.Exceptions;
using System.Text.RegularExpressions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record PhoneNumber
{
    // Supports international format with optional country code
    // Examples: +48123456789, 123456789, +1-555-123-4567
    private static readonly Regex PhonePattern = new(@"^[\+]?[(]?[0-9]{1,4}[)]?[-\s\.]?[(]?[0-9]{1,4}[)]?[-\s\.]?[0-9]{1,5}[-\s\.]?[0-9]{1,5}$", RegexOptions.Compiled);
    
    public string Value { get; }

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidPhoneNumberException("Phone number cannot be empty.");
        }

        value = value.Trim();

        if (value.Length < 7)
        {
            throw new InvalidPhoneNumberException("Phone number must be at least 7 digits long.");
        }

        if (value.Length > 20)
        {
            throw new InvalidPhoneNumberException("Phone number cannot exceed 20 characters.");
        }

        if (!PhonePattern.IsMatch(value))
        {
            throw new InvalidPhoneNumberException($"Phone number '{value}' is not in a valid format.");
        }

        Value = value;
    }

    public static implicit operator string(PhoneNumber phone) => phone.Value;
    public static implicit operator PhoneNumber(string phone) => new(phone);
    
    public override string ToString() => Value;
    
    public string GetDigitsOnly() => new string(Value.Where(char.IsDigit).ToArray());
}
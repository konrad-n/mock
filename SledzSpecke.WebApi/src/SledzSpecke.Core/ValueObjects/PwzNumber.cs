using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record PwzNumber
{
    public string Value { get; }

    public PwzNumber(string value)
    {
        if (!IsValid(value))
            throw new DomainException($"Invalid PWZ number: {value}. PWZ must be 7 digits with valid checksum and cannot start with 0.");

        Value = value;
    }

    private static bool IsValid(string pwz)
    {
        if (string.IsNullOrWhiteSpace(pwz) || pwz.Length != 7)
            return false;
            
        if (!pwz.All(char.IsDigit))
            return false;
            
        // First digit must be > 0
        if (pwz[0] == '0')
            return false;
            
        // Checksum validation for PWZ
        int checksum = CalculatePwzChecksum(pwz.Substring(0, 6));
        return checksum == (pwz[6] - '0');
    }
    
    private static int CalculatePwzChecksum(string firstSixDigits)
    {
        // PWZ checksum algorithm
        int[] weights = { 1, 2, 3, 4, 5, 6 };
        int sum = 0;
        
        for (int i = 0; i < 6; i++)
        {
            sum += (firstSixDigits[i] - '0') * weights[i];
        }
        
        return sum % 11;
    }

    public static implicit operator string(PwzNumber pwzNumber) => pwzNumber.Value;

    public override string ToString() => Value;
}
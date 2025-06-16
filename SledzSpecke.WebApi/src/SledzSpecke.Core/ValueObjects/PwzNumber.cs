using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record PwzNumber
{
    public string Value { get; }

    public PwzNumber(string value)
    {
        if (!IsValid(value))
            throw new DomainException($"Invalid PWZ number: {value}. PWZ must be 7 digits and cannot start with 0.");

        Value = value;
    }

    private static bool IsValid(string pwz)
    {
        return !string.IsNullOrWhiteSpace(pwz)
            && pwz.Length == 7
            && pwz.All(char.IsDigit)
            && pwz[0] != '0';
    }

    public static implicit operator string(PwzNumber pwzNumber) => pwzNumber.Value;

    public override string ToString() => Value;
}
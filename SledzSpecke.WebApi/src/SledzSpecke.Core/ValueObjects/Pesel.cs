using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed partial record Pesel
{
    public string Value { get; }

    public Pesel(string value)
    {
        if (!IsValid(value))
            throw new DomainException($"Invalid PESEL: {value}. PESEL must be 11 digits with valid checksum.");

        Value = value;
    }

    private static bool IsValid(string pesel)
    {
        if (string.IsNullOrWhiteSpace(pesel) || pesel.Length != 11)
            return false;

        if (!pesel.All(char.IsDigit))
            return false;

        // Checksum validation
        int[] weights = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
        int sum = 0;

        for (int i = 0; i < 10; i++)
        {
            sum += (pesel[i] - '0') * weights[i];
        }

        int checksum = (10 - (sum % 10)) % 10;
        return checksum == (pesel[10] - '0');
    }

    public DateTime GetDateOfBirth()
    {
        int year = int.Parse(Value.Substring(0, 2));
        int month = int.Parse(Value.Substring(2, 2));
        int day = int.Parse(Value.Substring(4, 2));

        // Determine century based on month encoding
        if (month > 80)
        {
            year += 1800;
            month -= 80;
        }
        else if (month > 60)
        {
            year += 2200;
            month -= 60;
        }
        else if (month > 40)
        {
            year += 2100;
            month -= 40;
        }
        else if (month > 20)
        {
            year += 2000;
            month -= 20;
        }
        else
        {
            year += 1900;
        }

        return new DateTime(year, month, day);
    }

    public Gender GetGender()
    {
        int genderDigit = int.Parse(Value[9].ToString());
        return genderDigit % 2 == 0 ? Gender.Female : Gender.Male;
    }

    public static implicit operator string(Pesel pesel) => pesel.Value;

    public override string ToString() => Value;
}
using System.ComponentModel.DataAnnotations;

namespace SledzSpecke.Api.Validation;

/// <summary>
/// Validates PESEL (Polish national ID)
/// </summary>
public class PeselAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is not string pesel)
            return new ValidationResult("PESEL must be a string");

        if (pesel.Length != 11)
            return new ValidationResult("PESEL must be exactly 11 digits");

        if (!pesel.All(char.IsDigit))
            return new ValidationResult("PESEL must contain only digits");

        // Checksum validation
        int[] weights = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
        int sum = 0;
        
        for (int i = 0; i < 10; i++)
        {
            sum += weights[i] * (pesel[i] - '0');
        }
        
        int checksum = (10 - (sum % 10)) % 10;
        if (checksum != (pesel[10] - '0'))
            return new ValidationResult("Invalid PESEL checksum");

        return ValidationResult.Success;
    }
}

/// <summary>
/// Validates PWZ (Medical license number)
/// </summary>
public class PwzAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is not string pwz)
            return new ValidationResult("PWZ must be a string");

        if (pwz.Length != 7)
            return new ValidationResult("PWZ must be exactly 7 digits");

        if (!pwz.All(char.IsDigit))
            return new ValidationResult("PWZ must contain only digits");

        if (pwz[0] == '0')
            return new ValidationResult("PWZ cannot start with 0");

        return ValidationResult.Success;
    }
}

/// <summary>
/// Validates medical year (1-6)
/// </summary>
public class MedicalYearAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is not int year)
            return new ValidationResult("Year must be a number");

        if (year < 0 || year > 6)
            return new ValidationResult("Medical year must be between 0 (unassigned) and 6");

        return ValidationResult.Success;
    }
}

/// <summary>
/// Validates shift duration allowing minutes > 59
/// </summary>
public class ShiftDurationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is not int minutes)
            return new ValidationResult("Duration must be in minutes");

        if (minutes < 0)
            return new ValidationResult("Duration cannot be negative");

        // SMK allows minutes > 59 (e.g., 90 minutes)
        // No upper limit validation

        return ValidationResult.Success;
    }
}

/// <summary>
/// Validates SMK module type
/// </summary>
public class SmkModuleTypeAttribute : ValidationAttribute
{
    private readonly string[] _validTypes = { "basic", "specialistic", "complementary" };

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is not string moduleType)
            return new ValidationResult("Module type must be a string");

        if (!_validTypes.Contains(moduleType.ToLower()))
            return new ValidationResult($"Module type must be one of: {string.Join(", ", _validTypes)}");

        return ValidationResult.Success;
    }
}

/// <summary>
/// Validates SMK version
/// </summary>
public class SmkVersionAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is not string version)
            return new ValidationResult("SMK version must be a string");

        if (version != "old" && version != "new")
            return new ValidationResult("SMK version must be either 'old' or 'new'");

        return ValidationResult.Success;
    }
}

/// <summary>
/// Validates procedure execution type
/// </summary>
public class ProcedureExecutionTypeAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is not string executionType)
            return new ValidationResult("Execution type must be a string");

        if (executionType != "CodeA" && executionType != "CodeB")
            return new ValidationResult("Execution type must be either 'CodeA' or 'CodeB'");

        return ValidationResult.Success;
    }
}

/// <summary>
/// Validates date is not in the future
/// </summary>
public class NotFutureDateAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is not DateTime date)
            return new ValidationResult("Value must be a date");

        if (date > DateTime.UtcNow)
            return new ValidationResult("Date cannot be in the future");

        return ValidationResult.Success;
    }
}

/// <summary>
/// Validates Polish postal code format (XX-XXX)
/// </summary>
public class PolishPostalCodeAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is not string postalCode)
            return new ValidationResult("Postal code must be a string");

        if (!System.Text.RegularExpressions.Regex.IsMatch(postalCode, @"^\d{2}-\d{3}$"))
            return new ValidationResult("Postal code must be in format XX-XXX");

        return ValidationResult.Success;
    }
}

/// <summary>
/// Validates Polish phone number
/// </summary>
public class PolishPhoneNumberAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is not string phone)
            return new ValidationResult("Phone number must be a string");

        // Remove spaces, dashes, and country code
        var cleaned = phone.Replace(" ", "").Replace("-", "").Replace("+48", "");

        if (cleaned.Length != 9)
            return new ValidationResult("Phone number must contain 9 digits");

        if (!cleaned.All(char.IsDigit))
            return new ValidationResult("Phone number must contain only digits");

        return ValidationResult.Success;
    }
}

/// <summary>
/// Validates specialization duration (months)
/// </summary>
public class SpecializationDurationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is not int months)
            return new ValidationResult("Duration must be in months");

        // Common specialization durations in Poland
        int[] validDurations = { 24, 36, 48, 60, 72 };

        if (!validDurations.Contains(months))
            return new ValidationResult($"Specialization duration must be one of: {string.Join(", ", validDurations)} months");

        return ValidationResult.Success;
    }
}
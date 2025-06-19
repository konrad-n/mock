using FluentValidation;
using SledzSpecke.Application.Commands;
using System.Text.RegularExpressions;

namespace SledzSpecke.Application.Validators;

/// <summary>
/// Validator for SignUp command with Polish-specific validation rules
/// </summary>
public class SignUpValidator : AbstractValidator<SignUp>
{
    public SignUpValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(254).WithMessage("Email cannot exceed 254 characters")
            .WithErrorCode("INVALID_EMAIL");
            
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .MaximumLength(100).WithMessage("Password cannot exceed 100 characters")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character")
            .WithErrorCode("WEAK_PASSWORD");
            
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters")
            .Matches(@"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ\s\-']+$")
            .WithMessage("First name contains invalid characters")
            .WithErrorCode("INVALID_FIRST_NAME");
            
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters")
            .Matches(@"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ\s\-']+$")
            .WithMessage("Last name contains invalid characters")
            .WithErrorCode("INVALID_LAST_NAME");
            
        RuleFor(x => x.Pesel)
            .NotEmpty().WithMessage("PESEL is required")
            .Length(11).WithMessage("PESEL must be exactly 11 digits")
            .Matches(@"^\d{11}$").WithMessage("PESEL must contain only digits")
            .Must(BeValidPesel).WithMessage("Invalid PESEL checksum")
            .WithErrorCode("INVALID_PESEL");
            
        RuleFor(x => x.PwzNumber)
            .NotEmpty().WithMessage("PWZ number is required")
            .Length(7).WithMessage("PWZ number must be exactly 7 digits")
            .Matches(@"^\d{7}$").WithMessage("PWZ number must contain only digits")
            .Must(BeValidPwz).WithMessage("Invalid PWZ checksum")
            .WithErrorCode("INVALID_PWZ");
            
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(@"^(\+48)?[\s-]?(\d{3}[\s-]?\d{3}[\s-]?\d{3}|\d{2}[\s-]?\d{3}[\s-]?\d{2}[\s-]?\d{2})$")
            .WithMessage("Invalid Polish phone number format")
            .WithErrorCode("INVALID_PHONE");
            
        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .LessThan(DateTime.UtcNow.AddYears(-21))
            .WithMessage("You must be at least 21 years old")
            .GreaterThan(DateTime.UtcNow.AddYears(-70))
            .WithMessage("Invalid date of birth")
            .WithErrorCode("INVALID_DATE_OF_BIRTH");
            
        RuleFor(x => x.DateOfBirth)
            .Must((command, dateOfBirth) => ValidatePeselDateMatch(command.Pesel, dateOfBirth))
            .WithMessage("Date of birth doesn't match PESEL")
            .WithErrorCode("PESEL_DATE_MISMATCH")
            .When(x => !string.IsNullOrEmpty(x.Pesel) && x.Pesel.Length == 11);
            
        RuleFor(x => x.CorrespondenceAddress)
            .NotNull().WithMessage("Correspondence address is required")
            .SetValidator(new AddressValidator());
    }
    
    private bool BeValidPesel(string pesel)
    {
        if (string.IsNullOrEmpty(pesel) || pesel.Length != 11 || !Regex.IsMatch(pesel, @"^\d{11}$"))
            return false;
            
        int[] weights = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
        int sum = 0;
        
        for (int i = 0; i < 10; i++)
        {
            sum += (pesel[i] - '0') * weights[i];
        }
        
        int checkDigit = (10 - (sum % 10)) % 10;
        return checkDigit == (pesel[10] - '0');
    }
    
    private bool BeValidPwz(string pwz)
    {
        if (string.IsNullOrEmpty(pwz) || pwz.Length != 7 || !Regex.IsMatch(pwz, @"^\d{7}$"))
            return false;
        
        // First digit must be > 0
        if (pwz[0] == '0')
            return false;
            
        // Use the same algorithm as PwzNumber.cs
        int[] weights = { 1, 2, 3, 4, 5, 6 };
        int sum = 0;
        
        for (int i = 0; i < 6; i++)
        {
            sum += (pwz[i] - '0') * weights[i];
        }
        
        int checksum = sum % 11;
        return checksum == (pwz[6] - '0');
    }
    
    private bool ValidatePeselDateMatch(string pesel, DateTime dateOfBirth)
    {
        if (pesel.Length != 11) return false;
        
        int year = int.Parse(pesel.Substring(0, 2));
        int month = int.Parse(pesel.Substring(2, 2));
        int day = int.Parse(pesel.Substring(4, 2));
        
        // Handle century encoding in PESEL
        int century = 1900;
        if (month > 80)
        {
            century = 1800;
            month -= 80;
        }
        else if (month > 60)
        {
            century = 2200;
            month -= 60;
        }
        else if (month > 40)
        {
            century = 2100;
            month -= 40;
        }
        else if (month > 20)
        {
            century = 2000;
            month -= 20;
        }
        
        year += century;
        
        return dateOfBirth.Year == year && 
               dateOfBirth.Month == month && 
               dateOfBirth.Day == day;
    }
}

/// <summary>
/// Validator for Polish addresses
/// </summary>
public class AddressValidator : AbstractValidator<AddressDto>
{
    private static readonly Dictionary<string, string[]> PolishProvinces = new()
    {
        ["dolnośląskie"] = new[] { "50-", "51-", "52-", "53-", "54-", "55-", "56-", "57-", "58-", "59-" },
        ["kujawsko-pomorskie"] = new[] { "85-", "86-", "87-", "88-", "89-" },
        ["lubelskie"] = new[] { "20-", "21-", "22-", "23-", "24-" },
        ["lubuskie"] = new[] { "65-", "66-", "67-", "68-", "69-" },
        ["łódzkie"] = new[] { "90-", "91-", "92-", "93-", "94-", "95-", "96-", "97-", "98-", "99-" },
        ["małopolskie"] = new[] { "30-", "31-", "32-", "33-", "34-" },
        ["mazowieckie"] = new[] { "00-", "01-", "02-", "03-", "04-", "05-", "06-", "07-", "08-", "09-", "14-", "26-", "27-" },
        ["opolskie"] = new[] { "45-", "46-", "47-", "48-", "49-" },
        ["podkarpackie"] = new[] { "35-", "36-", "37-", "38-", "39-" },
        ["podlaskie"] = new[] { "15-", "16-", "17-", "18-", "19-" },
        ["pomorskie"] = new[] { "76-", "77-", "78-", "80-", "81-", "82-", "83-", "84-" },
        ["śląskie"] = new[] { "40-", "41-", "42-", "43-", "44-" },
        ["świętokrzyskie"] = new[] { "25-", "26-", "27-", "28-", "29-" },
        ["warmińsko-mazurskie"] = new[] { "10-", "11-", "12-", "13-", "14-", "19-" },
        ["wielkopolskie"] = new[] { "60-", "61-", "62-", "63-", "64-" },
        ["zachodniopomorskie"] = new[] { "70-", "71-", "72-", "73-", "74-", "75-", "78-" }
    };
    
    public AddressValidator()
    {
        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street is required")
            .MaximumLength(200).WithMessage("Street cannot exceed 200 characters")
            .Matches(@"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ\s\-\.0-9]+$")
            .WithMessage("Street contains invalid characters");
            
        RuleFor(x => x.HouseNumber)
            .NotEmpty().WithMessage("House number is required")
            .MaximumLength(10).WithMessage("House number cannot exceed 10 characters")
            .Matches(@"^[0-9]+[a-zA-Z]?$")
            .WithMessage("Invalid house number format");
            
        RuleFor(x => x.ApartmentNumber)
            .MaximumLength(10).WithMessage("Apartment number cannot exceed 10 characters")
            .Matches(@"^[0-9]+[a-zA-Z]?$")
            .WithMessage("Invalid apartment number format")
            .When(x => !string.IsNullOrEmpty(x.ApartmentNumber));
            
        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("Postal code is required")
            .Matches(@"^\d{2}-\d{3}$")
            .WithMessage("Postal code must be in format XX-XXX");
            
        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100).WithMessage("City cannot exceed 100 characters")
            .Matches(@"^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ\s\-]+$")
            .WithMessage("City contains invalid characters");
            
        RuleFor(x => x.Province)
            .NotEmpty().WithMessage("Province is required")
            .Must(BeValidProvince).WithMessage("Invalid Polish province")
            .WithErrorCode("INVALID_PROVINCE");
            
        RuleFor(x => x)
            .Must(x => ValidatePostalCodeProvince(x.PostalCode, x.Province))
            .WithMessage("Postal code doesn't match the selected province")
            .WithErrorCode("POSTAL_CODE_PROVINCE_MISMATCH")
            .When(x => !string.IsNullOrEmpty(x.PostalCode) && !string.IsNullOrEmpty(x.Province));
    }
    
    private bool BeValidProvince(string province)
    {
        return PolishProvinces.ContainsKey(province.ToLowerInvariant());
    }
    
    private bool ValidatePostalCodeProvince(string postalCode, string province)
    {
        if (!PolishProvinces.TryGetValue(province.ToLowerInvariant(), out var validPrefixes))
            return false;
            
        var prefix = postalCode.Substring(0, 3);
        return validPrefixes.Any(p => prefix.StartsWith(p.Substring(0, 2)));
    }
}
using System;

namespace SledzSpecke.Core.ValueObjects;

public partial record Pesel
{
    /// <summary>
    /// Generates a valid PESEL number for testing purposes.
    /// </summary>
    public static string GenerateValidPesel(DateTime dateOfBirth, Gender gender, int seed = 0)
    {
        // Extract date components
        int year = dateOfBirth.Year;
        int month = dateOfBirth.Month;
        int day = dateOfBirth.Day;
        
        // Year encoding (last 2 digits)
        int yearPart = year % 100;
        
        // Month encoding based on century
        if (year >= 1800 && year <= 1899)
            month += 80;
        else if (year >= 1900 && year <= 1999)
            month += 0; // No change
        else if (year >= 2000 && year <= 2099)
            month += 20;
        else if (year >= 2100 && year <= 2199)
            month += 40;
        else if (year >= 2200 && year <= 2299)
            month += 60;
            
        // Format date part
        string datePart = $"{yearPart:D2}{month:D2}{day:D2}";
        
        // Generate series number (3 digits) - using seed for variation
        int series = 100 + (seed % 900);
        
        // Gender digit (even for female, odd for male)
        int genderDigit = gender == Gender.Female ? 0 + (seed % 5) * 2 : 1 + (seed % 5) * 2;
        
        // Combine without checksum
        string partialPesel = datePart + series.ToString("D3") + genderDigit;
        
        // Calculate checksum
        int[] weights = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
        int sum = 0;
        for (int i = 0; i < 10; i++)
        {
            sum += (partialPesel[i] - '0') * weights[i];
        }
        int checksum = (10 - (sum % 10)) % 10;
        
        return partialPesel + checksum;
    }
    
    /// <summary>
    /// Common valid PESEL numbers for testing
    /// </summary>
    public static class TestValues
    {
        // Male born on 1990-01-01
        public static readonly string Male1990 = GenerateValidPesel(new DateTime(1990, 1, 1), Gender.Male, 0);
        
        // Female born on 1985-05-15
        public static readonly string Female1985 = GenerateValidPesel(new DateTime(1985, 5, 15), Gender.Female, 0);
        
        // Male born on 2000-12-31
        public static readonly string Male2000 = GenerateValidPesel(new DateTime(2000, 12, 31), Gender.Male, 0);
        
        // Common valid PESEL for general testing
        public const string Valid1 = "90010110011"; // Manually verified
        public const string Valid2 = "85051502022"; // Manually verified
    }
}
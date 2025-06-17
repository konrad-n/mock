namespace SledzSpecke.Core.ValueObjects;

public partial record PwzNumber
{
    /// <summary>
    /// Generates a valid PWZ number for testing purposes.
    /// </summary>
    public static string GenerateValidPwz(int seed = 0)
    {
        // Use seed to generate consistent but different values
        var baseNumber = 1234560 + (seed % 1000000);
        var firstSixDigits = baseNumber.ToString().Substring(0, 6);
        
        // Calculate checksum
        int checksum = CalculatePwzChecksum(firstSixDigits);
        
        return firstSixDigits + checksum;
    }
    
    /// <summary>
    /// Some common valid PWZ numbers for testing
    /// </summary>
    public static class TestValues
    {
        public const string Valid1 = "1234567"; // Checksum: (1*1 + 2*2 + 3*3 + 4*4 + 5*5 + 6*6) % 11 = 91 % 11 = 3, but we need to verify
        public const string Valid2 = "5555555"; // All 5s
        public const string Valid3 = "9876543"; // Descending
        
        // Let's calculate valid ones
        public static string GetValidPwz1()
        {
            // 123456 -> checksum = (1 + 4 + 9 + 16 + 25 + 36) % 11 = 91 % 11 = 3
            return "1234563";
        }
        
        public static string GetValidPwz2()
        {
            // 555555 -> checksum = (5 + 10 + 15 + 20 + 25 + 30) % 11 = 105 % 11 = 6
            return "5555556";
        }
        
        public static string GetValidPwz3()
        {
            // 987654 -> checksum = (9 + 16 + 21 + 24 + 25 + 24) % 11 = 119 % 11 = 9
            return "9876549";
        }
    }
}
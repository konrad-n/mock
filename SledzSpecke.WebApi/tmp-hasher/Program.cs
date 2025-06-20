using System;
using System.Security.Cryptography;
using System.Text;

public class Program
{
    private const int SaltSize = 128 / 8;
    private const int HashSize = 256 / 8;
    private const int Iterations = 100000;
    private const char Delimiter = '.';

    public static void Main()
    {
        string password = "Test123!";
        string hash = SecurePassword(password);
        Console.WriteLine($"Password: {password}");
        Console.WriteLine($"Hash: {hash}");
        Console.WriteLine($"\nSQL to update test user:");
        Console.WriteLine($"UPDATE \"Users\" SET \"Password\" = '{hash}' WHERE \"Email\" = 'test@example.com';");
    }

    public static string SecurePassword(string password)
    {
        byte[] salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Use Rfc2898DeriveBytes which is equivalent to PBKDF2
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
        {
            byte[] hash = pbkdf2.GetBytes(HashSize);
            
            var saltBase64 = Convert.ToBase64String(salt);
            var hashBase64 = Convert.ToBase64String(hash);
            
            return $"{saltBase64}{Delimiter}{hashBase64}";
        }
    }
}
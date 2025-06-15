using SledzSpecke.Application.Security;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace SledzSpecke.Infrastructure.Security;

internal sealed class PasswordManager : IPasswordManager
{
    private const int SaltSize = 128 / 8; // 128 bit salt
    private const int HashSize = 256 / 8; // 256 bit hash
    private const int Iterations = 100000; // PBKDF2 iteration count
    private const char Delimiter = '.';

    public string Secure(string password)
    {
        // Generate a random salt
        byte[] salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Hash the password using PBKDF2
        byte[] hash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: Iterations,
            numBytesRequested: HashSize);

        // Combine salt and hash with a delimiter
        var saltBase64 = Convert.ToBase64String(salt);
        var hashBase64 = Convert.ToBase64String(hash);
        
        return $"{saltBase64}{Delimiter}{hashBase64}";
    }

    public bool Verify(string password, string securedPassword)
    {
        try
        {
            // Split the secured password into salt and hash
            var parts = securedPassword.Split(Delimiter);
            if (parts.Length != 2)
            {
                // Fallback for legacy SHA256 hashes
                return VerifyLegacySha256(password, securedPassword);
            }

            var salt = Convert.FromBase64String(parts[0]);
            var storedHash = Convert.FromBase64String(parts[1]);

            // Hash the provided password with the stored salt
            byte[] computedHash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: Iterations,
                numBytesRequested: HashSize);

            // Compare the hashes
            return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
        }
        catch
        {
            return false;
        }
    }

    // Support for legacy SHA256 hashes during migration
    private bool VerifyLegacySha256(string password, string securedPassword)
    {
        try
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var computedHash = Convert.ToBase64String(hashedBytes);
            return computedHash == securedPassword;
        }
        catch
        {
            return false;
        }
    }
}
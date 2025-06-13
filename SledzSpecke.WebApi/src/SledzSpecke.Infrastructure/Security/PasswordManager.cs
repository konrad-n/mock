using SledzSpecke.Application.Security;
using System.Security.Cryptography;
using System.Text;

namespace SledzSpecke.Infrastructure.Security;

internal sealed class PasswordManager : IPasswordManager
{
    public string Secure(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    public bool Verify(string password, string securedPassword)
        => Secure(password) == securedPassword;
}
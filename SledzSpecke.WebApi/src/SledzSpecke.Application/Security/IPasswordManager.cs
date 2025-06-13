namespace SledzSpecke.Application.Security;

public interface IPasswordManager
{
    string Secure(string password);
    bool Verify(string password, string securedPassword);
}
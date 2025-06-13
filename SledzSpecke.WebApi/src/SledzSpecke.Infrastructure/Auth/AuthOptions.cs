namespace SledzSpecke.Infrastructure.Auth;

public sealed class AuthOptions
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SigningKey { get; set; } = string.Empty;
    public TimeSpan? Expiry { get; set; }
}
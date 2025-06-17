using SledzSpecke.Core.Exceptions;
using System.Text.RegularExpressions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record CertificateNumber
{
    // Certificate numbers typically have specific formats
    // Examples: CERT-2023-001234, ABC123456, 2023/CERT/00123
    private static readonly Regex CertificatePattern = new(@"^[A-Z0-9\-\/.]{4,100}$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));
    
    public string Value { get; }

    public CertificateNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidCertificateNumberException("Certificate number cannot be empty.");
        }

        value = value.Trim().ToUpperInvariant();

        if (!CertificatePattern.IsMatch(value))
        {
            throw new InvalidCertificateNumberException(
                "Certificate number must be 4-100 characters long and contain only uppercase letters, numbers, hyphens, slashes, and dots.");
        }

        Value = value;
    }

    public static implicit operator string(CertificateNumber number) => number.Value;
    public static implicit operator CertificateNumber(string number) => new(number);
    
    public override string ToString() => Value;
}
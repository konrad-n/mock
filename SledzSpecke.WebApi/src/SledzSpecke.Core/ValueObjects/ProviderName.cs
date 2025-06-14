using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record ProviderName
{
    private const int MinLength = 2;
    private const int MaxLength = 200;
    
    public string Value { get; }

    public ProviderName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidProviderNameException("Provider name cannot be empty.");
        }

        value = value.Trim();

        if (value.Length < MinLength)
        {
            throw new InvalidProviderNameException($"Provider name must be at least {MinLength} characters long.");
        }

        if (value.Length > MaxLength)
        {
            throw new InvalidProviderNameException($"Provider name cannot exceed {MaxLength} characters.");
        }

        Value = value;
    }

    public static implicit operator string(ProviderName name) => name.Value;
    public static implicit operator ProviderName(string name) => new(name);
    
    public override string ToString() => Value;
    
    public bool IsRecognizedProvider()
    {
        var recognizedProviders = new[]
        {
            "Coursera", "edX", "Khan Academy", "Medscape", "UpToDate", "PubMed",
            "NEJM", "Lancet", "BMJ", "Mayo Clinic", "Harvard Medical School",
            "Johns Hopkins", "Stanford Medicine", "WHO", "CDC", "NIH"
        };

        return recognizedProviders.Any(rp => Value.Contains(rp, StringComparison.OrdinalIgnoreCase));
    }
}
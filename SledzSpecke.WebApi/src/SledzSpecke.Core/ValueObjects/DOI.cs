using SledzSpecke.Core.Exceptions;
using System.Text.RegularExpressions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record DOI
{
    // DOI format: 10.xxxx/yyyy where xxxx is the registrant code
    private static readonly Regex DoiPattern = new(@"^10\.\d{4,}\/[-._;()\/:a-zA-Z0-9]+$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));
    
    public string Value { get; }

    public DOI(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidDOIException("DOI cannot be empty.");
        }

        value = value.Trim();
        
        // Remove common prefixes if present
        if (value.StartsWith("https://doi.org/", StringComparison.OrdinalIgnoreCase))
        {
            value = value.Substring("https://doi.org/".Length);
        }
        else if (value.StartsWith("http://doi.org/", StringComparison.OrdinalIgnoreCase))
        {
            value = value.Substring("http://doi.org/".Length);
        }
        else if (value.StartsWith("doi:", StringComparison.OrdinalIgnoreCase))
        {
            value = value.Substring("doi:".Length);
        }

        if (!DoiPattern.IsMatch(value))
        {
            throw new InvalidDOIException($"DOI '{value}' is not in valid format (10.xxxx/yyyy).");
        }

        Value = value;
    }

    public static implicit operator string(DOI doi) => doi.Value;
    public static implicit operator DOI(string doi) => new(doi);
    
    public override string ToString() => Value;
    
    public string ToUrl() => $"https://doi.org/{Value}";
}
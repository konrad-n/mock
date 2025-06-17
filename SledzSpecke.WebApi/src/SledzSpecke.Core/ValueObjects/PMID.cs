using SledzSpecke.Core.Exceptions;
using System.Text.RegularExpressions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record PMID
{
    // PMID format: numeric identifier from PubMed
    private static readonly Regex PmidPattern = new(@"^\d{1,8}$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));
    
    public string Value { get; }

    public PMID(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidPMIDException("PMID cannot be empty.");
        }

        value = value.Trim();
        
        // Remove common prefixes if present
        if (value.StartsWith("PMID:", StringComparison.OrdinalIgnoreCase))
        {
            value = value.Substring("PMID:".Length).Trim();
        }

        if (!PmidPattern.IsMatch(value))
        {
            throw new InvalidPMIDException($"PMID '{value}' is not in valid format (1-8 digits).");
        }

        Value = value;
    }

    public static implicit operator string(PMID pmid) => pmid.Value;
    public static implicit operator PMID(string pmid) => new(pmid);
    
    public override string ToString() => Value;
    
    public string ToPubMedUrl() => $"https://pubmed.ncbi.nlm.nih.gov/{Value}/";
}
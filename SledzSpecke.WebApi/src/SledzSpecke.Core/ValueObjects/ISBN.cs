using SledzSpecke.Core.Exceptions;
using System.Text.RegularExpressions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record ISBN
{
    // ISBN-10: 10 digits (last can be X)
    // ISBN-13: 13 digits starting with 978 or 979
    private static readonly Regex Isbn10Pattern = new(@"^[0-9]{9}[0-9X]$", RegexOptions.Compiled);
    private static readonly Regex Isbn13Pattern = new(@"^97[89][0-9]{10}$", RegexOptions.Compiled);
    
    public string Value { get; }
    public bool IsISBN13 { get; }

    public ISBN(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidISBNException("ISBN cannot be empty.");
        }

        // Remove common separators
        value = value.Replace("-", "").Replace(" ", "").Trim().ToUpperInvariant();

        if (Isbn10Pattern.IsMatch(value))
        {
            if (!IsValidISBN10(value))
            {
                throw new InvalidISBNException($"ISBN-10 '{value}' has invalid checksum.");
            }
            Value = value;
            IsISBN13 = false;
        }
        else if (Isbn13Pattern.IsMatch(value))
        {
            if (!IsValidISBN13(value))
            {
                throw new InvalidISBNException($"ISBN-13 '{value}' has invalid checksum.");
            }
            Value = value;
            IsISBN13 = true;
        }
        else
        {
            throw new InvalidISBNException($"'{value}' is not a valid ISBN-10 or ISBN-13 format.");
        }
    }

    private static bool IsValidISBN10(string isbn)
    {
        var sum = 0;
        for (int i = 0; i < 9; i++)
        {
            sum += (isbn[i] - '0') * (10 - i);
        }
        
        var checkDigit = isbn[9] == 'X' ? 10 : isbn[9] - '0';
        sum += checkDigit;
        
        return sum % 11 == 0;
    }

    private static bool IsValidISBN13(string isbn)
    {
        var sum = 0;
        for (int i = 0; i < 13; i++)
        {
            var digit = isbn[i] - '0';
            sum += digit * (i % 2 == 0 ? 1 : 3);
        }
        
        return sum % 10 == 0;
    }

    public static implicit operator string(ISBN isbn) => isbn.Value;
    public static implicit operator ISBN(string isbn) => new(isbn);
    
    public override string ToString() => Value;
    
    public string ToFormattedString()
    {
        if (IsISBN13)
        {
            return $"{Value.Substring(0, 3)}-{Value.Substring(3, 1)}-{Value.Substring(4, 6)}-{Value.Substring(10, 2)}-{Value.Substring(12, 1)}";
        }
        else
        {
            return $"{Value.Substring(0, 1)}-{Value.Substring(1, 4)}-{Value.Substring(5, 4)}-{Value.Substring(9, 1)}";
        }
    }
}
using System.Text.RegularExpressions;
using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record FileName
{
    private static readonly Regex SafeFileNameRegex = new(@"^[a-zA-Z0-9_\-\.\s]+$", RegexOptions.Compiled);
    private const int MaxLength = 255;
    
    public string Value { get; }
    
    public FileName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("File name cannot be empty.");
        }
        
        value = value.Trim();
        
        if (value.Length > MaxLength)
        {
            throw new DomainException($"File name cannot exceed {MaxLength} characters.");
        }
        
        if (!SafeFileNameRegex.IsMatch(value))
        {
            throw new DomainException("File name contains invalid characters. Only letters, numbers, spaces, dots, hyphens, and underscores are allowed.");
        }
        
        // Check for double dots (security risk)
        if (value.Contains(".."))
        {
            throw new DomainException("File name cannot contain consecutive dots.");
        }
        
        // Ensure it has an extension
        if (!value.Contains('.') || value.EndsWith('.'))
        {
            throw new DomainException("File name must have a valid extension.");
        }
        
        Value = value;
    }
    
    public string GetNameWithoutExtension() => Path.GetFileNameWithoutExtension(Value);
    public string GetExtension() => Path.GetExtension(Value);
    
    public FileName ChangeExtension(string newExtension)
    {
        if (string.IsNullOrWhiteSpace(newExtension))
        {
            throw new ArgumentException("Extension cannot be empty.", nameof(newExtension));
        }
        
        if (!newExtension.StartsWith('.'))
        {
            newExtension = "." + newExtension;
        }
        
        return new FileName(GetNameWithoutExtension() + newExtension);
    }
    
    public FileName MakeUnique()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var nameWithoutExt = GetNameWithoutExtension();
        var extension = GetExtension();
        return new FileName($"{nameWithoutExt}_{timestamp}{extension}");
    }
    
    public static implicit operator string(FileName fileName) => fileName.Value;
    public static implicit operator FileName(string value) => new(value);
    
    public override string ToString() => Value;
}
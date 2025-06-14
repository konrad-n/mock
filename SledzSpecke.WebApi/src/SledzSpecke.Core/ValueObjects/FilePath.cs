using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record FilePath
{
    public string Value { get; }

    public FilePath(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidFilePathException("File path cannot be empty.");
        }

        if (value.Length > 500)
        {
            throw new InvalidFilePathException("File path cannot exceed 500 characters.");
        }

        // Check for invalid characters in file path
        var invalidChars = Path.GetInvalidPathChars();
        if (value.Any(c => invalidChars.Contains(c)))
        {
            throw new InvalidFilePathException("File path contains invalid characters.");
        }

        // Additional validation for security
        if (value.Contains("..") || value.Contains("~"))
        {
            throw new InvalidFilePathException("File path cannot contain directory traversal patterns.");
        }

        Value = value.Trim();
    }

    public static implicit operator string(FilePath path) => path.Value;
    public static implicit operator FilePath(string path) => new(path);
    
    public override string ToString() => Value;
    
    public string GetFileName() => Path.GetFileName(Value);
    public string GetExtension() => Path.GetExtension(Value);
    public string GetDirectoryName() => Path.GetDirectoryName(Value) ?? string.Empty;
}
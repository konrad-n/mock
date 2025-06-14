using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record ContentType
{
    private static readonly HashSet<string> AllowedContentTypes = new()
    {
        "application/pdf",
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/gif",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "text/plain",
        "text/csv"
    };
    
    public string Value { get; }
    
    public ContentType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("Content type cannot be empty.");
        }
        
        var normalizedValue = value.ToLowerInvariant().Trim();
        
        if (!AllowedContentTypes.Contains(normalizedValue))
        {
            throw new DomainException($"Content type '{value}' is not allowed. Allowed types: {string.Join(", ", AllowedContentTypes)}");
        }
        
        Value = normalizedValue;
    }
    
    public bool IsImage() => Value.StartsWith("image/");
    public bool IsPdf() => Value == "application/pdf";
    public bool IsDocument() => Value.Contains("word") || Value.Contains("document") || Value.Contains("text");
    public bool IsSpreadsheet() => Value.Contains("excel") || Value.Contains("spreadsheet") || Value == "text/csv";
    
    public string GetFileExtension() => Value switch
    {
        "application/pdf" => ".pdf",
        "image/jpeg" => ".jpg",
        "image/jpg" => ".jpg",
        "image/png" => ".png",
        "image/gif" => ".gif",
        "application/msword" => ".doc",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => ".docx",
        "application/vnd.ms-excel" => ".xls",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" => ".xlsx",
        "text/plain" => ".txt",
        "text/csv" => ".csv",
        _ => string.Empty
    };
    
    public static implicit operator string(ContentType contentType) => contentType.Value;
    public static implicit operator ContentType(string value) => new(value);
    
    public override string ToString() => Value;
}
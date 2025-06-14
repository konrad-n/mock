namespace SledzSpecke.Infrastructure.Options;

public sealed class FileStorageOptions
{
    public string? BasePath { get; set; }
    public long MaxFileSize { get; set; } = 10 * 1024 * 1024; // 10 MB default
    public string[] AllowedExtensions { get; set; } = Array.Empty<string>();
}
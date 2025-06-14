using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record FileSize
{
    private const long MaxFileSizeInBytes = 10 * 1024 * 1024; // 10 MB
    
    public long Bytes { get; }
    
    public FileSize(long bytes)
    {
        if (bytes < 0)
        {
            throw new DomainException("File size cannot be negative.");
        }
        
        if (bytes > MaxFileSizeInBytes)
        {
            throw new DomainException($"File size cannot exceed {MaxFileSizeInBytes / (1024 * 1024)} MB.");
        }
        
        Bytes = bytes;
    }
    
    public double ToKilobytes() => Bytes / 1024.0;
    public double ToMegabytes() => Bytes / (1024.0 * 1024.0);
    
    public static implicit operator long(FileSize size) => size.Bytes;
    public static implicit operator FileSize(long bytes) => new(bytes);
    
    public override string ToString()
    {
        return Bytes switch
        {
            < 1024 => $"{Bytes} B",
            < 1024 * 1024 => $"{ToKilobytes():F2} KB",
            _ => $"{ToMegabytes():F2} MB"
        };
    }
}
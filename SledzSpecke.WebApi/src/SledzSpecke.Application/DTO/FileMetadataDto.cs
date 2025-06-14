namespace SledzSpecke.Application.DTO;

public sealed class FileMetadataDto
{
    public int Id { get; init; }
    public string FileName { get; init; } = null!;
    public string ContentType { get; init; } = null!;
    public long FileSize { get; init; }
    public string FileSizeFormatted { get; init; } = null!;
    public DateTime UploadedAt { get; init; }
    public string? Description { get; init; }
    public string EntityType { get; init; } = null!;
    public int EntityId { get; init; }
}
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

public class FileMetadata
{
    public int Id { get; private set; }
    public FileName FileName { get; private set; }
    public FilePath FilePath { get; private set; }
    public ContentType ContentType { get; private set; }
    public FileSize FileSize { get; private set; }
    public UserId UploadedByUserId { get; private set; }
    public DateTime UploadedAt { get; private set; }
    public string? Description { get; private set; }
    public string EntityType { get; private set; }
    public int EntityId { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    
    private FileMetadata() { } // EF Core
    
    public FileMetadata(FileName fileName, FilePath filePath, ContentType contentType, 
        FileSize fileSize, UserId uploadedByUserId, string entityType, int entityId, string? description = null)
    {
        FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
        FileSize = fileSize ?? throw new ArgumentNullException(nameof(fileSize));
        UploadedByUserId = uploadedByUserId ?? throw new ArgumentNullException(nameof(uploadedByUserId));
        EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
        EntityId = entityId;
        Description = description;
        UploadedAt = DateTime.UtcNow;
        IsDeleted = false;
    }
    
    public void MarkAsDeleted()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }
    
    public void UpdateDescription(string? description)
    {
        Description = description;
    }
}
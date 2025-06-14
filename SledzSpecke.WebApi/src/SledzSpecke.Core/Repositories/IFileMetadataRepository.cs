using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Repositories;

public interface IFileMetadataRepository
{
    Task<FileMetadata?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<FileMetadata?> GetByFilePathAsync(FilePath filePath, CancellationToken cancellationToken = default);
    Task<IEnumerable<FileMetadata>> GetByEntityAsync(string entityType, int entityId, CancellationToken cancellationToken = default);
    Task<IEnumerable<FileMetadata>> GetByUserAsync(UserId userId, CancellationToken cancellationToken = default);
    Task AddAsync(FileMetadata fileMetadata, CancellationToken cancellationToken = default);
    Task UpdateAsync(FileMetadata fileMetadata, CancellationToken cancellationToken = default);
    Task<IEnumerable<FileMetadata>> GetOrphanedFilesAsync(DateTime olderThan, CancellationToken cancellationToken = default);
}
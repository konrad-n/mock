using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Core.Specifications.Common;
using SledzSpecke.Infrastructure.DAL.Specifications.FileMetadata;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

/// <summary>
/// Refactored repository for FileMetadata using BaseRepository and Specification pattern
/// </summary>
internal sealed class RefactoredFileMetadataRepository : BaseRepository<FileMetadata>, IFileMetadataRepository
{
    public RefactoredFileMetadataRepository(SledzSpeckeDbContext dbContext) : base(dbContext)
    {
    }

    public override async Task<FileMetadata?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var spec = new FileMetadataByIdSpecification(id);
        return await GetSingleBySpecificationAsync(spec, cancellationToken);
    }

    public async Task<FileMetadata?> GetByFilePathAsync(FilePath filePath, CancellationToken cancellationToken = default)
    {
        var spec = new FileMetadataByFilePathSpecification(filePath);
        return await GetSingleBySpecificationAsync(spec, cancellationToken);
    }

    public async Task<IEnumerable<FileMetadata>> GetByEntityAsync(string entityType, int entityId, CancellationToken cancellationToken = default)
    {
        var spec = new FileMetadataByEntitySpecification(entityType, entityId)
            .And(new NotDeletedSpecification<FileMetadata>());
        
        return await GetBySpecificationAsync(spec, f => f.UploadedAt, ascending: false, cancellationToken);
    }

    public async Task<IEnumerable<FileMetadata>> GetByUserAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var spec = new FileMetadataByUserSpecification(userId)
            .And(new NotDeletedSpecification<FileMetadata>());
        
        return await GetBySpecificationAsync(spec, f => f.UploadedAt, ascending: false, cancellationToken);
    }

    public override async Task AddAsync(FileMetadata fileMetadata, CancellationToken cancellationToken = default)
    {
        await base.AddAsync(fileMetadata, cancellationToken);
    }

    public Task UpdateAsync(FileMetadata fileMetadata, CancellationToken cancellationToken = default)
    {
        base.Update(fileMetadata);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<FileMetadata>> GetOrphanedFilesAsync(DateTime olderThan, CancellationToken cancellationToken = default)
    {
        var spec = new OrphanedFileMetadataSpecification(olderThan);
        return await GetBySpecificationAsync(spec, cancellationToken);
    }
}
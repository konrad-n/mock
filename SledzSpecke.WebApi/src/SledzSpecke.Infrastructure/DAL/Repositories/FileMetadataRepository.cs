using Microsoft.EntityFrameworkCore;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.DAL.Repositories;

internal sealed class FileMetadataRepository : IFileMetadataRepository
{
    private readonly SledzSpeckeDbContext _dbContext;
    
    public FileMetadataRepository(SledzSpeckeDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<FileMetadata?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.FileMetadata
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }
    
    public async Task<FileMetadata?> GetByFilePathAsync(FilePath filePath, CancellationToken cancellationToken = default)
    {
        return await _dbContext.FileMetadata
            .FirstOrDefaultAsync(f => f.FilePath == filePath, cancellationToken);
    }
    
    public async Task<IEnumerable<FileMetadata>> GetByEntityAsync(string entityType, int entityId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.FileMetadata
            .Where(f => f.EntityType == entityType && f.EntityId == entityId && !f.IsDeleted)
            .OrderByDescending(f => f.UploadedAt)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<FileMetadata>> GetByUserAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.FileMetadata
            .Where(f => f.UploadedByUserId == userId && !f.IsDeleted)
            .OrderByDescending(f => f.UploadedAt)
            .ToListAsync(cancellationToken);
    }
    
    public async Task AddAsync(FileMetadata fileMetadata, CancellationToken cancellationToken = default)
    {
        await _dbContext.FileMetadata.AddAsync(fileMetadata, cancellationToken);
    }
    
    public Task UpdateAsync(FileMetadata fileMetadata, CancellationToken cancellationToken = default)
    {
        _dbContext.FileMetadata.Update(fileMetadata);
        return Task.CompletedTask;
    }
    
    public async Task<IEnumerable<FileMetadata>> GetOrphanedFilesAsync(DateTime olderThan, CancellationToken cancellationToken = default)
    {
        // Get files that are deleted and older than specified date
        return await _dbContext.FileMetadata
            .Where(f => f.IsDeleted && f.DeletedAt < olderThan)
            .ToListAsync(cancellationToken);
    }
}
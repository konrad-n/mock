using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Security;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Application.Queries.Handlers;

internal sealed class GetEntityFilesHandler : IQueryHandler<GetEntityFiles, IEnumerable<FileMetadataDto>>
{
    private readonly IFileMetadataRepository _fileMetadataRepository;
    private readonly IUserContext _userContext;
    
    public GetEntityFilesHandler(
        IFileMetadataRepository fileMetadataRepository,
        IUserContext userContext)
    {
        _fileMetadataRepository = fileMetadataRepository;
        _userContext = userContext;
    }
    
    public async Task<IEnumerable<FileMetadataDto>> HandleAsync(GetEntityFiles query)
    {
        var userId = _userContext.UserId;
        if (userId is null)
        {
            return Enumerable.Empty<FileMetadataDto>();
        }
        
        var files = await _fileMetadataRepository.GetByEntityAsync(query.EntityType, query.EntityId);
        
        return files
            .Where(f => !f.IsDeleted)
            .Select(f => new FileMetadataDto
            {
                Id = f.Id,
                FileName = f.FileName,
                ContentType = f.ContentType,
                FileSize = f.FileSize,
                FileSizeFormatted = f.FileSize.ToString(),
                UploadedAt = f.UploadedAt,
                Description = f.Description,
                EntityType = f.EntityType,
                EntityId = f.EntityId
            })
            .ToList();
    }
}
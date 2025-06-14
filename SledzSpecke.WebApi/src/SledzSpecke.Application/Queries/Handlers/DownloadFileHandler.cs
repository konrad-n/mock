using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Security;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Application.Queries.Handlers;

public sealed class DownloadFileHandler : IQueryHandler<DownloadFile, FileDownloadResult>
{
    private readonly IFileMetadataRepository _fileMetadataRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUserContext _userContext;
    
    public DownloadFileHandler(
        IFileMetadataRepository fileMetadataRepository,
        IFileStorageService fileStorageService,
        IUserContext userContext)
    {
        _fileMetadataRepository = fileMetadataRepository;
        _fileStorageService = fileStorageService;
        _userContext = userContext;
    }
    
    public async Task<FileDownloadResult> HandleAsync(DownloadFile query)
    {
        var userId = _userContext.UserId;
        if (userId is null)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }
        
        // Get file metadata
        var fileMetadata = await _fileMetadataRepository.GetByIdAsync(query.FileId);
        if (fileMetadata is null)
        {
            throw new FileNotFoundException("File not found.");
        }
        
        if (fileMetadata.IsDeleted)
        {
            throw new InvalidOperationException("File has been deleted.");
        }
        
        // Check if user has access to the file
        // For now, we'll allow access if the user uploaded it or if it's their profile picture
        if (fileMetadata.UploadedByUserId.Value != userId.Value && 
            !(fileMetadata.EntityType == "User" && fileMetadata.EntityId == userId.Value))
        {
            // For other entities, we should check if user has access to that entity
            // This is a simplified check - in production, you'd want more sophisticated authorization
            throw new UnauthorizedAccessException("Access denied.");
        }
        
        // Download file from storage
        var downloadResult = await _fileStorageService.DownloadAsync(fileMetadata.FilePath);
        if (downloadResult.IsFailure)
        {
            throw new InvalidOperationException($"Failed to download file: {downloadResult.Error}");
        }
        
        var (stream, contentType, fileName) = downloadResult.Value;
        
        var result = new FileDownloadResult
        {
            FileStream = stream,
            FileName = fileMetadata.FileName,
            ContentType = fileMetadata.ContentType,
            FileSize = fileMetadata.FileSize
        };
        
        return result;
    }
}
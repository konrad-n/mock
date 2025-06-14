using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Security;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

internal sealed class UploadFileHandler : IResultCommandHandler<UploadFile, FileMetadataDto>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IFileMetadataRepository _fileMetadataRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    
    public UploadFileHandler(
        IFileStorageService fileStorageService,
        IFileMetadataRepository fileMetadataRepository,
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
        _fileStorageService = fileStorageService;
        _fileMetadataRepository = fileMetadataRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }
    
    public async Task<Result<FileMetadataDto>> HandleAsync(UploadFile command)
    {
        var userId = _userContext.UserId;
        if (userId is null)
        {
            return Result.Failure<FileMetadataDto>("User is not authenticated.");
        }
        
        // Validate file
        if (command.File is null || command.File.Length == 0)
        {
            return Result.Failure<FileMetadataDto>("No file provided.");
        }
        
        try
        {
            // Create value objects
            var fileName = new FileName(command.File.FileName);
            var fileSize = new FileSize(command.File.Length);
            var contentType = new ContentType(command.File.ContentType ?? "application/octet-stream");
            
            // Make filename unique
            var uniqueFileName = fileName.MakeUnique();
            
            // Upload file to storage
            using var stream = command.File.OpenReadStream();
            var uploadResult = await _fileStorageService.UploadAsync(
                stream, 
                uniqueFileName, 
                contentType);
            
            if (uploadResult.IsFailure)
            {
                return Result.Failure<FileMetadataDto>($"Failed to upload file: {uploadResult.Error}");
            }
            
            var filePath = uploadResult.Value;
            
            // Create file metadata
            var fileMetadata = new FileMetadata(
                uniqueFileName,
                filePath,
                contentType,
                fileSize,
                new UserId(userId.Value),
                command.EntityType,
                command.EntityId,
                command.Description);
            
            // Save metadata
            await _fileMetadataRepository.AddAsync(fileMetadata);
            await _unitOfWork.SaveChangesAsync();
            
            // Return DTO
            var dto = new FileMetadataDto
            {
                Id = fileMetadata.Id,
                FileName = fileMetadata.FileName,
                ContentType = fileMetadata.ContentType,
                FileSize = fileMetadata.FileSize,
                FileSizeFormatted = fileMetadata.FileSize.ToString(),
                UploadedAt = fileMetadata.UploadedAt,
                Description = fileMetadata.Description,
                EntityType = fileMetadata.EntityType,
                EntityId = fileMetadata.EntityId
            };
            
            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Failure<FileMetadataDto>($"File upload failed: {ex.Message}");
        }
    }
}
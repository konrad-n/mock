using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Security;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class DeleteFileHandler : IResultCommandHandler<DeleteFile>
{
    private readonly IFileMetadataRepository _fileMetadataRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    
    public DeleteFileHandler(
        IFileMetadataRepository fileMetadataRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
        _fileMetadataRepository = fileMetadataRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }
    
    public async Task<Result> HandleAsync(DeleteFile command, CancellationToken cancellationToken = default)
    {
        var userId = _userContext.UserId;
        if (userId is null)
        {
            return Result.Failure("User is not authenticated.");
        }
        
        var fileMetadata = await _fileMetadataRepository.GetByIdAsync(command.FileId);
        if (fileMetadata is null)
        {
            return Result.Failure("File not found.");
        }
        
        if (fileMetadata.IsDeleted)
        {
            return Result.Failure("File has already been deleted.");
        }
        
        // Check if user has permission to delete
        if (fileMetadata.UploadedByUserId.Value != userId.Value)
        {
            return Result.Failure("You don't have permission to delete this file.");
        }
        
        // Mark as deleted in database (soft delete)
        fileMetadata.MarkAsDeleted();
        await _fileMetadataRepository.UpdateAsync(fileMetadata);
        
        // Delete from storage
        var deleteResult = await _fileStorageService.DeleteAsync(fileMetadata.FilePath);
        if (deleteResult.IsFailure)
        {
            // Log the error but don't fail the operation
            // File is already marked as deleted in database
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}
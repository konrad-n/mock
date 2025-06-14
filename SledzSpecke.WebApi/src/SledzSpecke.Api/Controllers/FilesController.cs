using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Commands;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;

namespace SledzSpecke.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/files")]
public class FilesController : BaseResultController
{
    private readonly IResultCommandHandler<UploadFile, FileMetadataDto> _uploadFileHandler;
    private readonly IQueryHandler<DownloadFile, FileDownloadResult> _downloadFileHandler;
    private readonly IQueryHandler<GetEntityFiles, IEnumerable<FileMetadataDto>> _getEntityFilesHandler;
    private readonly IResultCommandHandler<DeleteFile> _deleteFileHandler;
    
    public FilesController(
        IResultCommandHandler<UploadFile, FileMetadataDto> uploadFileHandler,
        IQueryHandler<DownloadFile, FileDownloadResult> downloadFileHandler,
        IQueryHandler<GetEntityFiles, IEnumerable<FileMetadataDto>> getEntityFilesHandler,
        IResultCommandHandler<DeleteFile> deleteFileHandler)
    {
        _uploadFileHandler = uploadFileHandler;
        _downloadFileHandler = downloadFileHandler;
        _getEntityFilesHandler = getEntityFilesHandler;
        _deleteFileHandler = deleteFileHandler;
    }
    
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<FileMetadataDto>> UploadFile([FromForm] UploadFile command, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var result = await _uploadFileHandler.HandleAsync(command);
        return HandleResult(result);
    }
    
    [HttpGet("{fileId:int}/download")]
    public async Task<IActionResult> DownloadFile(int fileId, CancellationToken cancellationToken)
    {
        var query = new DownloadFile { FileId = fileId };
        var download = await _downloadFileHandler.HandleAsync(query);
        
        return File(
            download.FileStream, 
            download.ContentType, 
            download.FileName);
    }
    
    [HttpGet("entity/{entityType}/{entityId:int}")]
    public async Task<IActionResult> GetEntityFiles(string entityType, int entityId, CancellationToken cancellationToken)
    {
        var query = new GetEntityFiles 
        { 
            EntityType = entityType, 
            EntityId = entityId 
        };
        
        var files = await _getEntityFilesHandler.HandleAsync(query);
        return Ok(files);
    }
    
    [HttpDelete("{fileId:int}")]
    public async Task<IActionResult> DeleteFile(int fileId, CancellationToken cancellationToken)
    {
        var command = new DeleteFile { FileId = fileId };
        var result = await _deleteFileHandler.HandleAsync(command);
        return HandleResult(result);
    }
}
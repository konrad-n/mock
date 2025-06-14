using Microsoft.Extensions.Options;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Infrastructure.Options;

namespace SledzSpecke.Infrastructure.Services;

internal sealed class FileStorageService : IFileStorageService
{
    private readonly FileStorageOptions _options;
    private readonly string _basePath;
    
    public FileStorageService(IOptions<FileStorageOptions> options)
    {
        _options = options.Value;
        _basePath = _options.BasePath ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        
        // Ensure base directory exists
        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }
    }
    
    public async Task<Result<FilePath>> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        try
        {
            // Create subdirectory based on date
            var dateDirectory = DateTime.UtcNow.ToString("yyyy/MM/dd");
            var fullDirectory = Path.Combine(_basePath, dateDirectory);
            
            if (!Directory.Exists(fullDirectory))
            {
                Directory.CreateDirectory(fullDirectory);
            }
            
            // Generate unique file name
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var fullPath = Path.Combine(fullDirectory, uniqueFileName);
            
            // Save file
            using var fileStreamWrite = File.Create(fullPath);
            await fileStream.CopyToAsync(fileStreamWrite, cancellationToken);
            
            // Return relative path
            var relativePath = Path.Combine(dateDirectory, uniqueFileName).Replace('\\', '/');
            return Result.Success(new FilePath(relativePath));
        }
        catch (Exception ex)
        {
            return Result.Failure<FilePath>($"Failed to upload file: {ex.Message}");
        }
    }
    
    public async Task<Result<(Stream Stream, string ContentType, string FileName)>> DownloadAsync(FilePath filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            var fullPath = Path.Combine(_basePath, filePath.Value);
            
            if (!File.Exists(fullPath))
            {
                return Result.Failure<(Stream, string, string)>("File not found.");
            }
            
            var fileStream = File.OpenRead(fullPath);
            var fileName = Path.GetFileName(fullPath);
            var contentType = GetContentTypeFromExtension(Path.GetExtension(fileName));
            
            return Result.Success<(Stream, string, string)>((fileStream, contentType, fileName));
        }
        catch (Exception ex)
        {
            return Result.Failure<(Stream, string, string)>($"Failed to download file: {ex.Message}");
        }
    }
    
    public async Task<Result> DeleteAsync(FilePath filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            var fullPath = Path.Combine(_basePath, filePath.Value);
            
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete file: {ex.Message}");
        }
    }
    
    public async Task<Result<bool>> ExistsAsync(FilePath filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            var fullPath = Path.Combine(_basePath, filePath.Value);
            return Result.Success(File.Exists(fullPath));
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"Failed to check file existence: {ex.Message}");
        }
    }
    
    public async Task<Result<long>> GetFileSizeAsync(FilePath filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            var fullPath = Path.Combine(_basePath, filePath.Value);
            
            if (!File.Exists(fullPath))
            {
                return Result.Failure<long>("File not found.");
            }
            
            var fileInfo = new FileInfo(fullPath);
            return Result.Success(fileInfo.Length);
        }
        catch (Exception ex)
        {
            return Result.Failure<long>($"Failed to get file size: {ex.Message}");
        }
    }
    
    private string GetContentTypeFromExtension(string extension)
    {
        return extension.ToLowerInvariant() switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".txt" => "text/plain",
            ".csv" => "text/csv",
            _ => "application/octet-stream"
        };
    }
}
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Repositories;

public interface IFileStorageService
{
    Task<Result<FilePath>> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<Result<(Stream Stream, string ContentType, string FileName)>> DownloadAsync(FilePath filePath, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(FilePath filePath, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsAsync(FilePath filePath, CancellationToken cancellationToken = default);
    Task<Result<long>> GetFileSizeAsync(FilePath filePath, CancellationToken cancellationToken = default);
}
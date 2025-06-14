using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Infrastructure.Services;

internal sealed class FileCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<FileCleanupService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24); // Run daily
    private readonly TimeSpan _fileRetentionPeriod = TimeSpan.FromDays(7); // Keep deleted files for 7 days
    
    public FileCleanupService(IServiceProvider serviceProvider, ILogger<FileCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupOrphanedFiles(stoppingToken);
                await Task.Delay(_checkInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // When the stopping token is canceled, for example, a call made from services.msc,
                // we shouldn't exit with a non-zero exit code. In other words, this is expected...
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during file cleanup");
                
                // Wait a bit before retrying
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
    
    private async Task CleanupOrphanedFiles(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var fileMetadataRepository = scope.ServiceProvider.GetRequiredService<IFileMetadataRepository>();
        var fileStorageService = scope.ServiceProvider.GetRequiredService<IFileStorageService>();
        
        _logger.LogInformation("Starting orphaned file cleanup...");
        
        var cutoffDate = DateTime.UtcNow.Subtract(_fileRetentionPeriod);
        var orphanedFiles = await fileMetadataRepository.GetOrphanedFilesAsync(cutoffDate, cancellationToken);
        
        var fileCount = 0;
        foreach (var file in orphanedFiles)
        {
            try
            {
                // Delete from storage
                var deleteResult = await fileStorageService.DeleteAsync(file.FilePath, cancellationToken);
                if (deleteResult.IsSuccess)
                {
                    _logger.LogInformation("Deleted orphaned file: {FilePath}", file.FilePath.Value);
                    fileCount++;
                }
                else
                {
                    _logger.LogWarning("Failed to delete orphaned file {FilePath}: {Error}", 
                        file.FilePath.Value, deleteResult.Error);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting orphaned file: {FilePath}", file.FilePath.Value);
            }
        }
        
        _logger.LogInformation("Orphaned file cleanup completed. Deleted {Count} files.", fileCount);
    }
}
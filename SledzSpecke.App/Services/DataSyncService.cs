using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Infrastructure.Database.Context;

namespace SledzSpecke.App.Services
{
    public class DataSyncService : IDataSyncService
    {
        private readonly ILogger<DataSyncService> _logger;
        private readonly IApplicationDbContext _dbContext;
        private readonly IFileSystemService _fileSystemService;

        public DataSyncService(
            ILogger<DataSyncService> logger,
            IApplicationDbContext dbContext,
            IFileSystemService fileSystemService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _fileSystemService = fileSystemService;
        }

        public async Task<bool> SyncAllDataAsync()
        {
            try
            {
                _logger.LogInformation("Starting data synchronization");
                // Implement data synchronization logic here
                await Task.Delay(1000); // Simulate sync
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error synchronizing data");
                return false;
            }
        }

        public async Task<bool> CreateBackupAsync()
        {
            try
            {
                _logger.LogInformation("Creating backup");

                // Get database file path
                var dbPath = _fileSystemService.GetAppDataDirectory();
                var dbFilePath = Path.Combine(dbPath, "sledzspecke.db3");

                // Create backup directory if it doesn't exist
                var backupDir = Path.Combine(_fileSystemService.GetAppDataDirectory(), "Backups");
                Directory.CreateDirectory(backupDir);

                // Create a backup with timestamp
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var backupPath = Path.Combine(backupDir, $"sledzspecke_backup_{timestamp}.db3");

                // Copy database file to backup location
                await Task.Run(() => File.Copy(dbFilePath, backupPath, true));

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating backup");
                return false;
            }
        }

        public async Task<bool> RestoreFromBackupAsync()
        {
            try
            {
                _logger.LogInformation("Restoring from backup");

                // Get backup directory
                var backupDir = Path.Combine(_fileSystemService.GetAppDataDirectory(), "Backups");
                if (!Directory.Exists(backupDir))
                {
                    _logger.LogWarning("No backups found");
                    return false;
                }

                // Get latest backup file
                var backupFiles = Directory.GetFiles(backupDir, "sledzspecke_backup_*.db3");
                if (backupFiles.Length == 0)
                {
                    _logger.LogWarning("No backup files found");
                    return false;
                }

                var latestBackup = backupFiles
                    .OrderByDescending(f => File.GetLastWriteTime(f))
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(latestBackup))
                {
                    return false;
                }

                // Get database file path
                var dbPath = _fileSystemService.GetAppDataDirectory();
                var dbFilePath = Path.Combine(dbPath, "sledzspecke.db3");

                // Close database connection
                await _dbContext.GetConnection().CloseAsync();

                // Copy backup file to database location
                await Task.Run(() => File.Copy(latestBackup, dbFilePath, true));

                // Reinitialize database
                await _dbContext.InitializeAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring from backup");
                return false;
            }
        }

        public async Task<bool> ClearAllDataAsync()
        {
            try
            {
                _logger.LogInformation("Clearing all data");

                // Close database connection
                await _dbContext.GetConnection().CloseAsync();

                // Get database file path
                var dbPath = _fileSystemService.GetAppDataDirectory();
                var dbFilePath = Path.Combine(dbPath, "sledzspecke.db3");

                // Delete database file
                if (File.Exists(dbFilePath))
                {
                    File.Delete(dbFilePath);
                }

                // Reinitialize database
                await _dbContext.InitializeAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing all data");
                return false;
            }
        }
    }
}

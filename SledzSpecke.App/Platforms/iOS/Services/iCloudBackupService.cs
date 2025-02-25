using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Foundation;
using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Services.Backup;
using UIKit;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Platforms.iOS.Services
{
    public class iCloudBackupService : ICloudBackupService
    {
        private readonly ILogger<iCloudBackupService> _logger;
        private readonly IFileSystemService _fileSystemService;
        private readonly NSUbiquitousKeyValueStore _keyValueStore;
        
        // URL do iCloud container
        private NSUrl _iCloudUrl;
        
        public iCloudBackupService(
            ILogger<iCloudBackupService> logger,
            IFileSystemService fileSystemService)
        {
            _logger = logger;
            _fileSystemService = fileSystemService;
            _keyValueStore = NSUbiquitousKeyValueStore.DefaultStore;
            
            // Inicjalizacja URL do kontenera iCloud
            InitializeiCloudContainer();
        }
        
        private void InitializeiCloudContainer()
        {
            try
            {
                var fileManager = NSFileManager.DefaultManager;
                _iCloudUrl = fileManager.GetUrlForUbiquityContainer("iCloud.com.yourcompany.sledzspecke");
                
                if (_iCloudUrl == null)
                {
                    _logger.LogWarning("iCloud container is not available");
                }
                else
                {
                    // Utwórz folder Documents w kontenerze iCloud
                    var documentsUrl = _iCloudUrl.Append("Documents", true);
                    if (!fileManager.FileExists(documentsUrl.Path))
                    {
                        NSError error;
                        fileManager.CreateDirectory(documentsUrl.Path, true, null, out error);
                        
                        if (error != null)
                        {
                            _logger.LogError("Error creating iCloud Documents directory: {Error}", error.LocalizedDescription);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing iCloud container");
            }
        }
        
        public async Task<bool> BackupToCloudAsync(string userId)
        {
            try
            {
                if (_iCloudUrl == null)
                {
                    _logger.LogWarning("iCloud container is not available");
                    return false;
                }
                
                // Utwórz tymczasową kopię bazy danych
                var dbPath = Path.Combine(_fileSystemService.GetAppDataDirectory(), "sledzspecke.db3");
                var backupName = $"sledzspecke_backup_{DateTime.Now:yyyyMMddHHmmss}.db3";
                var backupPath = Path.Combine(Path.GetTempPath(), backupName);
                
                File.Copy(dbPath, backupPath, true);
                
                // Utwórz metadane kopii zapasowej
                var metadata = new BackupMetadata
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    AppVersion = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"].ToString(),
                    DeviceName = UIDevice.CurrentDevice.Name,
                    PlatformVersion = UIDevice.CurrentDevice.SystemVersion
                };
                
                var metadataJson = System.Text.Json.JsonSerializer.Serialize(metadata);
                var metadataPath = Path.Combine(Path.GetTempPath(), $"backup_metadata_{DateTime.Now:yyyyMMddHHmmss}.json");
                File.WriteAllText(metadataPath, metadataJson);
                
                // Przenieś pliki do iCloud
                var fileManager = NSFileManager.DefaultManager;
                var iCloudDocumentsUrl = _iCloudUrl.Append("Documents", true);
                var iCloudBackupUrl = iCloudDocumentsUrl.Append(backupName, false);
                var iCloudMetadataUrl = iCloudDocumentsUrl.Append(Path.GetFileName(metadataPath), false);
                
                NSError error;
                
                // Przenieś plik bazy danych
                if (fileManager.FileExists(iCloudBackupUrl.Path))
                {
                    fileManager.Remove(iCloudBackupUrl, out error);
                    if (error != null)
                    {
                        _logger.LogError("Error removing existing backup: {Error}", error.LocalizedDescription);
                        return false;
                    }
                }
                
                bool backupMoved = fileManager.Copy(backupPath, iCloudBackupUrl.Path, out error);
                if (!backupMoved || error != null)
                {
                    _logger.LogError("Error moving backup to iCloud: {Error}", error?.LocalizedDescription ?? "Unknown error");
                    return false;
                }
                
                // Przenieś plik metadanych
                if (fileManager.FileExists(iCloudMetadataUrl.Path))
                {
                    fileManager.Remove(iCloudMetadataUrl, out error);
                }
                
                bool metadataMoved = fileManager.Copy(metadataPath, iCloudMetadataUrl.Path, out error);
                if (!metadataMoved || error != null)
                {
                    _logger.LogError("Error moving metadata to iCloud: {Error}", error?.LocalizedDescription ?? "Unknown error");
                    return false;
                }
                
                // Zapisz informację o ostatniej kopii zapasowej
                _keyValueStore.SetString(DateTime.UtcNow.ToString("o"), "LastBackupDate");
                _keyValueStore.SetString(backupName, "LastBackupName");
                _keyValueStore.Synchronize();
                
                // Usuń pliki tymczasowe
                try
                {
                    File.Delete(backupPath);
                    File.Delete(metadataPath);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error cleaning up temporary files");
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating iCloud backup");
                return false;
            }
        }
        
        public async Task<bool> RestoreFromCloudAsync(string userId)
        {
            try
            {
                if (_iCloudUrl == null)
                {
                    _logger.LogWarning("iCloud container is not available");
                    return false;
                }
                
                // Pobierz nazwę ostatniej kopii zapasowej
                var lastBackupName = _keyValueStore.GetString("LastBackupName");
                if (string.IsNullOrEmpty(lastBackupName))
                {
                    _logger.LogWarning("No backup found in iCloud");
                    return false;
                }
                
                var fileManager = NSFileManager.DefaultManager;
                var iCloudDocumentsUrl = _iCloudUrl.Append("Documents", true);
                var iCloudBackupUrl = iCloudDocumentsUrl.Append(lastBackupName, false);
                
                if (!fileManager.FileExists(iCloudBackupUrl.Path))
                {
                    _logger.LogWarning("Backup file not found in iCloud");
                    return false;
                }
                
                // Kopiuj plik kopii zapasowej do folderu tymczasowego
                var tempBackupPath = Path.Combine(Path.GetTempPath(), "restore_temp.db3");
                NSError error;
                bool fileCopied = fileManager.Copy(iCloudBackupUrl.Path, tempBackupPath, out error);
                
                if (!fileCopied || error != null)
                {
                    _logger.LogError("Error copying backup from iCloud: {Error}", error?.LocalizedDescription ?? "Unknown error");
                    return false;
                }
                
                // Zastąp lokalny plik bazy danych
                var dbPath = Path.Combine(_fileSystemService.GetAppDataDirectory(), "sledzspecke.db3");
                
                // Utwórz kopię bieżącej bazy danych na wszelki wypadek
                var currentBackupPath = Path.Combine(Path.GetTempPath(), "current_db_backup.db3");
                File.Copy(dbPath, currentBackupPath, true);
                
                try
                {
                    // Zastąp plik bazy danych
                    File.Copy(tempBackupPath, dbPath, true);
                    
                    // Usuń pliki tymczasowe
                    File.Delete(tempBackupPath);
                    
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error restoring database");
                    
                    // Próba przywrócenia poprzedniej bazy danych
                    try
                    {
                        File.Copy(currentBackupPath, dbPath, true);
                    }
                    catch
                    {
                        _logger.LogError("Failed to restore original database");
                    }
                    
                    return false;
                }
                finally
                {
                    // Usuń kopię bieżącej bazy danych
                    try
                    {
                        File.Delete(currentBackupPath);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring from iCloud backup");
                return false;
            }
        }
        
        public async Task<DateTime?> GetLastBackupDateAsync(string userId)
        {
            try
            {
                var lastBackupDateString = _keyValueStore.GetString("LastBackupDate");
                if (string.IsNullOrEmpty(lastBackupDateString))
                {
                    return null;
                }
                
                return DateTime.Parse(lastBackupDateString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting last backup date");
                return null;
            }
        }
        
        public async Task<List<BackupInfo>> GetAvailableBackupsAsync(string userId)
        {
            try
            {
                if (_iCloudUrl == null)
                {
                    return new List<BackupInfo>();
                }
                
                var backups = new List<BackupInfo>();
                var fileManager = NSFileManager.DefaultManager;
                var iCloudDocumentsUrl = _iCloudUrl.Append("Documents", true);
                
                NSError error;
                var fileUrls = fileManager.GetDirectoryContent(iCloudDocumentsUrl, null, NSDirectoryEnumerationOptions.SkipsHiddenFiles, out error);
                
                if (error != null)
                {
                    _logger.LogError("Error getting iCloud directory content: {Error}", error.LocalizedDescription);
                    return backups;
                }
                
                foreach (var fileUrl in fileUrls)
                {
                    if (fileUrl.LastPathComponent.StartsWith("sledzspecke_backup_") && fileUrl.LastPathComponent.EndsWith(".db3"))
                    {
                        try
                        {
                            var fileName = fileUrl.LastPathComponent;
                            var dateString = fileName.Substring(18, 14); // Format: yyyyMMddHHmmss
                            var backupDate = DateTime.ParseExact(dateString, "yyyyMMddHHmmss", null);
                            
                            var attributes = fileManager.GetAttributes(fileUrl.Path, out error);
                            long fileSize = 0;
                            if (error == null && attributes != null)
                            {
                                fileSize = attributes.Size;
                            }
                            
                            var backupInfo = new BackupInfo
                            {
                                Id = fileName,
                                CreatedAt = backupDate,
                                SizeInBytes = fileSize,
                                AppVersion = "Unknown", // Można próbować odczytać z pliku metadanych
                                DeviceName = "iCloud"
                            };
                            
                            backups.Add(backupInfo);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Error parsing backup file: {FileName}", fileUrl.LastPathComponent);
                        }
                    }
                }
                
                // Sortuj według daty utworzenia (od najnowszej)
                backups.Sort((a, b) => b.CreatedAt.CompareTo(a.CreatedAt));
                
                return backups;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available backups");
                return new List<BackupInfo>();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Services.Backup;
using Windows.Storage;

namespace SledzSpecke.App.Platforms.Windows.Services
{
    public class OneDriveBackupService : ICloudBackupService
    {
        private readonly ILogger<OneDriveBackupService> _logger;
        private readonly IFileSystemService _fileSystemService;
        private readonly HttpClient _httpClient;
        private readonly IPublicClientApplication _msalClient;
        
        // OneDrive API endpoints
        private const string GraphApiBaseUrl = "https://graph.microsoft.com/v1.0";
        private const string OneDriveRootPath = "/me/drive/root:/SledzSpecke";
        
        // MSAL configuration
        private const string ClientId = "your-client-id"; // Należy zastąpić rzeczywistym identyfikatorem klienta z portalu Azure
        private readonly string[] _scopes = { "Files.ReadWrite", "Files.ReadWrite.All" };
        
        private string _accessToken;
        
        public OneDriveBackupService(
            ILogger<OneDriveBackupService> logger,
            IFileSystemService fileSystemService)
        {
            _logger = logger;
            _fileSystemService = fileSystemService;
            _httpClient = new HttpClient();
            
            // Inicjalizacja MSAL dla uwierzytelniania Microsoft
            _msalClient = PublicClientApplicationBuilder
                .Create(ClientId)
                .WithRedirectUri("https://login.microsoftonline.com/common/oauth2/nativeclient")
                .WithAuthority(AadAuthorityAudience.AzureAdAndPersonalMicrosoftAccount)
                .Build();
        }
        
        public async Task<bool> BackupToCloudAsync(string userId)
        {
            try
            {
                // Uwierzytelnienie użytkownika i uzyskanie tokenu dostępu
                if (!await AuthenticateAsync())
                {
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
                    AppVersion = GetAppVersion(),
                    DeviceName = GetDeviceName(),
                    PlatformVersion = GetSystemVersion()
                };
                
                var metadataJson = JsonSerializer.Serialize(metadata);
                var metadataPath = Path.Combine(Path.GetTempPath(), $"backup_metadata_{DateTime.Now:yyyyMMddHHmmss}.json");
                File.WriteAllText(metadataPath, metadataJson);
                
                // Utwórz folder kopii zapasowych w OneDrive (jeśli nie istnieje)
                await EnsureBackupFolderExistsAsync();
                
                // Prześlij plik bazy danych do OneDrive
                await UploadFileToOneDriveAsync(backupPath, $"{OneDriveRootPath}/{backupName}");
                
                // Prześlij plik metadanych do OneDrive
                await UploadFileToOneDriveAsync(metadataPath, $"{OneDriveRootPath}/{Path.GetFileName(metadataPath)}");
                
                // Zapisz informację o ostatniej kopii zapasowej
                var localSettings = ApplicationData.Current.LocalSettings;
                localSettings.Values["LastOneDriveBackupDate"] = DateTime.UtcNow.ToString("o");
                localSettings.Values["LastOneDriveBackupName"] = backupName;
                
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
                _logger.LogError(ex, "Error creating OneDrive backup");
                return false;
            }
        }
        
        public async Task<bool> RestoreFromCloudAsync(string userId)
        {
            try
            {
                // Uwierzytelnienie użytkownika i uzyskanie tokenu dostępu
                if (!await AuthenticateAsync())
                {
                    return false;
                }
                
                // Pobierz listę dostępnych kopii zapasowych
                var backups = await GetAvailableBackupsAsync(userId);
                if (backups.Count == 0)
                {
                    _logger.LogWarning("No backups found in OneDrive");
                    return false;
                }
                
                // Wybierz najnowszą kopię zapasową
                var latestBackup = backups[0]; // Zakładamy, że lista jest posortowana od najnowszej
                
                // Pobierz plik kopii zapasowej z OneDrive
                var tempBackupPath = Path.Combine(Path.GetTempPath(), "restore_temp.db3");
                await DownloadFileFromOneDriveAsync($"{OneDriveRootPath}/{latestBackup.Id}", tempBackupPath);
                
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
                _logger.LogError(ex, "Error restoring from OneDrive backup");
                return false;
            }
        }
        
        public async Task<DateTime?> GetLastBackupDateAsync(string userId)
        {
            try
            {
                var localSettings = ApplicationData.Current.LocalSettings;
                var lastBackupDateString = localSettings.Values["LastOneDriveBackupDate"] as string;
                
                if (string.IsNullOrEmpty(lastBackupDateString))
                {
                    // Jeśli nie ma lokalnej informacji, spróbuj pobrać z OneDrive
                    var backups = await GetAvailableBackupsAsync(userId);
                    if (backups.Count > 0)
                    {
                        return backups[0].CreatedAt;
                    }
                    
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
                // Uwierzytelnienie użytkownika i uzyskanie tokenu dostępu
                if (!await AuthenticateAsync())
                {
                    return new List<BackupInfo>();
                }
                
                // Pobierz listę plików z folderu kopii zapasowych
                var files = await GetFilesFromOneDriveAsync(OneDriveRootPath);
                
                var backups = new List<BackupInfo>();
                
                foreach (var file in files)
                {
                    if (file.Name.StartsWith("sledzspecke_backup_") && file.Name.EndsWith(".db3"))
                    {
                        try
                        {
                            var dateString = file.Name.Substring(18, 14); // Format: yyyyMMddHHmmss
                            var backupDate = DateTime.ParseExact(dateString, "yyyyMMddHHmmss", null);
                            
                            var backupInfo = new BackupInfo
                            {
                                Id = file.Name,
                                CreatedAt = backupDate,
                                SizeInBytes = file.Size,
                                AppVersion = "Unknown", // Można próbować odczytać z pliku metadanych
                                DeviceName = "OneDrive"
                            };
                            
                            backups.Add(backupInfo);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Error parsing backup file: {FileName}", file.Name);
                        }
                    }
                }
                
                // Sortuj według daty utworzenia (od najnowszej)
                backups.Sort((a, b) => b.CreatedAt.CompareTo(a.CreatedAt));
                
                return backups;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available backups from OneDrive");
                return new List<BackupInfo>();
            }
        }
        
        private async Task<bool> AuthenticateAsync()
        {
            try
            {
                // Sprawdź, czy mamy już ważny token
                if (!string.IsNullOrEmpty(_accessToken))
                {
                    return true;
                }
                
                // Próba pobrania tokenu z pamięci podręcznej
                var accounts = await _msalClient.GetAccountsAsync();
                AuthenticationResult result;
                
                try
                {
                    result = await _msalClient
                        .AcquireTokenSilent(_scopes, accounts.FirstOrDefault())
                        .ExecuteAsync();
                }
                catch (MsalUiRequiredException)
                {
                    // Brak tokenu w pamięci podręcznej, konieczne uwierzytelnienie interaktywne
                    try
                    {
                        result = await _msalClient
                            .AcquireTokenInteractive(_scopes)
                            .WithPrompt(Prompt.SelectAccount)
                            .ExecuteAsync();
                    }
                    catch (MsalException ex)
                    {
                        _logger.LogError(ex, "Error authenticating with Microsoft");
                        return false;
                    }
                }
                
                _accessToken = result.AccessToken;
                
                // Konfiguracja nagłówka autoryzacji w kliencie HTTP
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authenticating with Microsoft");
                return false;
            }
        }
        
        private async Task EnsureBackupFolderExistsAsync()
        {
            try
            {
                // Sprawdź, czy folder kopii zapasowych istnieje
                var url = $"{GraphApiBaseUrl}/me/drive/root:/SledzSpecke";
                var response = await _httpClient.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                {
                    // Folder nie istnieje, utwórz go
                    var createFolderUrl = $"{GraphApiBaseUrl}/me/drive/root/children";
                    var folderContent = new
                    {
                        name = "SledzSpecke",
                        folder = new { },
                        "@microsoft.graph.conflictBehavior" = "replace"
                    };
                    
                    var folderJson = JsonSerializer.Serialize(folderContent);
                    var content = new StringContent(folderJson, Encoding.UTF8, "application/json");
                    
                    var createResponse = await _httpClient.PostAsync(createFolderUrl, content);
                    
                    if (!createResponse.IsSuccessStatusCode)
                    {
                        throw new Exception($"Error creating backup folder: {createResponse.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ensuring backup folder exists");
                throw;
            }
        }
        
        private async Task UploadFileToOneDriveAsync(string localFilePath, string oneDrivePath)
        {
            try
            {
                using (var fileStream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read))
                {
                    var content = new StreamContent(fileStream);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    
                    // Pobierz ścieżkę do wysłania pliku
                    var uploadUrl = $"{GraphApiBaseUrl}{oneDrivePath}:/content";
                    
                    // Wyślij plik
                    var response = await _httpClient.PutAsync(uploadUrl, content);
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        throw new Exception($"Error uploading file: {response.StatusCode}. {errorContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file to OneDrive");
                throw;
            }
        }
        
        private async Task DownloadFileFromOneDriveAsync(string oneDrivePath, string localFilePath)
        {
            try
            {
                // Pobierz URL do pobierania pliku
                var downloadUrl = $"{GraphApiBaseUrl}{oneDrivePath}:/content";
                
                // Pobierz plik
                var response = await _httpClient.GetAsync(downloadUrl);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error downloading file: {response.StatusCode}. {errorContent}");
                }
                
                // Zapisz plik lokalnie
                using (var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write))
                {
                    await response.Content.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file from OneDrive");
                throw;
            }
        }
        
        private async Task<List<OneDriveFile>> GetFilesFromOneDriveAsync(string folderPath)
        {
            try
            {
                // Pobierz URL do listy plików
                var url = $"{GraphApiBaseUrl}{folderPath}:/children";
                
                // Pobierz listę plików
                var response = await _httpClient.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error getting files: {response.StatusCode}. {errorContent}");
                }
                
                // Parsuj odpowiedź
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<OneDriveFilesResponse>(content);
                
                return result.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting files from OneDrive");
                throw;
            }
        }
        
        private string GetAppVersion()
        {
            try
            {
                var package = Windows.ApplicationModel.Package.Current;
                var version = package.Id.Version;
                return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
            }
            catch
            {
                return "1.0.0.0";
            }
        }
        
        private string GetDeviceName()
        {
            try
            {
                return Environment.MachineName;
            }
            catch
            {
                return "Windows Device";
            }
        }
        
        private string GetSystemVersion()
        {
            try
            {
                return Environment.OSVersion.VersionString;
            }
            catch
            {
                return "Windows";
            }
        }
        
        // Klasy pomocnicze do parsowania odpowiedzi OneDrive
        private class OneDriveFilesResponse
        {
            public List<OneDriveFile> Value { get; set; }
        }
        
        private class OneDriveFile
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public long Size { get; set; }
            public DateTime CreatedDateTime { get; set; }
            public DateTime LastModifiedDateTime { get; set; }
        }
    }
}

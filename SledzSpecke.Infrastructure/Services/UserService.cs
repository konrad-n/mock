using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Infrastructure.Database.Repositories;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IFileSystemService _fileSystemService;
        private readonly ILogger<UserService> _logger;
        private int _currentUserId = 1; // Domyślne ID dla uproszczenia

        public UserService(
            IUserRepository userRepository,
            IFileSystemService fileSystemService,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _fileSystemService = fileSystemService;
            _logger = logger;
        }

        public async Task<int> GetCurrentUserIdAsync()
        {
            // W rzeczywistej implementacji należałoby pobrać ID z mechanizmu autentykacji
            return _currentUserId;
        }

        public async Task<User> GetCurrentUserAsync()
        {
            try
            {
                var userId = await GetCurrentUserIdAsync();
                return await _userRepository.GetByIdAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user");
                return null;
            }
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                if (user == null)
                    return false;

                user.ModifiedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                return false;
            }
        }

        public async Task<bool> ExportUserDataAsync(string filePath = null)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null)
                {
                    return false;
                }

                if (string.IsNullOrEmpty(filePath))
                {
                    // Jeśli nie podano ścieżki, użyj domyślnej lokalizacji w folderze aplikacji
                    string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    filePath = Path.Combine(documentsPath, $"UserData_{DateTime.Now.ToString("yyyyMMdd")}.json");
                }

                var userData = new
                {
                    User = user,
                    ExportDate = DateTime.Now
                };

                var json = JsonSerializer.Serialize(userData, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(filePath, json);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting user data");
                return false;
            }
        }

        public async Task<bool> ImportUserDataAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;

                var json = File.ReadAllText(filePath);
                var importData = JsonSerializer.Deserialize<JsonElement>(json);

                if (importData.TryGetProperty("User", out var userElement))
                {
                    var user = JsonSerializer.Deserialize<User>(userElement.GetRawText());
                    if (user != null)
                    {
                        // Zaktualizuj tylko dane, które chcemy zaimportować
                        var currentUser = await GetCurrentUserAsync();
                        if (currentUser != null)
                        {
                            currentUser.Name = user.Name;
                            currentUser.CurrentSpecializationId = user.CurrentSpecializationId;
                            currentUser.SpecializationStartDate = user.SpecializationStartDate;
                            currentUser.ExpectedEndDate = user.ExpectedEndDate;

                            return await UpdateUserAsync(currentUser);
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing user data");
                return false;
            }
        }

        public async Task<bool> LogoutAsync()
        {
            try
            {
                // W rzeczywistej implementacji należałoby wyczyścić dane sesji, 
                // tokeny uwierzytelniające itp.

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return false;
            }
        }

        public async Task<bool> UpdateSpecializationAsync(Specialization specialization)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null || specialization == null)
                {
                    return false;
                }

                user.CurrentSpecialization = specialization;
                user.CurrentSpecializationId = specialization.Id;
                user.ExpectedEndDate = user.SpecializationStartDate.AddDays(specialization.DurationInWeeks * 7);

                return await UpdateUserAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating specialization");
                return false;
            }
        }
    }
}

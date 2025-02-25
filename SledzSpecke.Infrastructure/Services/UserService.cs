using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Infrastructure.Database.Repositories;
using System;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        private int _currentUserId = 1; // Domyślne ID dla uproszczenia

        public UserService(
            IUserRepository userRepository,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
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

        public async Task<bool> ExportUserDataAsync()
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null)
                {
                    return false;
                }

                // Tutaj należałoby zaimplementować eksport danych użytkownika
                // do pliku lub usługi zewnętrznej

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting user data");
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
    }
}

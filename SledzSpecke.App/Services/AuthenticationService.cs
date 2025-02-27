using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Models;
using SledzSpecke.Infrastructure.Database;
using System.Security.Cryptography;
using System.Text;

namespace SledzSpecke.App.Services
{
    public class AuthenticationService
    {
        private readonly DatabaseService _databaseService;
        private readonly ILogger<AuthenticationService> _logger;
        private User _currentUser;

        public bool IsAuthenticated => _currentUser != null;
        public User CurrentUser => _currentUser;

        public AuthenticationService(DatabaseService databaseService, ILogger<AuthenticationService> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        public async Task<bool> RegisterAsync(string username, string email, string password, int specializationTypeId)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _databaseService.QueryAsync<User>("SELECT * FROM Users WHERE Email = ?", email);
                if (existingUser.Count > 0)
                {
                    _logger.LogWarning("Registration failed: User with email {Email} already exists", email);
                    return false;
                }

                // Hash password
                string passwordHash = HashPassword(password);

                // Create new user
                var user = new User
                {
                    Username = username,
                    Email = email,
                    PasswordHash = passwordHash,
                    SpecializationTypeId = specializationTypeId,
                    CreatedAt = DateTime.UtcNow
                };

                // Save user to database
                await _databaseService.SaveAsync(user);

                // Create user settings
                var settings = new UserSettings
                {
                    Username = username,
                    EnableNotifications = true,
                    EnableAutoSync = true,
                    UseDarkTheme = false
                };
                await _databaseService.SaveUserSettingsAsync(settings);

                _logger.LogInformation("User registered successfully: {Username}", username);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                return false;
            }
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                var users = await _databaseService.QueryAsync<User>("SELECT * FROM Users WHERE Email = ?", email);
                if (users.Count == 0)
                {
                    _logger.LogWarning("Login failed: User with email {Email} not found", email);
                    return false;
                }

                var user = users[0];

                // Verify password
                bool isPasswordValid = VerifyPassword(password, user.PasswordHash);
                if (!isPasswordValid)
                {
                    _logger.LogWarning("Login failed: Invalid password for user {Email}", email);
                    return false;
                }

                // Set current user
                _currentUser = user;

                // Update last login
                user.LastLoginAt = DateTime.UtcNow;
                await _databaseService.SaveAsync(user);

                _logger.LogInformation("User logged in successfully: {Username}", user.Username);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login");
                return false;
            }
        }

        public void Logout()
        {
            _currentUser = null;
            _logger.LogInformation("User logged out");
        }

        public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            if (_currentUser == null)
            {
                _logger.LogWarning("Change password failed: No user is logged in");
                return false;
            }

            try
            {
                // Verify current password
                bool isCurrentPasswordValid = VerifyPassword(currentPassword, _currentUser.PasswordHash);
                if (!isCurrentPasswordValid)
                {
                    _logger.LogWarning("Change password failed: Invalid current password");
                    return false;
                }

                // Hash new password
                string newPasswordHash = HashPassword(newPassword);

                // Update password
                _currentUser.PasswordHash = newPasswordHash;
                await _databaseService.SaveAsync(_currentUser);

                _logger.LogInformation("Password changed successfully for user {Username}", _currentUser.Username);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password change");
                return false;
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            string hashedPassword = HashPassword(password);
            return hashedPassword == storedHash;
        }

        public async Task<bool> SeedTestUserAsync()
        {
            try
            {
                // Check if test user already exists
                var existingUsers = await _databaseService.QueryAsync<User>("SELECT * FROM Users WHERE Email = ?", "olo@pozakontrololo.com");
                if (existingUsers.Count > 0)
                {
                    // User already exists, no need to create it again
                    _logger.LogInformation("Test user already exists");
                    return true;
                }

                // Get hematology specialization type
                var specializationTypes = await _databaseService.QueryAsync<SpecializationType>("SELECT * FROM SpecializationTypes WHERE Name = ?", "Hematologia");
                int specializationTypeId = 28; // Default ID for hematology

                if (specializationTypes.Count > 0)
                {
                    specializationTypeId = specializationTypes[0].Id;
                }
                else
                {
                    // Use App.DataManager instead of creating a new instance
                    await App.DataManager.GetAllSpecializationTypesAsync();

                    // Try again
                    specializationTypes = await _databaseService.QueryAsync<SpecializationType>("SELECT * FROM SpecializationTypes WHERE Name = ?", "Hematologia");
                    if (specializationTypes.Count > 0)
                    {
                        specializationTypeId = specializationTypes[0].Id;
                    }
                }

                // Create the test user
                var testUser = new User
                {
                    Username = "Olo Pozakontrolo",
                    Email = "olo@pozakontrololo.com",
                    PasswordHash = HashPassword("gucio"),
                    SpecializationTypeId = specializationTypeId,
                    CreatedAt = DateTime.UtcNow
                };

                // Save the user
                await _databaseService.SaveAsync(testUser);

                // Create user settings
                var settings = new UserSettings
                {
                    Username = testUser.Username,
                    EnableNotifications = true,
                    EnableAutoSync = true,
                    UseDarkTheme = false
                };
                await _databaseService.SaveUserSettingsAsync(settings);

                // Initialize specialization for the user - use App.DataManager instead
                var specialization = await App.DataManager.InitializeSpecializationForUserAsync(specializationTypeId, testUser.Username);

                _logger.LogInformation("Test user seeded successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding test user");
                return false;
            }
        }
    }
}
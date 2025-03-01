using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Models;
using SledzSpecke.Infrastructure.Database;
using System.Security.Cryptography;
using System.Text;

namespace SledzSpecke.App.Services
{
    public class AuthenticationService : IAuthenticationService
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
                _logger.LogInformation("Attempting login for user: {Email}", email);

                var users = await _databaseService.QueryAsync<User>("SELECT * FROM Users WHERE Email = ?", email);
                _logger.LogDebug("Query returned {Count} users for email: {Email}", users.Count, email);

                if (users.Count == 0)
                {
                    _logger.LogWarning("Login failed: User with email {Email} not found", email);
                    return false;
                }

                var user = users[0];
                _logger.LogDebug("Found user: {Username} with ID: {Id}", user.Username, user.Id);

                // Verify password
                bool isPasswordValid = VerifyPassword(password, user.PasswordHash);
                _logger.LogDebug("Password verification result for user {Email}: {Result}", email, isPasswordValid ? "Valid" : "Invalid");

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
                _logger.LogError(ex, "Error during user login for {Email}", email);
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

        // Fragment do dodania w AuthenticationService.cs w metodzie SeedTestUserAsync
        public async Task<bool> SeedTestUserAsync()
        {
            try
            {
                _logger.LogInformation("Seeding test user");

                // Check if test user already exists
                var existingUsers = await _databaseService.QueryAsync<User>("SELECT * FROM Users WHERE Email = ?", "olo@pozakontrololo.com");
                if (existingUsers.Count > 0)
                {
                    // User already exists, no need to create it again
                    _logger.LogInformation("Test user already exists");
                    return true;
                }

                // Ensure specialization types are available
                _logger.LogDebug("Getting specialization types");
                var types = await _databaseService.GetAllAsync<SpecializationType>();
                if (types.Count == 0)
                {
                    _logger.LogInformation("No specialization types found, adding them");
                    var seedTypes = SledzSpecke.Infrastructure.Database.Initialization.SpecializationTypeSeeder.SeedSpecializationTypes();
                    foreach (var type in seedTypes)
                    {
                        await _databaseService.SaveAsync(type);
                    }
                }

                // Get hematology specialization type
                _logger.LogDebug("Getting hematology specialization type");
                var specializationTypes = await _databaseService.QueryAsync<SpecializationType>("SELECT * FROM SpecializationTypes WHERE Name = ?", "Hematologia");
                int specializationTypeId = 28; // Default ID for hematology

                if (specializationTypes.Count > 0)
                {
                    specializationTypeId = specializationTypes[0].Id;
                    _logger.LogDebug("Found hematology specialization type with ID: {Id}", specializationTypeId);
                }
                else
                {
                    _logger.LogWarning("Hematology specialization type not found, using default ID: {Id}", specializationTypeId);
                }

                // Create the test user
                _logger.LogInformation("Creating test user");
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
                _logger.LogDebug("Test user saved with ID: {Id}", testUser.Id);

                // Create user settings
                _logger.LogInformation("Creating user settings for test user");
                var settings = new UserSettings
                {
                    Username = testUser.Username,
                    EnableNotifications = true,
                    EnableAutoSync = true,
                    UseDarkTheme = false,
                    CurrentSpecializationId = 0 // Will be updated when specialization is created
                };
                await _databaseService.SaveUserSettingsAsync(settings);

                // Zainicjuj dane szablonowe dla hematologii
                await _databaseService.InitializeSpecializationTemplateDataAsync(specializationTypeId);

                // Create specialization
                _logger.LogInformation("Creating specialization for test user");
                var specialization = new Specialization
                {
                    Name = "Hematologia",
                    StartDate = DateTime.Now,
                    ExpectedEndDate = DateTime.Now.AddYears(5),
                    BaseDurationWeeks = 261,
                    BasicModuleDurationWeeks = 104,
                    SpecialisticModuleDurationWeeks = 157,
                    VacationDaysPerYear = 26,
                    SelfEducationDaysPerYear = 6,
                    StatutoryHolidaysPerYear = 13,
                    RequiredDutyHoursPerWeek = 10.0833,
                    RequiresPublication = true,
                    RequiredConferences = 3,
                    SpecializationTypeId = specializationTypeId
                };

                await _databaseService.SaveAsync(specialization);
                _logger.LogDebug("Specialization saved with ID: {Id}", specialization.Id);

                // Update user settings with specialization ID
                _logger.LogInformation("Updating user settings with specialization ID");
                settings.CurrentSpecializationId = specialization.Id;
                await _databaseService.SaveUserSettingsAsync(settings);

                _logger.LogInformation("Test user seeded successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding test user");
                return false;
            }
        }

        private async Task SaveRelatedDataAsync(Specialization specialization)
        {
            _logger.LogInformation("Saving related data for specialization");

            try
            {
                // Save courses
                foreach (var course in specialization.RequiredCourses)
                {
                    course.SpecializationId = specialization.Id;
                    await _databaseService.SaveAsync(course);
                }
                _logger.LogDebug("Saved {Count} courses", specialization.RequiredCourses.Count);

                // Save internships
                foreach (var internship in specialization.RequiredInternships)
                {
                    internship.SpecializationId = specialization.Id;
                    await _databaseService.SaveAsync(internship);
                }
                _logger.LogDebug("Saved {Count} internships", specialization.RequiredInternships.Count);

                // Save procedures
                foreach (var procedure in specialization.RequiredProcedures)
                {
                    procedure.SpecializationId = specialization.Id;
                    await _databaseService.SaveAsync(procedure);
                }
                _logger.LogDebug("Saved {Count} procedures", specialization.RequiredProcedures.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving related data for specialization");
                throw;
            }
        }
    }
}
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Core.Models;
using SledzSpecke.Infrastructure.Database;
using System.Security.Cryptography;
using System.Text;

namespace SledzSpecke.App.Services.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<AuthenticationService> _logger;
        private User _currentUser;

        public bool IsAuthenticated => this._currentUser != null;
        public User CurrentUser => this._currentUser;

        public AuthenticationService(
            IDatabaseService databaseService,
            ILogger<AuthenticationService> logger)
        {
            this._databaseService = databaseService;
            this._logger = logger;
        }

        public async Task<bool> RegisterAsync(string username, string email, string password, int specializationTypeId)
        {
            try
            {
                // Check if user already exists
                var existingUser = await this._databaseService.QueryAsync<User>("SELECT * FROM Users WHERE Email = ?", email);
                if (existingUser.Count > 0)
                {
                    this._logger.LogWarning("Registration failed: User with email {Email} already exists", email);
                    return false;
                }

                // Hash password
                string passwordHash = this.HashPassword(password);

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
                await this._databaseService.SaveAsync(user);

                // Create user settings
                var settings = new UserSettings
                {
                    Username = username,
                    EnableNotifications = true,
                    EnableAutoSync = true,
                    UseDarkTheme = false
                };
                await this._databaseService.SaveUserSettingsAsync(settings);

                this._logger.LogInformation("User registered successfully: {Username}", username);
                return true;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error during user registration");
                return false;
            }
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                this._logger.LogInformation("Attempting login for user: {Email}", email);

                var users = await this._databaseService.QueryAsync<User>("SELECT * FROM Users WHERE Email = ?", email);
                this._logger.LogDebug("Query returned {Count} users for email: {Email}", users.Count, email);

                if (users.Count == 0)
                {
                    this._logger.LogWarning("Login failed: User with email {Email} not found", email);
                    return false;
                }

                var user = users[0];
                this._logger.LogDebug("Found user: {Username} with ID: {Id}", user.Username, user.Id);

                // Verify password
                bool isPasswordValid = this.VerifyPassword(password, user.PasswordHash);
                this._logger.LogDebug("Password verification result for user {Email}: {Result}", email, isPasswordValid ? "Valid" : "Invalid");

                if (!isPasswordValid)
                {
                    this._logger.LogWarning("Login failed: Invalid password for user {Email}", email);
                    return false;
                }

                // Set current user
                this._currentUser = user;

                // Update last login
                user.LastLoginAt = DateTime.UtcNow;
                await this._databaseService.SaveAsync(user);

                this._logger.LogInformation("User logged in successfully: {Username}", user.Username);
                return true;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error during user login for {Email}", email);
                return false;
            }
        }

        public void Logout()
        {
            this._currentUser = null;
            this._logger.LogInformation("User logged out");
        }

        public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            if (this._currentUser == null)
            {
                this._logger.LogWarning("Change password failed: No user is logged in");
                return false;
            }

            try
            {
                // Verify current password
                bool isCurrentPasswordValid = this.VerifyPassword(currentPassword, this._currentUser.PasswordHash);
                if (!isCurrentPasswordValid)
                {
                    this._logger.LogWarning("Change password failed: Invalid current password");
                    return false;
                }

                // Hash new password
                string newPasswordHash = this.HashPassword(newPassword);

                // Update password
                this._currentUser.PasswordHash = newPasswordHash;
                await this._databaseService.SaveAsync(this._currentUser);

                this._logger.LogInformation("Password changed successfully for user {Username}", this._currentUser.Username);
                return true;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error during password change");
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
            string hashedPassword = this.HashPassword(password);
            return hashedPassword == storedHash;
        }

        // Fragment do dodania w AuthenticationService.cs w metodzie SeedTestUserAsync
        public async Task<bool> SeedTestUserAsync()
        {
            try
            {
                this._logger.LogInformation("Seeding test user");

                // Check if test user already exists
                var existingUsers = await this._databaseService.QueryAsync<User>("SELECT * FROM Users WHERE Email = ?", "olo@pozakontrololo.com");
                if (existingUsers.Count > 0)
                {
                    // User already exists, no need to create it again
                    this._logger.LogInformation("Test user already exists");
                    return true;
                }

                // Ensure specialization types are available
                this._logger.LogDebug("Getting specialization types");
                var types = await this._databaseService.GetAllAsync<SpecializationType>();
                if (types.Count == 0)
                {
                    this._logger.LogInformation("No specialization types found, adding them");
                    var seedTypes = Infrastructure.Database.Initialization.SpecializationTypeSeeder.SeedSpecializationTypes();
                    foreach (var type in seedTypes)
                    {
                        await this._databaseService.SaveAsync(type);
                    }
                }

                // Get hematology specialization type
                this._logger.LogDebug("Getting hematology specialization type");
                var specializationTypes = await this._databaseService.QueryAsync<SpecializationType>("SELECT * FROM SpecializationTypes WHERE Name = ?", "Hematologia");
                int specializationTypeId = 28; // Default ID for hematology

                if (specializationTypes.Count > 0)
                {
                    specializationTypeId = specializationTypes[0].Id;
                    this._logger.LogDebug("Found hematology specialization type with ID: {Id}", specializationTypeId);
                }
                else
                {
                    this._logger.LogWarning("Hematology specialization type not found, using default ID: {Id}", specializationTypeId);
                }

                // Create the test user
                this._logger.LogInformation("Creating test user");
                var testUser = new User
                {
                    Username = "Olo Pozakontrolo",
                    Email = "olo@pozakontrololo.com",
                    PasswordHash = this.HashPassword("gucio"),
                    SpecializationTypeId = specializationTypeId,
                    CreatedAt = DateTime.UtcNow
                };

                // Save the user
                await this._databaseService.SaveAsync(testUser);
                this._logger.LogDebug("Test user saved with ID: {Id}", testUser.Id);

                // Create user settings
                this._logger.LogInformation("Creating user settings for test user");
                var settings = new UserSettings
                {
                    Username = testUser.Username,
                    EnableNotifications = true,
                    EnableAutoSync = true,
                    UseDarkTheme = false,
                    CurrentSpecializationId = 0 // Will be updated when specialization is created
                };
                await this._databaseService.SaveUserSettingsAsync(settings);

                // Zainicjuj dane szablonowe dla hematologii
                await this._databaseService.InitializeSpecializationTemplateDataAsync(specializationTypeId);

                // Create specialization
                this._logger.LogInformation("Creating specialization for test user");
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

                await this._databaseService.SaveAsync(specialization);
                this._logger.LogDebug("Specialization saved with ID: {Id}", specialization.Id);

                // Update user settings with specialization ID
                this._logger.LogInformation("Updating user settings with specialization ID");
                settings.CurrentSpecializationId = specialization.Id;
                await this._databaseService.SaveUserSettingsAsync(settings);

                this._logger.LogInformation("Test user seeded successfully");
                return true;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error seeding test user");
                return false;
            }
        }

        private async Task SaveRelatedDataAsync(Specialization specialization)
        {
            this._logger.LogInformation("Saving related data for specialization");

            try
            {
                // Save courses
                foreach (var course in specialization.RequiredCourses)
                {
                    course.SpecializationId = specialization.Id;
                    await this._databaseService.SaveAsync(course);
                }
                this._logger.LogDebug("Saved {Count} courses", specialization.RequiredCourses.Count);

                // Save internships
                foreach (var internship in specialization.RequiredInternships)
                {
                    internship.SpecializationId = specialization.Id;
                    await this._databaseService.SaveAsync(internship);
                }
                this._logger.LogDebug("Saved {Count} internships", specialization.RequiredInternships.Count);

                // Save procedures
                foreach (var procedure in specialization.RequiredProcedures)
                {
                    procedure.SpecializationId = specialization.Id;
                    await this._databaseService.SaveAsync(procedure);
                }
                this._logger.LogDebug("Saved {Count} procedures", specialization.RequiredProcedures.Count);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error saving related data for specialization");
                throw;
            }
        }
    }
}
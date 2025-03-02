using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Core.Models;
using SledzSpecke.Infrastructure.Database;

namespace SledzSpecke.App.Services.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IDatabaseService databaseService;
        private readonly ILogger<AuthenticationService> logger;
        private User currentUser;

        public bool IsAuthenticated => this.currentUser != null;

        public User CurrentUser => this.currentUser;

        public AuthenticationService(
            IDatabaseService databaseService,
            ILogger<AuthenticationService> logger)
        {
            this.databaseService = databaseService;
            this.logger = logger;
        }

        public async Task<bool> RegisterAsync(string username, string email, string password, int specializationTypeId)
        {
            try
            {
                var existingUser = await this.databaseService.QueryAsync<User>("SELECT * FROM Users WHERE Email = ?", email);
                if (existingUser.Count > 0)
                {
                    this.logger.LogWarning("Registration failed: User with email {Email} already exists", email);
                    return false;
                }

                string passwordHash = this.HashPassword(password);

                var user = new User
                {
                    Username = username,
                    Email = email,
                    PasswordHash = passwordHash,
                    SpecializationTypeId = specializationTypeId,
                    CreatedAt = DateTime.UtcNow,
                };

                await this.databaseService.SaveAsync(user);

                var settings = new UserSettings
                {
                    Username = username,
                    EnableNotifications = true,
                    EnableAutoSync = true,
                    UseDarkTheme = false,
                };
                await this.databaseService.SaveUserSettingsAsync(settings);

                this.logger.LogInformation("User registered successfully: {Username}", username);
                return true;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error during user registration");
                return false;
            }
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                this.logger.LogInformation("Attempting login for user: {Email}", email);

                var users = await this.databaseService.QueryAsync<User>("SELECT * FROM Users WHERE Email = ?", email);
                this.logger.LogDebug("Query returned {Count} users for email: {Email}", users.Count, email);

                if (users.Count == 0)
                {
                    this.logger.LogWarning("Login failed: User with email {Email} not found", email);
                    return false;
                }

                var user = users[0];
                this.logger.LogDebug("Found user: {Username} with ID: {Id}", user.Username, user.Id);

                bool isPasswordValid = this.VerifyPassword(password, user.PasswordHash);
                this.logger.LogDebug("Password verification result for user {Email}: {Result}", email, isPasswordValid ? "Valid" : "Invalid");

                if (!isPasswordValid)
                {
                    this.logger.LogWarning("Login failed: Invalid password for user {Email}", email);
                    return false;
                }

                this.currentUser = user;

                user.LastLoginAt = DateTime.UtcNow;
                await this.databaseService.SaveAsync(user);

                this.logger.LogInformation("User logged in successfully: {Username}", user.Username);
                return true;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error during user login for {Email}", email);
                return false;
            }
        }

        public void Logout()
        {
            this.currentUser = null;
            this.logger.LogInformation("User logged out");
        }

        public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            if (this.currentUser == null)
            {
                this.logger.LogWarning("Change password failed: No user is logged in");
                return false;
            }

            try
            {
                bool isCurrentPasswordValid = this.VerifyPassword(currentPassword, this.currentUser.PasswordHash);
                if (!isCurrentPasswordValid)
                {
                    this.logger.LogWarning("Change password failed: Invalid current password");
                    return false;
                }

                string newPasswordHash = this.HashPassword(newPassword);
                this.currentUser.PasswordHash = newPasswordHash;
                await this.databaseService.SaveAsync(this.currentUser);

                this.logger.LogInformation("Password changed successfully for user {Username}", this.currentUser.Username);
                return true;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error during password change");
                return false;
            }
        }

        public async Task<bool> SeedTestUserAsync()
        {
            try
            {
                this.logger.LogInformation("Seeding test user");
                var existingUsers = await this.databaseService.QueryAsync<User>("SELECT * FROM Users WHERE Email = ?", "olo@pozakontrololo.com");
                if (existingUsers.Count > 0)
                {
                    this.logger.LogInformation("Test user already exists");
                    return true;
                }
                this.logger.LogDebug("Getting specialization types");
                var types = await this.databaseService.GetAllAsync<SpecializationType>();
                if (types.Count == 0)
                {
                    this.logger.LogInformation("No specialization types found, adding them");
                    var seedTypes = Infrastructure.Database.Initialization.SpecializationTypeSeeder.SeedSpecializationTypes();
                    foreach (var type in seedTypes)
                    {
                        await this.databaseService.SaveAsync(type);
                    }
                }

                this.logger.LogDebug("Getting hematology specialization type");
                var specializationTypes = await this.databaseService.QueryAsync<SpecializationType>("SELECT * FROM SpecializationTypes WHERE Name = ?", "Hematologia");
                int specializationTypeId = 28;

                if (specializationTypes.Count > 0)
                {
                    specializationTypeId = specializationTypes[0].Id;
                    this.logger.LogDebug("Found hematology specialization type with ID: {Id}", specializationTypeId);
                }
                else
                {
                    this.logger.LogWarning("Hematology specialization type not found, using default ID: {Id}", specializationTypeId);
                }

                this.logger.LogInformation("Creating test user");
                var testUser = new User
                {
                    Username = "Olo Pozakontrolo",
                    Email = "olo@pozakontrololo.com",
                    PasswordHash = this.HashPassword("gucio"),
                    SpecializationTypeId = specializationTypeId,
                    CreatedAt = DateTime.UtcNow,
                };

                await this.databaseService.SaveAsync(testUser);
                this.logger.LogDebug("Test user saved with ID: {Id}", testUser.Id);

                this.logger.LogInformation("Creating user settings for test user");
                var settings = new UserSettings
                {
                    Username = testUser.Username,
                    EnableNotifications = true,
                    EnableAutoSync = true,
                    UseDarkTheme = false,
                    CurrentSpecializationId = 0,
                };
                await this.databaseService.SaveUserSettingsAsync(settings);
                await this.databaseService.InitializeSpecializationTemplateDataAsync(specializationTypeId);

                this.logger.LogInformation("Creating specialization for test user");
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
                    SpecializationTypeId = specializationTypeId,
                };

                await this.databaseService.SaveAsync(specialization);
                this.logger.LogDebug("Specialization saved with ID: {Id}", specialization.Id);
                this.logger.LogInformation("Updating user settings with specialization ID");
                settings.CurrentSpecializationId = specialization.Id;
                await this.databaseService.SaveUserSettingsAsync(settings);

                this.logger.LogInformation("Test user seeded successfully");
                return true;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error seeding test user");
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
    }
}
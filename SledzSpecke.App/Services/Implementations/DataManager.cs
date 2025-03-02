using System.Text.Json;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Core.Models;
using SledzSpecke.Infrastructure.Database;
using SledzSpecke.Infrastructure.Database.Initialization;

namespace SledzSpecke.App.Services.Implementations
{
    public class DataManager : IDataManager
    {
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<DataManager> _logger;
        private static readonly string _appDataFolder = FileSystem.AppDataDirectory;
        private static readonly string _specializationFile = Path.Combine(_appDataFolder, "specialization.json");

        private Specialization _specialization;

        public DataManager(
            IDatabaseService databaseService,
            ILogger<DataManager> logger)
        {
            this._databaseService = databaseService;
            this._logger = logger;

            if (!Directory.Exists(_appDataFolder))
            {
                Directory.CreateDirectory(_appDataFolder);
            }
        }

        public async Task<Specialization> LoadSpecializationAsync()
        {
            try
            {
                // First try to get from database
                var specialization = await this._databaseService.GetCurrentSpecializationAsync();
                if (specialization != null)
                {
                    this._specialization = specialization;
                    return specialization;
                }

                // If nothing found, seed default data
                this._specialization = DataSeeder.SeedHematologySpecialization();
                await this.SaveSpecializationAsync(this._specialization);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error loading specialization data");

                // W przypadku błędu, zwracamy nową instancję specjalizacji
                this._specialization = DataSeeder.SeedHematologySpecialization();
                await this.SaveSpecializationAsync(this._specialization);
            }

            return this._specialization;
        }

        public async Task SaveSpecializationAsync(Specialization specialization)
        {
            try
            {
                this._specialization = specialization;

                // Inicjalizacja danych szablonowych jeśli potrzebne
                if (!await this._databaseService.HasSpecializationTemplateDataAsync(specialization.SpecializationTypeId))
                {
                    await this._databaseService.InitializeSpecializationTemplateDataAsync(specialization.SpecializationTypeId);
                }

                // Save to database
                await this._databaseService.SaveAsync(specialization);

                // Save user settings
                var settings = await this._databaseService.GetUserSettingsAsync();
                settings.CurrentSpecializationId = specialization.Id;
                await this._databaseService.SaveUserSettingsAsync(settings);

                // Save to file as backup
                string json = JsonSerializer.Serialize(specialization, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                await File.WriteAllTextAsync(_specializationFile, json);

                this._logger.LogInformation("Specialization saved successfully");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error saving specialization data");
                throw;
            }
        }

        public async Task<bool> DeleteAllDataAsync()
        {
            try
            {
                // Delete from database
                await this._databaseService.DeleteAllDataAsync();

                // Delete file backup
                if (File.Exists(_specializationFile))
                {
                    File.Delete(_specializationFile);
                }

                this._specialization = null;
                this._logger.LogInformation("All data deleted successfully");
                return true;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error deleting all data");
                return false;
            }
        }

        public async Task<List<SpecializationType>> GetAllSpecializationTypesAsync()
        {
            try
            {
                var types = await this._databaseService.GetAllAsync<SpecializationType>();

                if (types.Count == 0)
                {
                    // Seed specialization types if none exist
                    types = SpecializationTypeSeeder.SeedSpecializationTypes();
                    foreach (var type in types)
                    {
                        await this._databaseService.SaveAsync(type);
                    }
                }

                return types;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error getting specialization types");
                return SpecializationTypeSeeder.SeedSpecializationTypes();
            }
        }

        public async Task<Specialization> InitializeSpecializationForUserAsync(int specializationTypeId, string username)
        {
            try
            {
                // Sprawdź, czy dane szablonowe już istnieją
                if (!await this._databaseService.HasSpecializationTemplateDataAsync(specializationTypeId))
                {
                    // Zainicjuj dane szablonowe
                    await this._databaseService.InitializeSpecializationTemplateDataAsync(specializationTypeId);
                }

                // Get specialization type
                var specializationType = await this._databaseService.GetByIdAsync<SpecializationType>(specializationTypeId);
                if (specializationType == null)
                {
                    this._logger.LogError("Specialization type with ID {Id} not found", specializationTypeId);
                    return null;
                }

                // Create new specialization based on type
                var newSpecialization = new Specialization
                {
                    Name = specializationType.Name,
                    StartDate = DateTime.Now,
                    ExpectedEndDate = DateTime.Now.AddDays(specializationType.BaseDurationWeeks * 7),
                    BaseDurationWeeks = specializationType.BaseDurationWeeks,
                    BasicModuleDurationWeeks = specializationType.BasicModuleDurationWeeks,
                    SpecialisticModuleDurationWeeks = specializationType.SpecialisticModuleDurationWeeks,
                    VacationDaysPerYear = specializationType.VacationDaysPerYear,
                    SelfEducationDaysPerYear = specializationType.SelfEducationDaysPerYear,
                    StatutoryHolidaysPerYear = specializationType.StatutoryHolidaysPerYear,
                    RequiredDutyHoursPerWeek = specializationType.RequiredDutyHoursPerWeek,
                    RequiresPublication = specializationType.RequiresPublication,
                    RequiredConferences = specializationType.RequiredConferences,
                    SpecializationTypeId = specializationType.Id
                };

                // Save specialization to database
                await this._databaseService.SaveAsync(newSpecialization);

                // Update user settings
                var settings = await this._databaseService.GetUserSettingsAsync();
                settings.Username = username;
                settings.CurrentSpecializationId = newSpecialization.Id;
                await this._databaseService.SaveUserSettingsAsync(settings);

                this._logger.LogInformation("Specialization initialized successfully for user {Username}", username);
                return newSpecialization;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error initializing specialization for user");
                return null;
            }
        }

        public string GetSpecializationName()
        {
            return this._specialization?.Name ?? "Brak specjalizacji";
        }
    }
}
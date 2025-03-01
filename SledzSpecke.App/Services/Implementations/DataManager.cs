using System.Text.Json;
using Microsoft.Extensions.Logging;
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

        public DataManager(ILogger<DataManager> logger)
        {
            _databaseService = App.DatabaseService;
            _logger = logger;

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
                var specialization = await _databaseService.GetCurrentSpecializationAsync();
                if (specialization != null)
                {
                    _specialization = specialization;
                    return specialization;
                }

                // If nothing found, seed default data
                _specialization = DataSeeder.SeedHematologySpecialization();
                await SaveSpecializationAsync(_specialization);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading specialization data");

                // W przypadku błędu, zwracamy nową instancję specjalizacji
                _specialization = DataSeeder.SeedHematologySpecialization();
                await SaveSpecializationAsync(_specialization);
            }

            return _specialization;
        }

        public async Task SaveSpecializationAsync(Specialization specialization)
        {
            try
            {
                _specialization = specialization;

                // Inicjalizacja danych szablonowych jeśli potrzebne
                if (!await _databaseService.HasSpecializationTemplateDataAsync(specialization.SpecializationTypeId))
                {
                    await _databaseService.InitializeSpecializationTemplateDataAsync(specialization.SpecializationTypeId);
                }

                // Save to database
                await _databaseService.SaveAsync(specialization);

                // Save user settings
                var settings = await _databaseService.GetUserSettingsAsync();
                settings.CurrentSpecializationId = specialization.Id;
                await _databaseService.SaveUserSettingsAsync(settings);

                // Save to file as backup
                string json = JsonSerializer.Serialize(specialization, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                await File.WriteAllTextAsync(_specializationFile, json);

                _logger.LogInformation("Specialization saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving specialization data");
                throw;
            }
        }

        public async Task<bool> DeleteAllDataAsync()
        {
            try
            {
                // Delete from database
                await _databaseService.DeleteAllDataAsync();

                // Delete file backup
                if (File.Exists(_specializationFile))
                {
                    File.Delete(_specializationFile);
                }

                _specialization = null;
                _logger.LogInformation("All data deleted successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting all data");
                return false;
            }
        }

        public async Task<List<SpecializationType>> GetAllSpecializationTypesAsync()
        {
            try
            {
                var types = await _databaseService.GetAllAsync<SpecializationType>();

                if (types.Count == 0)
                {
                    // Seed specialization types if none exist
                    types = SpecializationTypeSeeder.SeedSpecializationTypes();
                    foreach (var type in types)
                    {
                        await _databaseService.SaveAsync(type);
                    }
                }

                return types;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting specialization types");
                return SpecializationTypeSeeder.SeedSpecializationTypes();
            }
        }

        public async Task<Specialization> InitializeSpecializationForUserAsync(int specializationTypeId, string username)
        {
            try
            {
                // Sprawdź, czy dane szablonowe już istnieją
                if (!await _databaseService.HasSpecializationTemplateDataAsync(specializationTypeId))
                {
                    // Zainicjuj dane szablonowe
                    await _databaseService.InitializeSpecializationTemplateDataAsync(specializationTypeId);
                }

                // Get specialization type
                var specializationType = await _databaseService.GetByIdAsync<SpecializationType>(specializationTypeId);
                if (specializationType == null)
                {
                    _logger.LogError("Specialization type with ID {Id} not found", specializationTypeId);
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
                await _databaseService.SaveAsync(newSpecialization);

                // Update user settings
                var settings = await _databaseService.GetUserSettingsAsync();
                settings.Username = username;
                settings.CurrentSpecializationId = newSpecialization.Id;
                await _databaseService.SaveUserSettingsAsync(settings);

                _logger.LogInformation("Specialization initialized successfully for user {Username}", username);
                return newSpecialization;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing specialization for user");
                return null;
            }
        }

        public string GetSpecializationName()
        {
            return _specialization?.Name ?? "Brak specjalizacji";
        }
    }
}
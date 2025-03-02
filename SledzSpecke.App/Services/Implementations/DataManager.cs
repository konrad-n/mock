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
        private readonly IDatabaseService databaseService;
        private readonly ILogger<DataManager> logger;
        private static readonly string appDataFolder = FileSystem.AppDataDirectory;
        private static readonly string specializationFile = Path.Combine(appDataFolder, "specialization.json");

        private Specialization specialization;

        public DataManager(
            IDatabaseService databaseService,
            ILogger<DataManager> logger)
        {
            this.databaseService = databaseService;
            this.logger = logger;

            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }
        }

        public async Task<Specialization> LoadSpecializationAsync()
        {
            try
            {
                var currentSpecialization = await this.databaseService.GetCurrentSpecializationAsync();
                if (currentSpecialization != null)
                {
                    this.specialization = currentSpecialization;

                    return currentSpecialization;
                }

                this.specialization = DataSeeder.SeedHematologySpecialization();
                await this.SaveSpecializationAsync(this.specialization);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error loading specialization data");

                this.specialization = DataSeeder.SeedHematologySpecialization();
                await this.SaveSpecializationAsync(this.specialization);
            }

            return this.specialization;
        }

        public async Task SaveSpecializationAsync(Specialization specialization)
        {
            try
            {
                this.specialization = specialization;

                if (!await this.databaseService.HasSpecializationTemplateDataAsync(specialization.SpecializationTypeId))
                {
                    await this.databaseService.InitializeSpecializationTemplateDataAsync(specialization.SpecializationTypeId);
                }

                await this.databaseService.SaveAsync(specialization);
                var settings = await this.databaseService.GetUserSettingsAsync();
                settings.CurrentSpecializationId = specialization.Id;
                await this.databaseService.SaveUserSettingsAsync(settings);
                string json = JsonSerializer.Serialize(specialization, new JsonSerializerOptions
                {
                    WriteIndented = true,
                });
                await File.WriteAllTextAsync(specializationFile, json);

                this.logger.LogInformation("Specialization saved successfully");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error saving specialization data");
                throw;
            }
        }

        public async Task<bool> DeleteAllDataAsync()
        {
            try
            {
                await this.databaseService.DeleteAllDataAsync();
                if (File.Exists(specializationFile))
                {
                    File.Delete(specializationFile);
                }

                this.specialization = null;
                this.logger.LogInformation("All data deleted successfully");
                return true;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error deleting all data");
                return false;
            }
        }

        public async Task<List<SpecializationType>> GetAllSpecializationTypesAsync()
        {
            try
            {
                var types = await this.databaseService.GetAllAsync<SpecializationType>();

                if (types.Count == 0)
                {
                    types = SpecializationTypeSeeder.SeedSpecializationTypes();
                    foreach (var type in types)
                    {
                        await this.databaseService.SaveAsync(type);
                    }
                }

                return types;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error getting specialization types");
                return SpecializationTypeSeeder.SeedSpecializationTypes();
            }
        }

        public async Task<Specialization> InitializeSpecializationForUserAsync(int specializationTypeId, string username)
        {
            try
            {
                if (!await this.databaseService.HasSpecializationTemplateDataAsync(specializationTypeId))
                {
                    await this.databaseService.InitializeSpecializationTemplateDataAsync(specializationTypeId);
                }

                var specializationType = await this.databaseService.GetByIdAsync<SpecializationType>(specializationTypeId);
                if (specializationType == null)
                {
                    this.logger.LogError("Specialization type with ID {Id} not found", specializationTypeId);
                    return null;
                }

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
                    SpecializationTypeId = specializationType.Id,
                };

                await this.databaseService.SaveAsync(newSpecialization);
                var settings = await this.databaseService.GetUserSettingsAsync();
                settings.Username = username;
                settings.CurrentSpecializationId = newSpecialization.Id;
                await this.databaseService.SaveUserSettingsAsync(settings);

                this.logger.LogInformation("Specialization initialized successfully for user {Username}", username);
                return newSpecialization;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error initializing specialization for user");
                return null;
            }
        }

        public string GetSpecializationName()
        {
            return this.specialization?.Name ?? "Brak specjalizacji";
        }
    }
}
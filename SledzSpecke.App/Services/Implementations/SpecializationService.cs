using Microsoft.Extensions.Logging;
using SledzSpecke.App.Services.Interfaces;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;
using SledzSpecke.Infrastructure.Database;
using SledzSpecke.Infrastructure.Database.Initialization;

namespace SledzSpecke.App.Services.Implementations
{
    public class SpecializationService : ISpecializationService
    {
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<SpecializationService> _logger;

        public SpecializationService(
            IDatabaseService databaseService,
            ILogger<SpecializationService> logger)
        {
            this._databaseService = databaseService;
            this._logger = logger;
        }

        public async Task<Specialization> GetSpecializationAsync()
        {
            try
            {
                // Get current specialization from database
                var specialization = await this._databaseService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    this._logger.LogInformation("No current specialization found. Creating default specialization.");
                    // Create default specialization if none exists
                    specialization = DataSeeder.SeedHematologySpecialization();
                    await this.SaveSpecializationAsync(specialization);
                }

                // Load related data
                await this.LoadRelatedDataAsync(specialization);

                return specialization;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error getting specialization");
                // Nie zwracaj tutaj seedera - to zmniejszy spójność danych
                throw;
            }
        }

        private async Task LoadRelatedDataAsync(Specialization specialization)
        {
            try
            {
                // Load courses
                var courses = await this._databaseService.QueryAsync<Course>("SELECT * FROM Courses WHERE SpecializationId = ?", specialization.Id);
                specialization.RequiredCourses = courses;

                // Load internships
                var internships = await this._databaseService.QueryAsync<Internship>("SELECT * FROM Internships WHERE SpecializationId = ?", specialization.Id);
                specialization.RequiredInternships = internships;

                // Load procedures
                var procedures = await this._databaseService.QueryAsync<MedicalProcedure>("SELECT * FROM MedicalProcedures WHERE SpecializationId = ?", specialization.Id);
                specialization.RequiredProcedures = procedures;

                // Load procedure entries
                foreach (var procedure in specialization.RequiredProcedures)
                {
                    var entries = await this._databaseService.QueryAsync<ProcedureEntry>("SELECT * FROM ProcedureEntries WHERE ProcedureId = ?", procedure.Id);
                    procedure.Entries = entries;
                    procedure.CompletedCount = entries.Count;
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error loading related data for specialization");
                throw;
            }
        }

        public async Task SaveSpecializationAsync(Specialization specialization)
        {
            try
            {
                // Sprawdź czy dane szablonowe istnieją, jeśli nie - zainicjuj je
                if (!await this._databaseService.HasSpecializationTemplateDataAsync(specialization.SpecializationTypeId))
                {
                    await this._databaseService.InitializeSpecializationTemplateDataAsync(specialization.SpecializationTypeId);

                    // Po inicjalizacji ponownie pobierz specjalizację
                    specialization = await this._databaseService.GetCurrentSpecializationAsync();
                    if (specialization == null)
                    {
                        throw new Exception("Failed to initialize specialization template data");
                    }
                }

                // Save specialization
                await this._databaseService.SaveAsync(specialization);

                // Update user settings
                var settings = await this._databaseService.GetUserSettingsAsync();
                settings.CurrentSpecializationId = specialization.Id;
                await this._databaseService.SaveUserSettingsAsync(settings);

                this._logger.LogInformation("Specialization saved successfully");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error saving specialization");
                throw;
            }
        }

        public async Task<List<Course>> GetCoursesAsync(ModuleType moduleType)
        {
            try
            {
                var specialization = await this.GetSpecializationAsync();
                return specialization.RequiredCourses
                    .Where(c => c.Module == moduleType)
                    .ToList();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error getting courses");
                return new List<Course>();
            }
        }

        public async Task<List<Internship>> GetInternshipsAsync(ModuleType moduleType)
        {
            try
            {
                var specialization = await this.GetSpecializationAsync();
                return specialization.RequiredInternships
                    .Where(i => i.Module == moduleType)
                    .ToList();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error getting internships");
                return new List<Internship>();
            }
        }

        public async Task<List<MedicalProcedure>> GetProceduresAsync(ModuleType moduleType, ProcedureType procedureType)
        {
            try
            {
                var specialization = await this.GetSpecializationAsync();
                return specialization.RequiredProcedures
                    .Where(p => p.Module == moduleType && p.ProcedureType == procedureType)
                    .ToList();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error getting procedures");
                return new List<MedicalProcedure>();
            }
        }

        public async Task SaveCourseAsync(Course course)
        {
            try
            {
                var specialization = await this.GetSpecializationAsync();
                course.SpecializationId = specialization.Id;

                await this._databaseService.SaveAsync(course);

                var existingCourse = specialization.RequiredCourses.FirstOrDefault(c => c.Id == course.Id);
                if (existingCourse != null)
                {
                    // Update existing course in memory
                    var index = specialization.RequiredCourses.IndexOf(existingCourse);
                    specialization.RequiredCourses[index] = course;
                }
                else
                {
                    // Add new course
                    specialization.RequiredCourses.Add(course);
                }

                this._logger.LogInformation("Course saved successfully");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error saving course");
                throw;
            }
        }

        public async Task SaveInternshipAsync(Internship internship)
        {
            try
            {
                var specialization = await this.GetSpecializationAsync();
                internship.SpecializationId = specialization.Id;

                await this._databaseService.SaveAsync(internship);

                var existingInternship = specialization.RequiredInternships.FirstOrDefault(i => i.Id == internship.Id);
                if (existingInternship != null)
                {
                    // Update existing internship in memory
                    var index = specialization.RequiredInternships.IndexOf(existingInternship);
                    specialization.RequiredInternships[index] = internship;
                }
                else
                {
                    // Add new internship
                    specialization.RequiredInternships.Add(internship);
                }

                this._logger.LogInformation("Internship saved successfully");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error saving internship");
                throw;
            }
        }

        public async Task SaveProcedureAsync(MedicalProcedure procedure)
        {
            try
            {
                var specialization = await this.GetSpecializationAsync();
                procedure.SpecializationId = specialization.Id;

                await this._databaseService.SaveAsync(procedure);

                // Save procedure entries
                foreach (var entry in procedure.Entries)
                {
                    entry.ProcedureId = procedure.Id;
                    await this._databaseService.SaveAsync(entry);
                }

                var existingProcedure = specialization.RequiredProcedures.FirstOrDefault(p => p.Id == procedure.Id);
                if (existingProcedure != null)
                {
                    // Update existing procedure in memory
                    var index = specialization.RequiredProcedures.IndexOf(existingProcedure);
                    specialization.RequiredProcedures[index] = procedure;
                }
                else
                {
                    // Add new procedure
                    specialization.RequiredProcedures.Add(procedure);
                }

                this._logger.LogInformation("Procedure saved successfully");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error saving procedure");
                throw;
            }
        }

        public async Task AddProcedureEntryAsync(MedicalProcedure procedure, ProcedureEntry entry)
        {
            try
            {
                entry.ProcedureId = procedure.Id;
                await this._databaseService.SaveAsync(entry);

                // Update procedure
                procedure.Entries.Add(entry);
                procedure.CompletedCount = procedure.Entries.Count;
                await this._databaseService.SaveAsync(procedure);

                this._logger.LogInformation("Procedure entry added successfully");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error adding procedure entry");
                throw;
            }
        }
    }
}
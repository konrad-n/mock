using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;
using SledzSpecke.Infrastructure.Database;
using SledzSpecke.Infrastructure.Database.Initialization;

namespace SledzSpecke.App.Services.Implementations
{
    public class SpecializationService : ISpecializationService
    {
        private readonly DatabaseService _databaseService;
        private readonly ILogger<SpecializationService> _logger;

        public SpecializationService(DatabaseService databaseService, ILogger<SpecializationService> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        public async Task<Specialization> GetSpecializationAsync()
        {
            try
            {
                // Get current specialization from database
                var specialization = await _databaseService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    _logger.LogInformation("No current specialization found. Creating default specialization.");
                    // Create default specialization if none exists
                    specialization = DataSeeder.SeedHematologySpecialization();
                    await SaveSpecializationAsync(specialization);
                }

                // Load related data
                await LoadRelatedDataAsync(specialization);

                return specialization;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting specialization");
                // Nie zwracaj tutaj seedera - to zmniejszy spójność danych
                throw;
            }
        }

        private async Task LoadRelatedDataAsync(Specialization specialization)
        {
            try
            {
                // Load courses
                var courses = await _databaseService.QueryAsync<Course>("SELECT * FROM Courses WHERE SpecializationId = ?", specialization.Id);
                specialization.RequiredCourses = courses;

                // Load internships
                var internships = await _databaseService.QueryAsync<Internship>("SELECT * FROM Internships WHERE SpecializationId = ?", specialization.Id);
                specialization.RequiredInternships = internships;

                // Load procedures
                var procedures = await _databaseService.QueryAsync<MedicalProcedure>("SELECT * FROM MedicalProcedures WHERE SpecializationId = ?", specialization.Id);
                specialization.RequiredProcedures = procedures;

                // Load procedure entries
                foreach (var procedure in specialization.RequiredProcedures)
                {
                    var entries = await _databaseService.QueryAsync<ProcedureEntry>("SELECT * FROM ProcedureEntries WHERE ProcedureId = ?", procedure.Id);
                    procedure.Entries = entries;
                    procedure.CompletedCount = entries.Count;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading related data for specialization");
                throw;
            }
        }

        public async Task SaveSpecializationAsync(Specialization specialization)
        {
            try
            {
                // Sprawdź czy dane szablonowe istnieją, jeśli nie - zainicjuj je
                if (!await _databaseService.HasSpecializationTemplateDataAsync(specialization.SpecializationTypeId))
                {
                    await _databaseService.InitializeSpecializationTemplateDataAsync(specialization.SpecializationTypeId);

                    // Po inicjalizacji ponownie pobierz specjalizację
                    specialization = await _databaseService.GetCurrentSpecializationAsync();
                    if (specialization == null)
                    {
                        throw new Exception("Failed to initialize specialization template data");
                    }
                }

                // Save specialization
                await _databaseService.SaveAsync(specialization);

                // Update user settings
                var settings = await _databaseService.GetUserSettingsAsync();
                settings.CurrentSpecializationId = specialization.Id;
                await _databaseService.SaveUserSettingsAsync(settings);

                _logger.LogInformation("Specialization saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving specialization");
                throw;
            }
        }

        public async Task<List<Course>> GetCoursesAsync(ModuleType moduleType)
        {
            try
            {
                var specialization = await GetSpecializationAsync();
                return specialization.RequiredCourses
                    .Where(c => c.Module == moduleType)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting courses");
                return new List<Course>();
            }
        }

        public async Task<List<Internship>> GetInternshipsAsync(ModuleType moduleType)
        {
            try
            {
                var specialization = await GetSpecializationAsync();
                return specialization.RequiredInternships
                    .Where(i => i.Module == moduleType)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting internships");
                return new List<Internship>();
            }
        }

        public async Task<List<MedicalProcedure>> GetProceduresAsync(ModuleType moduleType, ProcedureType procedureType)
        {
            try
            {
                var specialization = await GetSpecializationAsync();
                return specialization.RequiredProcedures
                    .Where(p => p.Module == moduleType && p.ProcedureType == procedureType)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting procedures");
                return new List<MedicalProcedure>();
            }
        }

        public async Task SaveCourseAsync(Course course)
        {
            try
            {
                var specialization = await GetSpecializationAsync();
                course.SpecializationId = specialization.Id;

                await _databaseService.SaveAsync(course);

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

                _logger.LogInformation("Course saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving course");
                throw;
            }
        }

        public async Task SaveInternshipAsync(Internship internship)
        {
            try
            {
                var specialization = await GetSpecializationAsync();
                internship.SpecializationId = specialization.Id;

                await _databaseService.SaveAsync(internship);

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

                _logger.LogInformation("Internship saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving internship");
                throw;
            }
        }

        public async Task SaveProcedureAsync(MedicalProcedure procedure)
        {
            try
            {
                var specialization = await GetSpecializationAsync();
                procedure.SpecializationId = specialization.Id;

                await _databaseService.SaveAsync(procedure);

                // Save procedure entries
                foreach (var entry in procedure.Entries)
                {
                    entry.ProcedureId = procedure.Id;
                    await _databaseService.SaveAsync(entry);
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

                _logger.LogInformation("Procedure saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving procedure");
                throw;
            }
        }

        public async Task AddProcedureEntryAsync(MedicalProcedure procedure, ProcedureEntry entry)
        {
            try
            {
                entry.ProcedureId = procedure.Id;
                await _databaseService.SaveAsync(entry);

                // Update procedure
                procedure.Entries.Add(entry);
                procedure.CompletedCount = procedure.Entries.Count;
                await _databaseService.SaveAsync(procedure);

                _logger.LogInformation("Procedure entry added successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding procedure entry");
                throw;
            }
        }
    }
}
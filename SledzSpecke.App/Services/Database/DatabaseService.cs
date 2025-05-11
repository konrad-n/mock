using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Logging;
using SQLite;
using System.Diagnostics;

namespace SledzSpecke.App.Services.Database
{
    public class DatabaseService : IDatabaseService
    {
        private SQLiteAsyncConnection database;
        private bool isInitialized = false;
        private readonly Dictionary<int, Models.Specialization> _specializationCache = new();
        private readonly Dictionary<int, List<Module>> _moduleCache = new();
        private readonly IExceptionHandlerService _exceptionHandler;
        private readonly ILoggingService _loggingService;

        public DatabaseService(IExceptionHandlerService exceptionHandler, ILoggingService loggingService)
        {
            _exceptionHandler = exceptionHandler ?? throw new ArgumentNullException(nameof(exceptionHandler));
            _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
        }

        public async Task InitializeAsync()
        {
            if (this.isInitialized)
            {
                return;
            }

            await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var databasePath = Constants.DatabasePath;
                var databaseDirectory = Path.GetDirectoryName(databasePath);

                if (!Directory.Exists(databaseDirectory))
                {
                    Directory.CreateDirectory(databaseDirectory);
                }

                this.database = new SQLiteAsyncConnection(databasePath, Constants.Flags);

                await this.database.CreateTableAsync<User>();
                await this.database.CreateTableAsync<Models.Specialization>();
                await this.database.CreateTableAsync<Module>();
                await this.database.CreateTableAsync<Internship>();
                await this.database.CreateTableAsync<MedicalShift>();
                await this.database.CreateTableAsync<Procedure>();
                await this.database.CreateTableAsync<Course>();
                await this.database.CreateTableAsync<SelfEducation>();
                await this.database.CreateTableAsync<Publication>();
                await this.database.CreateTableAsync<EducationalActivity>();
                await this.database.CreateTableAsync<Absence>();
                await this.database.CreateTableAsync<Models.Recognition>();
                await this.database.CreateTableAsync<SpecializationProgram>();
                await this.database.CreateTableAsync<RealizedMedicalShiftOldSMK>();
                await this.database.CreateTableAsync<RealizedMedicalShiftNewSMK>();
                await this.database.CreateTableAsync<RealizedProcedureNewSMK>();
                await this.database.CreateTableAsync<RealizedProcedureOldSMK>();
                await this.database.CreateTableAsync<RealizedInternshipNewSMK>();
                await this.database.CreateTableAsync<RealizedInternshipOldSMK>();

                this.isInitialized = true;
            }, null, "Nie udało się zainicjalizować bazy danych.", 3, 1000);
        }

        public async Task<User> GetUserAsync(int id)
        {
            await this.InitializeAsync();

            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var user = await this.database.Table<User>().FirstOrDefaultAsync(u => u.UserId == id);
                if (user == null)
                {
                    throw new ResourceNotFoundException(
                        $"User with ID {id} not found",
                        $"Nie znaleziono użytkownika o ID {id}",
                        null,
                        new Dictionary<string, object> { { "UserId", id } });
                }
                return user;
            },
            new Dictionary<string, object> { { "UserId", id } },
            $"Nie udało się pobrać użytkownika o ID {id}",
            3, 800);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (string.IsNullOrEmpty(username))
                {
                    throw new InvalidInputException(
                        "Username cannot be null or empty",
                        "Nazwa użytkownika nie może być pusta");
                }

                return await this.database.Table<User>().FirstOrDefaultAsync(u => u.Username == username);
            },
            new Dictionary<string, object> { { "Username", username } },
            $"Nie udało się pobrać użytkownika o nazwie {username}");
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                return await this.database.Table<User>().ToListAsync();
            }, null, "Nie udało się pobrać listy użytkowników");
        }

        public async Task<int> SaveUserAsync(User user)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (user == null)
                {
                    throw new InvalidInputException(
                        "User cannot be null",
                        "Użytkownik nie może być pusty");
                }

                if (user.UserId != 0)
                {
                    await this.database.UpdateAsync(user);
                    return user.UserId;
                }
                else
                {
                    await this.database.InsertAsync(user);
                    var lastId = await this.database.ExecuteScalarAsync<int>("SELECT last_insert_rowid()");
                    user.UserId = lastId;
                    return lastId;
                }
            },
            new Dictionary<string, object> { { "User", user?.UserId } },
            "Nie udało się zapisać danych użytkownika");
        }

        public async Task<Models.Specialization> GetSpecializationAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (_specializationCache.TryGetValue(id, out var cachedSpecialization))
                {
                    return cachedSpecialization;
                }

                var specialization = await database.Table<Models.Specialization>()
                    .FirstOrDefaultAsync(s => s.SpecializationId == id);

                if (specialization != null)
                {
                    _specializationCache[id] = specialization;
                }

                return specialization ?? new Models.Specialization();
            },
            new Dictionary<string, object> { { "SpecializationId", id } },
            $"Nie udało się pobrać specjalizacji o ID {id}");
        }

        public async Task<Models.Specialization> GetSpecializationWithModulesAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var specialization = await GetSpecializationAsync(id);
                if (specialization != null)
                {
                    specialization.Modules = await GetModulesAsync(specialization.SpecializationId);
                }
                return specialization;
            },
            new Dictionary<string, object> { { "SpecializationId", id } },
            $"Nie udało się pobrać specjalizacji z modułami o ID {id}");
        }

        public async Task<int> SaveSpecializationAsync(Models.Specialization specialization)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (specialization == null)
                {
                    throw new InvalidInputException(
                        "Specialization cannot be null",
                        "Specjalizacja nie może być pusta");
                }

                if (specialization.SpecializationId != 0)
                {
                    await this.database.UpdateAsync(specialization);

                    // Clear cache after update
                    if (_specializationCache.ContainsKey(specialization.SpecializationId))
                    {
                        _specializationCache.Remove(specialization.SpecializationId);
                    }

                    return specialization.SpecializationId;
                }
                else
                {
                    await this.database.InsertAsync(specialization);
                    var lastId = await this.database.ExecuteScalarAsync<int>("SELECT last_insert_rowid()");
                    specialization.SpecializationId = lastId;
                    return lastId;
                }
            },
            new Dictionary<string, object> { { "Specialization", specialization?.SpecializationId } },
            "Nie udało się zapisać danych specjalizacji");
        }

        public async Task<List<Models.Specialization>> GetAllSpecializationsAsync()
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                return await this.database.Table<Models.Specialization>().ToListAsync();
            }, null, "Nie udało się pobrać listy specjalizacji");
        }

        public async Task<int> UpdateSpecializationAsync(Models.Specialization specialization)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (specialization == null)
                {
                    throw new InvalidInputException(
                        "Specialization cannot be null",
                        "Specjalizacja nie może być pusta");
                }

                var result = await this.database.UpdateAsync(specialization);

                // Clear cache after update
                if (_specializationCache.ContainsKey(specialization.SpecializationId))
                {
                    _specializationCache.Remove(specialization.SpecializationId);
                }

                return result;
            },
            new Dictionary<string, object> { { "Specialization", specialization?.SpecializationId } },
            "Nie udało się zaktualizować danych specjalizacji");
        }

        public async Task CleanupSpecializationDataAsync(int specializationId)
        {
            await this.InitializeAsync();
            await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var modules = await GetModulesAsync(specializationId);
                foreach (var module in modules)
                {
                    await DeleteModuleAsync(module);
                }

                // Clear caches
                if (_specializationCache.ContainsKey(specializationId))
                {
                    _specializationCache.Remove(specializationId);
                }

                if (_moduleCache.ContainsKey(specializationId))
                {
                    _moduleCache.Remove(specializationId);
                }
            },
            new Dictionary<string, object> { { "SpecializationId", specializationId } },
            $"Nie udało się wyczyścić danych specjalizacji o ID {specializationId}");
        }

        public async Task<Module> GetModuleAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var module = await this.database.Table<Module>().FirstOrDefaultAsync(m => m.ModuleId == id);
                if (module == null)
                {
                    throw new ResourceNotFoundException(
                        $"Module with ID {id} not found",
                        $"Nie znaleziono modułu o ID {id}");
                }
                return module;
            },
            new Dictionary<string, object> { { "ModuleId", id } },
            $"Nie udało się pobrać modułu o ID {id}");
        }

        public async Task<List<Module>> GetModulesAsync(int specializationId)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (_moduleCache.TryGetValue(specializationId, out var cachedModules))
                {
                    return cachedModules;
                }

                var modules = await database.Table<Module>()
                    .Where(m => m.SpecializationId == specializationId)
                    .ToListAsync();

                if (modules != null)
                {
                    _moduleCache[specializationId] = modules;
                }

                return modules ?? new List<Module>();
            },
            new Dictionary<string, object> { { "SpecializationId", specializationId } },
            $"Nie udało się pobrać listy modułów dla specjalizacji o ID {specializationId}");
        }

        public async Task<int> SaveModuleAsync(Module module)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (module == null)
                {
                    throw new InvalidInputException(
                        "Module cannot be null",
                        "Moduł nie może być pusty");
                }

                if (module.ModuleId != 0)
                {
                    return await this.database.UpdateAsync(module);
                }
                else
                {
                    return await this.database.InsertAsync(module);
                }
            },
            new Dictionary<string, object> { { "Module", module?.ModuleId }, { "SpecializationId", module?.SpecializationId } },
            "Nie udało się zapisać danych modułu");
        }

        public async Task<int> UpdateModuleAsync(Module module)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (module == null)
                {
                    throw new InvalidInputException(
                        "Module cannot be null",
                        "Moduł nie może być pusty");
                }

                // Clear cache after update
                if (_moduleCache.TryGetValue(module.SpecializationId, out _))
                {
                    _moduleCache.Remove(module.SpecializationId);
                }

                return await this.database.UpdateAsync(module);
            },
            new Dictionary<string, object> { { "Module", module?.ModuleId }, { "SpecializationId", module?.SpecializationId } },
            "Nie udało się zaktualizować danych modułu");
        }

        public async Task<int> DeleteModuleAsync(Module module)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (module == null)
                {
                    throw new InvalidInputException(
                        "Module cannot be null",
                        "Moduł nie może być pusty");
                }

                // Clear cache after delete
                if (_moduleCache.TryGetValue(module.SpecializationId, out _))
                {
                    _moduleCache.Remove(module.SpecializationId);
                }

                return await this.database.DeleteAsync(module);
            },
            new Dictionary<string, object> { { "Module", module?.ModuleId }, { "SpecializationId", module?.SpecializationId } },
            "Nie udało się usunąć modułu");
        }

        public async Task<Internship> GetInternshipAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var internship = await this.database.Table<Internship>().FirstOrDefaultAsync(i => i.InternshipId == id);
                if (internship == null)
                {
                    throw new ResourceNotFoundException(
                        $"Internship with ID {id} not found",
                        $"Nie znaleziono stażu o ID {id}");
                }
                return internship;
            },
            new Dictionary<string, object> { { "InternshipId", id } },
            $"Nie udało się pobrać stażu o ID {id}");
        }

        public async Task<List<Internship>> GetInternshipsAsync(int? specializationId = null, int? moduleId = null)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var query = this.database.Table<Internship>();

                if (specializationId.HasValue)
                {
                    query = query.Where(i => i.SpecializationId == specializationId);
                }

                if (moduleId.HasValue)
                {
                    query = query.Where(i => i.ModuleId == moduleId);
                }

                return await query.ToListAsync();
            },
            new Dictionary<string, object> {
                { "SpecializationId", specializationId },
                { "ModuleId", moduleId }
            },
            "Nie udało się pobrać listy staży");
        }

        public async Task<int> SaveInternshipAsync(Internship internship)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "Internship cannot be null",
                        "Staż nie może być pusty");
                }

                if (internship.InternshipId != 0)
                {
                    return await this.database.UpdateAsync(internship);
                }
                else
                {
                    return await this.database.InsertAsync(internship);
                }
            },
            new Dictionary<string, object> {
                { "Internship", internship?.InternshipId },
                { "SpecializationId", internship?.SpecializationId },
                { "ModuleId", internship?.ModuleId }
            },
            "Nie udało się zapisać danych stażu");
        }

        public async Task<int> DeleteInternshipAsync(Internship internship)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "Internship cannot be null",
                        "Staż nie może być pusty");
                }

                return await this.database.DeleteAsync(internship);
            },
            new Dictionary<string, object> { { "Internship", internship?.InternshipId } },
            "Nie udało się usunąć stażu");
        }

        public async Task<MedicalShift> GetMedicalShiftAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var shift = await this.database.Table<MedicalShift>().FirstOrDefaultAsync(s => s.ShiftId == id);
                if (shift == null)
                {
                    throw new ResourceNotFoundException(
                        $"MedicalShift with ID {id} not found",
                        $"Nie znaleziono dyżuru o ID {id}");
                }
                return shift;
            },
            new Dictionary<string, object> { { "ShiftId", id } },
            $"Nie udało się pobrać dyżuru o ID {id}");
        }

        public async Task<List<MedicalShift>> GetMedicalShiftsAsync(int? internshipId = null)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var query = this.database.Table<MedicalShift>();

                if (internshipId.HasValue)
                {
                    query = query.Where(s => s.InternshipId == internshipId);
                }

                return await query.ToListAsync();
            },
            new Dictionary<string, object> { { "InternshipId", internshipId } },
            "Nie udało się pobrać listy dyżurów");
        }

        public async Task<int> SaveMedicalShiftAsync(MedicalShift shift)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (shift == null)
                {
                    throw new InvalidInputException(
                        "MedicalShift cannot be null",
                        "Dyżur nie może być pusty");
                }

                if (shift.ShiftId != 0)
                {
                    return await this.database.UpdateAsync(shift);
                }
                else
                {
                    return await this.database.InsertAsync(shift);
                }
            },
            new Dictionary<string, object> { { "Shift", shift?.ShiftId }, { "InternshipId", shift?.InternshipId } },
            "Nie udało się zapisać danych dyżuru");
        }

        public async Task<int> DeleteMedicalShiftAsync(MedicalShift shift)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (shift == null)
                {
                    throw new InvalidInputException(
                        "MedicalShift cannot be null",
                        "Dyżur nie może być pusty");
                }

                return await this.database.DeleteAsync(shift);
            },
            new Dictionary<string, object> { { "Shift", shift?.ShiftId } },
            "Nie udało się usunąć dyżuru");
        }

        public async Task<Procedure> GetProcedureAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var procedure = await this.database.Table<Procedure>().FirstOrDefaultAsync(p => p.ProcedureId == id);
                if (procedure == null)
                {
                    throw new ResourceNotFoundException(
                        $"Procedure with ID {id} not found",
                        $"Nie znaleziono procedury o ID {id}");
                }
                return procedure;
            },
            new Dictionary<string, object> { { "ProcedureId", id } },
            $"Nie udało się pobrać procedury o ID {id}");
        }

        public async Task<List<Procedure>> GetProceduresAsync(int? internshipId = null, string searchText = null)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var query = this.database.Table<Procedure>();

                if (internshipId.HasValue)
                {
                    query = query.Where(p => p.InternshipId == internshipId);
                }

                var procedures = await query.ToListAsync();

                if (!string.IsNullOrEmpty(searchText))
                {
                    searchText = searchText.ToLowerInvariant();
                    procedures = procedures.Where(p =>
                        p.Code.ToLowerInvariant().Contains(searchText) ||
                        p.Location.ToLowerInvariant().Contains(searchText) ||
                        (p.PatientInitials != null && p.PatientInitials.ToLowerInvariant().Contains(searchText)) ||
                        (p.ProcedureGroup != null && p.ProcedureGroup.ToLowerInvariant().Contains(searchText))
                    ).ToList();
                }

                return procedures;
            },
            new Dictionary<string, object> { { "InternshipId", internshipId }, { "SearchText", searchText } },
            "Nie udało się pobrać listy procedur");
        }

        public async Task<int> SaveProcedureAsync(Procedure procedure)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (procedure == null)
                {
                    throw new InvalidInputException(
                        "Procedure cannot be null",
                        "Procedura nie może być pusta");
                }

                if (procedure.ProcedureId != 0)
                {
                    return await this.database.UpdateAsync(procedure);
                }
                else
                {
                    return await this.database.InsertAsync(procedure);
                }
            },
            new Dictionary<string, object> { { "Procedure", procedure?.ProcedureId }, { "InternshipId", procedure?.InternshipId } },
            "Nie udało się zapisać danych procedury");
        }

        public async Task<int> DeleteProcedureAsync(Procedure procedure)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (procedure == null)
                {
                    throw new InvalidInputException(
                        "Procedure cannot be null",
                        "Procedura nie może być pusta");
                }

                return await this.database.DeleteAsync(procedure);
            },
            new Dictionary<string, object> { { "Procedure", procedure?.ProcedureId } },
            "Nie udało się usunąć procedury");
        }

        public async Task<Course> GetCourseAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var course = await this.database.Table<Course>().FirstOrDefaultAsync(c => c.CourseId == id);
                if (course == null)
                {
                    throw new ResourceNotFoundException(
                        $"Course with ID {id} not found",
                        $"Nie znaleziono kursu o ID {id}");
                }
                return course;
            },
            new Dictionary<string, object> { { "CourseId", id } },
            $"Nie udało się pobrać kursu o ID {id}");
        }

        public async Task<List<Course>> GetCoursesAsync(int? specializationId = null, int? moduleId = null)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var query = this.database.Table<Course>();

                if (specializationId.HasValue)
                {
                    query = query.Where(c => c.SpecializationId == specializationId);
                }

                if (moduleId.HasValue)
                {
                    query = query.Where(c => c.ModuleId == moduleId);
                }

                return await query.ToListAsync();
            },
            new Dictionary<string, object> { { "SpecializationId", specializationId }, { "ModuleId", moduleId } },
            "Nie udało się pobrać listy kursów");
        }

        public async Task<int> SaveCourseAsync(Course course)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (course == null)
                {
                    throw new InvalidInputException(
                        "Course cannot be null",
                        "Kurs nie może być pusty");
                }

                if (course.CourseId != 0)
                {
                    return await this.database.UpdateAsync(course);
                }
                else
                {
                    return await this.database.InsertAsync(course);
                }
            },
            new Dictionary<string, object> {
                { "Course", course?.CourseId },
                { "SpecializationId", course?.SpecializationId },
                { "ModuleId", course?.ModuleId }
            },
            "Nie udało się zapisać danych kursu");
        }

        public async Task<int> DeleteCourseAsync(Course course)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (course == null)
                {
                    throw new InvalidInputException(
                        "Course cannot be null",
                        "Kurs nie może być pusty");
                }

                return await this.database.DeleteAsync(course);
            },
            new Dictionary<string, object> { { "Course", course?.CourseId } },
            "Nie udało się usunąć kursu");
        }

        public async Task<SelfEducation> GetSelfEducationAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var selfEducation = await this.database.Table<SelfEducation>().FirstOrDefaultAsync(s => s.SelfEducationId == id);
                if (selfEducation == null)
                {
                    throw new ResourceNotFoundException(
                        $"SelfEducation with ID {id} not found",
                        $"Nie znaleziono samokształcenia o ID {id}");
                }
                return selfEducation;
            },
            new Dictionary<string, object> { { "SelfEducationId", id } },
            $"Nie udało się pobrać samokształcenia o ID {id}");
        }

        public async Task<List<SelfEducation>> GetSelfEducationItemsAsync(int? specializationId = null, int? moduleId = null)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var query = this.database.Table<SelfEducation>();

                if (specializationId.HasValue)
                {
                    query = query.Where(s => s.SpecializationId == specializationId);
                }

                if (moduleId.HasValue)
                {
                    query = query.Where(s => s.ModuleId == moduleId);
                }

                return await query.ToListAsync();
            },
            new Dictionary<string, object> { { "SpecializationId", specializationId }, { "ModuleId", moduleId } },
            "Nie udało się pobrać listy samokształceń");
        }

        public async Task<int> SaveSelfEducationAsync(SelfEducation selfEducation)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (selfEducation == null)
                {
                    throw new InvalidInputException(
                        "SelfEducation cannot be null",
                        "Samokształcenie nie może być puste");
                }

                if (selfEducation.SelfEducationId != 0)
                {
                    return await this.database.UpdateAsync(selfEducation);
                }
                else
                {
                    return await this.database.InsertAsync(selfEducation);
                }
            },
            new Dictionary<string, object> {
                { "SelfEducation", selfEducation?.SelfEducationId },
                { "SpecializationId", selfEducation?.SpecializationId },
                { "ModuleId", selfEducation?.ModuleId }
            },
            "Nie udało się zapisać danych samokształcenia");
        }

        public async Task<int> DeleteSelfEducationAsync(SelfEducation selfEducation)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (selfEducation == null)
                {
                    throw new InvalidInputException(
                        "SelfEducation cannot be null",
                        "Samokształcenie nie może być puste");
                }

                return await this.database.DeleteAsync(selfEducation);
            },
            new Dictionary<string, object> { { "SelfEducation", selfEducation?.SelfEducationId } },
            "Nie udało się usunąć samokształcenia");
        }

        public async Task<Publication> GetPublicationAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var publication = await this.database.Table<Publication>().FirstOrDefaultAsync(p => p.PublicationId == id);
                if (publication == null)
                {
                    throw new ResourceNotFoundException(
                        $"Publication with ID {id} not found",
                        $"Nie znaleziono publikacji o ID {id}");
                }
                return publication;
            },
            new Dictionary<string, object> { { "PublicationId", id } },
            $"Nie udało się pobrać publikacji o ID {id}");
        }

        public async Task<List<Publication>> GetPublicationsAsync(int? specializationId = null, int? moduleId = null)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var query = this.database.Table<Publication>();

                if (specializationId.HasValue)
                {
                    query = query.Where(p => p.SpecializationId == specializationId);
                }

                if (moduleId.HasValue)
                {
                    query = query.Where(p => p.ModuleId == moduleId);
                }

                return await query.ToListAsync();
            },
            new Dictionary<string, object> { { "SpecializationId", specializationId }, { "ModuleId", moduleId } },
            "Nie udało się pobrać listy publikacji");
        }

        public async Task<int> SavePublicationAsync(Publication publication)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (publication == null)
                {
                    throw new InvalidInputException(
                        "Publication cannot be null",
                        "Publikacja nie może być pusta");
                }

                if (publication.PublicationId != 0)
                {
                    return await this.database.UpdateAsync(publication);
                }
                else
                {
                    return await this.database.InsertAsync(publication);
                }
            },
            new Dictionary<string, object> {
                { "Publication", publication?.PublicationId },
                { "SpecializationId", publication?.SpecializationId },
                { "ModuleId", publication?.ModuleId }
            },
            "Nie udało się zapisać danych publikacji");
        }

        public async Task<int> DeletePublicationAsync(Publication publication)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (publication == null)
                {
                    throw new InvalidInputException(
                        "Publication cannot be null",
                        "Publikacja nie może być pusta");
                }

                return await this.database.DeleteAsync(publication);
            },
            new Dictionary<string, object> { { "Publication", publication?.PublicationId } },
            "Nie udało się usunąć publikacji");
        }

        public async Task<EducationalActivity> GetEducationalActivityAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var activity = await this.database.Table<EducationalActivity>().FirstOrDefaultAsync(a => a.ActivityId == id);
                if (activity == null)
                {
                    throw new ResourceNotFoundException(
                        $"EducationalActivity with ID {id} not found",
                        $"Nie znaleziono aktywności edukacyjnej o ID {id}");
                }
                return activity;
            },
            new Dictionary<string, object> { { "ActivityId", id } },
            $"Nie udało się pobrać aktywności edukacyjnej o ID {id}");
        }

        public async Task<List<EducationalActivity>> GetEducationalActivitiesAsync(int? specializationId = null, int? moduleId = null)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var query = this.database.Table<EducationalActivity>();

                if (specializationId.HasValue)
                {
                    query = query.Where(a => a.SpecializationId == specializationId);
                }

                if (moduleId.HasValue)
                {
                    query = query.Where(a => a.ModuleId == moduleId);
                }

                return await query.ToListAsync();
            },
            new Dictionary<string, object> { { "SpecializationId", specializationId }, { "ModuleId", moduleId } },
            "Nie udało się pobrać listy aktywności edukacyjnych");
        }

        public async Task<int> SaveEducationalActivityAsync(EducationalActivity activity)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (activity == null)
                {
                    throw new InvalidInputException(
                        "EducationalActivity cannot be null",
                        "Aktywność edukacyjna nie może być pusta");
                }

                if (activity.ActivityId != 0)
                {
                    return await this.database.UpdateAsync(activity);
                }
                else
                {
                    return await this.database.InsertAsync(activity);
                }
            },
            new Dictionary<string, object> {
                { "Activity", activity?.ActivityId },
                { "SpecializationId", activity?.SpecializationId },
                { "ModuleId", activity?.ModuleId }
            },
            "Nie udało się zapisać danych aktywności edukacyjnej");
        }

        public async Task<int> DeleteEducationalActivityAsync(EducationalActivity activity)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (activity == null)
                {
                    throw new InvalidInputException(
                        "EducationalActivity cannot be null",
                        "Aktywność edukacyjna nie może być pusta");
                }

                return await this.database.DeleteAsync(activity);
            },
            new Dictionary<string, object> { { "Activity", activity?.ActivityId } },
            "Nie udało się usunąć aktywności edukacyjnej");
        }

        public async Task<Absence> GetAbsenceAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var absence = await this.database.Table<Absence>().FirstOrDefaultAsync(a => a.AbsenceId == id);
                if (absence == null)
                {
                    throw new ResourceNotFoundException(
                        $"Absence with ID {id} not found",
                        $"Nie znaleziono nieobecności o ID {id}");
                }
                return absence;
            },
            new Dictionary<string, object> { { "AbsenceId", id } },
            $"Nie udało się pobrać nieobecności o ID {id}");
        }

        public async Task<List<Absence>> GetAbsencesAsync(int specializationId)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                return await this.database.Table<Absence>().Where(a => a.SpecializationId == specializationId).ToListAsync();
            },
            new Dictionary<string, object> { { "SpecializationId", specializationId } },
            $"Nie udało się pobrać listy nieobecności dla specjalizacji o ID {specializationId}");
        }

        public async Task<int> SaveAbsenceAsync(Absence absence)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (absence == null)
                {
                    throw new InvalidInputException(
                        "Absence cannot be null",
                        "Nieobecność nie może być pusta");
                }

                if (absence.AbsenceId != 0)
                {
                    return await this.database.UpdateAsync(absence);
                }
                else
                {
                    return await this.database.InsertAsync(absence);
                }
            },
            new Dictionary<string, object> { { "Absence", absence?.AbsenceId }, { "SpecializationId", absence?.SpecializationId } },
            "Nie udało się zapisać danych nieobecności");
        }

        public async Task<int> DeleteAbsenceAsync(Absence absence)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (absence == null)
                {
                    throw new InvalidInputException(
                        "Absence cannot be null",
                        "Nieobecność nie może być pusta");
                }

                return await this.database.DeleteAsync(absence);
            },
            new Dictionary<string, object> { { "Absence", absence?.AbsenceId } },
            "Nie udało się usunąć nieobecności");
        }

        public async Task<Models.Recognition> GetRecognitionAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var recognition = await this.database.Table<Models.Recognition>().FirstOrDefaultAsync(r => r.RecognitionId == id);
                if (recognition == null)
                {
                    throw new ResourceNotFoundException(
                        $"Recognition with ID {id} not found",
                        $"Nie znaleziono uznania o ID {id}");
                }
                return recognition;
            },
            new Dictionary<string, object> { { "RecognitionId", id } },
            $"Nie udało się pobrać uznania o ID {id}");
        }

        public async Task<List<Models.Recognition>> GetRecognitionsAsync(int specializationId)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                return await this.database.Table<Models.Recognition>().Where(r => r.SpecializationId == specializationId).ToListAsync();
            },
            new Dictionary<string, object> { { "SpecializationId", specializationId } },
            $"Nie udało się pobrać listy uznań dla specjalizacji o ID {specializationId}");
        }

        public async Task<int> SaveRecognitionAsync(Models.Recognition recognition)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (recognition == null)
                {
                    throw new InvalidInputException(
                        "Recognition cannot be null",
                        "Uznanie nie może być puste");
                }

                if (recognition.RecognitionId != 0)
                {
                    return await this.database.UpdateAsync(recognition);
                }
                else
                {
                    return await this.database.InsertAsync(recognition);
                }
            },
            new Dictionary<string, object> { { "Recognition", recognition?.RecognitionId }, { "SpecializationId", recognition?.SpecializationId } },
            "Nie udało się zapisać danych uznania");
        }

        public async Task<int> DeleteRecognitionAsync(Models.Recognition recognition)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (recognition == null)
                {
                    throw new InvalidInputException(
                        "Recognition cannot be null",
                        "Uznanie nie może być puste");
                }

                return await this.database.DeleteAsync(recognition);
            },
            new Dictionary<string, object> { { "Recognition", recognition?.RecognitionId } },
            "Nie udało się usunąć uznania");
        }

        public async Task<SpecializationProgram> GetSpecializationProgramAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var program = await this.database.Table<SpecializationProgram>().FirstOrDefaultAsync(p => p.ProgramId == id);
                if (program == null)
                {
                    throw new ResourceNotFoundException(
                        $"SpecializationProgram with ID {id} not found",
                        $"Nie znaleziono programu specjalizacji o ID {id}");
                }
                return program;
            },
            new Dictionary<string, object> { { "ProgramId", id } },
            $"Nie udało się pobrać programu specjalizacji o ID {id}");
        }

        public async Task<SpecializationProgram> GetSpecializationProgramByCodeAsync(string code, SmkVersion smkVersion)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (string.IsNullOrEmpty(code))
                {
                    throw new InvalidInputException(
                        "Code cannot be null or empty",
                        "Kod nie może być pusty");
                }

                return await this.database.Table<SpecializationProgram>()
                    .FirstOrDefaultAsync(p => p.Code == code && p.SmkVersion == smkVersion);
            },
            new Dictionary<string, object> { { "Code", code }, { "SmkVersion", smkVersion } },
            $"Nie udało się pobrać programu specjalizacji o kodzie {code}");
        }

        public async Task<List<SpecializationProgram>> GetAllSpecializationProgramsAsync()
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                return await this.database.Table<SpecializationProgram>().ToListAsync();
            }, null, "Nie udało się pobrać listy programów specjalizacji");
        }

        public async Task<int> SaveSpecializationProgramAsync(SpecializationProgram program)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (program == null)
                {
                    throw new InvalidInputException(
                        "SpecializationProgram cannot be null",
                        "Program specjalizacji nie może być pusty");
                }

                if (program.ProgramId != 0)
                {
                    return await this.database.UpdateAsync(program);
                }
                else
                {
                    return await this.database.InsertAsync(program);
                }
            },
            new Dictionary<string, object> { { "Program", program?.ProgramId } },
            "Nie udało się zapisać danych programu specjalizacji");
        }

        public async Task MigrateShiftDataForModulesAsync()
        {
            await this.InitializeAsync();
            await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                bool columnExists = false;

                try
                {
                    var testQuery = "SELECT ModuleId FROM RealizedMedicalShiftNewSMK LIMIT 1";
                    await this.database.ExecuteScalarAsync<int>(testQuery);
                    columnExists = true;
                }
                catch
                {
                    await this.database.ExecuteAsync("ALTER TABLE RealizedMedicalShiftNewSMK ADD COLUMN ModuleId INTEGER");
                }

                var query = "SELECT * FROM RealizedMedicalShiftNewSMK WHERE ModuleId IS NULL OR ModuleId = 0";
                var shiftsToUpdate = await this.QueryAsync<RealizedMedicalShiftNewSMK>(query);

                if (shiftsToUpdate.Count == 0)
                {
                    return;
                }

                var specializations = await this.GetAllSpecializationsAsync();

                foreach (var specializationId in specializations.Select(x => x.SpecializationId))
                {
                    var modules = await this.GetModulesAsync(specializationId);
                    if (modules.Count == 0) continue;
                    var specializationShifts = shiftsToUpdate.Where(s => s.SpecializationId == specializationId).ToList();
                    if (specializationShifts.Count == 0) continue;

                    foreach (var shift in specializationShifts)
                    {
                        Module appropriateModule = null;
                        foreach (var module in modules)
                        {
                            if (string.IsNullOrEmpty(module.Structure)) continue;

                            var options = new System.Text.Json.JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true,
                                AllowTrailingCommas = true,
                                ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip
                            };

                            var moduleStructure = System.Text.Json.JsonSerializer.Deserialize<ModuleStructure>(module.Structure, options);
                            if (moduleStructure?.Internships != null &&
                                moduleStructure.Internships.Any(i => i.Id == shift.InternshipRequirementId))
                            {
                                appropriateModule = module;
                                break;
                            }
                        }

                        if (appropriateModule == null && modules.Count > 0)
                        {
                            appropriateModule = modules[0];
                        }

                        if (appropriateModule != null)
                        {
                            shift.ModuleId = appropriateModule.ModuleId;
                            await this.UpdateAsync(shift);
                        }
                    }
                }
            },
            null, "Nie udało się zmigrować danych dyżurów do modułów", 2, 2000);
        }

        public async Task<RealizedInternshipNewSMK> GetRealizedInternshipNewSMKAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var internship = await this.database.Table<RealizedInternshipNewSMK>().FirstOrDefaultAsync(i => i.RealizedInternshipId == id);
                if (internship == null)
                {
                    throw new ResourceNotFoundException(
                        $"RealizedInternshipNewSMK with ID {id} not found",
                        $"Nie znaleziono zrealizowanego stażu w nowym SMK o ID {id}");
                }
                return internship;
            },
            new Dictionary<string, object> { { "RealizedInternshipId", id } },
            $"Nie udało się pobrać zrealizowanego stażu w nowym SMK o ID {id}");
        }

        public async Task<List<RealizedInternshipNewSMK>> GetRealizedInternshipsNewSMKAsync(int? specializationId = null, int? moduleId = null, int? internshipRequirementId = null)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var query = this.database.Table<RealizedInternshipNewSMK>();

                if (specializationId.HasValue)
                {
                    query = query.Where(i => i.SpecializationId == specializationId);
                }

                if (moduleId.HasValue)
                {
                    query = query.Where(i => i.ModuleId == moduleId);
                }

                if (internshipRequirementId.HasValue)
                {
                    query = query.Where(i => i.InternshipRequirementId == internshipRequirementId);
                }

                return await query.ToListAsync();
            },
            new Dictionary<string, object> {
                { "SpecializationId", specializationId },
                { "ModuleId", moduleId },
                { "InternshipRequirementId", internshipRequirementId }
            },
            "Nie udało się pobrać listy zrealizowanych staży w nowym SMK");
        }

        public async Task<int> SaveRealizedInternshipNewSMKAsync(RealizedInternshipNewSMK internship)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "RealizedInternshipNewSMK cannot be null",
                        "Zrealizowany staż w nowym SMK nie może być pusty");
                }

                if (internship.RealizedInternshipId != 0)
                {
                    return await this.database.UpdateAsync(internship);
                }
                else
                {
                    return await this.database.InsertAsync(internship);
                }
            },
            new Dictionary<string, object> {
                { "RealizedInternship", internship?.RealizedInternshipId },
                { "SpecializationId", internship?.SpecializationId },
                { "ModuleId", internship?.ModuleId },
                { "InternshipRequirementId", internship?.InternshipRequirementId }
            },
            "Nie udało się zapisać danych zrealizowanego stażu w nowym SMK");
        }

        public async Task<int> DeleteRealizedInternshipNewSMKAsync(RealizedInternshipNewSMK internship)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "RealizedInternshipNewSMK cannot be null",
                        "Zrealizowany staż w nowym SMK nie może być pusty");
                }

                return await this.database.DeleteAsync(internship);
            },
            new Dictionary<string, object> { { "RealizedInternship", internship?.RealizedInternshipId } },
            "Nie udało się usunąć zrealizowanego stażu w nowym SMK");
        }

        public async Task<RealizedInternshipOldSMK> GetRealizedInternshipOldSMKAsync(int id)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var internship = await this.database.Table<RealizedInternshipOldSMK>().FirstOrDefaultAsync(i => i.RealizedInternshipId == id);
                if (internship == null)
                {
                    throw new ResourceNotFoundException(
                        $"RealizedInternshipOldSMK with ID {id} not found",
                        $"Nie znaleziono zrealizowanego stażu w starym SMK o ID {id}");
                }
                return internship;
            },
            new Dictionary<string, object> { { "RealizedInternshipId", id } },
            $"Nie udało się pobrać zrealizowanego stażu w starym SMK o ID {id}");
        }

        public async Task<List<RealizedInternshipOldSMK>> GetRealizedInternshipsOldSMKAsync(int? specializationId = null, int? year = null)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var query = this.database.Table<RealizedInternshipOldSMK>();

                if (specializationId.HasValue)
                {
                    query = query.Where(i => i.SpecializationId == specializationId);
                }

                if (year.HasValue)
                {
                    query = query.Where(i => i.Year == year);
                }

                return await query.ToListAsync();
            },
            new Dictionary<string, object> { { "SpecializationId", specializationId }, { "Year", year } },
            "Nie udało się pobrać listy zrealizowanych staży w starym SMK");
        }

        public async Task<int> SaveRealizedInternshipOldSMKAsync(RealizedInternshipOldSMK internship)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "RealizedInternshipOldSMK cannot be null",
                        "Zrealizowany staż w starym SMK nie może być pusty");
                }

                if (internship.RealizedInternshipId != 0)
                {
                    return await this.database.UpdateAsync(internship);
                }
                else
                {
                    return await this.database.InsertAsync(internship);
                }
            },
            new Dictionary<string, object> {
                { "RealizedInternship", internship?.RealizedInternshipId },
                { "SpecializationId", internship?.SpecializationId },
                { "Year", internship?.Year }
            },
            "Nie udało się zapisać danych zrealizowanego stażu w starym SMK");
        }

        public async Task<int> DeleteRealizedInternshipOldSMKAsync(RealizedInternshipOldSMK internship)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "RealizedInternshipOldSMK cannot be null",
                        "Zrealizowany staż w starym SMK nie może być pusty");
                }

                return await this.database.DeleteAsync(internship);
            },
            new Dictionary<string, object> { { "RealizedInternship", internship?.RealizedInternshipId } },
            "Nie udało się usunąć zrealizowanego stażu w starym SMK");
        }

        public async Task MigrateInternshipDataAsync()
        {
            await this.InitializeAsync();
            await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
            // Sprawdzenie istnienia kolumny w tabeli
            bool internshipRequirementIdExists = false;
            bool moduleIdExists = false;

            try
            {
                var testQuery = "SELECT InternshipRequirementId FROM RealizedInternshipNewSMK LIMIT 1";
                await this.database.ExecuteScalarAsync<int>(testQuery);
                internshipRequirementIdExists = true;
            }
            catch
            {
                await this.database.ExecuteAsync("ALTER TABLE RealizedInternshipNewSMK ADD COLUMN InternshipRequirementId INTEGER");
            }

            try
            {
                var testQuery = "SELECT ModuleId FROM RealizedInternshipNewSMK LIMIT 1";
                await this.database.ExecuteScalarAsync<int>(testQuery);
                moduleIdExists = true;
            }
            catch
            {
                await this.database.ExecuteAsync("ALTER TABLE RealizedInternshipNewSMK ADD COLUMN ModuleId INTEGER");
            }

            // Sprawdź i napraw istniejące realizacje z null InternshipName
            try
            {
                var realizationsWithNullNames = await this.database.Table<RealizedInternshipOldSMK>()
                    .Where(r => r.InternshipName == null)
                    .ToListAsync();

                foreach (var realization in realizationsWithNullNames)
                {
                    _loggingService.LogInformation($"Znaleziono realizację z pustą nazwą stażu, ID: {realization.RealizedInternshipId}");
                    // Próba naprawy - szukamy w oryginalnych stażach
                    var originalInternship = await this.database.Table<Internship>()
                        .FirstOrDefaultAsync(i => i.SpecializationId == realization.SpecializationId &&
                                                  i.DaysCount == realization.DaysCount);

                    if (originalInternship != null && !string.IsNullOrEmpty(originalInternship.InternshipName))
                    {
                        realization.InternshipName = originalInternship.InternshipName;
                        await this.database.UpdateAsync(realization);
                        _loggingService.LogInformation($"Naprawiono nazwę stażu: {realization.InternshipName}");
                    }
                    else
                    {
                        // Jeśli nie udało się znaleźć odpowiedniego stażu, użyj wartości domyślnej
                        realization.InternshipName = "Staż bez nazwy";
                        await this.database.UpdateAsync(realization);
                        _loggingService.LogInformation($"Ustawiono domyślną nazwę stażu dla ID: {realization.RealizedInternshipId}");
                    }
                }
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, $"Błąd podczas naprawy realizacji: {ex.Message}", new Dictionary<string, object> { { "ExceptionDetails", ex } });
            }

            // Pobierz aktualną wersję SMK użytkownika
            var userId = await Helpers.SettingsHelper.GetCurrentUserIdAsync();
            var user = await this.GetUserAsync(userId);

            if (user == null)
            {
                return;
            }

            // Pobranie wszystkich istniejących staży
            var internships = await this.database.Table<Internship>().Where(i => i.InternshipId > 0).ToListAsync();

            // Sprawdź, czy już istnieją realizacje dla tych staży
            var existingNewSMK = await this.database.Table<RealizedInternshipNewSMK>().ToListAsync();
            var existingOldSMK = await this.database.Table<RealizedInternshipOldSMK>().ToListAsync();

            // Jeśli realizacje już istnieją, pomijamy migrację
            if ((user.SmkVersion == SmkVersion.New && existingNewSMK.Count > 0) ||
                (user.SmkVersion == SmkVersion.Old && existingOldSMK.Count > 0))
            {
                return;
            }

            // Migracja danych
            foreach (var internship in internships)
            {
                if (user.SmkVersion == SmkVersion.New)
                {
                    // Ignorujemy staże z ID < 0 (to są wymagania stażowe, nie realizacje)
                    if (internship.InternshipId < 0)
                    {
                        continue;
                    }

                    var existingInternship = existingNewSMK
                        .FirstOrDefault(i => i.SpecializationId == internship.SpecializationId &&
                                           i.InternshipName == internship.InternshipName);

                    if (existingInternship != null)
                    {
                        continue;
                    }

                    var realizedInternship = new RealizedInternshipNewSMK
                    {
                        SpecializationId = internship.SpecializationId,
                        ModuleId = internship.ModuleId,
                        InternshipRequirementId = internship.InternshipId, // Ustawienie ID wymagania
                        InternshipName = internship.InternshipName ?? "Staż bez nazwy", // Upewniamy się, że nazwa nie jest null
                        InstitutionName = internship.InstitutionName,
                        DepartmentName = internship.DepartmentName,
                        StartDate = internship.StartDate,
                        EndDate = internship.EndDate,
                        DaysCount = internship.DaysCount,
                        IsCompleted = internship.IsCompleted,
                        IsApproved = internship.IsApproved,
                        IsRecognition = internship.IsRecognition,
                        RecognitionReason = internship.RecognitionReason,
                        RecognitionDaysReduction = internship.RecognitionDaysReduction,
                        IsPartialRealization = internship.IsPartialRealization,
                        SupervisorName = internship.SupervisorName,
                        SyncStatus = internship.SyncStatus,
                        AdditionalFields = internship.AdditionalFields
                    };

                        await this.database.InsertAsync(realizedInternship);
                    }
                    else // Stary SMK
                    {
                        // Ignorujemy staże z ID < 0 (to są wymagania stażowe, nie realizacje)
                        if (internship.InternshipId < 0)
                        {
                            continue;
                        }

                        var existingInternship = existingOldSMK
                            .FirstOrDefault(i => i.SpecializationId == internship.SpecializationId &&
                                               i.InternshipName == internship.InternshipName &&
                                               i.Year == internship.Year);

                        if (existingInternship != null)
                        {
                            continue;
                        }

                        var realizedInternship = new RealizedInternshipOldSMK
                        {
                            SpecializationId = internship.SpecializationId,
                            InternshipName = internship.InternshipName ?? "Staż bez nazwy", // Upewniamy się, że nazwa nie jest null
                            InstitutionName = internship.InstitutionName,
                            DepartmentName = internship.DepartmentName,
                            StartDate = internship.StartDate,
                            EndDate = internship.EndDate,
                            DaysCount = internship.DaysCount,
                            IsCompleted = internship.IsCompleted,
                            IsApproved = internship.IsApproved,
                            Year = internship.Year,
                            RequiresApproval = false, // Domyślna wartość
                            SupervisorName = internship.SupervisorName,
                            SyncStatus = internship.SyncStatus,
                            AdditionalFields = internship.AdditionalFields
                        };

                        await this.database.InsertAsync(realizedInternship);
                    }
                }
            },
            null, "Nie udało się zmigrować danych stażowych", 2, 2000);
        }

        public async Task<bool> TableExists(string tableName)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (string.IsNullOrEmpty(tableName))
                {
                    throw new InvalidInputException(
                        "Table name cannot be null or empty",
                        "Nazwa tabeli nie może być pusta");
                }

                var result = await this.database.ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=?", tableName);
                return result > 0;
            },
            new Dictionary<string, object> { { "TableName", tableName } },
            $"Nie udało się sprawdzić istnienia tabeli {tableName}");
        }

        public async Task<List<string>> ListTables()
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                var tableInfos = await this.database.QueryAsync<TableInfo>(
                    "SELECT name FROM sqlite_master WHERE type='table'");
                return tableInfos.Select(t => t.name).ToList();
            },
            null, "Nie udało się pobrać listy tabel");
        }

        public async Task DropTableIfExists(string tableName)
        {
            await this.InitializeAsync();
            await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (string.IsNullOrEmpty(tableName))
                {
                    throw new InvalidInputException(
                        "Table name cannot be null or empty",
                        "Nazwa tabeli nie może być pusta");
                }

                await this.database.ExecuteAsync($"DROP TABLE IF EXISTS {tableName}");
            },
            new Dictionary<string, object> { { "TableName", tableName } },
            $"Nie udało się usunąć tabeli {tableName}");
        }

        public async Task<int> GetTableRowCount(string tableName)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (string.IsNullOrEmpty(tableName))
                {
                    throw new InvalidInputException(
                        "Table name cannot be null or empty",
                        "Nazwa tabeli nie może być pusta");
                }

                try
                {
                    var result = await this.database.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM {tableName}");
                    return result;
                }
                catch
                {
                    return -1; // Tabela nie istnieje lub inny błąd
                }
            },
            new Dictionary<string, object> { { "TableName", tableName } },
            $"Nie udało się pobrać liczby wierszy w tabeli {tableName}");
        }

        public async Task FixRealizedInternshipNames()
        {
            await this.InitializeAsync();
            await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                // Pobierz wszystkie realizacje bez poprawnej nazwy
                var realizationsToFix = await this.database.Table<RealizedInternshipOldSMK>()
                    .Where(r => r.InternshipName == "Staż bez nazwy" || r.InternshipName == null)
                    .ToListAsync();

                _loggingService.LogInformation($"Znaleziono {realizationsToFix.Count} realizacji do naprawy");

                if (realizationsToFix.Count == 0)
                {
                    return;
                }

                // Pobierz wszystkie staże
                var internships = await this.database.Table<Internship>()
                    .Where(i => i.InternshipId < 0) // Tylko wymagania stażowe mają ID < 0
                    .ToListAsync();

                if (internships.Count == 0)
                {
                    _loggingService.LogWarning("Nie znaleziono wymagań stażowych do naprawy nazw");
                    return;
                }

                // Przypisz pierwszy staż do wszystkich realizacji bez nazwy
                // To rozwiązanie tymczasowe, aby pokazać dane
                var firstInternship = internships.FirstOrDefault();
                if (firstInternship != null)
                {
                    foreach (var realization in realizationsToFix)
                    {
                        realization.InternshipName = firstInternship.InternshipName;
                        await this.database.UpdateAsync(realization);
                    }

                    _loggingService.LogInformation($"Naprawiono nazwy realizacji, przypisując '{firstInternship.InternshipName}'");
                }
            },
            null, "Nie udało się naprawić nazw zrealizowanych staży", 2, 1500);
        }

        public async Task<List<T>> QueryAsync<T>(string query, params object[] args) where T : new()
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (string.IsNullOrEmpty(query))
                {
                    throw new InvalidInputException(
                        "Query cannot be null or empty",
                        "Zapytanie nie może być puste");
                }

                return await this.database.QueryAsync<T>(query, args);
            },
            new Dictionary<string, object> { { "Query", query } },
            "Nie udało się wykonać zapytania do bazy danych");
        }

        public async Task<int> ExecuteAsync(string query, params object[] args)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (string.IsNullOrEmpty(query))
                {
                    throw new InvalidInputException(
                        "Query cannot be null or empty",
                        "Zapytanie nie może być puste");
                }

                return await this.database.ExecuteAsync(query, args);
            },
            new Dictionary<string, object> { { "Query", query } },
            "Nie udało się wykonać polecenia w bazie danych");
        }

        public async Task<int> UpdateAsync<T>(T item)
        {
            await this.InitializeAsync();
            return await _exceptionHandler.ExecuteWithRetryAsync(async () =>
            {
                if (item == null)
                {
                    throw new InvalidInputException(
                        "Item cannot be null",
                        "Element nie może być pusty");
                }

                return await this.database.UpdateAsync(item);
            },
            new Dictionary<string, object> { { "ItemType", typeof(T).Name } },
            "Nie udało się zaktualizować danych w bazie");
        }

        public void ClearCache()
        {
            _specializationCache.Clear();
            _moduleCache.Clear();
            _loggingService.LogInformation("Cache wyczyszczony");
        }

        private class TableInfo
        {
            public string name { get; set; }
        }
    }
}

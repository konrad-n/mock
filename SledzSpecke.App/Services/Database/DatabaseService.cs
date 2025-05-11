using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Exceptions;
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

        public DatabaseService(IExceptionHandlerService exceptionHandler = null)
        {
            _exceptionHandler = exceptionHandler;
        }

        private async Task<T> SafeExecuteAsync<T>(Func<Task<T>> operation, string errorMessage)
        {
            if (_exceptionHandler != null)
            {
                return await _exceptionHandler.ExecuteAsync(operation, null, errorMessage);
            }
            else
            {
                try
                {
                    return await operation();
                }
                catch (SQLiteException ex)
                {
                    Debug.WriteLine($"SQLite Exception: {ex.Message}");
                    throw new DatabaseException(ex.Message, errorMessage, ex);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Database Exception: {ex.Message}");
                    throw new DatabaseException(ex.Message, errorMessage, ex);
                }
            }
        }

        private async Task SafeExecuteAsync(Func<Task> operation, string errorMessage)
        {
            if (_exceptionHandler != null)
            {
                await _exceptionHandler.ExecuteAsync(operation, null, errorMessage);
            }
            else
            {
                try
                {
                    await operation();
                }
                catch (SQLiteException ex)
                {
                    Debug.WriteLine($"SQLite Exception: {ex.Message}");
                    throw new DatabaseException(ex.Message, errorMessage, ex);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Database Exception: {ex.Message}");
                    throw new DatabaseException(ex.Message, errorMessage, ex);
                }
            }
        }

        public async Task InitializeAsync()
        {
            if (this.isInitialized)
            {
                return;
            }

            await SafeExecuteAsync(async () =>
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
            }, "Nie udało się zainicjalizować bazy danych.");
        }

        public async Task<User> GetUserAsync(int id)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                var user = await this.database.Table<User>().FirstOrDefaultAsync(u => u.UserId == id);
                if (user == null)
                {
                    throw new ResourceNotFoundException(
                        $"User with ID {id} not found",
                        $"Nie znaleziono użytkownika o ID {id}");
                }
                return user;
            }, $"Nie udało się pobrać użytkownika o ID {id}");
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                if (string.IsNullOrEmpty(username))
                {
                    throw new InvalidInputException(
                        "Username cannot be null or empty",
                        "Nazwa użytkownika nie może być pusta");
                }

                return await this.database.Table<User>().FirstOrDefaultAsync(u => u.Username == username);
            }, $"Nie udało się pobrać użytkownika o nazwie {username}");
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                return await this.database.Table<User>().ToListAsync();
            }, "Nie udało się pobrać listy użytkowników");
        }

        public async Task<int> SaveUserAsync(User user)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
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
            }, "Nie udało się zapisać danych użytkownika");
        }

        public async Task<Models.Specialization> GetSpecializationAsync(int id)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
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
            }, $"Nie udało się pobrać specjalizacji o ID {id}");
        }

        public async Task<Models.Specialization> GetSpecializationWithModulesAsync(int id)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                var specialization = await GetSpecializationAsync(id);
                if (specialization != null)
                {
                    specialization.Modules = await GetModulesAsync(specialization.SpecializationId);
                }
                return specialization;
            }, $"Nie udało się pobrać specjalizacji z modułami o ID {id}");
        }

        public async Task<int> SaveSpecializationAsync(Models.Specialization specialization)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
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
            }, "Nie udało się zapisać danych specjalizacji");
        }

        public async Task<List<Models.Specialization>> GetAllSpecializationsAsync()
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                return await this.database.Table<Models.Specialization>().ToListAsync();
            }, "Nie udało się pobrać listy specjalizacji");
        }

        public async Task<int> UpdateSpecializationAsync(Models.Specialization specialization)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
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
            }, "Nie udało się zaktualizować danych specjalizacji");
        }

        public async Task CleanupSpecializationDataAsync(int specializationId)
        {
            await this.InitializeAsync();
            await SafeExecuteAsync(async () =>
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
            }, $"Nie udało się wyczyścić danych specjalizacji o ID {specializationId}");
        }

        public async Task<Module> GetModuleAsync(int id)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                var module = await this.database.Table<Module>().FirstOrDefaultAsync(m => m.ModuleId == id);
                if (module == null)
                {
                    throw new ResourceNotFoundException(
                        $"Module with ID {id} not found",
                        $"Nie znaleziono modułu o ID {id}");
                }
                return module;
            }, $"Nie udało się pobrać modułu o ID {id}");
        }

        public async Task<List<Module>> GetModulesAsync(int specializationId)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
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
            }, $"Nie udało się pobrać listy modułów dla specjalizacji o ID {specializationId}");
        }

        public async Task<int> SaveModuleAsync(Module module)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
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
            }, "Nie udało się zapisać danych modułu");
        }

        public async Task<int> UpdateModuleAsync(Module module)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
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
            }, "Nie udało się zaktualizować danych modułu");
        }

        public async Task<int> DeleteModuleAsync(Module module)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
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
            }, "Nie udało się usunąć modułu");
        }

        public async Task<Internship> GetInternshipAsync(int id)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                var internship = await this.database.Table<Internship>().FirstOrDefaultAsync(i => i.InternshipId == id);
                if (internship == null)
                {
                    throw new ResourceNotFoundException(
                        $"Internship with ID {id} not found",
                        $"Nie znaleziono stażu o ID {id}");
                }
                return internship;
            }, $"Nie udało się pobrać stażu o ID {id}");
        }

        public async Task<List<Internship>> GetInternshipsAsync(int? specializationId = null, int? moduleId = null)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
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
            }, "Nie udało się pobrać listy staży");
        }

        public async Task<int> SaveInternshipAsync(Internship internship)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
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
            }, "Nie udało się zapisać danych stażu");
        }

        public async Task<int> DeleteInternshipAsync(Internship internship)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "Internship cannot be null",
                        "Staż nie może być pusty");
                }

                return await this.database.DeleteAsync(internship);
            }, "Nie udało się usunąć stażu");
        }

        public async Task<MedicalShift> GetMedicalShiftAsync(int id)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                var shift = await this.database.Table<MedicalShift>().FirstOrDefaultAsync(s => s.ShiftId == id);
                if (shift == null)
                {
                    throw new ResourceNotFoundException(
                        $"MedicalShift with ID {id} not found",
                        $"Nie znaleziono dyżuru o ID {id}");
                }
                return shift;
            }, $"Nie udało się pobrać dyżuru o ID {id}");
        }

        public async Task<List<MedicalShift>> GetMedicalShiftsAsync(int? internshipId = null)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                var query = this.database.Table<MedicalShift>();

                if (internshipId.HasValue)
                {
                    query = query.Where(s => s.InternshipId == internshipId);
                }

                return await query.ToListAsync();
            }, "Nie udało się pobrać listy dyżurów");
        }

        public async Task<int> SaveMedicalShiftAsync(MedicalShift shift)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
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
            }, "Nie udało się zapisać danych dyżuru");
        }

        public async Task<int> DeleteMedicalShiftAsync(MedicalShift shift)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                if (shift == null)
                {
                    throw new InvalidInputException(
                        "MedicalShift cannot be null",
                        "Dyżur nie może być pusty");
                }

                return await this.database.DeleteAsync(shift);
            }, "Nie udało się usunąć dyżuru");
        }

        public async Task<Procedure> GetProcedureAsync(int id)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                var procedure = await this.database.Table<Procedure>().FirstOrDefaultAsync(p => p.ProcedureId == id);
                if (procedure == null)
                {
                    throw new ResourceNotFoundException(
                        $"Procedure with ID {id} not found",
                        $"Nie znaleziono procedury o ID {id}");
                }
                return procedure;
            }, $"Nie udało się pobrać procedury o ID {id}");
        }

        public async Task<List<Procedure>> GetProceduresAsync(int? internshipId = null, string searchText = null)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
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
            }, "Nie udało się pobrać listy procedur");
        }

        public async Task<int> SaveProcedureAsync(Procedure procedure)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
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
            }, "Nie udało się zapisać danych procedury");
        }

        public async Task<int> DeleteProcedureAsync(Procedure procedure)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                if (procedure == null)
                {
                    throw new InvalidInputException(
                        "Procedure cannot be null",
                        "Procedura nie może być pusta");
                }

                return await this.database.DeleteAsync(procedure);
            }, "Nie udało się usunąć procedury");
        }

        public async Task<Course> GetCourseAsync(int id)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                var course = await this.database.Table<Course>().FirstOrDefaultAsync(c => c.CourseId == id);
                if (course == null)
                {
                    throw new ResourceNotFoundException(
                        $"Course with ID {id} not found",
                        $"Nie znaleziono kursu o ID {id}");
                }
                return course;
            }, $"Nie udało się pobrać kursu o ID {id}");
        }

        public async Task<List<Course>> GetCoursesAsync(int? specializationId = null, int? moduleId = null)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
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
            }, "Nie udało się pobrać listy kursów");
        }

        public async Task<int> SaveCourseAsync(Course course)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
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
            }, "Nie udało się zapisać danych kursu");
        }

        public async Task<int> DeleteCourseAsync(Course course)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                if (course == null)
                {
                    throw new InvalidInputException(
                        "Course cannot be null",
                        "Kurs nie może być pusty");
                }

                return await this.database.DeleteAsync(course);
            }, "Nie udało się usunąć kursu");
        }

        public async Task<SelfEducation> GetSelfEducationAsync(int id)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                var selfEducation = await this.database.Table<SelfEducation>().FirstOrDefaultAsync(s => s.SelfEducationId == id);
                if (selfEducation == null)
                {
                    throw new ResourceNotFoundException(
                        $"SelfEducation with ID {id} not found",
                        $"Nie znaleziono samokształcenia o ID {id}");
                }
                return selfEducation;
            }, $"Nie udało się pobrać samokształcenia o ID {id}");
        }

        public async Task<List<SelfEducation>> GetSelfEducationItemsAsync(int? specializationId = null, int? moduleId = null)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
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
            }, "Nie udało się pobrać listy samokształceń");
        }

        public async Task<int> SaveSelfEducationAsync(SelfEducation selfEducation)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
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
            }, "Nie udało się zapisać danych samokształcenia");
        }

        public async Task<int> DeleteSelfEducationAsync(SelfEducation selfEducation)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                if (selfEducation == null)
                {
                    throw new InvalidInputException(
                        "SelfEducation cannot be null",
                        "Samokształcenie nie może być puste");
                }

                return await this.database.DeleteAsync(selfEducation);
            }, "Nie udało się usunąć samokształcenia");
        }

        public async Task<Publication> GetPublicationAsync(int id)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                var publication = await this.database.Table<Publication>().FirstOrDefaultAsync(p => p.PublicationId == id);
                if (publication == null)
                {
                    throw new ResourceNotFoundException(
                        $"Publication with ID {id} not found",
                        $"Nie znaleziono publikacji o ID {id}");
                }
                return publication;
            }, $"Nie udało się pobrać publikacji o ID {id}");
        }

        public async Task<List<Publication>> GetPublicationsAsync(int? specializationId = null, int? moduleId = null)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
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
            }, "Nie udało się pobrać listy publikacji");
        }

        public async Task<int> SavePublicationAsync(Publication publication)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
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
            }, "Nie udało się zapisać danych publikacji");
        }

        public async Task<int> DeletePublicationAsync(Publication publication)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                if (publication == null)
                {
                    throw new InvalidInputException(
                        "Publication cannot be null",
                        "Publikacja nie może być pusta");
                }

                return await this.database.DeleteAsync(publication);
            }, "Nie udało się usunąć publikacji");
        }

        public async Task<EducationalActivity> GetEducationalActivityAsync(int id)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                var activity = await this.database.Table<EducationalActivity>().FirstOrDefaultAsync(a => a.ActivityId == id);
                if (activity == null)
                {
                    throw new ResourceNotFoundException(
                        $"EducationalActivity with ID {id} not found",
                        $"Nie znaleziono aktywności edukacyjnej o ID {id}");
                }
                return activity;
            }, $"Nie udało się pobrać aktywności edukacyjnej o ID {id}");
        }

        public async Task<List<EducationalActivity>> GetEducationalActivitiesAsync(int? specializationId = null, int? moduleId = null)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
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
            }, "Nie udało się pobrać listy aktywności edukacyjnych");
        }

        public async Task<int> SaveEducationalActivityAsync(EducationalActivity activity)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
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
            }, "Nie udało się zapisać danych aktywności edukacyjnej");
        }

        public async Task<int> DeleteEducationalActivityAsync(EducationalActivity activity)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                if (activity == null)
                {
                    throw new InvalidInputException(
                        "EducationalActivity cannot be null",
                        "Aktywność edukacyjna nie może być pusta");
                }

                return await this.database.DeleteAsync(activity);
            }, "Nie udało się usunąć aktywności edukacyjnej");
        }

        public async Task<Absence> GetAbsenceAsync(int id)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                var absence = await this.database.Table<Absence>().FirstOrDefaultAsync(a => a.AbsenceId == id);
                if (absence == null)
                {
                    throw new ResourceNotFoundException(
                        $"Absence with ID {id} not found",
                        $"Nie znaleziono nieobecności o ID {id}");
                }
                return absence;
            }, $"Nie udało się pobrać nieobecności o ID {id}");
        }

        public async Task<List<Absence>> GetAbsencesAsync(int specializationId)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                return await this.database.Table<Absence>().Where(a => a.SpecializationId == specializationId).ToListAsync();
            }, $"Nie udało się pobrać listy nieobecności dla specjalizacji o ID {specializationId}");
        }

        public async Task<int> SaveAbsenceAsync(Absence absence)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
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
            }, "Nie udało się zapisać danych nieobecności");
        }

        public async Task<int> DeleteAbsenceAsync(Absence absence)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                if (absence == null)
                {
                    throw new InvalidInputException(
                        "Absence cannot be null",
                        "Nieobecność nie może być pusta");
                }

                return await this.database.DeleteAsync(absence);
            }, "Nie udało się usunąć nieobecności");
        }

        public async Task<Models.Recognition> GetRecognitionAsync(int id)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                var recognition = await this.database.Table<Models.Recognition>().FirstOrDefaultAsync(r => r.RecognitionId == id);
                if (recognition == null)
                {
                    throw new ResourceNotFoundException(
                        $"Recognition with ID {id} not found",
                        $"Nie znaleziono uznania o ID {id}");
                }
                return recognition;
            }, $"Nie udało się pobrać uznania o ID {id}");
        }

        public async Task<List<Models.Recognition>> GetRecognitionsAsync(int specializationId)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                return await this.database.Table<Models.Recognition>().Where(r => r.SpecializationId == specializationId).ToListAsync();
            }, $"Nie udało się pobrać listy uznań dla specjalizacji o ID {specializationId}");
        }

        public async Task<int> SaveRecognitionAsync(Models.Recognition recognition)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
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
            }, "Nie udało się zapisać danych uznania");
        }

        public async Task<int> DeleteRecognitionAsync(Models.Recognition recognition)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                if (recognition == null)
                {
                    throw new InvalidInputException(
                        "Recognition cannot be null",
                        "Uznanie nie może być puste");
                }

                return await this.database.DeleteAsync(recognition);
            }, "Nie udało się usunąć uznania");
        }

        public async Task<SpecializationProgram> GetSpecializationProgramAsync(int id)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                var program = await this.database.Table<SpecializationProgram>().FirstOrDefaultAsync(p => p.ProgramId == id);
                if (program == null)
                {
                    throw new ResourceNotFoundException(
                        $"SpecializationProgram with ID {id} not found",
                        $"Nie znaleziono programu specjalizacji o ID {id}");
                }
                return program;
            }, $"Nie udało się pobrać programu specjalizacji o ID {id}");
        }

        public async Task<SpecializationProgram> GetSpecializationProgramByCodeAsync(string code, SmkVersion smkVersion)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                if (string.IsNullOrEmpty(code))
                {
                    throw new InvalidInputException(
                        "Code cannot be null or empty",
                        "Kod nie może być pusty");
                }

                return await this.database.Table<SpecializationProgram>()
                    .FirstOrDefaultAsync(p => p.Code == code && p.SmkVersion == smkVersion);
            }, $"Nie udało się pobrać programu specjalizacji o kodzie {code}");
        }

        public async Task<List<SpecializationProgram>> GetAllSpecializationProgramsAsync()
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                return await this.database.Table<SpecializationProgram>().ToListAsync();
            }, "Nie udało się pobrać listy programów specjalizacji");
        }

        public async Task<int> SaveSpecializationProgramAsync(SpecializationProgram program)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
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
            }, "Nie udało się zapisać danych programu specjalizacji");
        }

        public async Task MigrateShiftDataForModulesAsync()
        {
            await this.InitializeAsync();
            await SafeExecuteAsync(async () =>
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
            }, "Nie udało się zmigrować danych dyżurów do modułów");
        }

        public async Task<RealizedInternshipNewSMK> GetRealizedInternshipNewSMKAsync(int id)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                var internship = await this.database.Table<RealizedInternshipNewSMK>().FirstOrDefaultAsync(i => i.RealizedInternshipId == id);
                if (internship == null)
                {
                    throw new ResourceNotFoundException(
                        $"RealizedInternshipNewSMK with ID {id} not found",
                        $"Nie znaleziono zrealizowanego stażu w nowym SMK o ID {id}");
                }
                return internship;
            }, $"Nie udało się pobrać zrealizowanego stażu w nowym SMK o ID {id}");
        }

        public async Task<List<RealizedInternshipNewSMK>> GetRealizedInternshipsNewSMKAsync(int? specializationId = null, int? moduleId = null, int? internshipRequirementId = null)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
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
            }, "Nie udało się pobrać listy zrealizowanych staży w nowym SMK");
        }

        public async Task<int> SaveRealizedInternshipNewSMKAsync(RealizedInternshipNewSMK internship)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
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
            }, "Nie udało się zapisać danych zrealizowanego stażu w nowym SMK");
        }

        public async Task<int> DeleteRealizedInternshipNewSMKAsync(RealizedInternshipNewSMK internship)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "RealizedInternshipNewSMK cannot be null",
                        "Zrealizowany staż w nowym SMK nie może być pusty");
                }

                return await this.database.DeleteAsync(internship);
            }, "Nie udało się usunąć zrealizowanego stażu w nowym SMK");
        }

        public async Task<RealizedInternshipOldSMK> GetRealizedInternshipOldSMKAsync(int id)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                var internship = await this.database.Table<RealizedInternshipOldSMK>().FirstOrDefaultAsync(i => i.RealizedInternshipId == id);
                if (internship == null)
                {
                    throw new ResourceNotFoundException(
                        $"RealizedInternshipOldSMK with ID {id} not found",
                        $"Nie znaleziono zrealizowanego stażu w starym SMK o ID {id}");
                }
                return internship;
            }, $"Nie udało się pobrać zrealizowanego stażu w starym SMK o ID {id}");
        }

        public async Task<List<RealizedInternshipOldSMK>> GetRealizedInternshipsOldSMKAsync(int? specializationId = null, int? year = null)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
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
            }, "Nie udało się pobrać listy zrealizowanych staży w starym SMK");
        }

        public async Task<int> SaveRealizedInternshipOldSMKAsync(RealizedInternshipOldSMK internship)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
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
            }, "Nie udało się zapisać danych zrealizowanego stażu w starym SMK");
        }

        public async Task<int> DeleteRealizedInternshipOldSMKAsync(RealizedInternshipOldSMK internship)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "RealizedInternshipOldSMK cannot be null",
                        "Zrealizowany staż w starym SMK nie może być pusty");
                }

                return await this.database.DeleteAsync(internship);
            }, "Nie udało się usunąć zrealizowanego stażu w starym SMK");
        }

        public async Task MigrateInternshipDataAsync()
        {
            await this.InitializeAsync();
            await SafeExecuteAsync(async () =>
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
                        Debug.WriteLine($"Znaleziono realizację z pustą nazwą stażu, ID: {realization.RealizedInternshipId}");
                        // Próba naprawy - szukamy w oryginalnych stażach
                        var originalInternship = await this.database.Table<Internship>()
                            .FirstOrDefaultAsync(i => i.SpecializationId == realization.SpecializationId &&
                                                      i.DaysCount == realization.DaysCount);

                        if (originalInternship != null && !string.IsNullOrEmpty(originalInternship.InternshipName))
                        {
                            realization.InternshipName = originalInternship.InternshipName;
                            await this.database.UpdateAsync(realization);
                            Debug.WriteLine($"Naprawiono nazwę stażu: {realization.InternshipName}");
                        }
                        else
                        {
                            // Jeśli nie udało się znaleźć odpowiedniego stażu, użyj wartości domyślnej
                            realization.InternshipName = "Staż bez nazwy";
                            await this.database.UpdateAsync(realization);
                            Debug.WriteLine($"Ustawiono domyślną nazwę stażu dla ID: {realization.RealizedInternshipId}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Błąd podczas naprawy realizacji: {ex.Message}");
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
            }, "Nie udało się zmigrować danych stażowych");
        }

        public async Task<bool> TableExists(string tableName)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
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
            }, $"Nie udało się sprawdzić istnienia tabeli {tableName}");
        }

        public async Task<List<string>> ListTables()
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                var tableInfos = await this.database.QueryAsync<TableInfo>(
                    "SELECT name FROM sqlite_master WHERE type='table'");
                return tableInfos.Select(t => t.name).ToList();
            }, "Nie udało się pobrać listy tabel");
        }

        public async Task DropTableIfExists(string tableName)
        {
            await this.InitializeAsync();
            await SafeExecuteAsync(async () =>
            {
                if (string.IsNullOrEmpty(tableName))
                {
                    throw new InvalidInputException(
                        "Table name cannot be null or empty",
                        "Nazwa tabeli nie może być pusta");
                }

                await this.database.ExecuteAsync($"DROP TABLE IF EXISTS {tableName}");
            }, $"Nie udało się usunąć tabeli {tableName}");
        }

        public async Task<int> GetTableRowCount(string tableName)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
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
            }, $"Nie udało się pobrać liczby wierszy w tabeli {tableName}");
        }

        public async Task FixRealizedInternshipNames()
        {
            await this.InitializeAsync();
            await SafeExecuteAsync(async () =>
            {
                // Pobierz wszystkie realizacje bez poprawnej nazwy
                var realizationsToFix = await this.database.Table<RealizedInternshipOldSMK>()
                    .Where(r => r.InternshipName == "Staż bez nazwy" || r.InternshipName == null)
                    .ToListAsync();

                Debug.WriteLine($"Znaleziono {realizationsToFix.Count} realizacji do naprawy");

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
                    Debug.WriteLine("Nie znaleziono wymagań stażowych do naprawy nazw");
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

                    Debug.WriteLine($"Naprawiono nazwy realizacji, przypisując '{firstInternship.InternshipName}'");
                }
            }, "Nie udało się naprawić nazw zrealizowanych staży");
        }

        public async Task<List<T>> QueryAsync<T>(string query, params object[] args) where T : new()
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                if (string.IsNullOrEmpty(query))
                {
                    throw new InvalidInputException(
                        "Query cannot be null or empty",
                        "Zapytanie nie może być puste");
                }

                return await this.database.QueryAsync<T>(query, args);
            }, "Nie udało się wykonać zapytania do bazy danych");
        }

        public async Task<int> ExecuteAsync(string query, params object[] args)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                if (string.IsNullOrEmpty(query))
                {
                    throw new InvalidInputException(
                        "Query cannot be null or empty",
                        "Zapytanie nie może być puste");
                }

                return await this.database.ExecuteAsync(query, args);
            }, "Nie udało się wykonać polecenia w bazie danych");
        }

        public async Task<int> UpdateAsync<T>(T item)
        {
            await this.InitializeAsync();
            return await SafeExecuteAsync(async () =>
            {
                if (item == null)
                {
                    throw new InvalidInputException(
                        "Item cannot be null",
                        "Element nie może być pusty");
                }

                return await this.database.UpdateAsync(item);
            }, "Nie udało się zaktualizować danych w bazie");
        }

        public void ClearCache()
        {
            _specializationCache.Clear();
            _moduleCache.Clear();
        }

        private class TableInfo
        {
            public string name { get; set; }
        }
    }
}
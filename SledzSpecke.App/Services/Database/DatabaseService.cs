using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Services.Database
{
    public class DatabaseService : IDatabaseService
    {
        private SQLiteAsyncConnection database;
        private bool isInitialized = false;
        private readonly Dictionary<int, Models.Specialization> _specializationCache = new();
        private readonly Dictionary<int, List<Module>> _moduleCache = new();

        public async Task InitializeAsync()
        {
            if (this.isInitialized)
            {
                return;
            }

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
        }
        public async Task<User> GetUserAsync(int id)
        {
            await this.InitializeAsync();
            return await this.database.Table<User>().FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            await this.InitializeAsync();
            return await this.database.Table<User>().FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            await this.InitializeAsync();
            return await this.database.Table<User>().ToListAsync();
        }

        public async Task<int> SaveUserAsync(User user)
        {
            await this.InitializeAsync();

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
        }

        public async Task<Models.Specialization> GetSpecializationAsync(int id)
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
        }


        public async Task<Models.Specialization> GetSpecializationWithModulesAsync(int id)
        {
            var specialization = await GetSpecializationAsync(id);
            if (specialization != null)
            {
                specialization.Modules = await GetModulesAsync(specialization.SpecializationId);
            }
            return specialization;
        }

        public async Task<int> SaveSpecializationAsync(Models.Specialization specialization)
        {
            await this.InitializeAsync();

            if (specialization.SpecializationId != 0)
            {
                await this.database.UpdateAsync(specialization);
                return specialization.SpecializationId;
            }
            else
            {
                await this.database.InsertAsync(specialization);
                var lastId = await this.database.ExecuteScalarAsync<int>("SELECT last_insert_rowid()");
                specialization.SpecializationId = lastId;
                return lastId;
            }
        }

        public async Task<List<Models.Specialization>> GetAllSpecializationsAsync()
        {
            await this.InitializeAsync();
            return await this.database.Table<Models.Specialization>().ToListAsync();
        }

        public async Task<int> UpdateSpecializationAsync(Models.Specialization specialization)
        {
            await this.InitializeAsync();
            return await this.database.UpdateAsync(specialization);
        }

        public async Task CleanupSpecializationDataAsync(int specializationId)
        {
            var modules = await GetModulesAsync(specializationId);
            foreach (var module in modules)
            {
                await DeleteModuleAsync(module);
            }
        }

        public async Task<Module> GetModuleAsync(int id)
        {
            await this.InitializeAsync();
            return await this.database.Table<Module>().FirstOrDefaultAsync(m => m.ModuleId == id);
        }

        public async Task<List<Module>> GetModulesAsync(int specializationId)
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
        }

        public async Task<int> SaveModuleAsync(Module module)
        {
            await this.InitializeAsync();
            if (module.ModuleId != 0)
            {
                return await this.database.UpdateAsync(module);
            }
            else
            {
                return await this.database.InsertAsync(module);
            }
        }

        public async Task<int> UpdateModuleAsync(Module module)
        {
            await this.InitializeAsync();
            return await this.database.UpdateAsync(module);
        }

        public async Task<int> DeleteModuleAsync(Module module)
        {
            await this.InitializeAsync();
            return await this.database.DeleteAsync(module);
        }

        public async Task<Internship> GetInternshipAsync(int id)
        {
            await this.InitializeAsync();
            return await this.database.Table<Internship>().FirstOrDefaultAsync(i => i.InternshipId == id);
        }

        public async Task<List<Internship>> GetInternshipsAsync(int? specializationId = null, int? moduleId = null)
        {
            await this.InitializeAsync();
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
        }

        public async Task<int> SaveInternshipAsync(Internship internship)
        {
            await this.InitializeAsync();
            if (internship.InternshipId != 0)
            {
                return await this.database.UpdateAsync(internship);
            }
            else
            {
                return await this.database.InsertAsync(internship);
            }
        }

        public async Task<int> DeleteInternshipAsync(Internship internship)
        {
            await this.InitializeAsync();
            return await this.database.DeleteAsync(internship);
        }

        public async Task<MedicalShift> GetMedicalShiftAsync(int id)
        {
            await this.InitializeAsync();
            return await this.database.Table<MedicalShift>().FirstOrDefaultAsync(s => s.ShiftId == id);
        }

        public async Task<List<MedicalShift>> GetMedicalShiftsAsync(int? internshipId = null)
        {
            await this.InitializeAsync();
            var query = this.database.Table<MedicalShift>();

            if (internshipId.HasValue)
            {
                query = query.Where(s => s.InternshipId == internshipId);
            }

            return await query.ToListAsync();
        }

        public async Task<int> SaveMedicalShiftAsync(MedicalShift shift)
        {
            await this.InitializeAsync();
            if (shift.ShiftId != 0)
            {
                return await this.database.UpdateAsync(shift);
            }
            else
            {
                return await this.database.InsertAsync(shift);
            }
        }

        public async Task<int> DeleteMedicalShiftAsync(MedicalShift shift)
        {
            await this.InitializeAsync();
            return await this.database.DeleteAsync(shift);
        }

        public async Task<Procedure> GetProcedureAsync(int id)
        {
            await this.InitializeAsync();
            return await this.database.Table<Procedure>().FirstOrDefaultAsync(p => p.ProcedureId == id);
        }

        public async Task<List<Procedure>> GetProceduresAsync(int? internshipId = null, string searchText = null)
        {
            await this.InitializeAsync();
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
        }

        public async Task<int> SaveProcedureAsync(Procedure procedure)
        {
            await this.InitializeAsync();
            if (procedure.ProcedureId != 0)
            {
                return await this.database.UpdateAsync(procedure);
            }
            else
            {
                return await this.database.InsertAsync(procedure);
            }
        }

        public async Task<int> DeleteProcedureAsync(Procedure procedure)
        {
            await this.InitializeAsync();
            return await this.database.DeleteAsync(procedure);
        }

        public async Task<Course> GetCourseAsync(int id)
        {
            await this.InitializeAsync();
            return await this.database.Table<Course>().FirstOrDefaultAsync(c => c.CourseId == id);
        }

        public async Task<List<Course>> GetCoursesAsync(int? specializationId = null, int? moduleId = null)
        {
            await this.InitializeAsync();
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
        }

        public async Task<int> SaveCourseAsync(Course course)
        {
            await this.InitializeAsync();
            if (course.CourseId != 0)
            {
                return await this.database.UpdateAsync(course);
            }
            else
            {
                return await this.database.InsertAsync(course);
            }
        }

        public async Task<int> DeleteCourseAsync(Course course)
        {
            await this.InitializeAsync();
            return await this.database.DeleteAsync(course);
        }

        public async Task<SelfEducation> GetSelfEducationAsync(int id)
        {
            await this.InitializeAsync();
            return await this.database.Table<SelfEducation>().FirstOrDefaultAsync(s => s.SelfEducationId == id);
        }

        public async Task<List<SelfEducation>> GetSelfEducationItemsAsync(int? specializationId = null, int? moduleId = null)
        {
            await this.InitializeAsync();
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
        }

        public async Task<int> SaveSelfEducationAsync(SelfEducation selfEducation)
        {
            await this.InitializeAsync();
            if (selfEducation.SelfEducationId != 0)
            {
                return await this.database.UpdateAsync(selfEducation);
            }
            else
            {
                return await this.database.InsertAsync(selfEducation);
            }
        }

        public async Task<int> DeleteSelfEducationAsync(SelfEducation selfEducation)
        {
            await this.InitializeAsync();
            return await this.database.DeleteAsync(selfEducation);
        }

        public async Task<Publication> GetPublicationAsync(int id)
        {
            await this.InitializeAsync();
            return await this.database.Table<Publication>().FirstOrDefaultAsync(p => p.PublicationId == id);
        }

        public async Task<List<Publication>> GetPublicationsAsync(int? specializationId = null, int? moduleId = null)
        {
            await this.InitializeAsync();
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
        }

        public async Task<int> SavePublicationAsync(Publication publication)
        {
            await this.InitializeAsync();
            if (publication.PublicationId != 0)
            {
                return await this.database.UpdateAsync(publication);
            }
            else
            {
                return await this.database.InsertAsync(publication);
            }
        }

        public async Task<int> DeletePublicationAsync(Publication publication)
        {
            await this.InitializeAsync();
            return await this.database.DeleteAsync(publication);
        }

        public async Task<EducationalActivity> GetEducationalActivityAsync(int id)
        {
            await this.InitializeAsync();
            return await this.database.Table<EducationalActivity>().FirstOrDefaultAsync(a => a.ActivityId == id);
        }

        public async Task<List<EducationalActivity>> GetEducationalActivitiesAsync(int? specializationId = null, int? moduleId = null)
        {
            await this.InitializeAsync();
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
        }

        public async Task<int> SaveEducationalActivityAsync(EducationalActivity activity)
        {
            await this.InitializeAsync();
            if (activity.ActivityId != 0)
            {
                return await this.database.UpdateAsync(activity);
            }
            else
            {
                return await this.database.InsertAsync(activity);
            }
        }

        public async Task<int> DeleteEducationalActivityAsync(EducationalActivity activity)
        {
            await this.InitializeAsync();
            return await this.database.DeleteAsync(activity);
        }

        public async Task<Absence> GetAbsenceAsync(int id)
        {
            await this.InitializeAsync();
            return await this.database.Table<Absence>().FirstOrDefaultAsync(a => a.AbsenceId == id);
        }

        public async Task<List<Absence>> GetAbsencesAsync(int specializationId)
        {
            await this.InitializeAsync();
            return await this.database.Table<Absence>().Where(a => a.SpecializationId == specializationId).ToListAsync();
        }

        public async Task<int> SaveAbsenceAsync(Absence absence)
        {
            await this.InitializeAsync();
            if (absence.AbsenceId != 0)
            {
                return await this.database.UpdateAsync(absence);
            }
            else
            {
                return await this.database.InsertAsync(absence);
            }
        }

        public async Task<int> DeleteAbsenceAsync(Absence absence)
        {
            await this.InitializeAsync();
            return await this.database.DeleteAsync(absence);
        }

        public async Task<Models.Recognition> GetRecognitionAsync(int id)
        {
            await this.InitializeAsync();
            return await this.database.Table<Models.Recognition>().FirstOrDefaultAsync(r => r.RecognitionId == id);
        }

        public async Task<List<Models.Recognition>> GetRecognitionsAsync(int specializationId)
        {
            await this.InitializeAsync();
            return await this.database.Table<Models.Recognition>().Where(r => r.SpecializationId == specializationId).ToListAsync();
        }

        public async Task<int> SaveRecognitionAsync(Models.Recognition recognition)
        {
            await this.InitializeAsync();
            if (recognition.RecognitionId != 0)
            {
                return await this.database.UpdateAsync(recognition);
            }
            else
            {
                return await this.database.InsertAsync(recognition);
            }
        }

        public async Task<int> DeleteRecognitionAsync(Models.Recognition recognition)
        {
            await this.InitializeAsync();
            return await this.database.DeleteAsync(recognition);
        }

        public async Task<SpecializationProgram> GetSpecializationProgramAsync(int id)
        {
            await this.InitializeAsync();
            return await this.database.Table<SpecializationProgram>().FirstOrDefaultAsync(p => p.ProgramId == id);
        }

        public async Task<SpecializationProgram> GetSpecializationProgramByCodeAsync(string code, SmkVersion smkVersion)
        {
            await this.InitializeAsync();
            return await this.database.Table<SpecializationProgram>().FirstOrDefaultAsync(p => p.Code == code && p.SmkVersion == smkVersion);
        }

        public async Task<List<SpecializationProgram>> GetAllSpecializationProgramsAsync()
        {
            await this.InitializeAsync();
            return await this.database.Table<SpecializationProgram>().ToListAsync();
        }

        public async Task<int> SaveSpecializationProgramAsync(SpecializationProgram program)
        {
            await this.InitializeAsync();
            if (program.ProgramId != 0)
            {
                return await this.database.UpdateAsync(program);
            }
            else
            {
                return await this.database.InsertAsync(program);
            }
        }

        public async Task MigrateShiftDataForModulesAsync()
        {
            await this.InitializeAsync();
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
        }

        public async Task<RealizedInternshipNewSMK> GetRealizedInternshipNewSMKAsync(int id)
        {
            await this.InitializeAsync();
            return await this.database.Table<RealizedInternshipNewSMK>().FirstOrDefaultAsync(i => i.RealizedInternshipId == id);
        }

        public async Task<List<RealizedInternshipNewSMK>> GetRealizedInternshipsNewSMKAsync(int? specializationId = null, int? moduleId = null, int? internshipRequirementId = null)
        {
            await this.InitializeAsync();
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
        }

        public async Task<int> SaveRealizedInternshipNewSMKAsync(RealizedInternshipNewSMK internship)
        {
            await this.InitializeAsync();
            if (internship.RealizedInternshipId != 0)
            {
                return await this.database.UpdateAsync(internship);
            }
            else
            {
                return await this.database.InsertAsync(internship);
            }
        }

        public async Task<int> DeleteRealizedInternshipNewSMKAsync(RealizedInternshipNewSMK internship)
        {
            await this.InitializeAsync();
            return await this.database.DeleteAsync(internship);
        }

        public async Task<RealizedInternshipOldSMK> GetRealizedInternshipOldSMKAsync(int id)
        {
            await this.InitializeAsync();
            return await this.database.Table<RealizedInternshipOldSMK>().FirstOrDefaultAsync(i => i.RealizedInternshipId == id);
        }

        public async Task<List<RealizedInternshipOldSMK>> GetRealizedInternshipsOldSMKAsync(int? specializationId = null, int? year = null)
        {
            await this.InitializeAsync();
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
        }

        public async Task<int> SaveRealizedInternshipOldSMKAsync(RealizedInternshipOldSMK internship)
        {
            await this.InitializeAsync();
            if (internship.RealizedInternshipId != 0)
            {
                return await this.database.UpdateAsync(internship);
            }
            else
            {
                return await this.database.InsertAsync(internship);
            }
        }

        public async Task<int> DeleteRealizedInternshipOldSMKAsync(RealizedInternshipOldSMK internship)
        {
            await this.InitializeAsync();
            return await this.database.DeleteAsync(internship);
        }

        public async Task MigrateInternshipDataAsync()
        {
            await this.InitializeAsync();

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
                    System.Diagnostics.Debug.WriteLine($"Znaleziono realizację z pustą nazwą stażu, ID: {realization.RealizedInternshipId}");
                    // Próba naprawy - szukamy w oryginalnych stażach
                    var originalInternship = await this.database.Table<Internship>()
                        .FirstOrDefaultAsync(i => i.SpecializationId == realization.SpecializationId &&
                                                  i.DaysCount == realization.DaysCount);

                    if (originalInternship != null && !string.IsNullOrEmpty(originalInternship.InternshipName))
                    {
                        realization.InternshipName = originalInternship.InternshipName;
                        await this.database.UpdateAsync(realization);
                        System.Diagnostics.Debug.WriteLine($"Naprawiono nazwę stażu: {realization.InternshipName}");
                    }
                    else
                    {
                        // Jeśli nie udało się znaleźć odpowiedniego stażu, użyj wartości domyślnej
                        realization.InternshipName = "Staż bez nazwy";
                        await this.database.UpdateAsync(realization);
                        System.Diagnostics.Debug.WriteLine($"Ustawiono domyślną nazwę stażu dla ID: {realization.RealizedInternshipId}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas naprawy realizacji: {ex.Message}");
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
        }

        // Dodaj na końcu klasy DatabaseService
        public async Task<bool> TableExists(string tableName)
        {
            await this.InitializeAsync();
            try
            {
                var result = await this.database.ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=?", tableName);
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<string>> ListTables()
        {
            await this.InitializeAsync();
            var tableInfos = await this.database.QueryAsync<TableInfo>(
                "SELECT name FROM sqlite_master WHERE type='table'");
            return tableInfos.Select(t => t.name).ToList();
        }

        public async Task DropTableIfExists(string tableName)
        {
            await this.InitializeAsync();
            await this.database.ExecuteAsync($"DROP TABLE IF EXISTS {tableName}");
        }

        public async Task<int> GetTableRowCount(string tableName)
        {
            await this.InitializeAsync();
            try
            {
                var result = await this.database.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM {tableName}");
                return result;
            }
            catch
            {
                return -1; // Tabela nie istnieje lub inny błąd
            }
        }

        public async Task FixRealizedInternshipNames()
        {
            await this.InitializeAsync();

            // Pobierz wszystkie realizacje bez poprawnej nazwy
            var realizationsToFix = await this.database.Table<RealizedInternshipOldSMK>()
                .Where(r => r.InternshipName == "Staż bez nazwy" || r.InternshipName == null)
                .ToListAsync();

            System.Diagnostics.Debug.WriteLine($"Znaleziono {realizationsToFix.Count} realizacji do naprawy");

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
                System.Diagnostics.Debug.WriteLine("Nie znaleziono wymagań stażowych do naprawy nazw");
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

                System.Diagnostics.Debug.WriteLine($"Naprawiono nazwy realizacji, przypisując '{firstInternship.InternshipName}'");
            }
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
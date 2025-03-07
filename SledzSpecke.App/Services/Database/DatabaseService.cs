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

        public async Task InitializeAsync()
        {
            if (this.isInitialized)
            {
                return;
            }

            // Zapewnienie, że katalog bazy danych istnieje
            var databasePath = Constants.DatabasePath;
            var databaseDirectory = Path.GetDirectoryName(databasePath);

            if (!Directory.Exists(databaseDirectory))
            {
                Directory.CreateDirectory(databaseDirectory);
            }

            this.database = new SQLiteAsyncConnection(databasePath, Constants.Flags);

            // Tworzenie tabel dla wszystkich modeli
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

            this.isInitialized = true;
        }

        // User
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
                return await this.database.UpdateAsync(user);
            }
            else
            {
                return await this.database.InsertAsync(user);
            }
        }

        // Specialization
        public async Task<Models.Specialization> GetSpecializationAsync(int id)
        {
            await this.InitializeAsync();
            var specialization = await this.database.Table<Models.Specialization>().FirstOrDefaultAsync(s => s.SpecializationId == id);

            if (specialization != null)
            {
                specialization.Modules = await this.GetModulesAsync(specialization.SpecializationId);
            }

            return specialization;
        }

        public async Task<int> SaveSpecializationAsync(Models.Specialization specialization)
        {
            await this.InitializeAsync();
            if (specialization.SpecializationId != 0)
            {
                return await this.database.UpdateAsync(specialization);
            }
            else
            {
                return await this.database.InsertAsync(specialization);
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

        // Module
        public async Task<Module> GetModuleAsync(int id)
        {
            await this.InitializeAsync();
            return await this.database.Table<Module>().FirstOrDefaultAsync(m => m.ModuleId == id);
        }

        public async Task<List<Module>> GetModulesAsync(int specializationId)
        {
            await this.InitializeAsync();
            return await this.database.Table<Module>().Where(m => m.SpecializationId == specializationId).ToListAsync();
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

        // Internship
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

        // Medical Shifts
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

        // Procedures
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

            // Filtrowanie po tekście wyszukiwania (SQLite nie obsługuje bezpośrednio LIKE dla StringComparison.OrdinalIgnoreCase)
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

        // Courses
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

        // Self Education
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

        // Publications
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

        // Educational Activities
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

        // Absences
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

        // Recognitions
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

        // Specialization Programs
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
    }
}
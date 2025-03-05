// Custom implementation for tests that uses an existing open connection
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SQLite;

namespace SledzSpecke.Tests.TestUtilities
{
    public class SqliteTestDatabaseService : IDatabaseService
    {
        private readonly SQLiteConnection connection;
        private readonly SQLiteAsyncConnection database;
        private bool isInitialized = false;

        public SqliteTestDatabaseService(SQLiteConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this.database = new SQLiteAsyncConnection(connection.DatabasePath);
        }

        public async Task InitializeAsync()
        {
            if (this.isInitialized)
            {
                return;
            }

            // Create all tables
            await this.database.CreateTableAsync<User>();
            await this.database.CreateTableAsync<Specialization>();
            await this.database.CreateTableAsync<Module>();
            await this.database.CreateTableAsync<Internship>();
            await this.database.CreateTableAsync<MedicalShift>();
            await this.database.CreateTableAsync<Procedure>();
            await this.database.CreateTableAsync<Course>();
            await this.database.CreateTableAsync<SelfEducation>();
            await this.database.CreateTableAsync<Publication>();
            await this.database.CreateTableAsync<EducationalActivity>();
            await this.database.CreateTableAsync<Absence>();
            await this.database.CreateTableAsync<Recognition>();
            await this.database.CreateTableAsync<SpecializationProgram>();

            this.isInitialized = true;
        }

        public async Task<Internship> GetInternshipAsync(int id)
        {
            await this.InitializeAsync();
            return await this.database.Table<Internship>().FirstOrDefaultAsync(i => i.InternshipId == id)
                ?? throw new KeyNotFoundException($"Internship with ID {id} not found");
        }

        public async Task<int> SaveInternshipAsync(Internship internship)
        {
            await this.InitializeAsync();
            if (internship.InternshipId != 0)
            {
                await this.database.UpdateAsync(internship);
                return internship.InternshipId;
            }
            else
            {
                return await this.database.InsertAsync(internship);
            }
        }

        // The remaining interface methods implemented with NotImplementedException for brevity
        // Add implementation as needed for your tests
        public Task<int> DeleteAbsenceAsync(Absence absence) => throw new NotImplementedException();

        public Task<int> DeleteCourseAsync(Course course) => throw new NotImplementedException();

        public Task<int> DeleteEducationalActivityAsync(EducationalActivity activity) => throw new NotImplementedException();

        public Task<int> DeleteInternshipAsync(Internship internship) => throw new NotImplementedException();

        public Task<int> DeleteMedicalShiftAsync(MedicalShift shift) => throw new NotImplementedException();

        public Task<int> DeleteProcedureAsync(Procedure procedure) => throw new NotImplementedException();

        public Task<int> DeletePublicationAsync(Publication publication) => throw new NotImplementedException();

        public Task<int> DeleteRecognitionAsync(Recognition recognition) => throw new NotImplementedException();

        public Task<int> DeleteSelfEducationAsync(SelfEducation selfEducation) => throw new NotImplementedException();

        public Task<List<Absence>> GetAbsencesAsync(int specializationId) => throw new NotImplementedException();

        public Task<Absence> GetAbsenceAsync(int id) => throw new NotImplementedException();

        public Task<List<SpecializationProgram>> GetAllSpecializationProgramsAsync() => throw new NotImplementedException();

        public Task<List<Specialization>> GetAllSpecializationsAsync() => throw new NotImplementedException();

        public Task<Course> GetCourseAsync(int id) => throw new NotImplementedException();

        public Task<List<Course>> GetCoursesAsync(int? specializationId = null, int? moduleId = null) => throw new NotImplementedException();

        public Task<EducationalActivity> GetEducationalActivityAsync(int id) => throw new NotImplementedException();

        public Task<List<EducationalActivity>> GetEducationalActivitiesAsync(int? specializationId = null, int? moduleId = null) => throw new NotImplementedException();

        public Task<List<Internship>> GetInternshipsAsync(int? specializationId = null, int? moduleId = null) => throw new NotImplementedException();

        public Task<MedicalShift> GetMedicalShiftAsync(int id) => throw new NotImplementedException();

        public Task<List<MedicalShift>> GetMedicalShiftsAsync(int? internshipId = null) => throw new NotImplementedException();

        public Task<Module> GetModuleAsync(int id) => throw new NotImplementedException();

        public Task<List<Module>> GetModulesAsync(int specializationId) => throw new NotImplementedException();

        public Task<Procedure> GetProcedureAsync(int id) => throw new NotImplementedException();

        public Task<List<Procedure>> GetProceduresAsync(int? internshipId = null, string searchText = null) => throw new NotImplementedException();

        public Task<Publication> GetPublicationAsync(int id) => throw new NotImplementedException();

        public Task<List<Publication>> GetPublicationsAsync(int? specializationId = null, int? moduleId = null) => throw new NotImplementedException();

        public Task<Recognition> GetRecognitionAsync(int id) => throw new NotImplementedException();

        public Task<List<Recognition>> GetRecognitionsAsync(int specializationId) => throw new NotImplementedException();

        public Task<SelfEducation> GetSelfEducationAsync(int id) => throw new NotImplementedException();

        public Task<List<SelfEducation>> GetSelfEducationItemsAsync(int? specializationId = null, int? moduleId = null) => throw new NotImplementedException();

        public Task<Specialization> GetSpecializationAsync(int id) => throw new NotImplementedException();

        public Task<SpecializationProgram> GetSpecializationProgramAsync(int id) => throw new NotImplementedException();

        public Task<SpecializationProgram> GetSpecializationProgramByCodeAsync(string code, SmkVersion smkVersion) => throw new NotImplementedException();

        public Task<User> GetUserAsync(int id) => throw new NotImplementedException();

        public Task<User> GetUserByUsernameAsync(string username) => throw new NotImplementedException();

        public Task<int> SaveAbsenceAsync(Absence absence) => throw new NotImplementedException();

        public Task<int> SaveCourseAsync(Course course) => throw new NotImplementedException();

        public Task<int> SaveEducationalActivityAsync(EducationalActivity activity) => throw new NotImplementedException();

        public Task<int> SaveMedicalShiftAsync(MedicalShift shift) => throw new NotImplementedException();

        public Task<int> SaveModuleAsync(Module module) => throw new NotImplementedException();

        public Task<int> SaveProcedureAsync(Procedure procedure) => throw new NotImplementedException();

        public Task<int> SavePublicationAsync(Publication publication) => throw new NotImplementedException();

        public Task<int> SaveRecognitionAsync(Recognition recognition) => throw new NotImplementedException();

        public Task<int> SaveSelfEducationAsync(SelfEducation selfEducation) => throw new NotImplementedException();

        public Task<int> SaveSpecializationAsync(Specialization specialization) => throw new NotImplementedException();

        public Task<int> SaveSpecializationProgramAsync(SpecializationProgram program) => throw new NotImplementedException();

        public Task<int> SaveUserAsync(User user) => throw new NotImplementedException();

        public Task<int> UpdateModuleAsync(Module module) => throw new NotImplementedException();

        public Task<int> UpdateSpecializationAsync(Specialization specialization) => throw new NotImplementedException();
    }
}
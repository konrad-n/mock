using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SQLite;

namespace SledzSpecke.Tests.TestUtilities
{
    public class ModuleTestDatabaseService : IDatabaseService
    {
        private readonly SQLiteConnection connection;
        private SQLiteAsyncConnection database;
        private bool isInitialized = false;

        public ModuleTestDatabaseService(SQLiteConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public async Task InitializeAsync()
        {
            if (this.isInitialized)
            {
                return;
            }

            // Create an async connection that uses our existing open connection
            this.database = new SQLiteAsyncConnection(this.connection.DatabasePath);

            // Create just the tables we need for this test
            await this.database.CreateTableAsync<Module>();

            this.isInitialized = true;
        }

        // Implement only the methods needed for the test
        public async Task<Module> GetModuleAsync(int id)
        {
            await this.InitializeAsync();
            return await this.database.Table<Module>().FirstOrDefaultAsync(m => m.ModuleId == id);
        }

        public async Task<int> SaveModuleAsync(Module module)
        {
            await this.InitializeAsync();
            if (module.ModuleId != 0)
            {
                await this.database.UpdateAsync(module);
                return module.ModuleId;
            }
            else
            {
                return await this.database.InsertAsync(module);
            }
        }

        // The remaining interface methods implemented with NotImplementedException for brevity
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

        public Task<Internship> GetInternshipAsync(int id) => throw new NotImplementedException();

        public Task<List<Internship>> GetInternshipsAsync(int? specializationId = null, int? moduleId = null) => throw new NotImplementedException();

        public Task<MedicalShift> GetMedicalShiftAsync(int id) => throw new NotImplementedException();

        public Task<List<MedicalShift>> GetMedicalShiftsAsync(int? internshipId = null) => throw new NotImplementedException();

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

        public Task<int> SaveInternshipAsync(Internship internship) => throw new NotImplementedException();

        public Task<int> SaveMedicalShiftAsync(MedicalShift shift) => throw new NotImplementedException();

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

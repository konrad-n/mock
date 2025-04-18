﻿using SledzSpecke.App.Models;

namespace SledzSpecke.App.Services.Database
{
    public interface IDatabaseService
    {
        Task InitializeAsync();
        Task<User> GetUserAsync(int id);

        Task<User> GetUserByUsernameAsync(string username);

        Task<int> SaveUserAsync(User user);

        Task<List<User>> GetAllUsersAsync();

        Task<Models.Specialization> GetSpecializationAsync(int id);

        Task<int> SaveSpecializationAsync(Models.Specialization specialization);

        Task<List<Models.Specialization>> GetAllSpecializationsAsync();

        Task CleanupSpecializationDataAsync(int specializationId);

        Task<Module> GetModuleAsync(int id);

        Task<List<Module>> GetModulesAsync(int specializationId);

        Task<int> SaveModuleAsync(Module module);

        Task<int> UpdateModuleAsync(Module module);

        Task<int> DeleteModuleAsync(Module module);

        Task<Internship> GetInternshipAsync(int id);

        Task<List<Internship>> GetInternshipsAsync(int? specializationId = null, int? moduleId = null);

        Task<int> SaveInternshipAsync(Internship internship);

        Task<int> DeleteInternshipAsync(Internship internship);

        Task<RealizedInternshipNewSMK> GetRealizedInternshipNewSMKAsync(int id);

        Task<List<RealizedInternshipNewSMK>> GetRealizedInternshipsNewSMKAsync(int? specializationId = null, int? moduleId = null, int? internshipRequirementId = null);

        Task<int> SaveRealizedInternshipNewSMKAsync(RealizedInternshipNewSMK internship);

        Task<int> DeleteRealizedInternshipNewSMKAsync(RealizedInternshipNewSMK internship);

        Task<RealizedInternshipOldSMK> GetRealizedInternshipOldSMKAsync(int id);

        Task<List<RealizedInternshipOldSMK>> GetRealizedInternshipsOldSMKAsync(int? specializationId = null, int? year = null);

        Task<int> SaveRealizedInternshipOldSMKAsync(RealizedInternshipOldSMK internship);

        Task<int> DeleteRealizedInternshipOldSMKAsync(RealizedInternshipOldSMK internship);

        Task MigrateInternshipDataAsync();

        Task<MedicalShift> GetMedicalShiftAsync(int id);

        Task<List<MedicalShift>> GetMedicalShiftsAsync(int? internshipId = null);

        Task<int> SaveMedicalShiftAsync(MedicalShift shift);

        Task<int> DeleteMedicalShiftAsync(MedicalShift shift);

        Task MigrateShiftDataForModulesAsync();

        Task<Procedure> GetProcedureAsync(int id);

        Task<List<Procedure>> GetProceduresAsync(int? internshipId = null, string searchText = null);

        Task<int> SaveProcedureAsync(Procedure procedure);

        Task<int> DeleteProcedureAsync(Procedure procedure);

        Task<Course> GetCourseAsync(int id);

        Task<List<Course>> GetCoursesAsync(int? specializationId = null, int? moduleId = null);

        Task<int> SaveCourseAsync(Course course);

        Task<int> DeleteCourseAsync(Course course);

        Task<SelfEducation> GetSelfEducationAsync(int id);

        Task<List<SelfEducation>> GetSelfEducationItemsAsync(int? specializationId = null, int? moduleId = null);

        Task<int> SaveSelfEducationAsync(SelfEducation selfEducation);

        Task<int> DeleteSelfEducationAsync(SelfEducation selfEducation);

        Task<Publication> GetPublicationAsync(int id);

        Task<List<Publication>> GetPublicationsAsync(int? specializationId = null, int? moduleId = null);

        Task<int> SavePublicationAsync(Publication publication);

        Task<int> DeletePublicationAsync(Publication publication);

        Task<EducationalActivity> GetEducationalActivityAsync(int id);

        Task<List<EducationalActivity>> GetEducationalActivitiesAsync(int? specializationId = null, int? moduleId = null);

        Task<int> SaveEducationalActivityAsync(EducationalActivity activity);

        Task<int> DeleteEducationalActivityAsync(EducationalActivity activity);

        Task<Absence> GetAbsenceAsync(int id);

        Task<List<Absence>> GetAbsencesAsync(int specializationId);

        Task<int> SaveAbsenceAsync(Absence absence);

        Task<int> DeleteAbsenceAsync(Absence absence);

        Task<Models.Recognition> GetRecognitionAsync(int id);

        Task<List<Models.Recognition>> GetRecognitionsAsync(int specializationId);

        Task<int> SaveRecognitionAsync(Models.Recognition recognition);

        Task<int> DeleteRecognitionAsync(Models.Recognition recognition);

        Task<SpecializationProgram> GetSpecializationProgramAsync(int id);

        Task<SpecializationProgram> GetSpecializationProgramByCodeAsync(string code, Models.Enums.SmkVersion smkVersion);

        Task<List<SpecializationProgram>> GetAllSpecializationProgramsAsync();

        Task<int> SaveSpecializationProgramAsync(SpecializationProgram program);

        Task<int> UpdateSpecializationAsync(Models.Specialization specialization);

        Task<bool> TableExists(string tableName);

        Task<List<string>> ListTables();

        Task DropTableIfExists(string tableName);

        Task<int> GetTableRowCount(string tableName);

        Task FixRealizedInternshipNames();

        void ClearCache();
    }
}
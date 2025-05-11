using SledzSpecke.App.Models;

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

        Task<Module> GetModuleAsync(int id);

        Task<List<Module>> GetModulesAsync(int specializationId);

        Task<int> SaveModuleAsync(Module module);

        Task<int> UpdateModuleAsync(Module module);

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

        Task<List<Procedure>> GetProceduresAsync(int? internshipId = null, string searchText = null);

        Task<int> UpdateSpecializationAsync(Models.Specialization specialization);

        void ClearCache();
    }
}
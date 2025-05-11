using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Services.Specialization
{
    public interface ISpecializationService
    {
        Task<Models.Specialization> GetCurrentSpecializationAsync(bool includeModules = true);

        Task<Module> GetCurrentModuleAsync();

        Task SetCurrentModuleAsync(int moduleId);

        Task<List<Module>> GetModulesAsync(int specializationId);

        Task<bool> InitializeSpecializationModulesAsync(int specializationId);

        Task<List<Internship>> GetInternshipsAsync(int? moduleId = null);

        Task<List<MedicalShift>> GetMedicalShiftsAsync(int? internshipId = null);

        Task<bool> AddMedicalShiftAsync(MedicalShift shift);

        Task<bool> UpdateMedicalShiftAsync(MedicalShift shift);

        Task<bool> DeleteMedicalShiftAsync(int shiftId);

        event EventHandler<int> CurrentModuleChanged;

        Task<SpecializationStatistics> GetSpecializationStatisticsAsync(int? moduleId = null);

        Task UpdateModuleProgressAsync(int moduleId);

        Task<int> GetShiftCountAsync(int? moduleId = null);

        Task<int> GetInternshipCountAsync(int? moduleId = null);

        Task<List<RealizedInternshipNewSMK>> GetRealizedInternshipsNewSMKAsync(int? moduleId = null, int? internshipRequirementId = null);

        Task<List<RealizedInternshipOldSMK>> GetRealizedInternshipsOldSMKAsync(int? year = null);

        Task<RealizedInternshipNewSMK> GetRealizedInternshipNewSMKAsync(int id);

        Task<RealizedInternshipOldSMK> GetRealizedInternshipOldSMKAsync(int id);

        Task<bool> AddRealizedInternshipNewSMKAsync(RealizedInternshipNewSMK internship);

        Task<bool> AddRealizedInternshipOldSMKAsync(RealizedInternshipOldSMK internship);

        Task<bool> UpdateRealizedInternshipNewSMKAsync(RealizedInternshipNewSMK internship);

        Task<bool> UpdateRealizedInternshipOldSMKAsync(RealizedInternshipOldSMK internship);

        Task<bool> DeleteRealizedInternshipNewSMKAsync(int id);

        Task<bool> DeleteRealizedInternshipOldSMKAsync(int id);
    }
}
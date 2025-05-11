using SledzSpecke.App.Models;

namespace SledzSpecke.App.Services.MedicalShifts
{
    public interface IMedicalShiftsService
    {
        Task<MedicalShiftsSummary> GetShiftsSummaryAsync(int? year = null, int? internshipRequirementId = null);
        Task<List<int>> GetAvailableYearsAsync();
        Task<List<RealizedMedicalShiftOldSMK>> GetOldSMKShiftsAsync(int year);
        Task<RealizedMedicalShiftOldSMK> GetOldSMKShiftAsync(int shiftId);
        Task<bool> SaveOldSMKShiftAsync(RealizedMedicalShiftOldSMK shift);
        Task<bool> DeleteOldSMKShiftAsync(int shiftId);
        Task<List<InternshipRequirement>> GetAvailableInternshipRequirementsAsync();
        Task<List<RealizedMedicalShiftNewSMK>> GetNewSMKShiftsAsync(int internshipRequirementId);
        Task<bool> SaveNewSMKShiftAsync(RealizedMedicalShiftNewSMK shift);
    }
}
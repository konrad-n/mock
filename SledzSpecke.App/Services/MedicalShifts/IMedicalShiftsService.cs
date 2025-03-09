using SledzSpecke.App.Models;

namespace SledzSpecke.App.Services.MedicalShifts
{
    public interface IMedicalShiftsService
    {
        // Wspólne metody
        Task<MedicalShiftsSummary> GetShiftsSummaryAsync(int? year = null, int? internshipRequirementId = null);

        // Metody dla starego SMK
        Task<List<int>> GetAvailableYearsAsync();
        Task<List<RealizedMedicalShiftOldSMK>> GetOldSMKShiftsAsync(int year);
        Task<RealizedMedicalShiftOldSMK> GetOldSMKShiftAsync(int shiftId);
        Task<bool> SaveOldSMKShiftAsync(RealizedMedicalShiftOldSMK shift);
        Task<bool> DeleteOldSMKShiftAsync(int shiftId);

        // Metody dla nowego SMK
        Task<List<InternshipRequirement>> GetAvailableInternshipRequirementsAsync();
        Task<List<RealizedMedicalShiftNewSMK>> GetNewSMKShiftsAsync(int internshipRequirementId);
        Task<RealizedMedicalShiftNewSMK> GetNewSMKShiftAsync(int shiftId);
        Task<bool> SaveNewSMKShiftAsync(RealizedMedicalShiftNewSMK shift);
        Task<bool> DeleteNewSMKShiftAsync(int shiftId);

        // Wspólne metody
        Task<string> GetLastShiftLocationAsync(); // Pobieranie ostatnio używanej lokalizacji
    }
}
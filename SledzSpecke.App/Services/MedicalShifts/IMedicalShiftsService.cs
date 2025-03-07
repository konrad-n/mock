using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SledzSpecke.App.Models;

namespace SledzSpecke.App.Services.MedicalShifts
{
    public interface IMedicalShiftsService
    {
        // Metody dla starego SMK
        Task<List<RealizedMedicalShiftOldSMK>> GetOldSMKShiftsAsync(int year);
        Task<RealizedMedicalShiftOldSMK> GetOldSMKShiftAsync(int shiftId);
        Task<bool> SaveOldSMKShiftAsync(RealizedMedicalShiftOldSMK shift);
        Task<bool> DeleteOldSMKShiftAsync(int shiftId);

        // Metody dla nowego SMK
        Task<List<RealizedMedicalShiftNewSMK>> GetNewSMKShiftsAsync(int internshipRequirementId);
        Task<RealizedMedicalShiftNewSMK> GetNewSMKShiftAsync(int shiftId);
        Task<bool> SaveNewSMKShiftAsync(RealizedMedicalShiftNewSMK shift);
        Task<bool> DeleteNewSMKShiftAsync(int shiftId);

        // Wspólne metody
        Task<MedicalShiftsSummary> GetShiftsSummaryAsync(int? internshipRequirementId = null, int? year = null);
        Task<List<int>> GetAvailableYearsAsync(); // Dla starego SMK
        Task<List<InternshipRequirement>> GetAvailableInternshipRequirementsAsync(); // Dla nowego SMK
        Task<string> GetLastShiftLocationAsync(); // Pobieranie ostatnio używanej lokalizacji
    }
}
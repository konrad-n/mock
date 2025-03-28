using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Specialization;

namespace SledzSpecke.App.Services.MedicalShifts
{
    public partial class MedicalShiftsService : IMedicalShiftsService
    {
        private readonly IDatabaseService databaseService;
        private readonly IAuthService authService;
        private readonly ISpecializationService specializationService;

        public MedicalShiftsService(
            IDatabaseService databaseService,
            IAuthService authService,
            ISpecializationService specializationService)
        {
            this.databaseService = databaseService;
            this.authService = authService;
            this.specializationService = specializationService;
        }

        public async Task<List<RealizedMedicalShiftOldSMK>> GetOldSMKShiftsAsync(int year)
        {
            var user = await this.authService.GetCurrentUserAsync();
            if (user == null)
            {
                return new List<RealizedMedicalShiftOldSMK>();
            }

            var query = "SELECT * FROM RealizedMedicalShiftOldSMK WHERE SpecializationId = ? AND Year = ? ORDER BY StartDate DESC";
            var shifts = await this.databaseService.QueryAsync<RealizedMedicalShiftOldSMK>(query, user.SpecializationId, year);
            var filteredShifts = shifts.Where(s => s.SpecializationId == user.SpecializationId).ToList();

            return filteredShifts;
        }

        public async Task<RealizedMedicalShiftOldSMK> GetOldSMKShiftAsync(int shiftId)
        {
            var user = await this.authService.GetCurrentUserAsync();
            if (user == null)
            {
                return null;
            }

            var query = "SELECT * FROM RealizedMedicalShiftOldSMK WHERE ShiftId = ? AND SpecializationId = ?";
            var shifts = await this.databaseService.QueryAsync<RealizedMedicalShiftOldSMK>(query, shiftId, user.SpecializationId);

            if (shifts.Count > 0)
            {
                return shifts[0];
            }
            else
            {
                var checkQuery = "SELECT COUNT(*) FROM RealizedMedicalShiftOldSMK WHERE ShiftId = ?";
                await this.databaseService.QueryAsync<CountResult>(checkQuery, shiftId);

                return null;
            }
        }

        public async Task<bool> SaveOldSMKShiftAsync(RealizedMedicalShiftOldSMK shift)
        {
            var user = await this.authService.GetCurrentUserAsync();
            if (user == null)
            {
                return false;
            }

            shift.SpecializationId = user.SpecializationId;

            if (shift.ShiftId == 0)
            {
                int result = await this.databaseService.InsertAsync(shift);
                return result > 0;
            }
            else
            {
                var existingShift = await this.GetOldSMKShiftAsync(shift.ShiftId);
                if (existingShift != null && existingShift.SpecializationId != user.SpecializationId)
                {
                    return false;
                }

                int result = await this.databaseService.UpdateAsync(shift);
                return result > 0;
            }
        }

        public async Task<bool> DeleteOldSMKShiftAsync(int shiftId)
        {
            var user = await this.authService.GetCurrentUserAsync();
            if (user == null)
            {
                return false;
            }

            var shift = await this.GetOldSMKShiftAsync(shiftId);
            if (shift == null)
            {
                return false;
            }

            var query = "DELETE FROM RealizedMedicalShiftOldSMK WHERE ShiftId = ? AND SpecializationId = ?";
            int result = await this.databaseService.ExecuteAsync(query, shiftId, user.SpecializationId);

            return result > 0;
        }

        public async Task<List<RealizedMedicalShiftNewSMK>> GetNewSMKShiftsAsync(int internshipRequirementId)
        {
            var currentModule = await this.specializationService.GetCurrentModuleAsync();
            int? moduleId = currentModule?.ModuleId;
            var query = "SELECT * FROM RealizedMedicalShiftNewSMK WHERE InternshipRequirementId = ? AND ModuleId = ?";
            var shifts = await this.databaseService.QueryAsync<RealizedMedicalShiftNewSMK>(query, internshipRequirementId, moduleId);

            return shifts;
        }

        public async Task<RealizedMedicalShiftNewSMK> GetNewSMKShiftAsync(int shiftId)
        {
            await this.databaseService.InitializeAsync();

            // Pobierz dyżur o danym ID
            var query = "SELECT * FROM RealizedMedicalShiftNewSMK WHERE ShiftId = ?";
            var shifts = await this.databaseService.QueryAsync<RealizedMedicalShiftNewSMK>(query, shiftId);
            var shift = shifts.FirstOrDefault();

            if (shift != null)
            {
                // Uzupełnij nazwę stażu
                var internshipRequirements = await this.GetAvailableInternshipRequirementsAsync();
                var requirement = internshipRequirements.FirstOrDefault(r => r.Id == shift.InternshipRequirementId);
                shift.InternshipName = requirement?.Name ?? string.Empty;
            }

            return shift;
        }

        public async Task<bool> SaveNewSMKShiftAsync(RealizedMedicalShiftNewSMK shift)
        {
            var currentModule = await this.specializationService.GetCurrentModuleAsync();
            if (currentModule == null)
            {
                return false;
            }

            shift.ModuleId = currentModule.ModuleId;
            var specialization = await this.specializationService.GetCurrentSpecializationAsync();
            if (specialization == null)
            {
                return false;
            }

            shift.SpecializationId = specialization.SpecializationId;
            int result = await this.databaseService.InsertAsync(shift);
            return result > 0;
        }

        public async Task<bool> DeleteNewSMKShiftAsync(int shiftId)
        {
            await this.databaseService.InitializeAsync();
            var shift = await this.GetNewSMKShiftAsync(shiftId);
            if (shift == null)
            {
                return false;
            }

            await this.databaseService.DeleteAsync(shift);
            return true;
        }

        public async Task<MedicalShiftsSummary> GetShiftsSummaryAsync(int? year = null, int? internshipRequirementId = null)
        {
            var summary = new MedicalShiftsSummary();

            var currentModule = await this.specializationService.GetCurrentModuleAsync();
            int? moduleId = currentModule?.ModuleId;
            var specialization = await this.specializationService.GetCurrentSpecializationAsync();
            if (specialization == null)
            {
                return summary;
            }

            if (internshipRequirementId.HasValue)
            {
                string query = "SELECT * FROM RealizedMedicalShiftNewSMK WHERE SpecializationId = ? AND InternshipRequirementId = ? AND ModuleId = ?";
                var newSmkShifts = await this.databaseService.QueryAsync<RealizedMedicalShiftNewSMK>(
                    query, specialization.SpecializationId, internshipRequirementId.Value, moduleId);

                foreach (var shift in newSmkShifts)
                {
                    summary.TotalHours += shift.Hours;
                    summary.TotalMinutes += shift.Minutes;

                    if (shift.SyncStatus == SyncStatus.Synced)
                    {
                        summary.ApprovedHours += shift.Hours;
                        summary.ApprovedMinutes += shift.Minutes;
                    }
                }
            }
            else if (year.HasValue)
            {
                string query = "SELECT * FROM RealizedMedicalShiftOldSMK WHERE SpecializationId = ? AND Year = ?";
                var oldSmkShifts = await this.databaseService.QueryAsync<RealizedMedicalShiftOldSMK>(
                    query, specialization.SpecializationId, year.Value);

                foreach (var shift in oldSmkShifts)
                {
                    summary.TotalHours += shift.Hours;
                    summary.TotalMinutes += shift.Minutes;

                    if (shift.SyncStatus == SyncStatus.Synced)
                    {
                        summary.ApprovedHours += shift.Hours;
                        summary.ApprovedMinutes += shift.Minutes;
                    }
                }
            }
            else
            {
                string query = "SELECT * FROM RealizedMedicalShiftNewSMK WHERE SpecializationId = ? AND ModuleId = ?";
                var newSmkShifts = await this.databaseService.QueryAsync<RealizedMedicalShiftNewSMK>(
                    query, specialization.SpecializationId, moduleId);

                foreach (var shift in newSmkShifts)
                {
                    summary.TotalHours += shift.Hours;
                    summary.TotalMinutes += shift.Minutes;

                    if (shift.SyncStatus == SyncStatus.Synced)
                    {
                        summary.ApprovedHours += shift.Hours;
                        summary.ApprovedMinutes += shift.Minutes;
                    }
                }
            }

            summary.NormalizeTime();

            return summary;
        }

        public async Task<List<int>> GetAvailableYearsAsync()
        {
            var specialization = await this.specializationService.GetCurrentSpecializationAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                Converters = { new JsonStringEnumConverter() }
            };

            var specializationStructure = JsonSerializer.Deserialize<SpecializationStructure>(
                specialization.ProgramStructure, options);

            int totalYears = 0;
            if (specializationStructure.TotalDuration != null)
            {
                totalYears = specializationStructure.TotalDuration.Years;
                if (specializationStructure.TotalDuration.Months > 0)
                {
                    totalYears++;
                }
            }
            else
            {
                totalYears = specializationStructure.Modules?.Sum(m => m.Duration?.Years ?? 0) ?? 0;
                int additionalMonths = specializationStructure.Modules?.Sum(m => m.Duration?.Months ?? 0) ?? 0;
                if (additionalMonths > 0)
                {
                    totalYears += (additionalMonths / 12) + (additionalMonths % 12 > 0 ? 1 : 0);
                }
            }

            totalYears = Math.Max(1, totalYears);
            totalYears = Math.Min(6, totalYears);

            var years = Enumerable.Range(1, totalYears).ToList();

            return years;
        }

        public async Task<List<InternshipRequirement>> GetAvailableInternshipRequirementsAsync()
        {
            var module = await this.specializationService.GetCurrentModuleAsync();
            if (module == null || string.IsNullOrEmpty(module.Structure))
            {
                return new List<InternshipRequirement>();
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                Converters = { new JsonStringEnumConverter() }
            };

            var moduleStructure = JsonSerializer.Deserialize<ModuleStructure>(module.Structure, options);
            return moduleStructure?.Internships ?? new List<InternshipRequirement>();
        }

        public async Task<string> GetLastShiftLocationAsync()
        {
            await this.databaseService.InitializeAsync();

            var specialization = await this.specializationService.GetCurrentSpecializationAsync();
            if (specialization == null)
            {
                return string.Empty;
            }

            var query = "SELECT * FROM RealizedMedicalShiftOldSMK WHERE SpecializationId = ? ORDER BY ShiftId DESC LIMIT 1";
            var shifts = await this.databaseService.QueryAsync<RealizedMedicalShiftOldSMK>(query, specialization.SpecializationId);
            var lastShift = shifts.FirstOrDefault();

            return lastShift?.Location ?? string.Empty;
        }

        private class CountResult
        {
            public int Count { get; set; }
        }
    }
}
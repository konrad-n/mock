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
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Logging;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.Exceptions;

namespace SledzSpecke.App.Services.MedicalShifts
{
    public partial class MedicalShiftsService : BaseService, IMedicalShiftsService
    {
        private readonly IDatabaseService databaseService;
        private readonly IAuthService authService;
        private readonly ISpecializationService specializationService;

        public MedicalShiftsService(
            IDatabaseService databaseService,
            IAuthService authService,
            ISpecializationService specializationService,
            IExceptionHandlerService exceptionHandler,
            ILoggingService loggingService)
            : base(exceptionHandler, loggingService)
        {
            this.databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            this.authService = authService ?? throw new ArgumentNullException(nameof(authService));
            this.specializationService = specializationService ?? throw new ArgumentNullException(nameof(specializationService));
        }

        public async Task<List<RealizedMedicalShiftOldSMK>> GetOldSMKShiftsAsync(int year)
        {
            return await SafeExecuteAsync(async () =>
            {
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    Logger.LogWarning("Próba pobrania dyżurów bez zalogowanego użytkownika",
                        new Dictionary<string, object> { { "Year", year } });
                    return new List<RealizedMedicalShiftOldSMK>();
                }

                var query = "SELECT * FROM RealizedMedicalShiftOldSMK WHERE SpecializationId = ? AND Year = ? ORDER BY StartDate DESC";
                var shifts = await this.databaseService.QueryAsync<RealizedMedicalShiftOldSMK>(query, user.SpecializationId, year);
                var filteredShifts = shifts.Where(s => s.SpecializationId == user.SpecializationId).ToList();

                Logger.LogInformation($"Pobrano {filteredShifts.Count} dyżurów dla roku {year}",
                    new Dictionary<string, object> { { "Year", year }, { "Count", filteredShifts.Count } });

                return filteredShifts;
            },
            $"Wystąpił błąd podczas pobierania dyżurów dla roku {year}",
            new Dictionary<string, object> { { "Year", year } },
            withRetry: true);
        }

        public async Task<RealizedMedicalShiftOldSMK> GetOldSMKShiftAsync(int shiftId)
        {
            return await SafeExecuteAsync(async () =>
            {
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    Logger.LogWarning("Próba pobrania dyżuru bez zalogowanego użytkownika",
                        new Dictionary<string, object> { { "ShiftId", shiftId } });
                    return null;
                }

                var query = "SELECT * FROM RealizedMedicalShiftOldSMK WHERE ShiftId = ? AND SpecializationId = ?";
                var shifts = await this.databaseService.QueryAsync<RealizedMedicalShiftOldSMK>(query, shiftId, user.SpecializationId);

                if (shifts.Count > 0)
                {
                    Logger.LogInformation($"Pobrano dyżur o ID {shiftId}",
                        new Dictionary<string, object> { { "ShiftId", shiftId } });
                    return shifts[0];
                }
                else
                {
                    var checkQuery = "SELECT COUNT(*) FROM RealizedMedicalShiftOldSMK WHERE ShiftId = ?";
                    var count = await this.databaseService.QueryAsync<CountResult>(checkQuery, shiftId);

                    if (count.Count > 0 && count[0].Count > 0)
                    {
                        // Dyżur istnieje, ale należy do innego użytkownika
                        Logger.LogWarning($"Próba dostępu do dyżuru o ID {shiftId} należącego do innego użytkownika",
                            new Dictionary<string, object> { { "ShiftId", shiftId }, { "UserId", user.UserId } });

                        throw new ResourceNotFoundException(
                            $"Medical shift with ID {shiftId} not found for the current user",
                            $"Nie znaleziono dyżuru o ID {shiftId} dla bieżącego użytkownika.",
                            null,
                            new Dictionary<string, object> { { "ShiftId", shiftId }, { "UserId", user.UserId } });
                    }

                    Logger.LogInformation($"Nie znaleziono dyżuru o ID {shiftId}",
                        new Dictionary<string, object> { { "ShiftId", shiftId } });
                    return null;
                }
            },
            $"Wystąpił błąd podczas pobierania dyżuru o ID {shiftId}",
            new Dictionary<string, object> { { "ShiftId", shiftId } });
        }

        public async Task<bool> SaveOldSMKShiftAsync(RealizedMedicalShiftOldSMK shift)
        {
            return await SafeExecuteAsync(async () =>
            {
                if (shift == null)
                {
                    throw new InvalidInputException(
                        "Shift cannot be null",
                        "Obiekt dyżuru nie może być pusty.");
                }

                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    Logger.LogWarning("Próba zapisania dyżuru bez zalogowanego użytkownika");
                    throw new InvalidInputException(
                        "User not logged in",
                        "Aby zapisać dyżur, musisz być zalogowany.");
                }

                shift.SpecializationId = user.SpecializationId;

                if (shift.ShiftId == 0)
                {
                    // Nowy dyżur
                    Logger.LogInformation("Dodawanie nowego dyżuru",
                        new Dictionary<string, object> { { "SpecializationId", shift.SpecializationId } });

                    int result = await this.databaseService.InsertAsync(shift);
                    return result > 0;
                }
                else
                {
                    // Edycja istniejącego dyżuru
                    var existingShift = await this.GetOldSMKShiftAsync(shift.ShiftId);
                    if (existingShift != null && existingShift.SpecializationId != user.SpecializationId)
                    {
                        Logger.LogWarning("Próba edycji dyżuru należącego do innego użytkownika",
                            new Dictionary<string, object> {
                                { "ShiftId", shift.ShiftId },
                                { "RequestedSpecId", shift.SpecializationId },
                                { "ActualSpecId", existingShift.SpecializationId }
                            });

                        throw new ResourceNotFoundException(
                            "Cannot edit shift belonging to another user",
                            "Nie można edytować dyżuru należącego do innego użytkownika.");
                    }

                    Logger.LogInformation($"Aktualizacja dyżuru o ID {shift.ShiftId}",
                        new Dictionary<string, object> { { "ShiftId", shift.ShiftId } });

                    int result = await this.databaseService.UpdateAsync(shift);
                    return result > 0;
                }
            },
            "Wystąpił błąd podczas zapisywania dyżuru",
            new Dictionary<string, object> {
                { "ShiftId", shift?.ShiftId ?? 0 },
                { "IsNew", shift?.ShiftId == 0 }
            });
        }

        public async Task<bool> DeleteOldSMKShiftAsync(int shiftId)
        {
            return await SafeExecuteAsync(async () =>
            {
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    Logger.LogWarning("Próba usunięcia dyżuru bez zalogowanego użytkownika",
                        new Dictionary<string, object> { { "ShiftId", shiftId } });

                    throw new InvalidInputException(
                        "User not logged in",
                        "Aby usunąć dyżur, musisz być zalogowany.");
                }

                var shift = await this.GetOldSMKShiftAsync(shiftId);
                if (shift == null)
                {
                    Logger.LogWarning($"Próba usunięcia nieistniejącego dyżuru o ID {shiftId}",
                        new Dictionary<string, object> { { "ShiftId", shiftId } });

                    return false;
                }

                if (shift.SyncStatus == SyncStatus.Synced)
                {
                    Logger.LogWarning($"Próba usunięcia zsynchronizowanego dyżuru o ID {shiftId}",
                        new Dictionary<string, object> { { "ShiftId", shiftId } });

                    throw new BusinessRuleViolationException(
                        "Cannot delete synced shift",
                        "Nie można usunąć zsynchronizowanego dyżuru.");
                }

                Logger.LogInformation($"Usuwanie dyżuru o ID {shiftId}",
                    new Dictionary<string, object> { { "ShiftId", shiftId } });

                var query = "DELETE FROM RealizedMedicalShiftOldSMK WHERE ShiftId = ? AND SpecializationId = ?";
                int result = await this.databaseService.ExecuteAsync(query, shiftId, user.SpecializationId);

                return result > 0;
            },
            $"Wystąpił błąd podczas usuwania dyżuru o ID {shiftId}",
            new Dictionary<string, object> { { "ShiftId", shiftId } });
        }

        public async Task<List<RealizedMedicalShiftNewSMK>> GetNewSMKShiftsAsync(int internshipRequirementId)
        {
            return await SafeExecuteAsync(async () =>
            {
                var currentModule = await this.specializationService.GetCurrentModuleAsync();
                int? moduleId = currentModule?.ModuleId;

                if (!moduleId.HasValue)
                {
                    Logger.LogWarning("Próba pobrania dyżurów bez aktywnego modułu",
                        new Dictionary<string, object> { { "InternshipRequirementId", internshipRequirementId } });

                    return new List<RealizedMedicalShiftNewSMK>();
                }

                var query = "SELECT * FROM RealizedMedicalShiftNewSMK WHERE InternshipRequirementId = ? AND ModuleId = ?";
                var shifts = await this.databaseService.QueryAsync<RealizedMedicalShiftNewSMK>(query, internshipRequirementId, moduleId);

                Logger.LogInformation($"Pobrano {shifts.Count} dyżurów dla wymagania staży o ID {internshipRequirementId}",
                    new Dictionary<string, object> {
                        { "InternshipRequirementId", internshipRequirementId },
                        { "ModuleId", moduleId },
                        { "Count", shifts.Count }
                    });

                return shifts;
            },
            $"Wystąpił błąd podczas pobierania dyżurów dla wymagania staży o ID {internshipRequirementId}",
            new Dictionary<string, object> { { "InternshipRequirementId", internshipRequirementId } },
            withRetry: true);
        }

        public async Task<RealizedMedicalShiftNewSMK> GetNewSMKShiftAsync(int shiftId)
        {
            return await SafeExecuteAsync(async () =>
            {
                await this.databaseService.InitializeAsync();

                // Pobierz dyżur o danym ID
                var query = "SELECT * FROM RealizedMedicalShiftNewSMK WHERE ShiftId = ?";
                var shifts = await this.databaseService.QueryAsync<RealizedMedicalShiftNewSMK>(query, shiftId);
                var shift = shifts.FirstOrDefault();

                if (shift == null)
                {
                    Logger.LogWarning($"Nie znaleziono dyżuru o ID {shiftId}",
                        new Dictionary<string, object> { { "ShiftId", shiftId } });

                    return null;
                }

                // Uzupełnij nazwę stażu
                try
                {
                    var internshipRequirements = await this.GetAvailableInternshipRequirementsAsync();
                    var requirement = internshipRequirements.FirstOrDefault(r => r.Id == shift.InternshipRequirementId);
                    shift.InternshipName = requirement?.Name ?? string.Empty;

                    Logger.LogInformation($"Pobrano dyżur o ID {shiftId}",
                        new Dictionary<string, object> { { "ShiftId", shiftId } });
                }
                catch (Exception ex)
                {
                    // W przypadku błędu podczas uzyskiwania nazwy stażu, kontynuuj bez nazwy
                    Logger.LogWarning($"Nie udało się uzyskać nazwy stażu dla dyżuru o ID {shiftId}: {ex.Message}",
                        new Dictionary<string, object> { { "ShiftId", shiftId }, { "Error", ex.Message } });

                    shift.InternshipName = string.Empty;
                }

                return shift;
            },
            $"Wystąpił błąd podczas pobierania dyżuru o ID {shiftId}",
            new Dictionary<string, object> { { "ShiftId", shiftId } });
        }

        public async Task<bool> SaveNewSMKShiftAsync(RealizedMedicalShiftNewSMK shift)
        {
            return await SafeExecuteAsync(async () =>
            {
                if (shift == null)
                {
                    throw new InvalidInputException(
                        "Shift cannot be null",
                        "Obiekt dyżuru nie może być pusty.");
                }

                var currentModule = await this.specializationService.GetCurrentModuleAsync();
                if (currentModule == null)
                {
                    Logger.LogWarning("Próba zapisania dyżuru bez aktywnego modułu");

                    throw new ResourceNotFoundException(
                        "Current module not found",
                        "Nie znaleziono aktywnego modułu specjalizacji.");
                }

                shift.ModuleId = currentModule.ModuleId;

                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    Logger.LogWarning("Próba zapisania dyżuru bez aktywnej specjalizacji");

                    throw new ResourceNotFoundException(
                        "Current specialization not found",
                        "Nie znaleziono aktywnej specjalizacji.");
                }

                shift.SpecializationId = specialization.SpecializationId;

                Logger.LogInformation($"{(shift.ShiftId > 0 ? "Aktualizacja" : "Dodawanie")} dyżuru dla modułu {currentModule.ModuleId}",
                    new Dictionary<string, object> {
                        { "ShiftId", shift.ShiftId },
                        { "ModuleId", currentModule.ModuleId },
                        { "SpecializationId", specialization.SpecializationId }
                    });

                int result = await this.databaseService.InsertAsync(shift);
                return result > 0;
            },
            "Wystąpił błąd podczas zapisywania dyżuru",
            new Dictionary<string, object> {
                { "ShiftId", shift?.ShiftId ?? 0 },
                { "InternshipRequirementId", shift?.InternshipRequirementId ?? 0 }
            });
        }

        public async Task<bool> DeleteNewSMKShiftAsync(int shiftId)
        {
            return await SafeExecuteAsync(async () =>
            {
                await this.databaseService.InitializeAsync();
                var shift = await this.GetNewSMKShiftAsync(shiftId);

                if (shift == null)
                {
                    Logger.LogWarning($"Próba usunięcia nieistniejącego dyżuru o ID {shiftId}",
                        new Dictionary<string, object> { { "ShiftId", shiftId } });

                    return false;
                }

                if (shift.SyncStatus == SyncStatus.Synced)
                {
                    Logger.LogWarning($"Próba usunięcia zsynchronizowanego dyżuru o ID {shiftId}",
                        new Dictionary<string, object> { { "ShiftId", shiftId } });

                    throw new BusinessRuleViolationException(
                        "Cannot delete synced shift",
                        "Nie można usunąć zsynchronizowanego dyżuru.");
                }

                Logger.LogInformation($"Usuwanie dyżuru o ID {shiftId}",
                    new Dictionary<string, object> { { "ShiftId", shiftId } });

                await this.databaseService.DeleteAsync(shift);
                return true;
            },
            $"Wystąpił błąd podczas usuwania dyżuru o ID {shiftId}",
            new Dictionary<string, object> { { "ShiftId", shiftId } });
        }

        public async Task<MedicalShiftsSummary> GetShiftsSummaryAsync(int? year = null, int? internshipRequirementId = null)
        {
            return await SafeExecuteAsync(async () =>
            {
                var summary = new MedicalShiftsSummary();

                var currentModule = await this.specializationService.GetCurrentModuleAsync();
                int? moduleId = currentModule?.ModuleId;

                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    Logger.LogWarning("Próba pobrania podsumowania dyżurów bez aktywnej specjalizacji");
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

                    Logger.LogInformation($"Pobrano podsumowanie dyżurów dla wymagania staży o ID {internshipRequirementId}",
                        new Dictionary<string, object> {
                            { "InternshipRequirementId", internshipRequirementId },
                            { "TotalHours", summary.TotalHours },
                            { "TotalMinutes", summary.TotalMinutes }
                        });
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

                    Logger.LogInformation($"Pobrano podsumowanie dyżurów dla roku {year}",
                        new Dictionary<string, object> {
                            { "Year", year },
                            { "TotalHours", summary.TotalHours },
                            { "TotalMinutes", summary.TotalMinutes }
                        });
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

                    Logger.LogInformation($"Pobrano podsumowanie wszystkich dyżurów dla modułu {moduleId}",
                        new Dictionary<string, object> {
                            { "ModuleId", moduleId },
                            { "TotalHours", summary.TotalHours },
                            { "TotalMinutes", summary.TotalMinutes }
                        });
                }

                summary.NormalizeTime();

                return summary;
            },
            "Wystąpił błąd podczas pobierania podsumowania dyżurów",
            new Dictionary<string, object> {
                { "Year", year },
                { "InternshipRequirementId", internshipRequirementId }
            });
        }

        public async Task<List<int>> GetAvailableYearsAsync()
        {
            return await SafeExecuteAsync(async () =>
            {
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    Logger.LogWarning("Próba pobrania dostępnych lat bez aktywnej specjalizacji");

                    throw new ResourceNotFoundException(
                        "Current specialization not found",
                        "Nie znaleziono aktywnej specjalizacji.");
                }

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

                Logger.LogInformation($"Pobrano {years.Count} dostępnych lat dla specjalizacji",
                    new Dictionary<string, object> { { "TotalYears", totalYears } });

                return years;
            },
            "Wystąpił błąd podczas pobierania dostępnych lat",
            new Dictionary<string, object>());
        }

        public async Task<List<InternshipRequirement>> GetAvailableInternshipRequirementsAsync()
        {
            return await SafeExecuteAsync(async () =>
            {
                var module = await this.specializationService.GetCurrentModuleAsync();
                if (module == null || string.IsNullOrEmpty(module.Structure))
                {
                    Logger.LogWarning("Próba pobrania wymagań stażowych bez aktywnego modułu lub z pustą strukturą");

                    throw new ResourceNotFoundException(
                        "Current module not found or has empty structure",
                        "Nie znaleziono aktywnego modułu specjalizacji lub moduł ma pustą strukturę.");
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    Converters = { new JsonStringEnumConverter() }
                };

                var moduleStructure = JsonSerializer.Deserialize<ModuleStructure>(module.Structure, options);
                var requirements = moduleStructure?.Internships ?? new List<InternshipRequirement>();

                Logger.LogInformation($"Pobrano {requirements.Count} wymagań stażowych dla modułu {module.ModuleId}",
                    new Dictionary<string, object> { { "ModuleId", module.ModuleId }, { "Count", requirements.Count } });

                return requirements;
            },
            "Wystąpił błąd podczas pobierania wymagań stażowych",
            new Dictionary<string, object>());
        }

        public async Task<string> GetLastShiftLocationAsync()
        {
            return await SafeExecuteAsync(async () =>
            {
                await this.databaseService.InitializeAsync();

                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    Logger.LogWarning("Próba pobrania ostatniej lokalizacji dyżuru bez aktywnej specjalizacji");
                    return string.Empty;
                }

                var query = "SELECT * FROM RealizedMedicalShiftOldSMK WHERE SpecializationId = ? ORDER BY ShiftId DESC LIMIT 1";
                var shifts = await this.databaseService.QueryAsync<RealizedMedicalShiftOldSMK>(query, specialization.SpecializationId);
                var lastShift = shifts.FirstOrDefault();

                if (lastShift != null)
                {
                    Logger.LogInformation($"Pobrano ostatnią lokalizację dyżuru: {lastShift.Location ?? string.Empty}",
                        new Dictionary<string, object> { { "ShiftId", lastShift.ShiftId }, { "Location", lastShift.Location } });
                }
                else
                {
                    Logger.LogInformation("Nie znaleziono żadnych poprzednich dyżurów");
                }

                return lastShift?.Location ?? string.Empty;
            },
            "Wystąpił błąd podczas pobierania ostatniej lokalizacji dyżuru",
            new Dictionary<string, object>());
        }

        private class CountResult
        {
            public int Count { get; set; }
        }
    }
}
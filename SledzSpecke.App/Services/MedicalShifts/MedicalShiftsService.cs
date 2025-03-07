using System;
using System.Collections.Generic;
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
    public class MedicalShiftsService : IMedicalShiftsService
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

        // Implementacja metod dla starego SMK
        public async Task<List<RealizedMedicalShiftOldSMK>> GetOldSMKShiftsAsync(int year)
        {
            try
            {
                // Inicjalizacja bazy danych w razie potrzeby
                await this.databaseService.InitializeAsync();

                // Pobierz obecną specjalizację
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return new List<RealizedMedicalShiftOldSMK>();
                }

                // Pobierz wszystkie dyżury dla danej specjalizacji i roku szkolenia
                var query = "SELECT * FROM RealizedMedicalShiftOldSMK WHERE SpecializationId = ? AND Year = ? ORDER BY StartDate DESC";
                var shifts = await this.databaseService.QueryAsync<RealizedMedicalShiftOldSMK>(query, specialization.SpecializationId, year);

                return shifts;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas pobierania dyżurów starego SMK: {ex.Message}");
                return new List<RealizedMedicalShiftOldSMK>();
            }
        }

        public async Task<RealizedMedicalShiftOldSMK> GetOldSMKShiftAsync(int shiftId)
        {
            try
            {
                // Inicjalizacja bazy danych w razie potrzeby
                await this.databaseService.InitializeAsync();

                // Pobierz dyżur o danym ID
                var query = "SELECT * FROM RealizedMedicalShiftOldSMK WHERE ShiftId = ?";
                var shifts = await this.databaseService.QueryAsync<RealizedMedicalShiftOldSMK>(query, shiftId);

                return shifts.FirstOrDefault();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas pobierania dyżuru starego SMK: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> SaveOldSMKShiftAsync(RealizedMedicalShiftOldSMK shift)
        {
            try
            {
                // Inicjalizacja bazy danych w razie potrzeby
                await this.databaseService.InitializeAsync();

                // Pobierz obecną specjalizację
                if (shift.SpecializationId <= 0)
                {
                    var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                    if (specialization == null)
                    {
                        return false;
                    }

                    shift.SpecializationId = specialization.SpecializationId;
                }

                // Ustaw status synchronizacji
                if (shift.SyncStatus == 0)
                {
                    shift.SyncStatus = SyncStatus.NotSynced;
                }

                // Zapisz dyżur
                if (shift.ShiftId > 0)
                {
                    // Aktualizacja istniejącego dyżuru
                    await this.databaseService.UpdateAsync(shift);
                }
                else
                {
                    // Dodanie nowego dyżuru
                    await this.databaseService.InsertAsync(shift);
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas zapisywania dyżuru starego SMK: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteOldSMKShiftAsync(int shiftId)
        {
            try
            {
                // Inicjalizacja bazy danych w razie potrzeby
                await this.databaseService.InitializeAsync();

                // Pobierz dyżur o danym ID
                var shift = await this.GetOldSMKShiftAsync(shiftId);
                if (shift == null)
                {
                    return false;
                }

                // Usuń dyżur
                await this.databaseService.DeleteAsync(shift);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas usuwania dyżuru starego SMK: {ex.Message}");
                return false;
            }
        }

        // Implementacja metod dla nowego SMK
        public async Task<List<RealizedMedicalShiftNewSMK>> GetNewSMKShiftsAsync(int internshipRequirementId)
        {
            try
            {
                // Inicjalizacja bazy danych w razie potrzeby
                await this.databaseService.InitializeAsync();

                // Pobierz obecną specjalizację
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return new List<RealizedMedicalShiftNewSMK>();
                }

                // Pobierz wszystkie dyżury dla danej specjalizacji i wymagania stażowego
                var query = "SELECT * FROM RealizedMedicalShiftNewSMK WHERE SpecializationId = ? AND InternshipRequirementId = ? ORDER BY StartDate DESC";
                var shifts = await this.databaseService.QueryAsync<RealizedMedicalShiftNewSMK>(query, specialization.SpecializationId, internshipRequirementId);

                // Uzupełnij nazwę stażu
                var internshipRequirements = await this.GetAvailableInternshipRequirementsAsync();
                var requirement = internshipRequirements.FirstOrDefault(r => r.Id == internshipRequirementId);

                foreach (var shift in shifts)
                {
                    shift.InternshipName = requirement?.Name ?? string.Empty;
                }

                return shifts;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas pobierania dyżurów nowego SMK: {ex.Message}");
                return new List<RealizedMedicalShiftNewSMK>();
            }
        }

        public async Task<RealizedMedicalShiftNewSMK> GetNewSMKShiftAsync(int shiftId)
        {
            try
            {
                // Inicjalizacja bazy danych w razie potrzeby
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas pobierania dyżuru nowego SMK: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> SaveNewSMKShiftAsync(RealizedMedicalShiftNewSMK shift)
        {
            try
            {
                // Inicjalizacja bazy danych w razie potrzeby
                await this.databaseService.InitializeAsync();

                // Pobierz obecną specjalizację
                if (shift.SpecializationId <= 0)
                {
                    var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                    if (specialization == null)
                    {
                        return false;
                    }

                    shift.SpecializationId = specialization.SpecializationId;
                }

                // Ustaw status synchronizacji
                if (shift.SyncStatus == 0)
                {
                    shift.SyncStatus = SyncStatus.NotSynced;
                }

                // Zapisz dyżur
                if (shift.ShiftId > 0)
                {
                    // Aktualizacja istniejącego dyżuru
                    await this.databaseService.UpdateAsync(shift);
                }
                else
                {
                    // Dodanie nowego dyżuru
                    await this.databaseService.InsertAsync(shift);
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas zapisywania dyżuru nowego SMK: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteNewSMKShiftAsync(int shiftId)
        {
            try
            {
                // Inicjalizacja bazy danych w razie potrzeby
                await this.databaseService.InitializeAsync();

                // Pobierz dyżur o danym ID
                var shift = await this.GetNewSMKShiftAsync(shiftId);
                if (shift == null)
                {
                    return false;
                }

                // Usuń dyżur
                await this.databaseService.DeleteAsync(shift);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas usuwania dyżuru nowego SMK: {ex.Message}");
                return false;
            }
        }

        // Implementacja wspólnych metod
        public async Task<MedicalShiftsSummary> GetShiftsSummaryAsync(int? internshipRequirementId = null, int? year = null)
        {
            try
            {
                // Inicjalizacja bazy danych w razie potrzeby
                await this.databaseService.InitializeAsync();

                // Pobierz obecną specjalizację
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return new MedicalShiftsSummary();
                }

                // Pobierz dyżury w zależności od wersji SMK
                var user = await this.authService.GetCurrentUserAsync();

                if (user.SmkVersion == SmkVersion.Old)
                {
                    // Dla starego SMK - pobierz dyżury dla danego roku
                    if (!year.HasValue)
                    {
                        return new MedicalShiftsSummary();
                    }

                    var shifts = await this.GetOldSMKShiftsAsync(year.Value);

                    // Oblicz sumę godzin i minut
                    int totalHours = 0;
                    int totalMinutes = 0;
                    int approvedHours = 0;
                    int approvedMinutes = 0;

                    foreach (var shift in shifts)
                    {
                        totalHours += shift.Hours;
                        totalMinutes += shift.Minutes;

                        if (shift.SyncStatus == SyncStatus.Synced)
                        {
                            approvedHours += shift.Hours;
                            approvedMinutes += shift.Minutes;
                        }
                    }

                    var summary = new MedicalShiftsSummary
                    {
                        TotalHours = totalHours,
                        TotalMinutes = totalMinutes,
                        ApprovedHours = approvedHours,
                        ApprovedMinutes = approvedMinutes
                    };

                    // Normalizacja czasu
                    summary.NormalizeTime();

                    return summary;
                }
                else
                {
                    // Dla nowego SMK - pobierz dyżury dla danego wymagania stażowego
                    if (!internshipRequirementId.HasValue)
                    {
                        return new MedicalShiftsSummary();
                    }

                    var shifts = await this.GetNewSMKShiftsAsync(internshipRequirementId.Value);

                    // Oblicz sumę godzin i minut
                    int totalHours = 0;
                    int totalMinutes = 0;
                    int approvedHours = 0;
                    int approvedMinutes = 0;

                    foreach (var shift in shifts)
                    {
                        totalHours += shift.Hours;
                        totalMinutes += shift.Minutes;

                        if (shift.SyncStatus == SyncStatus.Synced)
                        {
                            approvedHours += shift.Hours;
                            approvedMinutes += shift.Minutes;
                        }
                    }

                    var summary = new MedicalShiftsSummary
                    {
                        TotalHours = totalHours,
                        TotalMinutes = totalMinutes,
                        ApprovedHours = approvedHours,
                        ApprovedMinutes = approvedMinutes
                    };

                    // Normalizacja czasu
                    summary.NormalizeTime();

                    return summary;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas pobierania podsumowania dyżurów: {ex.Message}");
                return new MedicalShiftsSummary();
            }
        }

        public async Task<List<int>> GetAvailableYearsAsync()
        {
            try
            {
                // Pobierz obecną specjalizację
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return new List<int>();
                }

                // Oblicz liczbę lat na podstawie planowanego czasu trwania specjalizacji
                var years = (specialization.PlannedEndDate.Year - specialization.StartDate.Year) + 1;

                // Ogranicz do co najmniej 1 roku i maksymalnie 6 lat
                years = Math.Max(1, Math.Min(years, 6));

                // Zwróć listę lat od 1 do obliczonej liczby lat
                return Enumerable.Range(1, years).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas pobierania dostępnych lat: {ex.Message}");
                return Enumerable.Range(1, 5).ToList(); // Domyślnie 5 lat
            }
        }

        public async Task<List<InternshipRequirement>> GetAvailableInternshipRequirementsAsync()
        {
            try
            {
                // Pobierz bieżący moduł
                var module = await this.specializationService.GetCurrentModuleAsync();
                if (module == null || string.IsNullOrEmpty(module.Structure))
                {
                    return new List<InternshipRequirement>();
                }

                // Opcje deserializacji JSON
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    Converters = { new JsonStringEnumConverter() }
                };

                try
                {
                    // Deserializacja struktury modułu
                    var moduleStructure = JsonSerializer.Deserialize<ModuleStructure>(module.Structure, options);
                    return moduleStructure?.Internships ?? new List<InternshipRequirement>();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Błąd podczas deserializacji struktury modułu: {ex.Message}");
                    return new List<InternshipRequirement>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas pobierania dostępnych wymagań stażowych: {ex.Message}");
                return new List<InternshipRequirement>();
            }
        }

        public async Task<string> GetLastShiftLocationAsync()
        {
            try
            {
                // Inicjalizacja bazy danych w razie potrzeby
                await this.databaseService.InitializeAsync();

                // Pobierz obecną specjalizację
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return string.Empty;
                }

                // Pobierz ostatni dyżur
                var query = "SELECT * FROM RealizedMedicalShiftOldSMK WHERE SpecializationId = ? ORDER BY ShiftId DESC LIMIT 1";
                var shifts = await this.databaseService.QueryAsync<RealizedMedicalShiftOldSMK>(query, specialization.SpecializationId);
                var lastShift = shifts.FirstOrDefault();

                return lastShift?.Location ?? string.Empty;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas pobierania ostatniej lokalizacji dyżuru: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
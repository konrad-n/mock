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

        // Implementacja metod dla starego SMK
        public async Task<List<RealizedMedicalShiftOldSMK>> GetOldSMKShiftsAsync(int year)
        {
            try
            {
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    System.Diagnostics.Debug.WriteLine("GetOldSMKShiftsAsync: Brak zalogowanego użytkownika");
                    return new List<RealizedMedicalShiftOldSMK>();
                }

                System.Diagnostics.Debug.WriteLine($"GetOldSMKShiftsAsync: Pobieranie dyżurów dla użytkownika {user.Username}, ID={user.UserId}, SpecializationId={user.SpecializationId}, rok={year}");

                // Pobierz dyżury dla wybranego roku i specjalizacji użytkownika
                var query = "SELECT * FROM RealizedMedicalShiftOldSMK WHERE SpecializationId = ? AND Year = ? ORDER BY StartDate DESC";
                var shifts = await this.databaseService.QueryAsync<RealizedMedicalShiftOldSMK>(query, user.SpecializationId, year);

                System.Diagnostics.Debug.WriteLine($"GetOldSMKShiftsAsync: Pobrano {shifts.Count} dyżurów dla specjalizacji {user.SpecializationId}, rok {year}");

                // Dodatkowe zabezpieczenie - sprawdź, czy każdy dyżur ma poprawne ID specjalizacji
                var filteredShifts = shifts.Where(s => s.SpecializationId == user.SpecializationId).ToList();

                if (filteredShifts.Count != shifts.Count)
                {
                    System.Diagnostics.Debug.WriteLine($"GetOldSMKShiftsAsync: UWAGA! Po dodatkowym filtrowaniu zostało {filteredShifts.Count} z {shifts.Count} dyżurów");
                }

                return filteredShifts;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetOldSMKShiftsAsync: Błąd - {ex.Message}");
                return new List<RealizedMedicalShiftOldSMK>();
            }
        }

        public async Task<RealizedMedicalShiftOldSMK> GetOldSMKShiftAsync(int shiftId)
        {
            try
            {
                // Pobierz aktualnego użytkownika
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    System.Diagnostics.Debug.WriteLine("GetOldSMKShiftAsync: Brak zalogowanego użytkownika");
                    return null;
                }

                System.Diagnostics.Debug.WriteLine($"GetOldSMKShiftAsync: Pobieranie dyżuru ID={shiftId} dla użytkownika {user.Username}, SpecializationId={user.SpecializationId}");

                // Pobierz dyżur o podanym ID, ale tylko dla specjalizacji bieżącego użytkownika
                var query = "SELECT * FROM RealizedMedicalShiftOldSMK WHERE ShiftId = ? AND SpecializationId = ?";
                var shifts = await this.databaseService.QueryAsync<RealizedMedicalShiftOldSMK>(query, shiftId, user.SpecializationId);

                if (shifts.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"GetOldSMKShiftAsync: Pobrano dyżur o ID {shiftId} dla specjalizacji {user.SpecializationId}");
                    return shifts[0];
                }
                else
                {
                    // Sprawdźmy czy dyżur w ogóle istnieje (nawet jeśli należy do innego użytkownika)
                    var checkQuery = "SELECT COUNT(*) FROM RealizedMedicalShiftOldSMK WHERE ShiftId = ?";
                    var count = await this.databaseService.QueryAsync<CountResult>(checkQuery, shiftId);

                    if (count.Count > 0 && count[0].Count > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"GetOldSMKShiftAsync: Dyżur o ID {shiftId} istnieje, ale NIE należy do użytkownika {user.Username}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"GetOldSMKShiftAsync: Nie znaleziono dyżuru o ID {shiftId}");
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetOldSMKShiftAsync: Błąd - {ex.Message}");
                return null;
            }
        }

        // Klasa pomocnicza do zliczania wyników
        private class CountResult
        {
            public int Count { get; set; }
        }

        public async Task<bool> SaveOldSMKShiftAsync(RealizedMedicalShiftOldSMK shift)
        {
            try
            {
                // Pobierz aktualnego użytkownika
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    System.Diagnostics.Debug.WriteLine("SaveOldSMKShiftAsync: Brak zalogowanego użytkownika");
                    return false;
                }

                // Zawsze aktualizuj ID specjalizacji na bieżące
                shift.SpecializationId = user.SpecializationId;
                System.Diagnostics.Debug.WriteLine($"SaveOldSMKShiftAsync: Ustawiono SpecializationId={shift.SpecializationId} dla użytkownika {user.Username}");

                // Sprawdź, czy to nowy dyżur czy aktualizacja istniejącego
                if (shift.ShiftId == 0)
                {
                    // Dodawanie nowego dyżuru
                    int result = await this.databaseService.InsertAsync(shift);
                    System.Diagnostics.Debug.WriteLine($"SaveOldSMKShiftAsync: Dodano nowy dyżur z ID {result}, SpecializationId={shift.SpecializationId}");
                    return result > 0;
                }
                else
                {
                    // Dodatkowe zabezpieczenie - sprawdź, czy dyżur należy do bieżącego użytkownika
                    var existingShift = await this.GetOldSMKShiftAsync(shift.ShiftId);
                    if (existingShift != null && existingShift.SpecializationId != user.SpecializationId)
                    {
                        System.Diagnostics.Debug.WriteLine($"SaveOldSMKShiftAsync: ODMOWA DOSTĘPU - próba edycji dyżuru innego użytkownika! ShiftId={shift.ShiftId}, SpecializationId dyżuru={existingShift.SpecializationId}, SpecializationId użytkownika={user.SpecializationId}");
                        return false;
                    }

                    // Aktualizacja istniejącego dyżuru
                    int result = await this.databaseService.UpdateAsync(shift);
                    System.Diagnostics.Debug.WriteLine($"SaveOldSMKShiftAsync: Zaktualizowano dyżur o ID {shift.ShiftId}, SpecializationId={shift.SpecializationId}, wynik: {result}");
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveOldSMKShiftAsync: Błąd - {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteOldSMKShiftAsync(int shiftId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"DeleteOldSMKShiftAsync: Rozpoczęto usuwanie dyżuru o ID {shiftId}");

                // Pobierz aktualnego użytkownika
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    System.Diagnostics.Debug.WriteLine("DeleteOldSMKShiftAsync: Brak zalogowanego użytkownika");
                    return false;
                }

                // Znajdź dyżur do usunięcia
                var shift = await this.GetOldSMKShiftAsync(shiftId);
                if (shift == null)
                {
                    System.Diagnostics.Debug.WriteLine($"DeleteOldSMKShiftAsync: Nie znaleziono dyżuru o ID {shiftId}");
                    return false;
                }

                // Sprawdź, czy dyżur należy do bieżącego użytkownika
                if (shift.SpecializationId != user.SpecializationId)
                {
                    System.Diagnostics.Debug.WriteLine($"DeleteOldSMKShiftAsync: ODMOWA DOSTĘPU - próba usunięcia dyżuru innego użytkownika! ShiftId={shiftId}, SpecializationId dyżuru={shift.SpecializationId}, SpecializationId użytkownika={user.SpecializationId}");
                    return false;
                }

                // Usuń dyżur
                var query = "DELETE FROM RealizedMedicalShiftOldSMK WHERE ShiftId = ? AND SpecializationId = ?";
                int result = await this.databaseService.ExecuteAsync(query, shiftId, user.SpecializationId);

                System.Diagnostics.Debug.WriteLine($"DeleteOldSMKShiftAsync: Usunięto dyżur o ID {shiftId}, SpecializationId={user.SpecializationId}, wynik: {result}");
                return result > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DeleteOldSMKShiftAsync: Błąd - {ex.Message}");
                return false;
            }
        }

        // Implementacja metod dla nowego SMK
        public async Task<List<RealizedMedicalShiftNewSMK>> GetNewSMKShiftsAsync(int internshipRequirementId)
        {
            try
            {
                // Pobierz aktualny moduł
                var currentModule = await this.specializationService.GetCurrentModuleAsync();
                int? moduleId = currentModule?.ModuleId;

                // Pobierz dyżury dla konkretnego wymagania stażowego I dla konkretnego modułu
                var query = "SELECT * FROM RealizedMedicalShiftNewSMK WHERE InternshipRequirementId = ? AND ModuleId = ?";
                var shifts = await this.databaseService.QueryAsync<RealizedMedicalShiftNewSMK>(query, internshipRequirementId, moduleId);

                return shifts;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas pobierania dyżurów: {ex.Message}");
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
                // Pobierz ID aktualnego modułu
                var currentModule = await this.specializationService.GetCurrentModuleAsync();
                if (currentModule == null)
                {
                    return false;
                }

                // Przypisz ID modułu do dyżuru
                shift.ModuleId = currentModule.ModuleId;

                // Pobierz ID specjalizacji
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return false;
                }

                // Przypisz ID specjalizacji
                shift.SpecializationId = specialization.SpecializationId;

                // Zapisz dyżur
                int result = await this.databaseService.InsertAsync(shift);
                return result > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas zapisywania dyżuru: {ex.Message}");
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

        public async Task<MedicalShiftsSummary> GetShiftsSummaryAsync(int? year = null, int? internshipRequirementId = null)
        {
            try
            {
                var summary = new MedicalShiftsSummary();

                // Pobierz aktualny moduł
                var currentModule = await this.specializationService.GetCurrentModuleAsync();
                int? moduleId = currentModule?.ModuleId;

                // Pobierz specjalizację
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return summary;
                }

                // Rozdzielimy zapytania na dwa rodzaje, zamiast używać abstrakcyjnej klasy bazowej
                if (internshipRequirementId.HasValue)
                {
                    // Filtruj po ID wymagania stażowego I po module - używamy konkretnego typu RealizedMedicalShiftNewSMK
                    string query = "SELECT * FROM RealizedMedicalShiftNewSMK WHERE SpecializationId = ? AND InternshipRequirementId = ? AND ModuleId = ?";
                    var newSmkShifts = await this.databaseService.QueryAsync<RealizedMedicalShiftNewSMK>(
                        query, specialization.SpecializationId, internshipRequirementId.Value, moduleId);

                    // Oblicz sumę godzin i minut dla nowego SMK
                    foreach (var shift in newSmkShifts)
                    {
                        summary.TotalHours += shift.Hours;
                        summary.TotalMinutes += shift.Minutes;

                        // Dla dyżurów zsynchronizowanych, dodaj do sumy zatwierdzonych
                        if (shift.SyncStatus == SyncStatus.Synced)
                        {
                            summary.ApprovedHours += shift.Hours;
                            summary.ApprovedMinutes += shift.Minutes;
                        }
                    }
                }
                else if (year.HasValue)
                {
                    // Filtruj po roku - używamy konkretnego typu RealizedMedicalShiftOldSMK
                    string query = "SELECT * FROM RealizedMedicalShiftOldSMK WHERE SpecializationId = ? AND Year = ?";
                    var oldSmkShifts = await this.databaseService.QueryAsync<RealizedMedicalShiftOldSMK>(
                        query, specialization.SpecializationId, year.Value);

                    // Oblicz sumę godzin i minut dla starego SMK
                    foreach (var shift in oldSmkShifts)
                    {
                        summary.TotalHours += shift.Hours;
                        summary.TotalMinutes += shift.Minutes;

                        // Dla dyżurów zsynchronizowanych, dodaj do sumy zatwierdzonych
                        if (shift.SyncStatus == SyncStatus.Synced)
                        {
                            summary.ApprovedHours += shift.Hours;
                            summary.ApprovedMinutes += shift.Minutes;
                        }
                    }
                }
                else
                {
                    // Domyślnie pobierz wszystkie dyżury dla bieżącego modułu - używamy konkretnego typu RealizedMedicalShiftNewSMK
                    string query = "SELECT * FROM RealizedMedicalShiftNewSMK WHERE SpecializationId = ? AND ModuleId = ?";
                    var newSmkShifts = await this.databaseService.QueryAsync<RealizedMedicalShiftNewSMK>(
                        query, specialization.SpecializationId, moduleId);

                    // Oblicz sumę godzin i minut dla nowego SMK
                    foreach (var shift in newSmkShifts)
                    {
                        summary.TotalHours += shift.Hours;
                        summary.TotalMinutes += shift.Minutes;

                        // Dla dyżurów zsynchronizowanych, dodaj do sumy zatwierdzonych
                        if (shift.SyncStatus == SyncStatus.Synced)
                        {
                            summary.ApprovedHours += shift.Hours;
                            summary.ApprovedMinutes += shift.Minutes;
                        }
                    }
                }

                // Normalizacja czasu (60 minut = 1 godzina)
                summary.NormalizeTime();

                return summary;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas obliczania podsumowania dyżurów: {ex.Message}");
                return new MedicalShiftsSummary();
            }
        }

        public async Task<List<int>> GetAvailableYearsAsync()
        {
            try
            {
                // Pobierz aktualnego użytkownika
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    System.Diagnostics.Debug.WriteLine("GetAvailableYearsAsync: Brak zalogowanego użytkownika");
                    return new List<int> { 1 }; // Zwracamy domyślny rok 1
                }

                System.Diagnostics.Debug.WriteLine($"GetAvailableYearsAsync: Pobieranie lat dla użytkownika {user.Username}, ID={user.UserId}, SpecializationId={user.SpecializationId}");

                // Pobierz aktualną specjalizację
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    System.Diagnostics.Debug.WriteLine("GetAvailableYearsAsync: Nie znaleziono aktualnej specjalizacji");
                    return new List<int> { 1 }; // Zwracamy domyślny rok 1
                }

                // Pobierz strukturę specjalizacji z JSON
                if (string.IsNullOrEmpty(specialization.ProgramStructure))
                {
                    System.Diagnostics.Debug.WriteLine("GetAvailableYearsAsync: Brak struktury programu specjalizacji");
                    return new List<int> { 1, 2, 3, 4, 5 }; // Domyślnie 5 lat specjalizacji
                }

                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        AllowTrailingCommas = true,
                        ReadCommentHandling = JsonCommentHandling.Skip,
                        Converters = { new JsonStringEnumConverter() }
                    };

                    // Deserializuj strukturę specjalizacji
                    var specializationStructure = JsonSerializer.Deserialize<SpecializationStructure>(
                        specialization.ProgramStructure, options);

                    if (specializationStructure == null)
                    {
                        System.Diagnostics.Debug.WriteLine("GetAvailableYearsAsync: Nie udało się zdeserializować struktury");
                        return new List<int> { 1, 2, 3, 4, 5 }; // Domyślnie 5 lat specjalizacji
                    }

                    // Oblicz liczbę lat specjalizacji na podstawie całkowitego czasu trwania
                    int totalYears = 0;
                    if (specializationStructure.TotalDuration != null)
                    {
                        totalYears = specializationStructure.TotalDuration.Years;
                        if (specializationStructure.TotalDuration.Months > 0)
                        {
                            totalYears++; // Dodaj dodatkowy rok, jeśli są dodatkowe miesiące
                        }
                    }
                    else
                    {
                        // Alternatywne podejście: sprawdź czas trwania z modułów
                        totalYears = specializationStructure.Modules?.Sum(m => m.Duration?.Years ?? 0) ?? 0;
                        int additionalMonths = specializationStructure.Modules?.Sum(m => m.Duration?.Months ?? 0) ?? 0;
                        if (additionalMonths > 0)
                        {
                            totalYears += (additionalMonths / 12) + (additionalMonths % 12 > 0 ? 1 : 0);
                        }
                    }

                    // Upewnij się, że mamy co najmniej 1 rok
                    totalYears = Math.Max(1, totalYears);
                    totalYears = Math.Min(6, totalYears); // Maksymalnie 6 lat

                    // Utwórz listę lat od 1 do całkowitej liczby lat
                    var years = Enumerable.Range(1, totalYears).ToList();
                    System.Diagnostics.Debug.WriteLine($"GetAvailableYearsAsync: Wygenerowano {years.Count} lat z JSON specjalizacji");

                    return years;
                }
                catch (Exception jsonEx)
                {
                    System.Diagnostics.Debug.WriteLine($"GetAvailableYearsAsync: Błąd deserializacji JSON - {jsonEx.Message}");
                    return new List<int> { 1, 2, 3, 4, 5 }; // Domyślnie 5 lat specjalizacji
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetAvailableYearsAsync: Błąd - {ex.Message}");
                return new List<int> { 1, 2, 3, 4, 5 }; // W przypadku błędu, zwróć 5 lat jako domyślne
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
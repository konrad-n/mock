using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;

namespace SledzSpecke.App.Services.Specialization
{
    public class SpecializationService : ISpecializationService
    {
        private readonly IDatabaseService databaseService;
        private readonly IAuthService authService;
        private readonly IDialogService dialogService;
        private readonly ModuleInitializer moduleInitializer;

        public SpecializationService(IDatabaseService databaseService, IAuthService authService, IDialogService dialogService)
        {
            this.databaseService = databaseService;
            this.authService = authService;
            this.dialogService = dialogService;
            this.moduleInitializer = new ModuleInitializer(databaseService);
        }

        public async Task<Models.Specialization> GetCurrentSpecializationAsync()
        {
            try
            {
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    System.Diagnostics.Debug.WriteLine("GetCurrentSpecializationAsync: Nie znaleziono użytkownika");
                    return null;
                }

                var specialization = await this.databaseService.GetSpecializationAsync(user.SpecializationId);
                System.Diagnostics.Debug.WriteLine($"GetCurrentSpecializationAsync: Znaleziono specjalizację {specialization?.Name ?? "null"}");

                // Jeśli specjalizacja ma moduły, ale ich nie załadowaliśmy, spróbuj je zainicjalizować
                if (specialization != null)
                {
                    var modules = await this.databaseService.GetModulesAsync(specialization.SpecializationId);
                    if (modules == null || modules.Count == 0)
                    {
                        System.Diagnostics.Debug.WriteLine("Specjalizacja ma moduły, ale nie są zainicjalizowane. Inicjalizuję...");
                        await this.moduleInitializer.InitializeModulesIfNeededAsync(specialization.SpecializationId);

                        // Pobierz specjalizację ponownie, aby zawierała zaktualizowane moduły
                        specialization = await this.databaseService.GetSpecializationAsync(user.SpecializationId);
                    }
                }

                return specialization;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w GetCurrentSpecializationAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<Module> GetCurrentModuleAsync()
        {
            try
            {
                var specialization = await this.GetCurrentSpecializationAsync();
                if (specialization == null || !specialization.CurrentModuleId.HasValue)
                {
                    System.Diagnostics.Debug.WriteLine("GetCurrentModuleAsync: Nie znaleziono modułu");
                    return null;
                }

                var module = await this.databaseService.GetModuleAsync(specialization.CurrentModuleId.Value);
                System.Diagnostics.Debug.WriteLine($"GetCurrentModuleAsync: Znaleziono moduł {module?.Name ?? "null"}");
                return module;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w GetCurrentModuleAsync: {ex.Message}");
                return null;
            }
        }

        public async Task SetCurrentModuleAsync(int moduleId)
        {
            try
            {
                var specialization = await this.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return;
                }

                var module = await this.databaseService.GetModuleAsync(moduleId);
                if (module == null || module.SpecializationId != specialization.SpecializationId)
                {
                    return;
                }

                // Aktualizuj bieżący moduł w specjalizacji
                specialization.CurrentModuleId = moduleId;
                await this.databaseService.UpdateSpecializationAsync(specialization);

                // Zapisz też w ustawieniach aplikacji
                await SettingsHelper.SetCurrentModuleIdAsync(moduleId);

                System.Diagnostics.Debug.WriteLine($"SetCurrentModuleAsync: Ustawiono moduł {moduleId}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w SetCurrentModuleAsync: {ex.Message}");
            }
        }

        public async Task<int> GetInternshipCountAsync(int? moduleId = null)
        {
            try
            {
                var specialization = await this.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return 0;
                }

                var internships = await this.databaseService.GetInternshipsAsync(
                    specializationId: specialization.SpecializationId,
                    moduleId: moduleId);

                int completedCount = internships.Count(i => i.IsCompleted);
                System.Diagnostics.Debug.WriteLine($"GetInternshipCountAsync: Znaleziono {completedCount} ukończonych staży z {internships.Count}");
                return completedCount;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w GetInternshipCountAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> GetProcedureCountAsync(int? moduleId = null)
        {
            try
            {
                var specialization = await this.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return 0;
                }

                int count = 0;

                // Jeśli mamy moduł, pobierz tylko procedury z tego modułu
                if (moduleId.HasValue)
                {
                    // Pobierz wszystkie staże w module
                    var internships = await this.databaseService.GetInternshipsAsync(
                        specializationId: specialization.SpecializationId,
                        moduleId: moduleId);

                    foreach (var internship in internships)
                    {
                        // Pobierz procedury dla każdego stażu
                        var procedures = await this.databaseService.GetProceduresAsync(internshipId: internship.InternshipId);
                        // Licz tylko procedury typu A (wykonywane samodzielnie)
                        count += procedures.Count(p => p.OperatorCode == "A");
                    }
                }
                else
                {
                    // Pobierz wszystkie staże
                    var internships = await this.databaseService.GetInternshipsAsync(
                        specializationId: specialization.SpecializationId);

                    foreach (var internship in internships)
                    {
                        var procedures = await this.databaseService.GetProceduresAsync(internshipId: internship.InternshipId);
                        count += procedures.Count(p => p.OperatorCode == "A");
                    }
                }

                System.Diagnostics.Debug.WriteLine($"GetProcedureCountAsync: Znaleziono {count} procedur");
                return count;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w GetProcedureCountAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> GetCourseCountAsync(int? moduleId = null)
        {
            try
            {
                var specialization = await this.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return 0;
                }

                var courses = await this.databaseService.GetCoursesAsync(
                    specializationId: specialization.SpecializationId,
                    moduleId: moduleId);

                System.Diagnostics.Debug.WriteLine($"GetCourseCountAsync: Znaleziono {courses.Count} kursów");
                return courses.Count;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w GetCourseCountAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> GetShiftCountAsync(int? moduleId = null)
        {
            try
            {
                var specialization = await this.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return 0;
                }

                double totalHoursDouble = 0;

                // Pobierz staże odpowiednie dla modułu lub całej specjalizacji
                var internships = await this.databaseService.GetInternshipsAsync(
                    specializationId: specialization.SpecializationId,
                    moduleId: moduleId);

                // Dla każdego stażu pobierz dyżury
                foreach (var internship in internships)
                {
                    var shifts = await this.databaseService.GetMedicalShiftsAsync(internshipId: internship.InternshipId);
                    foreach (var shift in shifts)
                    {
                        // Poprawka: używamy prawidłowego dzielenia zmiennoprzecinkowego
                        totalHoursDouble += shift.Hours + ((double)shift.Minutes / 60.0);
                    }
                }

                // Zaokrąglamy do najbliższej pełnej godziny
                int totalHours = (int)Math.Round(totalHoursDouble);

                System.Diagnostics.Debug.WriteLine($"GetShiftCountAsync: Znaleziono {totalHours} godzin dyżurów (dokładnie {totalHoursDouble})");
                return totalHours;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w GetShiftCountAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> GetSelfEducationCountAsync(int? moduleId = null)
        {
            try
            {
                var specialization = await this.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return 0;
                }

                var selfEducationItems = await this.databaseService.GetSelfEducationItemsAsync(
                    specializationId: specialization.SpecializationId,
                    moduleId: moduleId);

                System.Diagnostics.Debug.WriteLine($"GetSelfEducationCountAsync: Znaleziono {selfEducationItems.Count} elementów samokształcenia");
                return selfEducationItems.Count;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w GetSelfEducationCountAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> GetPublicationCountAsync(int? moduleId = null)
        {
            try
            {
                var specialization = await this.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return 0;
                }

                var publications = await this.databaseService.GetPublicationsAsync(
                    specializationId: specialization.SpecializationId,
                    moduleId: moduleId);

                System.Diagnostics.Debug.WriteLine($"GetPublicationCountAsync: Znaleziono {publications.Count} publikacji");
                return publications.Count;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w GetPublicationCountAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> GetEducationalActivityCountAsync(int? moduleId = null)
        {
            try
            {
                var specialization = await this.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return 0;
                }

                var activities = await this.databaseService.GetEducationalActivitiesAsync(
                    specializationId: specialization.SpecializationId,
                    moduleId: moduleId);

                System.Diagnostics.Debug.WriteLine($"GetEducationalActivityCountAsync: Znaleziono {activities.Count} aktywności edukacyjnych");
                return activities.Count;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w GetEducationalActivityCountAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> GetAbsenceCountAsync()
        {
            try
            {
                var specialization = await this.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return 0;
                }

                var absences = await this.databaseService.GetAbsencesAsync(specialization.SpecializationId);
                System.Diagnostics.Debug.WriteLine($"GetAbsenceCountAsync: Znaleziono {absences.Count} nieobecności");
                return absences.Count;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w GetAbsenceCountAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> GetRecognitionCountAsync()
        {
            try
            {
                var specialization = await this.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return 0;
                }

                var recognitions = await this.databaseService.GetRecognitionsAsync(specialization.SpecializationId);
                System.Diagnostics.Debug.WriteLine($"GetRecognitionCountAsync: Znaleziono {recognitions.Count} uznań");
                return recognitions.Count;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w GetRecognitionCountAsync: {ex.Message}");
                return 0;
            }
        }

        public async Task<SpecializationStatistics> GetSpecializationStatisticsAsync(int? moduleId = null)
        {
            try
            {
                var specialization = await this.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    System.Diagnostics.Debug.WriteLine("GetSpecializationStatisticsAsync: Nie znaleziono specjalizacji");
                    return new SpecializationStatistics();
                }

                // Dodany parametr moduleId - teraz przekazujemy go do CalculateFullStatisticsAsync
                var stats = await ProgressCalculator.CalculateFullStatisticsAsync(
                    this.databaseService,
                    specialization.SpecializationId,
                    moduleId);

                if (stats == null)
                {
                    System.Diagnostics.Debug.WriteLine("GetSpecializationStatisticsAsync: Nie udało się obliczyć statystyk");
                    return new SpecializationStatistics();
                }

                System.Diagnostics.Debug.WriteLine("GetSpecializationStatisticsAsync: Obliczono statystyki pomyślnie");
                return stats;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w GetSpecializationStatisticsAsync: {ex.Message}");
                return new SpecializationStatistics();
            }
        }

        public async Task UpdateSpecializationProgressAsync(int specializationId)
        {
            try
            {
                await ProgressCalculator.UpdateSpecializationProgressAsync(this.databaseService, specializationId);
                System.Diagnostics.Debug.WriteLine($"UpdateSpecializationProgressAsync: Zaktualizowano postęp specjalizacji {specializationId}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w UpdateSpecializationProgressAsync: {ex.Message}");
            }
        }

        public async Task UpdateModuleProgressAsync(int moduleId)
        {
            try
            {
                await ProgressCalculator.UpdateModuleProgressAsync(this.databaseService, moduleId);
                System.Diagnostics.Debug.WriteLine($"UpdateModuleProgressAsync: Zaktualizowano postęp modułu {moduleId}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w UpdateModuleProgressAsync: {ex.Message}");
            }
        }

        public async Task<DateTime> CalculateSpecializationEndDateAsync(int specializationId)
        {
            try
            {
                var specialization = await this.databaseService.GetSpecializationAsync(specializationId);
                if (specialization == null)
                {
                    return DateTime.Now.AddYears(5); // Domyślnie 5 lat
                }

                var absences = await this.databaseService.GetAbsencesAsync(specializationId);

                return DateCalculator.CalculateSpecializationEndDate(
                    specialization.StartDate,
                    (specialization.PlannedEndDate - specialization.StartDate).Days + 1,
                    absences);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w CalculateSpecializationEndDateAsync: {ex.Message}");
                return DateTime.Now.AddYears(5); // Domyślnie 5 lat
            }
        }

        public async Task<List<Module>> GetModulesAsync(int specializationId)
        {
            try
            {
                return await this.databaseService.GetModulesAsync(specializationId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w GetModulesAsync: {ex.Message}");
                return new List<Module>();
            }
        }

        public async Task<List<SpecializationProgram>> GetAvailableSpecializationProgramsAsync()
        {
            try
            {
                return await this.databaseService.GetAllSpecializationProgramsAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w GetAvailableSpecializationProgramsAsync: {ex.Message}");
                return new List<SpecializationProgram>();
            }
        }

        public async Task<SpecializationProgram> LoadSpecializationProgramAsync(string code, SmkVersion smkVersion)
        {
            try
            {
                return await SpecializationLoader.LoadSpecializationProgramAsync(code, smkVersion);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w LoadSpecializationProgramAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> InitializeSpecializationModulesAsync(int specializationId)
        {
            return await this.moduleInitializer.InitializeModulesIfNeededAsync(specializationId);
        }

        // Metody obsługi dyżurów medycznych, które należy dodać do klasy SpecializationService

        public async Task<List<MedicalShift>> GetMedicalShiftsAsync(int? internshipId = null)
        {
            try
            {
                return await this.databaseService.GetMedicalShiftsAsync(internshipId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w GetMedicalShiftsAsync: {ex.Message}");
                return new List<MedicalShift>();
            }
        }

        public async Task<MedicalShift> GetMedicalShiftAsync(int shiftId)
        {
            try
            {
                return await this.databaseService.GetMedicalShiftAsync(shiftId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w GetMedicalShiftAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> AddMedicalShiftAsync(MedicalShift shift)
        {
            try
            {
                int result = await this.databaseService.SaveMedicalShiftAsync(shift);

                // Aktualizacja postępu modułu
                var currentModule = await this.GetCurrentModuleAsync();
                if (currentModule != null)
                {
                    await this.UpdateModuleProgressAsync(currentModule.ModuleId);
                }

                return result > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w AddMedicalShiftAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateMedicalShiftAsync(MedicalShift shift)
        {
            try
            {
                int result = await this.databaseService.SaveMedicalShiftAsync(shift);

                // Aktualizacja postępu modułu
                var currentModule = await this.GetCurrentModuleAsync();
                if (currentModule != null)
                {
                    await this.UpdateModuleProgressAsync(currentModule.ModuleId);
                }

                return result > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w UpdateMedicalShiftAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteMedicalShiftAsync(int shiftId)
        {
            try
            {
                var shift = await this.databaseService.GetMedicalShiftAsync(shiftId);
                if (shift == null)
                {
                    return false;
                }

                int result = await this.databaseService.DeleteMedicalShiftAsync(shift);

                // Aktualizacja postępu modułu
                var currentModule = await this.GetCurrentModuleAsync();
                if (currentModule != null)
                {
                    await this.UpdateModuleProgressAsync(currentModule.ModuleId);
                }

                return result > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w DeleteMedicalShiftAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<Internship> GetInternshipAsync(int internshipId)
        {
            try
            {
                return await this.databaseService.GetInternshipAsync(internshipId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w GetInternshipAsync: {ex.Message}");
                return null;
            }
        }

        // Metoda zwracająca wszystkie staże z modułu (zdefiniowane w JSON)
        // Metoda zwracająca wszystkie staże z modułu (zdefiniowane w JSON)
        public async Task<List<Internship>> GetInternshipsAsync(int? moduleId = null)
        {
            try
            {
                var results = new List<Internship>();
                var currentSpecialization = await this.GetCurrentSpecializationAsync();

                if (currentSpecialization == null)
                {
                    return results;
                }

                // Jeśli moduleId nie jest podane, użyj bieżącego modułu
                if (!moduleId.HasValue && currentSpecialization.CurrentModuleId.HasValue)
                {
                    moduleId = currentSpecialization.CurrentModuleId.Value;
                }

                // Pobierz moduł
                Module module = null;
                if (moduleId.HasValue)
                {
                    module = await this.databaseService.GetModuleAsync(moduleId.Value);
                }
                else
                {
                    var modules = await this.databaseService.GetModulesAsync(currentSpecialization.SpecializationId);
                    if (modules.Count > 0)
                    {
                        module = modules[0];
                    }
                }

                if (module == null || string.IsNullOrEmpty(module.Structure))
                {
                    return results;
                }

                // Parsuj strukturę modułu z JSON
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var moduleStructure = System.Text.Json.JsonSerializer.Deserialize<ModuleStructure>(
                    module.Structure, options);

                if (moduleStructure?.Internships == null)
                {
                    return results;
                }

                // Pobierz również staże z bazy danych, które użytkownik już rozpoczął
                var userInternships = await this.databaseService.GetInternshipsAsync(
                    currentSpecialization.SpecializationId,
                    moduleId);

                // Konwertuj InternshipRequirement na Internship
                int id = 1;
                foreach (var requirement in moduleStructure.Internships)
                {
                    // Sprawdź, czy staż o tym kodzie już istnieje w bazie
                    var existingInternship = userInternships.FirstOrDefault(
                        i => i.InternshipName == requirement.Name);

                    if (existingInternship != null)
                    {
                        // Użyj istniejącego stażu
                        results.Add(existingInternship);
                    }
                    else
                    {
                        // Utwórz nowy obiekt stażu
                        results.Add(new Internship
                        {
                            InternshipId = -id,  // Tymczasowe ID (ujemne, by nie kolidowało z ID z bazy)
                            SpecializationId = currentSpecialization.SpecializationId,
                            ModuleId = moduleId,
                            InternshipName = requirement.Name,
                            DaysCount = requirement.WorkingDays,
                            StartDate = DateTime.Today,
                            EndDate = DateTime.Today.AddDays(requirement.WorkingDays),
                            Year = 1, // Domyślnie pierwszy rok
                            IsCompleted = false,
                            IsApproved = false
                        });
                        id++;
                    }
                }

                return results;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w GetInternshipsAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                return new List<Internship>();
            }
        }

        // Dodatkowa metoda, która zwraca tylko staże dodane przez użytkownika
        public async Task<List<Internship>> GetUserInternshipsAsync(int? moduleId = null)
        {
            try
            {
                var currentSpecialization = await this.GetCurrentSpecializationAsync();
                if (currentSpecialization == null)
                {
                    return new List<Internship>();
                }

                // Pobierz staże dodane przez użytkownika z bazy danych
                return await this.databaseService.GetInternshipsAsync(
                    specializationId: currentSpecialization.SpecializationId,
                    moduleId: moduleId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w GetUserInternshipsAsync: {ex.Message}");
                return new List<Internship>();
            }
        }

        public async Task<Internship> GetCurrentInternshipAsync()
        {
            try
            {
                // Ta metoda powinna zwracać aktualnie wybrany staż
                // Możemy to zrobić na podstawie ostatniego dodanego dyżuru lub staż, który jest obecnie w trakcie

                var specialization = await this.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return null;
                }

                var internships = await this.databaseService.GetInternshipsAsync(specialization.SpecializationId);

                // Znajdź trwający staż (gdzie dzisiejsza data jest pomiędzy datą rozpoczęcia i zakończenia)
                var today = DateTime.Today;
                var currentInternship = internships.FirstOrDefault(i =>
                    i.StartDate <= today && i.EndDate >= today);

                // Jeśli nie ma trwającego, weź ostatni dodany
                if (currentInternship == null && internships.Count > 0)
                {
                    currentInternship = internships.OrderByDescending(i => i.InternshipId).First();
                }

                return currentInternship;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w GetCurrentInternshipAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> AddInternshipAsync(Internship internship)
        {
            try
            {
                // Upewnij się, że staż jest powiązany z bieżącą specjalizacją
                if (internship.SpecializationId <= 0)
                {
                    var specialization = await this.GetCurrentSpecializationAsync();
                    if (specialization == null)
                    {
                        return false;
                    }

                    internship.SpecializationId = specialization.SpecializationId;
                }

                int result = await this.databaseService.SaveInternshipAsync(internship);

                // Aktualizacja postępu modułu
                if (internship.ModuleId.HasValue)
                {
                    await this.UpdateModuleProgressAsync(internship.ModuleId.Value);
                }

                return result > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w AddInternshipAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateInternshipAsync(Internship internship)
        {
            try
            {
                int result = await this.databaseService.SaveInternshipAsync(internship);

                // Aktualizacja postępu modułu
                if (internship.ModuleId.HasValue)
                {
                    await this.UpdateModuleProgressAsync(internship.ModuleId.Value);
                }

                return result > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w UpdateInternshipAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteInternshipAsync(int internshipId)
        {
            try
            {
                var internship = await this.databaseService.GetInternshipAsync(internshipId);
                if (internship == null)
                {
                    return false;
                }

                // Sprawdź, czy staż ma powiązane dyżury lub procedury
                var shifts = await this.databaseService.GetMedicalShiftsAsync(internshipId);
                var procedures = await this.databaseService.GetProceduresAsync(internshipId: internshipId);

                if (shifts.Count > 0 || procedures.Count > 0)
                {
                    // Nie można usunąć stażu z powiązanymi elementami
                    return false;
                }

                int result = await this.databaseService.DeleteInternshipAsync(internship);

                // Aktualizacja postępu modułu
                if (internship.ModuleId.HasValue)
                {
                    await this.UpdateModuleProgressAsync(internship.ModuleId.Value);
                }

                return result > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd w DeleteInternshipAsync: {ex.Message}");
                return false;
            }
        }

        public Task<User> GetCurrentUserAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<Procedure>> GetProceduresAsync(string searchText = null, int? internshipId = null)
        {
            throw new NotImplementedException();
        }

        public Task<Procedure> GetProcedureAsync(int procedureId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddProcedureAsync(Procedure procedure)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateProcedureAsync(Procedure procedure)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteProcedureAsync(int procedureId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Course>> GetCoursesAsync(int? moduleId = null)
        {
            throw new NotImplementedException();
        }

        public Task<Course> GetCourseAsync(int courseId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddCourseAsync(Course course)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateCourseAsync(Course course)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteCourseAsync(int courseId)
        {
            throw new NotImplementedException();
        }

        public Task<List<SelfEducation>> GetSelfEducationItemsAsync(int? moduleId = null)
        {
            throw new NotImplementedException();
        }

        public Task<SelfEducation> GetSelfEducationAsync(int selfEducationId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddSelfEducationAsync(SelfEducation selfEducation)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateSelfEducationAsync(SelfEducation selfEducation)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteSelfEducationAsync(int selfEducationId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Publication>> GetPublicationsAsync(int? moduleId = null)
        {
            throw new NotImplementedException();
        }

        public Task<Publication> GetPublicationAsync(int publicationId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddPublicationAsync(Publication publication)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdatePublicationAsync(Publication publication)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeletePublicationAsync(int publicationId)
        {
            throw new NotImplementedException();
        }

        public Task<List<EducationalActivity>> GetEducationalActivitiesAsync(int? moduleId = null)
        {
            throw new NotImplementedException();
        }

        public Task<EducationalActivity> GetEducationalActivityAsync(int activityId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddEducationalActivityAsync(EducationalActivity activity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateEducationalActivityAsync(EducationalActivity activity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteEducationalActivityAsync(int activityId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Absence>> GetAbsencesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Absence> GetAbsenceAsync(int absenceId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddAbsenceAsync(Absence absence)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAbsenceAsync(Absence absence)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAbsenceAsync(int absenceId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Models.Recognition>> GetRecognitionsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Models.Recognition> GetRecognitionAsync(int recognitionId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddRecognitionAsync(Models.Recognition recognition)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateRecognitionAsync(Models.Recognition recognition)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteRecognitionAsync(int recognitionId)
        {
            throw new NotImplementedException();
        }
    }
}

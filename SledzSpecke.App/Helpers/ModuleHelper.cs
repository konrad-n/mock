using System.Text.Json;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;

namespace SledzSpecke.App.Helpers
{
    public static class ModuleHelper
    {
        /// <summary>
        /// Sprawdza, czy dana specjalizacja ma strukturę z modułem podstawowym i specjalistycznym (dwumodułowa).
        /// </summary>
        /// <param name="specializationCode">Kod specjalizacji.</param>
        /// <returns>True, jeśli specjalizacja ma moduł podstawowy; w przeciwnym razie false.</returns>
        public static bool HasBasicModule(string specializationCode)
        {
            if (string.IsNullOrEmpty(specializationCode))
            {
                return false;
            }

            // Lista specjalizacji z modułem podstawowym
            var twoModulesSpecializations = new[]
            {
                "cardiology",
                "psychiatry",
            };

            return twoModulesSpecializations.Contains(specializationCode.ToLowerInvariant());
        }

        /// <summary>
        /// Zwraca kod modułu podstawowego dla danej specjalizacji.
        /// </summary>
        /// <param name="specializationCode">Kod specjalizacji.</param>
        /// <returns>Kod modułu podstawowego lub null, jeśli specjalizacja nie ma modułu podstawowego.</returns>
        public static string GetBasicModuleName(string specializationCode)
        {
            if (string.IsNullOrEmpty(specializationCode) || !HasBasicModule(specializationCode))
            {
                return null;
            }

            System.Diagnostics.Debug.WriteLine($"Szukam modułu podstawowego dla specjalizacji: {specializationCode}");

            // Dla kardiologii i pokrewnych specjalizacji wewnętrznych
            if (new[]
                {
                    "cardiology",
                    "psychiatry",
                }.Contains(specializationCode.ToLowerInvariant()))
            {
                System.Diagnostics.Debug.WriteLine("Znaleziono moduł podstawowy - internal_medicine");
                return "internal_medicine";
            }

            System.Diagnostics.Debug.WriteLine("Nie znaleziono modułu podstawowego");
            return null;
        }

        /// <summary>
        /// Tworzy moduły dla specjalizacji na podstawie jej kodu, wykorzystując dane z plików JSON.
        /// </summary>
        /// <param name="specializationCode">Kod specjalizacji.</param>
        /// <param name="startDate">Data rozpoczęcia specjalizacji.</param>
        /// <param name="smkVersion">Wersja SMK.</param>
        /// <returns>Lista modułów specjalizacji.</returns>
        public static async Task<List<Module>> CreateModulesForSpecializationAsync(string specializationCode, DateTime startDate, SmkVersion smkVersion)
        {
            System.Diagnostics.Debug.WriteLine($"Tworzenie modułów dla specjalizacji: {specializationCode}, wersja SMK: {smkVersion}");

            try
            {
                if (string.IsNullOrEmpty(specializationCode))
                {
                    System.Diagnostics.Debug.WriteLine("Kod specjalizacji jest pusty!");
                    return new List<Module>();
                }

                // Ładujemy program specjalizacji z pliku JSON
                var specializationProgram = await SpecializationLoader.LoadSpecializationProgramAsync(specializationCode, smkVersion);
                if (specializationProgram == null || string.IsNullOrEmpty(specializationProgram.Structure))
                {
                    System.Diagnostics.Debug.WriteLine("Nie znaleziono programu specjalizacji lub struktura jest pusta.");
                }

                System.Diagnostics.Debug.WriteLine("Załadowano program specjalizacji z pliku JSON.");

                // Deserializujemy strukturę specjalizacji
                // WAŻNA POPRAWKA: Dodajemy opcję PropertyNameCaseInsensitive = true aby ignorować wielkość liter
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true, // KLUCZOWA ZMIANA!
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };

                // Lista do przechowywania utworzonych modułów
                var modules = new List<Module>();

                // Parsujemy tablicę modułów z JSON
                try
                {
                    var jsonDocument = JsonDocument.Parse(specializationProgram.Structure);
                    if (!jsonDocument.RootElement.TryGetProperty("modules", out var modulesElement) ||
                        modulesElement.ValueKind != JsonValueKind.Array)
                    {
                        System.Diagnostics.Debug.WriteLine("Nie znaleziono tablicy 'modules' w JSON specjalizacji. Używam domyślnych.");
                    }

                    int moduleIndex = 0;
                    DateTime currentStartDate = startDate;

                    foreach (var moduleElement in modulesElement.EnumerateArray())
                    {
                        moduleIndex++;
                        System.Diagnostics.Debug.WriteLine($"Przetwarzanie modułu {moduleIndex}...");

                        // Pobieramy podstawowe informacje o module
                        string moduleName = string.Empty;
                        string moduleCode = string.Empty;
                        ModuleType moduleType = ModuleType.Specialistic; // Domyślnie specjalistyczny
                        int durationMonths = 0;
                        int workingDays = 0;

                        if (moduleElement.TryGetProperty("name", out var nameElement))
                        {
                            moduleName = nameElement.GetString();
                        }

                        if (moduleElement.TryGetProperty("code", out var codeElement))
                        {
                            moduleCode = codeElement.GetString();
                        }

                        if (moduleElement.TryGetProperty("moduleType", out var typeElement))
                        {
                            string typeString = typeElement.GetString();
                            if (!string.IsNullOrEmpty(typeString))
                            {
                                if (typeString.Equals("Basic", StringComparison.OrdinalIgnoreCase))
                                {
                                    moduleType = ModuleType.Basic;
                                }
                                else if (typeString.Equals("Specialistic", StringComparison.OrdinalIgnoreCase))
                                {
                                    moduleType = ModuleType.Specialistic;
                                }
                            }
                        }

                        // Jeśli typ modułu nie jest jawnie określony, spróbujmy określić go na podstawie nazwy
                        if (string.IsNullOrEmpty(moduleName))
                        {
                            if (moduleType == ModuleType.Basic)
                            {
                                moduleName = "Moduł podstawowy";
                            }
                            else
                            {
                                moduleName = $"Moduł specjalistyczny w zakresie {specializationCode}";
                            }
                        }

                        // Pobranie czasu trwania
                        if (moduleElement.TryGetProperty("duration", out var durationElement))
                        {
                            if (durationElement.TryGetProperty("years", out var yearsElement))
                            {
                                durationMonths = yearsElement.GetInt32() * 12;
                            }
                            if (durationElement.TryGetProperty("months", out var monthsElement))
                            {
                                durationMonths += monthsElement.GetInt32();
                            }
                        }
                        else if (moduleElement.TryGetProperty("durationMonths", out var monthsElement))
                        {
                            durationMonths = monthsElement.GetInt32();
                        }

                        // Domyślne wartości, jeśli nie znaleziono
                        if (durationMonths == 0)
                        {
                            if (moduleType == ModuleType.Basic)
                            {
                                durationMonths = 24; // 2 lata dla modułu podstawowego
                            }
                            else
                            {
                                durationMonths = 36; // 3 lata dla modułu specjalistycznego
                            }
                        }

                        // Pobranie liczby dni roboczych
                        if (moduleElement.TryGetProperty("workingDays", out var workingDaysElement))
                        {
                            workingDays = workingDaysElement.GetInt32();
                        }

                        System.Diagnostics.Debug.WriteLine($"Dane modułu: Nazwa={moduleName}, Kod={moduleCode}, Typ={moduleType}, Miesiące={durationMonths}, Dni robocze={workingDays}");

                        // Obliczamy datę końcową modułu
                        DateTime endDate = currentStartDate.AddMonths(durationMonths);

                        // Zliczamy liczby staży, kursów i procedur
                        int totalInternships = 0;
                        int totalCourses = 0;
                        int totalProceduresA = 0;
                        int totalProceduresB = 0;

                        // Zliczenie staży
                        if (moduleElement.TryGetProperty("internships", out var internshipsElement) &&
                            internshipsElement.ValueKind == JsonValueKind.Array)
                        {
                            totalInternships = internshipsElement.GetArrayLength();
                        }

                        // Zliczenie kursów
                        if (moduleElement.TryGetProperty("courses", out var coursesElement) &&
                            coursesElement.ValueKind == JsonValueKind.Array)
                        {
                            totalCourses = coursesElement.GetArrayLength();
                        }

                        // Zliczenie procedur
                        if (moduleElement.TryGetProperty("procedures", out var proceduresElement) &&
                            proceduresElement.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var procedureElement in proceduresElement.EnumerateArray())
                            {
                                if (procedureElement.TryGetProperty("requiredCountA", out var countAElement))
                                {
                                    totalProceduresA += countAElement.GetInt32();
                                }

                                if (procedureElement.TryGetProperty("requiredCountB", out var countBElement))
                                {
                                    totalProceduresB += countBElement.GetInt32();
                                }
                            }
                        }

                        int requiredShiftHours = 0;
                        double hoursPerWeek = 0;
                        string medicalShiftsDescription = null;

                        if (moduleElement.TryGetProperty("medicalShifts", out var medicalShiftsElement) &&
                            medicalShiftsElement.ValueKind == JsonValueKind.Object)
                        {
                            // Próbujemy pobrać bezpośrednio requiredShiftHours
                            if (medicalShiftsElement.TryGetProperty("requiredShiftHours", out var requiredHoursElement))
                            {
                                requiredShiftHours = requiredHoursElement.GetInt32();
                            }

                            // Pobieramy hoursPerWeek
                            if (medicalShiftsElement.TryGetProperty("hoursPerWeek", out var hoursPerWeekElement))
                            {
                                hoursPerWeek = hoursPerWeekElement.GetDouble();
                            }

                            // Pobieramy opis
                            if (medicalShiftsElement.TryGetProperty("description", out var descriptionElement))
                            {
                                medicalShiftsDescription = descriptionElement.GetString();
                            }
                        }

                        System.Diagnostics.Debug.WriteLine($"Zliczone elementy: Staże={totalInternships}, Kursy={totalCourses}, Procedury A={totalProceduresA}, Procedury B={totalProceduresB}");

                        // Pobieramy dane o samokształceniu
                        int selfEducationDays = 0;

                        // Próbujemy pobrać SelfEducationDays z moduleElement
                        if (moduleElement.TryGetProperty("selfEducation", out var selfEducationElement) &&
                            selfEducationElement.ValueKind == JsonValueKind.Object)
                        {
                            // Próbujemy pobrać totalDays
                            if (selfEducationElement.TryGetProperty("totalDays", out var totalDaysElement))
                            {
                                selfEducationDays = totalDaysElement.GetInt32();
                            }
                            // Alternatywnie, jeśli jest daysPerYear, możemy je wykorzystać
                            else if (selfEducationElement.TryGetProperty("daysPerYear", out var daysPerYearElement))
                            {
                                int daysPerYear = daysPerYearElement.GetInt32();
                                // Obliczenie liczby lat z czasu trwania modułu
                                int years = (int)Math.Ceiling(durationMonths / 12.0);
                                selfEducationDays = daysPerYear * years;
                            }
                        }

                        System.Diagnostics.Debug.WriteLine($"Pobrano dane o samokształceniu: SelfEducationDays={selfEducationDays}");

                        // Teraz tworzymy moduł z poprawnymi wartościami
                        var module = new Module
                        {
                            Name = moduleName,
                            Type = moduleType,
                            StartDate = currentStartDate,
                            EndDate = endDate,
                            Structure = moduleElement.ToString(),  // Zapisujemy cały JSON modułu

                            // Podstawowe statystyki zliczone wcześniej
                            CompletedInternships = 0,
                            TotalInternships = totalInternships,
                            CompletedCourses = 0,
                            TotalCourses = totalCourses,
                            CompletedProceduresA = 0,
                            TotalProceduresA = totalProceduresA,
                            CompletedProceduresB = 0,
                            TotalProceduresB = totalProceduresB,

                            // Nowe pola dotyczące dyżurów
                            CompletedShiftHours = 0,
                            RequiredShiftHours = requiredShiftHours,
                            WeeklyShiftHours = hoursPerWeek,

                            // Nowe pola dotyczące samokształcenia - POPRAWKA
                            CompletedSelfEducationDays = 0,
                            TotalSelfEducationDays = selfEducationDays
                        };

                        // Jeśli nie mamy bezpośrednio podanej wartości RequiredShiftHours, ale mamy hoursPerWeek,
                        // obliczamy wymaganą liczbę godzin
                        if (module.RequiredShiftHours == 0 && module.WeeklyShiftHours > 0)
                        {
                            // Obliczamy liczbę tygodni trwania modułu
                            TimeSpan duration = module.EndDate - module.StartDate;
                            int weeks = Math.Max(1, (int)(duration.TotalDays / 7));

                            // Obliczamy wymaganą liczbę godzin dyżurów
                            module.RequiredShiftHours = (int)Math.Round(module.WeeklyShiftHours * weeks);
                            System.Diagnostics.Debug.WriteLine($"Obliczono RequiredShiftHours: {module.RequiredShiftHours} h ({module.WeeklyShiftHours} h/tydzień × {weeks} tygodni)");
                        }
                        // Jeśli nie mamy ani RequiredShiftHours, ani WeeklyShiftHours, ustawiamy domyślną wartość
                        else if (module.RequiredShiftHours == 0 && module.WeeklyShiftHours == 0)
                        {
                            // Domyślna wartość - 10 godz. 5 min. tygodniowo
                            double defaultWeeklyHours = 10.083;

                            // Obliczamy liczbę tygodni trwania modułu
                            TimeSpan duration = module.EndDate - module.StartDate;
                            int weeks = Math.Max(1, (int)(duration.TotalDays / 7));

                            // Ustawiamy wartości
                            module.WeeklyShiftHours = defaultWeeklyHours;
                            module.RequiredShiftHours = (int)Math.Round(defaultWeeklyHours * weeks);
                            System.Diagnostics.Debug.WriteLine($"Ustawiono domyślne wartości dyżurów: {module.RequiredShiftHours} h ({defaultWeeklyHours} h/tydzień × {weeks} tygodni)");
                        }

                        modules.Add(module);
                        System.Diagnostics.Debug.WriteLine($"Dodano moduł: {module.Name}");

                        // Aktualizujemy datę rozpoczęcia dla następnego modułu
                        currentStartDate = endDate.AddDays(1);
                    }

                    // Jeśli udało się utworzyć moduły, zwracamy je
                    if (modules.Count > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Utworzono {modules.Count} modułów na podstawie danych JSON.");
                        return modules;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Błąd podczas parsowania modułów z JSON: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas tworzenia modułów z danych JSON: {ex.Message}");
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }

            return new List<Module>();
        }

        /// <summary>
        /// Inicjalizuje moduły specjalizacji na podstawie danych z plików JSON.
        /// </summary>
        /// <param name="databaseService">Serwis dostępu do bazy danych.</param>
        /// <param name="specializationId">ID specjalizacji.</param>
        /// <param name="modules">Lista modułów do inicjalizacji.</param>
        /// <returns>True, jeśli inicjalizacja się powiodła; w przeciwnym razie false.</returns>
        public static async Task<bool> InitializeModulesAsync(
            IDatabaseService databaseService,
            int specializationId,
            List<Module> modules)
        {
            try
            {
                if (databaseService == null || modules == null || modules.Count == 0)
                {
                    return false;
                }

                // Pobierz specjalizację
                var specialization = await databaseService.GetSpecializationAsync(specializationId);
                if (specialization == null)
                {
                    return false;
                }

                // Inicjalizuj moduły
                foreach (var module in modules)
                {
                    module.SpecializationId = specializationId;

                    // Zapisz moduł
                    await databaseService.SaveModuleAsync(module);
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd inicjalizacji modułów: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Oblicza datę zakończenia modułu uwzględniając nieobecności.
        /// </summary>
        /// <param name="module">Moduł do obliczenia daty zakończenia.</param>
        /// <param name="absences">Lista nieobecności do uwzględnienia.</param>
        /// <returns>Obliczona data zakończenia modułu.</returns>
        public static DateTime CalculateModuleEndDate(Module module, List<Absence> absences)
        {
            if (module == null)
            {
                return DateTime.Now;
            }

            // Podstawowa data zakończenia bez uwzględnienia nieobecności
            DateTime baseEndDate = module.EndDate;

            // Filtrowanie nieobecności dla tego modułu
            var relevantAbsences = absences.Where(a =>
                (a.StartDate >= module.StartDate && a.StartDate <= module.EndDate) ||
                (a.EndDate >= module.StartDate && a.EndDate <= module.EndDate) ||
                (a.StartDate <= module.StartDate && a.EndDate >= module.EndDate))
                .ToList();

            // Obliczenie przedłużenia
            int extensionDays = 0;
            foreach (var absence in relevantAbsences)
            {
                if (absence.Type == AbsenceType.Sick ||
                    absence.Type == AbsenceType.Maternity ||
                    absence.Type == AbsenceType.Paternity)
                {
                    // Oblicz dni przedłużenia (uwzględniając nakładanie się na moduł)
                    DateTime absenceStart = absence.StartDate < module.StartDate
                        ? module.StartDate
                        : absence.StartDate;

                    DateTime absenceEnd = absence.EndDate > module.EndDate
                        ? module.EndDate
                        : absence.EndDate;

                    int daysInModule = (absenceEnd - absenceStart).Days + 1;
                    extensionDays += daysInModule;
                }
            }

            // Uwzględnienie skróceń (uznania)
            int reductionDays = relevantAbsences
                .Where(a => a.Type == AbsenceType.Recognition)
                .Sum(a => (a.EndDate - a.StartDate).Days + 1);

            // Finalna data zakończenia
            return baseEndDate.AddDays(extensionDays - reductionDays);
        }
    }
}
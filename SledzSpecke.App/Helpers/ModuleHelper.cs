using System.Text.Json;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;

namespace SledzSpecke.App.Helpers
{
    public static class ModuleHelper
    {
        /// <summary>
        /// Sprawdza, czy dana specjalizacja ma strukturę modułową.
        /// </summary>
        /// <param name="specializationCode">Kod specjalizacji.</param>
        /// <returns>True, jeśli specjalizacja ma moduły; w przeciwnym razie false.</returns>
        public static bool IsModuleSpecialization(string specializationCode)
        {
            if (string.IsNullOrEmpty(specializationCode))
            {
                return false;
            }

            // Lista specjalizacji z modułami
            var moduleSpecializations = new[]
            {
                "kardiologia",
                "nefrologia",
                "gastroenterologia",
                "endokrynologia",
                "diabetologia",
                "reumatologia",
                "alergologia",
                "angiologia",
                "hematologia",
                "onkologia kliniczna",
                "pulmonologia",
            };

            return moduleSpecializations.Contains(specializationCode.ToLower());
        }

        /// <summary>
        /// Zwraca kod modułu podstawowego dla danej specjalizacji.
        /// </summary>
        /// <param name="specializationCode">Kod specjalizacji.</param>
        /// <returns>Kod modułu podstawowego lub null, jeśli nie znaleziono.</returns>
        public static string GetBasicModuleName(string specializationCode)
        {
            if (string.IsNullOrEmpty(specializationCode))
            {
                return null;
            }

            // Dla kardiologii i pokrewnych specjalizacji wewnętrznych
            if (new[]
                {
                    "kardiologia",
                    "nefrologia",
                    "gastroenterologia",
                    "endokrynologia",
                    "diabetologia",
                    "reumatologia",
                    "hematologia",
                    "alergologia",
                    "angiologia",
                    "onkologia kliniczna",
                    "pulmonologia",
                }.Contains(specializationCode.ToLower()))
            {
                return "internal_medicine";
            }

            // Dla innych specjalizacji można dodać podobne mapowania

            return null;
        }

        /// <summary>
        /// Tworzy moduły dla specjalizacji na podstawie jej kodu.
        /// </summary>
        /// <param name="specializationCode">Kod specjalizacji.</param>
        /// <param name="startDate">Data rozpoczęcia specjalizacji.</param>
        /// <returns>Lista modułów specjalizacji.</returns>
        public static List<Module> CreateModulesForSpecialization(string specializationCode, DateTime startDate)
        {
            System.Diagnostics.Debug.WriteLine($"Tworzenie modułów dla specjalizacji: {specializationCode}");

            try
            {
                if (string.IsNullOrEmpty(specializationCode))
                {
                    System.Diagnostics.Debug.WriteLine("Kod specjalizacji jest pusty!");
                    return new List<Module>();
                }

                if (!IsModuleSpecialization(specializationCode))
                {
                    System.Diagnostics.Debug.WriteLine($"Specjalizacja {specializationCode} nie zawiera modułów.");
                    return new List<Module>();
                }

                string basicCode = GetBasicModuleName(specializationCode);

                if (string.IsNullOrEmpty(basicCode))
                {
                    System.Diagnostics.Debug.WriteLine("Nie znaleziono kodu modułu podstawowego.");
                    return new List<Module>();
                }

                System.Diagnostics.Debug.WriteLine($"Kod modułu podstawowego: {basicCode}");

                // Tworzenie modułu podstawowego (trwa zwykle 2 lata)
                var basicModule = new Module
                {
                    Type = ModuleType.Basic,
                    Name = "Moduł podstawowy w zakresie chorób wewnętrznych",
                    StartDate = startDate,
                    EndDate = startDate.AddYears(2),
                    Structure = null, // Zostanie wypełnione później po załadowaniu programu specjalizacji
                };
                System.Diagnostics.Debug.WriteLine("Utworzono moduł podstawowy.");

                // Tworzenie modułu specjalistycznego (zwykle 3 lata lub więcej, w zależności od specjalizacji)
                var specialisticModule = new Module
                {
                    Type = ModuleType.Specialistic,
                    Name = $"Moduł specjalistyczny w zakresie {specializationCode}",
                    StartDate = startDate.AddYears(2),
                    EndDate = startDate.AddYears(5), // Standardowo 5 lat dla pełnej specjalizacji
                    Structure = null, // Zostanie wypełnione później po załadowaniu programu specjalizacji
                };
                System.Diagnostics.Debug.WriteLine("Utworzono moduł specjalistyczny.");

                return new List<Module> { basicModule, specialisticModule };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd tworzenia modułów: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                return new List<Module>();
            }
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

                    // Załaduj program specjalizacji dla modułu
                    string moduleCode = module.Type == ModuleType.Basic
                        ? GetBasicModuleName(specialization.ProgramCode)
                        : specialization.ProgramCode + "_specialistic";

                    // Załaduj strukturę modułu
                    SpecializationProgram program = null;
                    try
                    {
                        program = await databaseService.GetSpecializationProgramByCodeAsync(
                            moduleCode,
                            specialization.SmkVersion);

                        if (program != null && !string.IsNullOrEmpty(program.Structure))
                        {
                            module.Structure = program.Structure;

                            // Ustaw statystyki modułu na podstawie struktury
                            UpdateModuleStatisticsFromStructure(module, program.Structure);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Błąd ładowania programu modułu: {ex.Message}");
                    }

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
        /// Aktualizuje statystyki modułu na podstawie struktury programu specjalizacji.
        /// </summary>
        /// <param name="module">Moduł do aktualizacji.</param>
        /// <param name="structureJson">Struktura programu w formacie JSON.</param>
        private static void UpdateModuleStatisticsFromStructure(Module module, string structureJson)
        {
            try
            {
                if (string.IsNullOrEmpty(structureJson))
                {
                    return;
                }

                // Deserializuj strukturę
                JsonDocument doc = JsonDocument.Parse(structureJson);
                JsonElement root = doc.RootElement;

                // Pobierz liczby staży, kursów i procedur
                if (root.TryGetProperty("internships", out JsonElement internshipsElement) &&
                    internshipsElement.ValueKind == JsonValueKind.Array)
                {
                    module.TotalInternships = internshipsElement.GetArrayLength();
                }

                if (root.TryGetProperty("courses", out JsonElement coursesElement) &&
                    coursesElement.ValueKind == JsonValueKind.Array)
                {
                    module.TotalCourses = coursesElement.GetArrayLength();
                }

                if (root.TryGetProperty("procedures", out JsonElement proceduresElement) &&
                    proceduresElement.ValueKind == JsonValueKind.Array)
                {
                    int totalProceduresA = 0;
                    int totalProceduresB = 0;

                    foreach (JsonElement procedure in proceduresElement.EnumerateArray())
                    {
                        if (procedure.TryGetProperty("requiredCountA", out JsonElement requiredCountA))
                        {
                            totalProceduresA += requiredCountA.GetInt32();
                        }

                        if (procedure.TryGetProperty("requiredCountB", out JsonElement requiredCountB))
                        {
                            totalProceduresB += requiredCountB.GetInt32();
                        }
                    }

                    module.TotalProceduresA = totalProceduresA;
                    module.TotalProceduresB = totalProceduresB;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd aktualizacji statystyk modułu: {ex.Message}");
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

            // Obliczenie finalnej daty zakończenia
            return baseEndDate.AddDays(extensionDays - reductionDays);
        }
    }
}
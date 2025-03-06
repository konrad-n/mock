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
                "cardiology",
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

            return moduleSpecializations.Contains(specializationCode.ToLowerInvariant());
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

            System.Diagnostics.Debug.WriteLine($"Szukam modułu podstawowego dla specjalizacji: {specializationCode}");

            // Dla kardiologii i pokrewnych specjalizacji wewnętrznych
            if (new[]
                {
                    "cardiology",
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
                }.Contains(specializationCode.ToLowerInvariant()))
            {
                System.Diagnostics.Debug.WriteLine("Znaleziono moduł podstawowy - internal_medicine");
                return "internal_medicine";
            }

            System.Diagnostics.Debug.WriteLine("Nie znaleziono modułu podstawowego");
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
                    Structure = $"{{ \"moduleName\": \"Moduł podstawowy w zakresie chorób wewnętrznych\", \"moduleType\": \"Basic\", \"durationMonths\": 24 }}",
                    CompletedInternships = 0,
                    TotalInternships = 10,
                    CompletedCourses = 0,
                    TotalCourses = 10,
                    CompletedProceduresA = 0,
                    TotalProceduresA = 50,
                    CompletedProceduresB = 0,
                    TotalProceduresB = 30
                };
                System.Diagnostics.Debug.WriteLine("Utworzono moduł podstawowy.");

                // Tworzenie modułu specjalistycznego (zwykle 3 lata lub więcej, w zależności od specjalizacji)
                var specialisticModule = new Module
                {
                    Type = ModuleType.Specialistic,
                    Name = $"Moduł specjalistyczny w zakresie {specializationCode}",
                    StartDate = startDate.AddYears(2),
                    EndDate = startDate.AddYears(5), // Standardowo 5 lat dla pełnej specjalizacji
                    Structure = $"{{ \"moduleName\": \"Moduł specjalistyczny w zakresie {specializationCode}\", \"moduleType\": \"Specialistic\", \"durationMonths\": 36 }}",
                    CompletedInternships = 0,
                    TotalInternships = 8,
                    CompletedCourses = 0,
                    TotalCourses = 18,
                    CompletedProceduresA = 0,
                    TotalProceduresA = 100,
                    CompletedProceduresB = 0,
                    TotalProceduresB = 50
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

                // Pobierz użytkownika
                var users = await databaseService.GetAllUsersAsync();
                var user = users.FirstOrDefault();
                if (user == null)
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
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;

namespace SledzSpecke.App.Services.Specialization
{
    /// <summary>
    /// Klasa odpowiedzialna za inicjalizację modułów specjalizacji na podstawie danych JSON.
    /// </summary>
    public class ModuleInitializer
    {
        private readonly IDatabaseService databaseService;

        public ModuleInitializer(IDatabaseService databaseService)
        {
            this.databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        }

        /// <summary>
        /// Inicjalizuje moduły dla specjalizacji, jeśli jeszcze nie istnieją.
        /// </summary>
        /// <param name="specializationId">ID specjalizacji do inicjalizacji.</param>
        /// <returns>True, jeśli inicjalizacja się powiodła; w przeciwnym razie false.</returns>
        public async Task<bool> InitializeModulesIfNeededAsync(int specializationId)
        {
            try
            {
                // Pobierz specjalizację
                var specialization = await this.databaseService.GetSpecializationAsync(specializationId);
                if (specialization == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Specjalizacja {specializationId} nie istnieje lub nie ma modułów");
                    return false;
                }

                // Sprawdź, czy moduły już istnieją
                var existingModules = await this.databaseService.GetModulesAsync(specializationId);
                if (existingModules != null && existingModules.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Dla specjalizacji {specialization.Name} już istnieją moduły ({existingModules.Count})");
                    return true; // Moduły już istnieją, nie ma potrzeby inicjalizacji
                }

                System.Diagnostics.Debug.WriteLine($"Rozpoczynam inicjalizację modułów dla specjalizacji {specialization.Name}");

                // Pobierz użytkownika, aby uzyskać informację o wersji SMK
                SmkVersion smkVersion = SmkVersion.New; // Domyślnie nowa wersja
                var users = await this.databaseService.GetAllUsersAsync();
                var user = users.Find(u => u.SpecializationId == specializationId);
                if (user != null)
                {
                    smkVersion = user.SmkVersion;
                }

                // Utwórz moduły na podstawie danych JSON
                var modules = await ModuleHelper.CreateModulesForSpecializationAsync(
                    specialization.ProgramCode,
                    specialization.StartDate,
                    smkVersion);

                if (modules == null || modules.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Nie udało się utworzyć modułów z danych JSON");
                    return await this.FallbackModuleCreationAsync(specialization);
                }

                // Zapisz utworzone moduły do bazy danych
                foreach (var module in modules)
                {
                    module.SpecializationId = specializationId;
                    int moduleId = await this.databaseService.SaveModuleAsync(module);
                    System.Diagnostics.Debug.WriteLine($"Zapisano moduł: {module.Name} z ID: {moduleId}");
                }

                // Ustaw domyślny bieżący moduł (pierwszy na liście)
                if (modules.Count > 0 && modules[0].ModuleId > 0)
                {
                    specialization.CurrentModuleId = modules[0].ModuleId;
                    await this.databaseService.UpdateSpecializationAsync(specialization);
                    System.Diagnostics.Debug.WriteLine($"Ustawiono bieżący moduł na: {modules[0].Name}");
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas inicjalizacji modułów: {ex.Message}");
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Tworzy domyślne moduły dla specjalizacji, gdy nie udało się wczytać ich z JSON.
        /// </summary>
        private async Task<bool> FallbackModuleCreationAsync(Models.Specialization specialization)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Tworzenie awaryjnych modułów dla specjalizacji");

                var modules = new List<Module>();

                if (specialization.ProgramCode.ToLowerInvariant() == "cardiology")
                {
                    // Moduł podstawowy - Choroby wewnętrzne
                    var basicModule = new Module
                    {
                        Name = "Moduł podstawowy w zakresie chorób wewnętrznych",
                        Type = ModuleType.Basic,
                        SpecializationId = specialization.SpecializationId,
                        StartDate = specialization.StartDate,
                        EndDate = specialization.StartDate.AddYears(2),
                        CompletedInternships = 0,
                        TotalInternships = 12,
                        CompletedCourses = 0,
                        TotalCourses = 10,
                        CompletedProceduresA = 0,
                        TotalProceduresA = 100,
                        CompletedProceduresB = 0,
                        TotalProceduresB = 50
                    };

                    // Moduł specjalistyczny - Kardiologia
                    var specialisticModule = new Module
                    {
                        Name = "Moduł specjalistyczny w zakresie kardiologii",
                        Type = ModuleType.Specialistic,
                        SpecializationId = specialization.SpecializationId,
                        StartDate = basicModule.EndDate.AddDays(1),
                        EndDate = basicModule.EndDate.AddDays(1).AddYears(3),
                        CompletedInternships = 0,
                        TotalInternships = 11,
                        CompletedCourses = 0,
                        TotalCourses = 18,
                        CompletedProceduresA = 0,
                        TotalProceduresA = 200,
                        CompletedProceduresB = 0,
                        TotalProceduresB = 100
                    };

                    modules.Add(basicModule);
                    modules.Add(specialisticModule);
                }
                else
                {
                    // Dla innych specjalizacji tworzymy jeden moduł specjalistyczny
                    var specialisticModule = new Module
                    {
                        Name = $"Moduł specjalistyczny w zakresie {specialization.Name.ToLowerInvariant()}",
                        Type = ModuleType.Specialistic,
                        SpecializationId = specialization.SpecializationId,
                        StartDate = specialization.StartDate,
                        EndDate = specialization.StartDate.AddYears(5),
                        CompletedInternships = 0,
                        TotalInternships = 10,
                        CompletedCourses = 0,
                        TotalCourses = 10,
                        CompletedProceduresA = 0,
                        TotalProceduresA = 50,
                        CompletedProceduresB = 0,
                        TotalProceduresB = 30
                    };

                    modules.Add(specialisticModule);
                }

                // Zapisz utworzone moduły do bazy danych
                foreach (var module in modules)
                {
                    int moduleId = await this.databaseService.SaveModuleAsync(module);
                    System.Diagnostics.Debug.WriteLine($"Zapisano awaryjny moduł: {module.Name} z ID: {moduleId}");
                }

                // Ustaw domyślny bieżący moduł (pierwszy na liście)
                if (modules.Count > 0)
                {
                    var firstModule = modules[0];
                    // Sprawdzamy czy moduł został zapisany i ma prawidłowe ID
                    if (firstModule.ModuleId > 0)
                    {
                        specialization.CurrentModuleId = firstModule.ModuleId;
                        await this.databaseService.UpdateSpecializationAsync(specialization);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas tworzenia awaryjnych modułów: {ex.Message}");
                return false;
            }
        }
    }
}
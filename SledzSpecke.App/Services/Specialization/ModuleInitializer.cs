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
                System.Diagnostics.Debug.WriteLine($"=== INICJALIZACJA MODUŁÓW DLA SPECJALIZACJI {specializationId} ===");

                // Pobierz specjalizację
                var specialization = await this.databaseService.GetSpecializationAsync(specializationId);
                if (specialization == null)
                {
                    System.Diagnostics.Debug.WriteLine("Nie znaleziono specjalizacji!");
                    return false;
                }

                System.Diagnostics.Debug.WriteLine($"Znaleziono specjalizację: {specialization.Name}, Wersja SMK: {specialization.SmkVersion}");

                // Sprawdź istniejące moduły
                var existingModules = await this.databaseService.GetModulesAsync(specializationId);
                System.Diagnostics.Debug.WriteLine($"Znaleziono {existingModules?.Count ?? 0} istniejących modułów");

                if (existingModules?.Count > 0)
                {
                    // Sprawdź i napraw przypisania modułów
                    var incorrectModules = existingModules.Where(m => m.SpecializationId != specializationId).ToList();
                    if (incorrectModules.Any())
                    {
                        System.Diagnostics.Debug.WriteLine($"Znaleziono {incorrectModules.Count} modułów z nieprawidłowym SpecializationId");
                        foreach (var module in incorrectModules)
                        {
                            System.Diagnostics.Debug.WriteLine($"Naprawa modułu {module.Name}: zmiana SpecializationId z {module.SpecializationId} na {specializationId}");
                            module.SpecializationId = specializationId;
                            await this.databaseService.UpdateModuleAsync(module);
                        }
                    }

                    System.Diagnostics.Debug.WriteLine("Moduły już istnieją i są poprawnie przypisane");
                    return true;
                }

                // Tworzenie nowych modułów
                System.Diagnostics.Debug.WriteLine("Tworzenie nowych modułów...");
                var modules = await ModuleHelper.CreateModulesForSpecializationAsync(
                    specialization.ProgramCode,
                    specialization.StartDate,
                    specialization.SmkVersion,
                    specializationId);

                if (modules == null || modules.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Nie udało się utworzyć modułów!");
                    return false;
                }

                // Zapisz moduły
                foreach (var module in modules)
                {
                    module.SpecializationId = specializationId;
                    int moduleId = await this.databaseService.SaveModuleAsync(module);
                    System.Diagnostics.Debug.WriteLine($"Zapisano moduł: {module.Name} (ID: {moduleId})");
                }

                System.Diagnostics.Debug.WriteLine("=== INICJALIZACJA MODUŁÓW ZAKOŃCZONA SUKCESEM ===");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas inicjalizacji modułów: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
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
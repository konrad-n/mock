using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;

namespace SledzSpecke.App.Helpers
{
    public class ProgressCalculator
    {
        /// <summary>
        /// Aktualizacja statystyk postępu dla modułu.
        /// </summary>
        /// <param name="database">Serwis dostępu do bazy danych.</param>
        /// <param name="moduleId">ID modułu do aktualizacji.</param>
        /// <returns>Zadanie asynchroniczne.</returns>
        public static async Task UpdateModuleProgressAsync(
    IDatabaseService database,
    int moduleId)
        {
            var module = await database.GetModuleAsync(moduleId);
            if (module == null)
            {
                return;
            }

            // Pobranie staży dla modułu
            var internships = await database.GetInternshipsAsync(moduleId: moduleId);
            var completedInternships = internships.Count(i => i.IsCompleted);

            // Pobranie kursów dla modułu
            var courses = await database.GetCoursesAsync(moduleId: moduleId);

            // Pobranie procedur powiązanych ze stażami w module
            var procedures = new List<Procedure>();
            foreach (var internship in internships)
            {
                var internshipProcedures = await database.GetProceduresAsync(internshipId: internship.InternshipId);
                procedures.AddRange(internshipProcedures);
            }

            // Zliczenie procedur typu A i B osobno
            var proceduresA = procedures.Count(p => p.OperatorCode == "A");
            var proceduresB = procedures.Count(p => p.OperatorCode == "B");

            // Pobranie dyżurów powiązanych ze stażami w module
            var shifts = new List<MedicalShift>();
            foreach (var internship in internships)
            {
                var internshipShifts = await database.GetMedicalShiftsAsync(internshipId: internship.InternshipId);
                shifts.AddRange(internshipShifts);
            }

            // Obliczenie całkowitej liczby godzin dyżurów
            double totalShiftHours = shifts.Sum(s => s.Hours + ((double)s.Minutes / 60.0));
            int completedShiftHours = (int)Math.Round(totalShiftHours);

            // Pobranie elementów samokształcenia
            var selfEducationItems = await database.GetSelfEducationItemsAsync(moduleId: moduleId);
            int completedSelfEducationDays = selfEducationItems.Count;

            // Parsowanie struktury modułu z JSON
            ModuleStructure moduleStructure = null;
            if (!string.IsNullOrEmpty(module.Structure))
            {
                moduleStructure = JsonSerializer.Deserialize<ModuleStructure>(module.Structure,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        AllowTrailingCommas = true,
                        ReadCommentHandling = JsonCommentHandling.Skip,
                        Converters = { new JsonStringEnumConverter() }
                    });
            }

            // Aktualizacja statystyk modułu
            module.CompletedInternships = completedInternships;
            module.TotalInternships = moduleStructure?.Internships?.Count ?? 0;
            module.CompletedCourses = courses.Count;
            module.TotalCourses = moduleStructure?.Courses?.Count ?? 0;
            module.CompletedProceduresA = proceduresA;
            module.TotalProceduresA = moduleStructure?.Procedures?.Sum(p => p.RequiredCountA) ?? 0;
            module.CompletedProceduresB = proceduresB;
            module.TotalProceduresB = moduleStructure?.Procedures?.Sum(p => p.RequiredCountB) ?? 0;

            // Aktualizacja nowych pól
            module.CompletedShiftHours = completedShiftHours;
            module.CompletedSelfEducationDays = completedSelfEducationDays;

            // Jeśli RequiredShiftHours nie jest jeszcze ustawione, a mamy dane w strukturze,
            // ustawiamy je teraz
            if (module.RequiredShiftHours <= 0)
            {
                if (moduleStructure?.RequiredShiftHours > 0)
                {
                    module.RequiredShiftHours = moduleStructure.RequiredShiftHours;
                }
                else if (moduleStructure?.MedicalShifts?.HoursPerWeek > 0)
                {
                    // Obliczenie liczby tygodni
                    TimeSpan moduleDuration = module.EndDate - module.StartDate;
                    int weeks = Math.Max(1, (int)(moduleDuration.TotalDays / 7));

                    // Ustawienie wymaganej liczby godzin
                    module.WeeklyShiftHours = moduleStructure.MedicalShifts.HoursPerWeek;
                    module.RequiredShiftHours = (int)Math.Round(module.WeeklyShiftHours * weeks);
                }
                else
                {
                    // Domyślna wartość - 10 godz. 5 min. tygodniowo
                    module.WeeklyShiftHours = 10.083;

                    // Obliczenie liczby tygodni
                    TimeSpan moduleDuration = module.EndDate - module.StartDate;
                    int weeks = Math.Max(1, (int)(moduleDuration.TotalDays / 7));

                    // Ustawienie wymaganej liczby godzin
                    module.RequiredShiftHours = (int)Math.Round(module.WeeklyShiftHours * weeks);
                }
            }

            // Jeśli TotalSelfEducationDays nie jest jeszcze ustawione, a mamy dane w strukturze,
            // ustawiamy je teraz
            if (module.TotalSelfEducationDays <= 0 && moduleStructure?.SelfEducationDays > 0)
            {
                module.TotalSelfEducationDays = moduleStructure.SelfEducationDays;
            }

            // Zapisanie zaktualizowanego modułu
            await database.UpdateModuleAsync(module);

            // Aktualizacja globalnych statystyk dla specjalizacji
            await UpdateSpecializationProgressAsync(database, module.SpecializationId);
        }

        /// <summary>
        /// Aktualizacja statystyk dla całej specjalizacji.
        /// </summary>
        /// <param name="database">Serwis dostępu do bazy danych.</param>
        /// <param name="specializationId">ID specjalizacji do aktualizacji.</param>
        /// <returns>Zadanie asynchroniczne.</returns>
        public static async Task UpdateSpecializationProgressAsync(
            IDatabaseService database,
            int specializationId)
        {
            var specialization = await database.GetSpecializationAsync(specializationId);
            if (specialization == null)
            {
                return;
            }

            // Agregacja danych ze wszystkich modułów
            var modules = await database.GetModulesAsync(specializationId);

            specialization.CompletedInternships = modules.Sum(m => m.CompletedInternships);
            specialization.TotalInternships = modules.Sum(m => m.TotalInternships);
            specialization.CompletedCourses = modules.Sum(m => m.CompletedCourses);
            specialization.TotalCourses = modules.Sum(m => m.TotalCourses);

            // Zapisanie zaktualizowanej specjalizacji
            await database.UpdateSpecializationAsync(specialization);
        }

        /// <summary>
        /// Obliczanie pełnych statystyk dla specjalizacji lub modułu.
        /// </summary>
        /// <param name="database">Serwis dostępu do bazy danych.</param>
        /// <param name="specializationId">ID specjalizacji.</param>
        /// <param name="moduleId">ID modułu (opcjonalnie).</param>
        /// <returns>Pełne statystyki specjalizacji.</returns>
        public static async Task<SpecializationStatistics> CalculateFullStatisticsAsync(
            IDatabaseService database,
            int specializationId,
            int? moduleId = null)
        {
            var specialization = await database.GetSpecializationAsync(specializationId);
            if (specialization == null)
            {
                return new SpecializationStatistics(); // Pusta statystyka z wartościami zerowymi
            }

            var stats = new SpecializationStatistics();

            // Parsowanie struktury specjalizacji z JSON
            SpecializationStructure structure = null;
            if (!string.IsNullOrEmpty(specialization.ProgramStructure))
            {
                structure = JsonSerializer.Deserialize<SpecializationStructure>(specialization.ProgramStructure);
            }
            if (moduleId.HasValue)
            {
                // Jeśli podano moduleId, obliczamy statystyki tylko dla tego modułu
                var module = await database.GetModuleAsync(moduleId.Value);
                if (module == null)
                {
                    return stats;
                }

                // Ustawiamy bazowe statystyki z obiektu modułu
                stats.CompletedInternships = module.CompletedInternships;
                stats.RequiredInternships = module.TotalInternships;
                stats.CompletedCourses = module.CompletedCourses;
                stats.RequiredCourses = module.TotalCourses;
                stats.CompletedProceduresA = module.CompletedProceduresA;
                stats.RequiredProceduresA = module.TotalProceduresA;
                stats.CompletedProceduresB = module.CompletedProceduresB;
                stats.RequiredProceduresB = module.TotalProceduresB;

                // Parsowanie struktury modułu
                ModuleStructure moduleStructure = null;
                if (!string.IsNullOrEmpty(module.Structure))
                {
                    try
                    {
                        // Dodajemy opcję PropertyNameCaseInsensitive, żeby adresować problem z wielkością liter
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            AllowTrailingCommas = true,
                            ReadCommentHandling = JsonCommentHandling.Skip,
                            Converters = { new JsonStringEnumConverter() }
                        };
                        moduleStructure = JsonSerializer.Deserialize<ModuleStructure>(module.Structure, options);

                        System.Diagnostics.Debug.WriteLine($"Sparsowano strukturę modułu: {(moduleStructure == null ? "null" : "ok")}");
                        if (moduleStructure?.MedicalShifts != null)
                        {
                            System.Diagnostics.Debug.WriteLine($"MedicalShifts znalezione: {moduleStructure.MedicalShifts.HoursPerWeek} h/tydzień");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("MedicalShifts nie znalezione w strukturze modułu");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Błąd podczas deserializacji struktury modułu: {ex.Message}");
                    }
                }

                // Obliczenie dni stażowych
                var internships = await database.GetInternshipsAsync(moduleId: moduleId);
                var completedInternships = internships.Where(i => i.IsCompleted).ToList();

                stats.CompletedInternshipDays = completedInternships.Sum(i => i.DaysCount);
                stats.RequiredInternshipWorkingDays = moduleStructure?.Internships?.Sum(i => i.WorkingDays) ?? 0;

                // Pobierz dyżury medyczne (zarówno dla nowego jak i starego SMK)
                var oldSmkShiftsQuery = "SELECT * FROM RealizedMedicalShiftOldSMK WHERE SpecializationId = ?";
                var oldSmkShifts = await database.QueryAsync<RealizedMedicalShiftOldSMK>(oldSmkShiftsQuery, specialization.SpecializationId);

                var newSmkShiftsQuery = "SELECT * FROM RealizedMedicalShiftNewSMK WHERE SpecializationId = ?";
                var newSmkShifts = await database.QueryAsync<RealizedMedicalShiftNewSMK>(newSmkShiftsQuery, specialization.SpecializationId);

                // Oblicz całkowitą liczbę godzin dyżurów
                double totalShiftHours = 0;

                // Oblicz godziny z dyżurów starego SMK
                foreach (var shift in oldSmkShifts)
                {
                    totalShiftHours += shift.Hours + ((double)shift.Minutes / 60.0);
                }

                // Oblicz godziny z dyżurów nowego SMK
                foreach (var shift in newSmkShifts)
                {
                    totalShiftHours += shift.Hours + ((double)shift.Minutes / 60.0);
                }

                stats.CompletedShiftHours = (int)Math.Round(totalShiftHours);

                // Pobieramy liczbę wymaganych godzin dyżurów bezpośrednio z modułu
                stats.RequiredShiftHours = module.RequiredShiftHours;

                // Jeśli z jakiegoś powodu RequiredShiftHours w module jest zerem,
                // próbujemy pobrać tę wartość z struktury modułu lub obliczyć
                if (stats.RequiredShiftHours == 0)
                {
                    // Sprawdźmy najpierw, czy bezpośrednio mamy podaną liczbę godzin w JSON
                    if (moduleStructure != null && moduleStructure.RequiredShiftHours > 0)
                    {
                        stats.RequiredShiftHours = moduleStructure.RequiredShiftHours;
                        System.Diagnostics.Debug.WriteLine($"Używam RequiredShiftHours z ModuleStructure JSON: {stats.RequiredShiftHours} h");

                        // Aktualizujemy pole w module
                        module.RequiredShiftHours = stats.RequiredShiftHours;
                        await database.UpdateModuleAsync(module);
                    }
                    else
                    {
                        // Jeśli nie mamy bezpośrednio podanej liczby godzin, obliczamy na podstawie hoursPerWeek
                        double weeklyHours = module.WeeklyShiftHours;

                        // Jeśli WeeklyShiftHours w module jest zerem, próbujemy pobrać z JSON lub używamy domyślnej
                        if (weeklyHours <= 0)
                        {
                            if (moduleStructure?.MedicalShifts != null && moduleStructure.MedicalShifts.HoursPerWeek > 0)
                            {
                                weeklyHours = moduleStructure.MedicalShifts.HoursPerWeek;
                                System.Diagnostics.Debug.WriteLine($"Używam hoursPerWeek z ModuleStructure JSON: {weeklyHours} h/tydzień");

                                // Aktualizujemy pole w module
                                module.WeeklyShiftHours = weeklyHours;
                            }
                            else
                            {
                                // Domyślna wartość
                                weeklyHours = 10.083; // 10 godz. 5 min. tygodniowo
                                System.Diagnostics.Debug.WriteLine($"Używam domyślnych godzin dyżurów: {weeklyHours} h/tydzień");

                                // Aktualizujemy pole w module
                                module.WeeklyShiftHours = weeklyHours;
                            }
                        }

                        // Obliczenie liczby tygodni
                        TimeSpan moduleDuration = module.EndDate - module.StartDate;
                        int weeks = Math.Max(1, (int)(moduleDuration.TotalDays / 7));

                        // Obliczamy wymaganą liczbę godzin
                        stats.RequiredShiftHours = (int)Math.Round(weeklyHours * weeks);
                        System.Diagnostics.Debug.WriteLine($"Obliczone wymagane godziny dyżurów: {stats.RequiredShiftHours} h ({weeklyHours} h/tydzień × {weeks} tygodni)");

                        // Aktualizujemy pole w module
                        module.RequiredShiftHours = stats.RequiredShiftHours;
                        await database.UpdateModuleAsync(module);
                    }
                }

                // Samokształcenie
                var selfEducationItems = await database.GetSelfEducationItemsAsync(moduleId: moduleId);
                stats.SelfEducationDaysUsed = selfEducationItems.Count;
                stats.SelfEducationDaysTotal = moduleStructure?.SelfEducationDays ?? 0;

                // Inne aktywności
                var educationalActivities = await database.GetEducationalActivitiesAsync(moduleId: moduleId);
                stats.EducationalActivitiesCompleted = educationalActivities.Count;

                var publications = await database.GetPublicationsAsync(moduleId: moduleId);
                stats.PublicationsCompleted = publications.Count;

                return stats;
            }
            else
            {
                // Jeśli nie podano moduleId, obliczamy statystyki dla całej specjalizacji (dotychczasowa implementacja)
                var modules = await database.GetModulesAsync(specializationId);

                // Agregacja danych ze wszystkich modułów
                stats.CompletedInternships = modules.Sum(m => m.CompletedInternships);
                stats.RequiredInternships = modules.Sum(m => m.TotalInternships);
                stats.CompletedCourses = modules.Sum(m => m.CompletedCourses);
                stats.RequiredCourses = modules.Sum(m => m.TotalCourses);
                stats.CompletedProceduresA = modules.Sum(m => m.CompletedProceduresA);
                stats.RequiredProceduresA = modules.Sum(m => m.TotalProceduresA);
                stats.CompletedProceduresB = modules.Sum(m => m.CompletedProceduresB);
                stats.RequiredProceduresB = modules.Sum(m => m.TotalProceduresB);

                // Obliczenie dni stażowych
                var internships = new List<Internship>();
                foreach (var module in modules)
                {
                    var moduleInternships = await database.GetInternshipsAsync(moduleId: module.ModuleId);
                    internships.AddRange(moduleInternships.Where(i => i.IsCompleted));
                }

                stats.CompletedInternshipDays = internships.Sum(i => i.DaysCount);
                stats.RequiredInternshipWorkingDays = structure?.TotalWorkingDays ?? 0;

                // Pobierz dyżury powiązane ze stażami
                var allShifts = new List<MedicalShift>();
                foreach (var internship in internships)
                {
                    var shifts = await database.GetMedicalShiftsAsync(internship.InternshipId);
                    allShifts.AddRange(shifts);
                }

                stats.CompletedShiftHours = (int)Math.Round(allShifts.Sum(s => s.Hours + ((double)s.Minutes / 60.0)));
                stats.RequiredShiftHours = (int)Math.Round(
                    CalculateRequiredShiftHours(structure, specialization.PlannedEndDate - specialization.StartDate));

                // Samokształcenie
                var selfEducationItems = await database.GetSelfEducationItemsAsync(specializationId: specializationId);
                stats.SelfEducationDaysUsed = selfEducationItems.Count;
                stats.SelfEducationDaysTotal = structure?.SelfEducation?.TotalDays ?? 0;

                // Inne aktywności
                var educationalActivities = await database.GetEducationalActivitiesAsync(specializationId: specializationId);
                stats.EducationalActivitiesCompleted = educationalActivities.Count;

                var publications = await database.GetPublicationsAsync(specializationId: specializationId);
                stats.PublicationsCompleted = publications.Count;

                // Nieobecności
                var absences = await database.GetAbsencesAsync(specializationId);
                stats.AbsenceDays = absences.Sum(a => a.DaysCount);
                stats.AbsenceDaysExtendingSpecialization = absences
                    .Where(a => a.ExtendsSpecialization)
                    .Sum(a => a.DaysCount);
            }

            return stats;
        }

        /// <summary>
        /// Oblicza wymaganą liczbę godzin dyżurów na podstawie struktury specjalizacji.
        /// </summary>
        /// <param name="structure">Struktura specjalizacji z JSONa.</param>
        /// <param name="duration">Czas trwania specjalizacji.</param>
        /// <returns>Wymagana liczba godzin dyżurów.</returns>
        private static double CalculateRequiredShiftHours(SpecializationStructure structure, TimeSpan duration)
        {
            // Dodajemy diagnostykę, aby zobaczyć, co otrzymuje funkcja
            System.Diagnostics.Debug.WriteLine($"CalculateRequiredShiftHours: structure is {(structure == null ? "null" : "not null")}");
            System.Diagnostics.Debug.WriteLine($"CalculateRequiredShiftHours: duration is {duration.TotalDays} days");

            // Jeśli struktura lub struktura.MedicalShifts jest null, użyj domyślnej wartości
            if (structure?.MedicalShifts == null)
            {
                System.Diagnostics.Debug.WriteLine("CalculateRequiredShiftHours: structure.MedicalShifts is null, using default value");

                // Domyślna wartość - 10 godzin 5 minut tygodniowo (zgodnie z dokumentacją)
                double defaultHoursPerWeek = 10.083; // 10 + (5/60)
                int weeksInSpec = Math.Max(1, (int)(duration.TotalDays / 7)); // minimum 1 tydzień

                double defaultValue = defaultHoursPerWeek * weeksInSpec;
                System.Diagnostics.Debug.WriteLine($"CalculateRequiredShiftHours: Default calculation: {defaultHoursPerWeek} * {weeksInSpec} = {defaultValue}");

                return defaultValue;
            }

            // Obliczenie na podstawie tygodniowego wymogu z JSONa
            double weeklyHours = structure.MedicalShifts.HoursPerWeek;
            System.Diagnostics.Debug.WriteLine($"CalculateRequiredShiftHours: Weekly hours from structure: {weeklyHours}");

            // Jeśli weeklyHours jest zbyt małe (praktycznie zero), użyj domyślnej wartości
            if (weeklyHours < 0.1)
            {
                System.Diagnostics.Debug.WriteLine("CalculateRequiredShiftHours: Weekly hours too small, using default");
                weeklyHours = 10.083; // 10 godzin 5 minut
            }

            int weeks = Math.Max(1, (int)(duration.TotalDays / 7));
            double result = weeklyHours * weeks;

            System.Diagnostics.Debug.WriteLine($"CalculateRequiredShiftHours: Final calculation: {weeklyHours} * {weeks} = {result}");
            return result;
        }

        /// <summary>
        /// Oblicza ogólny postęp dla modułu lub całej specjalizacji.
        /// </summary>
        /// <param name="database">Serwis dostępu do bazy danych.</param>
        /// <param name="specializationId">ID specjalizacji.</param>
        /// <param name="moduleId">ID modułu (opcjonalnie).</param>
        /// <returns>Wartość postępu (0.0 - 1.0).</returns>
        public static async Task<double> GetOverallProgressAsync(
            IDatabaseService database,
            int specializationId,
            int? moduleId = null)
        {
            if (moduleId.HasValue)
            {
                // Obliczanie postępu dla konkretnego modułu
                var module = await database.GetModuleAsync(moduleId.Value);
                if (module == null)
                {
                    return 0;
                }

                // Wagi dla różnych kategorii
                const double internshipWeight = 0.35;
                const double courseWeight = 0.25;
                const double procedureWeight = 0.30;
                const double otherWeight = 0.10;

                // Obliczanie procentu ukończenia dla każdej kategorii
                double internshipProgress = module.TotalInternships > 0
                    ? (double)module.CompletedInternships / module.TotalInternships
                    : 0;

                double courseProgress = module.TotalCourses > 0
                    ? (double)module.CompletedCourses / module.TotalCourses
                    : 0;

                // Postęp w procedurach (osobno A i B)
                double procedureProgressA = module.TotalProceduresA > 0
                    ? (double)module.CompletedProceduresA / module.TotalProceduresA
                    : 0;

                double procedureProgressB = module.TotalProceduresB > 0
                    ? (double)module.CompletedProceduresB / module.TotalProceduresB
                    : 0;

                // Połączony postęp procedur
                double procedureProgress;

                if (module.TotalProceduresA + module.TotalProceduresB > 0)
                {
                    // Ważenie według liczby wymaganych procedur każdego typu
                    procedureProgress =
                        (procedureProgressA * module.TotalProceduresA +
                         procedureProgressB * module.TotalProceduresB) /
                        (module.TotalProceduresA + module.TotalProceduresB);
                }
                else
                {
                    procedureProgress = 0;
                }

                // Pobierz liczby elementów samokształcenia i publikacji
                var selfEducationItems = await database.GetSelfEducationItemsAsync(moduleId: moduleId);
                var publications = await database.GetPublicationsAsync(moduleId: moduleId);
                var educationalActivities = await database.GetEducationalActivitiesAsync(moduleId: moduleId);

                // Zakładamy, że dla innych aktywności (samokształcenie, publikacje) 
                // nie ma ustalonego maksimum, więc liczymy to jako proporcję do ilości
                double otherActivitiesProgress = 0;
                int totalOtherItems = selfEducationItems.Count + publications.Count + educationalActivities.Count;

                // Jeśli są jakieś elementy, przyznajemy punkty proporcjonalnie do ich liczby
                // (maksymalnie do 10 elementów uznajemy za 100% postępu w tej kategorii)
                if (totalOtherItems > 0)
                {
                    otherActivitiesProgress = Math.Min(1.0, totalOtherItems / 10.0);
                }

                // Ważony ogólny postęp - zmieniamy obliczanie wagi innych aktywności
                double overallProgress = (internshipProgress * internshipWeight) +
                                         (courseProgress * courseWeight) +
                                         (procedureProgress * procedureWeight) +
                                         (otherActivitiesProgress * otherWeight);

                return Math.Min(1.0, overallProgress);
            }
            else
            {
                // Obliczanie postępu dla całej specjalizacji
                var stats = await CalculateFullStatisticsAsync(database, specializationId);
                return stats.GetOverallProgress();
            }
        }
    }
}
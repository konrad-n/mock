using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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

            // Parsowanie struktury modułu z JSON
            ModuleStructure moduleStructure = null;
            if (!string.IsNullOrEmpty(module.Structure))
            {
                moduleStructure = JsonSerializer.Deserialize<ModuleStructure>(module.Structure);
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
        /// Obliczanie pełnych statystyk dla specjalizacji.
        /// </summary>
        /// <param name="database">Serwis dostępu do bazy danych.</param>
        /// <param name="specializationId">ID specjalizacji.</param>
        /// <returns>Pełne statystyki specjalizacji.</returns>
        public static async Task<SpecializationStatistics> CalculateFullStatisticsAsync(
            IDatabaseService database,
            int specializationId)
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
            stats.RequiredInternshipDays = structure?.TotalWorkingDays ?? 0;

            // Pobierz dyżury powiązane ze stażami
            var allShifts = new List<MedicalShift>();
            foreach (var internship in internships)
            {
                var shifts = await database.GetMedicalShiftsAsync(internship.InternshipId);
                allShifts.AddRange(shifts);
            }

            stats.CompletedShiftHours = (int)Math.Round(allShifts.Sum(s => s.Hours + (s.Minutes / 60.0)));
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
            if (structure?.MedicalShifts == null)
            {
                return 0;
            }

            // Obliczenie na podstawie tygodniowego wymogu z JSONa
            double weeklyHours = structure.MedicalShifts.HoursPerWeek;
            int weeks = (int)(duration.TotalDays / 7);

            return weeklyHours * weeks;
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

                // Ważony ogólny postęp
                double overallProgress = (internshipProgress * internshipWeight) +
                                         (courseProgress * courseWeight) +
                                         (procedureProgress * procedureWeight) +
                                         otherWeight; // Wkład innych aktywności

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
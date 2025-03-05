using System.Text.Json;
using SledzSpecke.App.Models;
using SledzSpecke.App.Services.Database;

namespace SledzSpecke.App.Helpers
{
    public class ProgressCalculator
    {
        // Aktualizacja statystyk postępu dla modułu
        public static async Task UpdateModuleProgressAsync(
            IDatabaseService database,
            int moduleId)
        {
            var module = await database.GetModuleAsync(moduleId);
            if (module == null)
            {
                return;
            }

            // Pobranie internships dla modułu
            var internships = await database.GetInternshipsAsync(moduleId: moduleId);
            var completedInternships = internships.Count(i => i.IsCompleted);

            // Pobranie kursów dla modułu
            var courses = await database.GetCoursesAsync(moduleId: moduleId);

            // Pobranie procedur powiązanych z internships w module
            var procedures = new List<Procedure>();
            foreach (var internship in internships)
            {
                var internshipProcedures = await database.GetProceduresAsync(internshipId: internship.InternshipId);
                procedures.AddRange(internshipProcedures);
            }

            // Zliczanie procedur typu A i B
            var proceduresA = procedures.Count(p => p.OperatorCode == "A");
            var proceduresB = procedures.Count(p => p.OperatorCode == "B");

            // Wczytanie definicji z programu modułu
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

        // Aktualizacja statystyk dla całej specjalizacji
        public static async Task UpdateSpecializationProgressAsync(
            IDatabaseService database,
            int specializationId)
        {
            var specialization = await database.GetSpecializationAsync(specializationId);
            if (specialization == null)
            {
                return;
            }

            if (specialization.HasModules)
            {
                // Jeśli specjalizacja ma moduły, agreguj dane z modułów
                var modules = await database.GetModulesAsync(specializationId);

                specialization.CompletedInternships = modules.Sum(m => m.CompletedInternships);
                specialization.TotalInternships = modules.Sum(m => m.TotalInternships);
                specialization.CompletedCourses = modules.Sum(m => m.CompletedCourses);
                specialization.TotalCourses = modules.Sum(m => m.TotalCourses);
            }
            else
            {
                // Jeśli nie ma modułów, oblicz statystyki bezpośrednio
                var internships = await database.GetInternshipsAsync(specializationId: specializationId);
                specialization.CompletedInternships = internships.Count(i => i.IsCompleted);

                var courses = await database.GetCoursesAsync(specializationId: specializationId);
                specialization.CompletedCourses = courses.Count;

                // Wczytanie definicji z programu specjalizacji dla wymaganych wartości
                SpecializationStructure specializationStructure = null;
                if (!string.IsNullOrEmpty(specialization.ProgramStructure))
                {
                    specializationStructure = JsonSerializer.Deserialize<SpecializationStructure>(
                        specialization.ProgramStructure);

                    specialization.TotalInternships = specializationStructure?.Internships?.Count ?? 0;
                    specialization.TotalCourses = specializationStructure?.Courses?.Count ?? 0;
                }
            }

            // Zapisanie zaktualizowanej specjalizacji
            await database.UpdateSpecializationAsync(specialization);
        }

        // Obliczanie pełnych statystyk dla specjalizacji
        public static async Task<SpecializationStatistics> CalculateFullStatisticsAsync(
            IDatabaseService database,
            int specializationId)
        {
            var specialization = await database.GetSpecializationAsync(specializationId);
            if (specialization == null)
            {
                return new SpecializationStatistics();
            }

            var stats = new SpecializationStatistics();

            // Pobierz dane o strukturze specjalizacji
            SpecializationStructure structure = null;
            if (!string.IsNullOrEmpty(specialization.ProgramStructure))
            {
                structure = JsonSerializer.Deserialize<SpecializationStructure>(specialization.ProgramStructure);
            }

            if (specialization.HasModules)
            {
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

                // Pobierz dane o dyżurach
                var allShifts = new List<MedicalShift>();
                foreach (var internship in internships)
                {
                    var shifts = await database.GetMedicalShiftsAsync(internship.InternshipId);
                    allShifts.AddRange(shifts);
                }

                stats.CompletedShiftHours = (int)Math.Round(allShifts.Sum(s => s.Hours + (s.Minutes / 60.0)));
                stats.RequiredShiftHours = (int)Math.Round(CalculateRequiredShiftHours(structure, specialization.PlannedEndDate - specialization.StartDate));
            }
            else
            {
                // Pobierz dane o internships
                var internships = await database.GetInternshipsAsync(specializationId: specializationId);
                stats.CompletedInternships = internships.Count(i => i.IsCompleted);
                stats.RequiredInternships = structure?.Internships?.Count ?? 0;
                stats.CompletedInternshipDays = internships.Where(i => i.IsCompleted).Sum(i => i.DaysCount);
                stats.RequiredInternshipDays = structure?.TotalWorkingDays ?? 0;

                // Pobierz dane o kursach
                var courses = await database.GetCoursesAsync(specializationId: specializationId);
                stats.CompletedCourses = courses.Count;
                stats.RequiredCourses = structure?.Courses?.Count ?? 0;

                // Pobierz dane o procedurach
                var procedures = new List<Procedure>();
                foreach (var internship in internships)
                {
                    var internshipProcedures = await database.GetProceduresAsync(internshipId: internship.InternshipId);
                    procedures.AddRange(internshipProcedures);
                }

                stats.CompletedProceduresA = procedures.Count(p => p.OperatorCode == "A");
                stats.CompletedProceduresB = procedures.Count(p => p.OperatorCode == "B");

                // Łączna liczba wymaganych procedur z definicji specjalizacji
                stats.RequiredProceduresA = structure?.Procedures?.Sum(p => p.RequiredCountA) ?? 0;
                stats.RequiredProceduresB = structure?.Procedures?.Sum(p => p.RequiredCountB) ?? 0;

                // Pobierz dane o dyżurach
                var allShifts = new List<MedicalShift>();
                foreach (var internship in internships)
                {
                    var shifts = await database.GetMedicalShiftsAsync(internship.InternshipId);
                    allShifts.AddRange(shifts);
                }

                stats.CompletedShiftHours = (int)Math.Round(allShifts.Sum(s => s.Hours + (s.Minutes / 60.0)));
                stats.RequiredShiftHours = (int)Math.Round(CalculateRequiredShiftHours(structure, specialization.PlannedEndDate - specialization.StartDate));
            }

            // Pobierz dane o samokształceniu
            var selfEducationItems = await database.GetSelfEducationItemsAsync(specializationId: specializationId);
            stats.SelfEducationDaysUsed = selfEducationItems.Count;
            stats.SelfEducationDaysTotal = structure?.SelfEducation?.TotalDays ?? 0;

            // Pobierz dane o aktywnościach edukacyjnych i publikacjach
            var educationalActivities = await database.GetEducationalActivitiesAsync(specializationId: specializationId);
            stats.EducationalActivitiesCompleted = educationalActivities.Count;

            var publications = await database.GetPublicationsAsync(specializationId: specializationId);
            stats.PublicationsCompleted = publications.Count;

            // Pobierz dane o absencjach
            var absences = await database.GetAbsencesAsync(specializationId);
            stats.AbsenceDays = absences.Sum(a => a.DaysCount);
            stats.AbsenceDaysExtendingSpecialization = absences.Where(a => a.ExtendsSpecialization).Sum(a => a.DaysCount);

            return stats;
        }

        // Metoda obliczająca wymaganą liczbę godzin dyżurów na podstawie programu specjalizacji
        private static double CalculateRequiredShiftHours(SpecializationStructure structure, TimeSpan duration)
        {
            if (structure?.MedicalShifts == null)
            {
                return 0;
            }

            // Obliczenie na podstawie tygodniowego wymogu
            double weeklyHours = structure.MedicalShifts.HoursPerWeek;
            int weeks = (int)(duration.TotalDays / 7);

            return weeklyHours * weeks;
        }

        // Metoda pobierająca dane o postępach dla modułu lub całej specjalizacji
        public static async Task<double> GetOverallProgressAsync(
            IDatabaseService database,
            int specializationId,
            int? moduleId = null)
        {
            if (moduleId.HasValue)
            {
                // Obliczanie postępu dla modułu
                var module = await database.GetModuleAsync(moduleId.Value);
                if (module == null)
                {
                    return 0;
                }

                // Wagi dla różnych kategorii
                const double internshipWeight = 0.35;
                const double courseWeight = 0.25;
                const double procedureWeight = 0.3;
                const double otherWeight = 0.1;

                // Obliczanie procentu ukończenia dla każdej kategorii
                double internshipProgress = module.TotalInternships > 0
                    ? (double)module.CompletedInternships / module.TotalInternships
                    : 0;

                double courseProgress = module.TotalCourses > 0
                    ? (double)module.CompletedCourses / module.TotalCourses
                    : 0;

                double procedureProgressA = module.TotalProceduresA > 0
                    ? (double)module.CompletedProceduresA / module.TotalProceduresA
                    : 0;

                double procedureProgressB = module.TotalProceduresB > 0
                    ? (double)module.CompletedProceduresB / module.TotalProceduresB
                    : 0;

                double procedureProgress = (procedureProgressA + procedureProgressB) / 2;

                // Średni procent ukończenia ważony
                double overallProgress = (internshipProgress * internshipWeight) +
                                         (courseProgress * courseWeight) +
                                         (procedureProgress * procedureWeight) +
                                         otherWeight; // Pozostałe aktywności

                return Math.Min(1.0, overallProgress);
            }
            else
            {
                // Obliczanie postępu dla całej specjalizacji
                var stats = await CalculateFullStatisticsAsync(database, specializationId);
                if (stats == null)
                {
                    return 0;
                }

                return stats.GetOverallProgress();
            }
        }
    }
}
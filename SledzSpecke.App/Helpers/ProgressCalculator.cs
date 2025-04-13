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
        public static async Task UpdateModuleProgressAsync(
            IDatabaseService database,
            int moduleId)
        {
            var module = await database.GetModuleAsync(moduleId);
            if (module == null)
            {
                return;
            }

            var internships = await database.GetInternshipsAsync(moduleId: moduleId);
            var completedInternships = internships.Count(i => i.IsCompleted);
            var courses = await database.GetCoursesAsync(moduleId: moduleId);
            var procedures = new List<Procedure>();

            foreach (var internship in internships)
            {
                var internshipProcedures = await database.GetProceduresAsync(internshipId: internship.InternshipId);
                procedures.AddRange(internshipProcedures);
            }

            var proceduresA = procedures.Count(p => p.OperatorCode == "A");
            var proceduresB = procedures.Count(p => p.OperatorCode == "B");
            var shifts = new List<MedicalShift>();

            foreach (var internship in internships)
            {
                var internshipShifts = await database.GetMedicalShiftsAsync(internshipId: internship.InternshipId);
                shifts.AddRange(internshipShifts);
            }

            double totalShiftHours = shifts.Sum(s => s.Hours + ((double)s.Minutes / 60.0));
            int completedShiftHours = (int)Math.Round(totalShiftHours);
            var selfEducationItems = await database.GetSelfEducationItemsAsync(moduleId: moduleId);
            int completedSelfEducationDays = selfEducationItems.Count;
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

            module.CompletedInternships = completedInternships;
            module.TotalInternships = moduleStructure?.Internships?.Count ?? 0;
            module.CompletedCourses = courses.Count;
            module.TotalCourses = moduleStructure?.Courses?.Count ?? 0;
            module.CompletedProceduresA = proceduresA;
            module.TotalProceduresA = moduleStructure?.Procedures?.Sum(p => p.RequiredCountA) ?? 0;
            module.CompletedProceduresB = proceduresB;
            module.TotalProceduresB = moduleStructure?.Procedures?.Sum(p => p.RequiredCountB) ?? 0;
            module.CompletedShiftHours = completedShiftHours;
            module.CompletedSelfEducationDays = completedSelfEducationDays;

            if (module.RequiredShiftHours <= 0)
            {
                if (moduleStructure?.RequiredShiftHours > 0)
                {
                    module.RequiredShiftHours = moduleStructure.RequiredShiftHours;
                }
                else if (moduleStructure?.MedicalShifts?.HoursPerWeek > 0)
                {
                    TimeSpan moduleDuration = module.EndDate - module.StartDate;
                    int weeks = Math.Max(1, (int)(moduleDuration.TotalDays / 7));
                    module.WeeklyShiftHours = moduleStructure.MedicalShifts.HoursPerWeek;
                    module.RequiredShiftHours = (int)Math.Round(module.WeeklyShiftHours * weeks);
                }
                else
                {
                    module.WeeklyShiftHours = 10.083;
                    TimeSpan moduleDuration = module.EndDate - module.StartDate;
                    int weeks = Math.Max(1, (int)(moduleDuration.TotalDays / 7));
                    module.RequiredShiftHours = (int)Math.Round(module.WeeklyShiftHours * weeks);
                }
            }

            if (module.TotalSelfEducationDays <= 0 && moduleStructure?.SelfEducationDays > 0)
            {
                module.TotalSelfEducationDays = moduleStructure.SelfEducationDays;
            }

            await database.UpdateModuleAsync(module);
            await UpdateSpecializationProgressAsync(database, module.SpecializationId);
        }

        public static async Task UpdateSpecializationProgressAsync(
            IDatabaseService database,
            int specializationId)
        {
            var specialization = await database.GetSpecializationAsync(specializationId);
            if (specialization == null)
            {
                return;
            }

            var modules = await database.GetModulesAsync(specializationId);
            specialization.CompletedInternships = modules.Sum(m => m.CompletedInternships);
            specialization.TotalInternships = modules.Sum(m => m.TotalInternships);
            specialization.CompletedCourses = modules.Sum(m => m.CompletedCourses);
            specialization.TotalCourses = modules.Sum(m => m.TotalCourses);
            await database.UpdateSpecializationAsync(specialization);
        }

        public static async Task<SpecializationStatistics> CalculateFullStatisticsAsync(
    IDatabaseService databaseService,
    int specializationId,
    int? moduleId = null)
        {
            var statistics = new SpecializationStatistics();

            var specialization = await databaseService.GetSpecializationAsync(specializationId);
            if (specialization == null)
            {
                return statistics;
            }

            var user = await databaseService.GetUserByUsernameAsync(SettingsHelper.GetCurrentUsername());
            if (user == null)
            {
                return statistics;
            }

            // Pobieramy moduły specjalizacji
            var modules = await databaseService.GetModulesAsync(specializationId);
            if (modules.Count == 0)
            {
                return statistics;
            }

            // Jeśli nie podano konkretnego modułu, bierzemy aktualny moduł
            if (!moduleId.HasValue && specialization.CurrentModuleId.HasValue)
            {
                moduleId = specialization.CurrentModuleId.Value;
            }

            // Obliczanie statystyk staży
            if (user.SmkVersion == SmkVersion.New)
            {
                // Pobieramy zrealizowane staże dla modułu
                var realizedInternships = await databaseService.GetRealizedInternshipsNewSMKAsync(
                    specializationId,
                    moduleId);

                statistics.CompletedInternshipDays = realizedInternships.Sum(r => r.DaysCount);

                // Obliczamy liczbę ukończonych staży
                var internshipRequirements = await databaseService.GetInternshipsAsync(
                    specializationId,
                    moduleId);

                foreach (var requirement in internshipRequirements)
                {
                    var requirementRealizations = realizedInternships
                        .Where(r => r.InternshipRequirementId == requirement.InternshipId)
                        .ToList();

                    int totalDays = requirementRealizations.Sum(r => r.DaysCount);
                    if (totalDays >= requirement.DaysCount)
                    {
                        statistics.CompletedInternships++;
                    }
                }

                statistics.RequiredInternships = internshipRequirements.Count;
                statistics.RequiredInternshipWorkingDays = internshipRequirements.Sum(i => i.DaysCount);
            }
            else
            {
                // Dla starego SMK
                var realizedInternships = await databaseService.GetRealizedInternshipsOldSMKAsync(specializationId);
                statistics.CompletedInternshipDays = realizedInternships.Sum(r => r.DaysCount);

                // Grupujemy po nazwie stażu
                var internshipGroups = realizedInternships
                    .GroupBy(r => r.InternshipName)
                    .ToDictionary(g => g.Key, g => g.Sum(r => r.DaysCount));

                // Pobieramy staże wymagane
                var internshipRequirements = await databaseService.GetInternshipsAsync(
                    specializationId,
                    moduleId);

                foreach (var requirement in internshipRequirements)
                {
                    if (internshipGroups.TryGetValue(requirement.InternshipName, out int totalDays))
                    {
                        if (totalDays >= requirement.DaysCount)
                        {
                            statistics.CompletedInternships++;
                        }
                    }
                }

                statistics.RequiredInternships = internshipRequirements.Count;
                statistics.RequiredInternshipWorkingDays = internshipRequirements.Sum(i => i.DaysCount);
            }

            // Obliczanie statystyk kursów
            var courses = await databaseService.GetCoursesAsync(specializationId, moduleId);
            statistics.CompletedCourses = courses.Count;

            Module currentModule = null;
            if (moduleId.HasValue)
            {
                currentModule = modules.FirstOrDefault(m => m.ModuleId == moduleId.Value);
            }
            else if (modules.Count > 0)
            {
                currentModule = modules[0];
            }

            if (currentModule != null)
            {
                statistics.RequiredCourses = currentModule.TotalCourses;
                statistics.RequiredProceduresA = currentModule.TotalProceduresA;
                statistics.RequiredProceduresB = currentModule.TotalProceduresB;
                statistics.RequiredShiftHours = currentModule.RequiredShiftHours;
                statistics.SelfEducationDaysTotal = currentModule.TotalSelfEducationDays;
            }

            // Obliczanie statystyk dyżurów
            if (user.SmkVersion == SmkVersion.New)
            {
                if (moduleId.HasValue)
                {
                    var newShifts = await databaseService.QueryAsync<RealizedMedicalShiftNewSMK>(
                        "SELECT * FROM RealizedMedicalShiftNewSMK WHERE SpecializationId = ? AND ModuleId = ?",
                        specializationId, moduleId.Value);

                    double totalHours = 0;

                    foreach (var shift in newShifts)
                    {
                        totalHours += shift.Hours + (shift.Minutes / 60.0);
                    }

                    statistics.CompletedShiftHours = (int)Math.Round(totalHours);
                }
                else
                {
                    var newShifts = await databaseService.QueryAsync<RealizedMedicalShiftNewSMK>(
                        "SELECT * FROM RealizedMedicalShiftNewSMK WHERE SpecializationId = ?",
                        specializationId);

                    double totalHours = 0;

                    foreach (var shift in newShifts)
                    {
                        totalHours += shift.Hours + (shift.Minutes / 60.0);
                    }

                    statistics.CompletedShiftHours = (int)Math.Round(totalHours);
                }
            }
            else
            {
                var oldShifts = await databaseService.QueryAsync<RealizedMedicalShiftOldSMK>(
                    "SELECT * FROM RealizedMedicalShiftOldSMK WHERE SpecializationId = ?",
                    specializationId);

                double totalHours = 0;

                foreach (var shift in oldShifts)
                {
                    totalHours += shift.Hours + (shift.Minutes / 60.0);
                }

                // Filtrowanie dla modułu podstawowego (lata 1-2) i specjalistycznego (lata 3+)
                if (moduleId.HasValue)
                {
                    var module = modules.FirstOrDefault(m => m.ModuleId == moduleId.Value);
                    if (module != null)
                    {
                        if (module.Type == ModuleType.Basic)
                        {
                            oldShifts = oldShifts.Where(s => s.Year <= 2).ToList();
                        }
                        else
                        {
                            oldShifts = oldShifts.Where(s => s.Year >= 3).ToList();
                        }

                        totalHours = 0;
                        foreach (var shift in oldShifts)
                        {
                            totalHours += shift.Hours + (shift.Minutes / 60.0);
                        }
                    }
                }

                statistics.CompletedShiftHours = (int)Math.Round(totalHours);
            }

            // Obliczanie statystyk procedur
            if (user.SmkVersion == SmkVersion.New)
            {
                if (moduleId.HasValue)
                {
                    var newProcedures = await databaseService.QueryAsync<RealizedProcedureNewSMK>(
                        "SELECT * FROM RealizedProcedureNewSMK WHERE SpecializationId = ? AND ModuleId = ?",
                        specializationId, moduleId.Value);

                    statistics.CompletedProceduresA = newProcedures.Sum(p => p.CountA);
                    statistics.CompletedProceduresB = newProcedures.Sum(p => p.CountB);
                }
                else
                {
                    var newProcedures = await databaseService.QueryAsync<RealizedProcedureNewSMK>(
                        "SELECT * FROM RealizedProcedureNewSMK WHERE SpecializationId = ?",
                        specializationId);

                    statistics.CompletedProceduresA = newProcedures.Sum(p => p.CountA);
                    statistics.CompletedProceduresB = newProcedures.Sum(p => p.CountB);
                }
            }
            else
            {
                var oldProcedures = await databaseService.QueryAsync<RealizedProcedureOldSMK>(
                    "SELECT * FROM RealizedProcedureOldSMK WHERE SpecializationId = ?",
                    specializationId);

                // W starym SMK tylko operator jest używany
                int countA = 0;
                int countB = 0;

                foreach (var procedure in oldProcedures)
                {
                    if (!string.IsNullOrEmpty(procedure.Code))
                    {
                        if (procedure.Code.ToUpperInvariant().Contains("A"))
                        {
                            countA++;
                        }
                        else if (procedure.Code.ToUpperInvariant().Contains("B"))
                        {
                            countB++;
                        }
                    }
                }

                statistics.CompletedProceduresA = countA;
                statistics.CompletedProceduresB = countB;

                // Filtrowanie dla modułu
                if (moduleId.HasValue)
                {
                    var module = modules.FirstOrDefault(m => m.ModuleId == moduleId.Value);
                    if (module != null)
                    {
                        if (module.Type == ModuleType.Basic)
                        {
                            var filteredProcedures = oldProcedures.Where(p => p.Year <= 2).ToList();

                            countA = 0;
                            countB = 0;

                            foreach (var procedure in filteredProcedures)
                            {
                                if (!string.IsNullOrEmpty(procedure.Code))
                                {
                                    if (procedure.Code.ToUpperInvariant().Contains("A"))
                                    {
                                        countA++;
                                    }
                                    else if (procedure.Code.ToUpperInvariant().Contains("B"))
                                    {
                                        countB++;
                                    }
                                }
                            }

                            statistics.CompletedProceduresA = countA;
                            statistics.CompletedProceduresB = countB;
                        }
                        else
                        {
                            var filteredProcedures = oldProcedures.Where(p => p.Year >= 3).ToList();

                            countA = 0;
                            countB = 0;

                            foreach (var procedure in filteredProcedures)
                            {
                                if (!string.IsNullOrEmpty(procedure.Code))
                                {
                                    if (procedure.Code.ToUpperInvariant().Contains("A"))
                                    {
                                        countA++;
                                    }
                                    else if (procedure.Code.ToUpperInvariant().Contains("B"))
                                    {
                                        countB++;
                                    }
                                }
                            }

                            statistics.CompletedProceduresA = countA;
                            statistics.CompletedProceduresB = countB;
                        }
                    }
                }
            }

            // Obliczanie statystyk samokształcenia
            var selfEducation = await databaseService.GetSelfEducationItemsAsync(
                specializationId,
                moduleId);

            statistics.SelfEducationDaysUsed = selfEducation.Count;

            // Obliczanie statystyk działań edukacyjnych
            var educationalActivities = await databaseService.GetEducationalActivitiesAsync(
                specializationId,
                moduleId);

            statistics.EducationalActivitiesCompleted = educationalActivities.Count;

            // Obliczanie statystyk publikacji
            var publications = await databaseService.GetPublicationsAsync(
                specializationId,
                moduleId);

            statistics.PublicationsCompleted = publications.Count;

            // Obliczanie statystyk nieobecności
            var absences = await databaseService.GetAbsencesAsync(specializationId);

            statistics.AbsenceDays = absences.Sum(a => (a.EndDate - a.StartDate).Days + 1);

            statistics.AbsenceDaysExtendingSpecialization = absences
                .Where(a => a.ExtendsSpecialization)
                .Sum(a => (a.EndDate - a.StartDate).Days + 1);

            return statistics;
        }

        private static double CalculateRequiredShiftHours(SpecializationStructure structure, TimeSpan duration)
        {
            if (structure?.MedicalShifts == null)
            {
                double defaultHoursPerWeek = 10.083;
                int weeksInSpec = Math.Max(1, (int)(duration.TotalDays / 7));
                double defaultValue = defaultHoursPerWeek * weeksInSpec;

                return defaultValue;
            }

            double weeklyHours = structure.MedicalShifts.HoursPerWeek;

            if (weeklyHours < 0.1)
            {
                weeklyHours = 10.083;
            }

            int weeks = Math.Max(1, (int)(duration.TotalDays / 7));
            double result = weeklyHours * weeks;

            System.Diagnostics.Debug.WriteLine($"CalculateRequiredShiftHours: Final calculation: {weeklyHours} * {weeks} = {result}");
            return result;
        }

        public static async Task<double> GetOverallProgressAsync(
            IDatabaseService database,
            int specializationId,
            int? moduleId = null)
        {
            if (moduleId.HasValue)
            {
                var module = await database.GetModuleAsync(moduleId.Value);
                if (module == null)
                {
                    return 0;
                }

                const double internshipWeight = 0.35;
                const double courseWeight = 0.25;
                const double procedureWeight = 0.30;
                const double otherWeight = 0.10;

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

                double procedureProgress;

                if (module.TotalProceduresA + module.TotalProceduresB > 0)
                {
                    procedureProgress =
                        (procedureProgressA * module.TotalProceduresA +
                         procedureProgressB * module.TotalProceduresB) /
                        (module.TotalProceduresA + module.TotalProceduresB);
                }
                else
                {
                    procedureProgress = 0;
                }

                var selfEducationItems = await database.GetSelfEducationItemsAsync(moduleId: moduleId);
                var publications = await database.GetPublicationsAsync(moduleId: moduleId);
                var educationalActivities = await database.GetEducationalActivitiesAsync(moduleId: moduleId);
                double otherActivitiesProgress = 0;
                int totalOtherItems = selfEducationItems.Count + publications.Count + educationalActivities.Count;

                if (totalOtherItems > 0)
                {
                    otherActivitiesProgress = Math.Min(1.0, totalOtherItems / 10.0);
                }

                double overallProgress = (internshipProgress * internshipWeight) +
                                         (courseProgress * courseWeight) +
                                         (procedureProgress * procedureWeight) +
                                         (otherActivitiesProgress * otherWeight);

                return Math.Min(1.0, overallProgress);
            }
            else
            {
                var stats = await CalculateFullStatisticsAsync(database, specializationId);
                return stats.GetOverallProgress();
            }
        }
    }
}
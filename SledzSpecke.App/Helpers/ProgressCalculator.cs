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
            IDatabaseService database,
            int specializationId,
            int? moduleId = null)
        {
            var specialization = await database.GetSpecializationAsync(specializationId);
            if (specialization == null)
            {
                return new SpecializationStatistics();
            }

            var stats = new SpecializationStatistics();
            SpecializationStructure structure = null;

            if (!string.IsNullOrEmpty(specialization.ProgramStructure))
            {
                structure = JsonSerializer.Deserialize<SpecializationStructure>(specialization.ProgramStructure);
            }

            if (moduleId.HasValue)
            {
                var module = await database.GetModuleAsync(moduleId.Value);

                if (module == null)
                {
                    return stats;
                }

                stats.CompletedInternships = module.CompletedInternships;
                stats.RequiredInternships = module.TotalInternships;
                stats.CompletedCourses = module.CompletedCourses;
                stats.RequiredCourses = module.TotalCourses;
                stats.CompletedProceduresA = module.CompletedProceduresA;
                stats.RequiredProceduresA = module.TotalProceduresA;
                stats.CompletedProceduresB = module.CompletedProceduresB;
                stats.RequiredProceduresB = module.TotalProceduresB;
                ModuleStructure moduleStructure = null;

                if (!string.IsNullOrEmpty(module.Structure))
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        AllowTrailingCommas = true,
                        ReadCommentHandling = JsonCommentHandling.Skip,
                        Converters = { new JsonStringEnumConverter() }
                    };

                    moduleStructure = JsonSerializer.Deserialize<ModuleStructure>(module.Structure, options);
                }

                var internships = await database.GetInternshipsAsync(moduleId: moduleId);
                var completedInternships = internships.Where(i => i.IsCompleted).ToList();

                stats.CompletedInternshipDays = completedInternships.Sum(i => i.DaysCount);
                stats.RequiredInternshipWorkingDays = moduleStructure?.Internships?.Sum(i => i.WorkingDays) ?? 0;

                var oldSmkShiftsQuery = "SELECT * FROM RealizedMedicalShiftOldSMK WHERE SpecializationId = ?";
                var oldSmkShifts = await database.QueryAsync<RealizedMedicalShiftOldSMK>(oldSmkShiftsQuery, specialization.SpecializationId);

                var newSmkShiftsQuery = "SELECT * FROM RealizedMedicalShiftNewSMK WHERE SpecializationId = ?";
                var newSmkShifts = await database.QueryAsync<RealizedMedicalShiftNewSMK>(newSmkShiftsQuery, specialization.SpecializationId);

                double totalShiftHours = 0;

                foreach (var shift in oldSmkShifts)
                {
                    totalShiftHours += shift.Hours + ((double)shift.Minutes / 60.0);
                }

                foreach (var shift in newSmkShifts)
                {
                    totalShiftHours += shift.Hours + ((double)shift.Minutes / 60.0);
                }

                stats.CompletedShiftHours = (int)Math.Round(totalShiftHours);
                stats.RequiredShiftHours = module.RequiredShiftHours;

                if (stats.RequiredShiftHours == 0)
                {
                    if (moduleStructure != null && moduleStructure.RequiredShiftHours > 0)
                    {
                        stats.RequiredShiftHours = moduleStructure.RequiredShiftHours;
                        module.RequiredShiftHours = stats.RequiredShiftHours;
                        await database.UpdateModuleAsync(module);
                    }
                    else
                    {
                        double weeklyHours = module.WeeklyShiftHours;

                        if (weeklyHours <= 0)
                        {
                            if (moduleStructure?.MedicalShifts != null && moduleStructure.MedicalShifts.HoursPerWeek > 0)
                            {
                                weeklyHours = moduleStructure.MedicalShifts.HoursPerWeek;
                                module.WeeklyShiftHours = weeklyHours;
                            }
                            else
                            {
                                weeklyHours = 10.083;
                                module.WeeklyShiftHours = weeklyHours;
                            }
                        }

                        TimeSpan moduleDuration = module.EndDate - module.StartDate;
                        int weeks = Math.Max(1, (int)(moduleDuration.TotalDays / 7));
                        stats.RequiredShiftHours = (int)Math.Round(weeklyHours * weeks);
                        module.RequiredShiftHours = stats.RequiredShiftHours;
                        await database.UpdateModuleAsync(module);
                    }
                }

                var selfEducationItems = await database.GetSelfEducationItemsAsync(moduleId: moduleId);
                stats.SelfEducationDaysUsed = selfEducationItems.Count;
                stats.SelfEducationDaysTotal = moduleStructure?.SelfEducationDays ?? 0;

                var educationalActivities = await database.GetEducationalActivitiesAsync(moduleId: moduleId);
                stats.EducationalActivitiesCompleted = educationalActivities.Count;

                var publications = await database.GetPublicationsAsync(moduleId: moduleId);
                stats.PublicationsCompleted = publications.Count;

                return stats;
            }
            else
            {
                var modules = await database.GetModulesAsync(specializationId);

                stats.CompletedInternships = modules.Sum(m => m.CompletedInternships);
                stats.RequiredInternships = modules.Sum(m => m.TotalInternships);
                stats.CompletedCourses = modules.Sum(m => m.CompletedCourses);
                stats.RequiredCourses = modules.Sum(m => m.TotalCourses);
                stats.CompletedProceduresA = modules.Sum(m => m.CompletedProceduresA);
                stats.RequiredProceduresA = modules.Sum(m => m.TotalProceduresA);
                stats.CompletedProceduresB = modules.Sum(m => m.CompletedProceduresB);
                stats.RequiredProceduresB = modules.Sum(m => m.TotalProceduresB);

                var internships = new List<Internship>();
                foreach (var module in modules)
                {
                    var moduleInternships = await database.GetInternshipsAsync(moduleId: module.ModuleId);
                    internships.AddRange(moduleInternships.Where(i => i.IsCompleted));
                }

                stats.CompletedInternshipDays = internships.Sum(i => i.DaysCount);
                stats.RequiredInternshipWorkingDays = structure?.TotalWorkingDays ?? 0;

                var allShifts = new List<MedicalShift>();
                foreach (var internship in internships)
                {
                    var shifts = await database.GetMedicalShiftsAsync(internship.InternshipId);
                    allShifts.AddRange(shifts);
                }

                stats.CompletedShiftHours = (int)Math.Round(allShifts.Sum(s => s.Hours + ((double)s.Minutes / 60.0)));
                stats.RequiredShiftHours = (int)Math.Round(
                    CalculateRequiredShiftHours(structure, specialization.PlannedEndDate - specialization.StartDate));

                var selfEducationItems = await database.GetSelfEducationItemsAsync(specializationId: specializationId);
                stats.SelfEducationDaysUsed = selfEducationItems.Count;
                stats.SelfEducationDaysTotal = structure?.SelfEducation?.TotalDays ?? 0;

                var educationalActivities = await database.GetEducationalActivitiesAsync(specializationId: specializationId);
                stats.EducationalActivitiesCompleted = educationalActivities.Count;

                var publications = await database.GetPublicationsAsync(specializationId: specializationId);
                stats.PublicationsCompleted = publications.Count;

                var absences = await database.GetAbsencesAsync(specializationId);
                stats.AbsenceDays = absences.Sum(a => a.DaysCount);
                stats.AbsenceDaysExtendingSpecialization = absences
                    .Where(a => a.ExtendsSpecialization)
                    .Sum(a => a.DaysCount);
            }

            return stats;
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
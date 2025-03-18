using System.Text.Json;
using System.Text.Json.Serialization;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;

namespace SledzSpecke.App.Helpers
{
    public static class ModuleHelper
    {
        public static bool HasBasicModule(string specializationCode)
        {
            if (string.IsNullOrEmpty(specializationCode))
            {
                return false;
            }

            var twoModulesSpecializations = new[]
            {
                "cardiology",
                "psychiatry",
            };

            return twoModulesSpecializations.Contains(specializationCode.ToLowerInvariant());
        }

        /// <summary>
        public static string GetBasicModuleName(string specializationCode)
        {
            if (string.IsNullOrEmpty(specializationCode) || !HasBasicModule(specializationCode))
            {
                return null;
            }

            if (new[]
                {
                    "cardiology",
                    "psychiatry",
                }.Contains(specializationCode.ToLowerInvariant()))
            {
                return "internal_medicine";
            }

            return null;
        }

        public static async Task<List<Module>> CreateModulesForSpecializationAsync(
            string specializationCode,
            DateTime startDate,
            SmkVersion smkVersion,
            int specializationId)
        {
            try
            {
                if (string.IsNullOrEmpty(specializationCode))
                {
                    return new List<Module>();
                }

                var specializationProgram = await SpecializationLoader.LoadSpecializationProgramAsync(specializationCode, smkVersion);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    Converters =
                    {
                        new JsonStringEnumConverter(),
                        new ModuleTypeJsonConverter(),
                    }
                };

                var modules = new List<Module>();

                try
                {
                    var jsonDocument = JsonDocument.Parse(specializationProgram.Structure);
                    if (!jsonDocument.RootElement.TryGetProperty("modules", out var modulesElement) ||
                        modulesElement.ValueKind != JsonValueKind.Array)
                    {
                        System.Diagnostics.Debug.WriteLine("Nie znaleziono tablicy 'modules' w JSON specjalizacji.");
                    }

                    int moduleIndex = 0;
                    DateTime currentStartDate = startDate;

                    foreach (var moduleElement in modulesElement.EnumerateArray())
                    {
                        moduleIndex++;

                        string moduleName = string.Empty;
                        string moduleCode = string.Empty;
                        ModuleType moduleType = ModuleType.Specialistic;
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

                        if (durationMonths == 0)
                        {
                            if (moduleType == ModuleType.Basic)
                            {
                                durationMonths = 24;
                            }
                            else
                            {
                                durationMonths = 36;
                            }
                        }

                        if (moduleElement.TryGetProperty("workingDays", out var workingDaysElement))
                        {
                            workingDays = workingDaysElement.GetInt32();
                        }

                        DateTime endDate = currentStartDate.AddMonths(durationMonths);

                        int totalInternships = 0;
                        int totalCourses = 0;
                        int totalProceduresA = 0;
                        int totalProceduresB = 0;

                        if (moduleElement.TryGetProperty("internships", out var internshipsElement) &&
                            internshipsElement.ValueKind == JsonValueKind.Array)
                        {
                            totalInternships = internshipsElement.GetArrayLength();
                        }

                        if (moduleElement.TryGetProperty("courses", out var coursesElement) &&
                            coursesElement.ValueKind == JsonValueKind.Array)
                        {
                            totalCourses = coursesElement.GetArrayLength();
                        }

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
                            if (medicalShiftsElement.TryGetProperty("requiredShiftHours", out var requiredHoursElement))
                            {
                                requiredShiftHours = requiredHoursElement.GetInt32();
                            }

                            if (medicalShiftsElement.TryGetProperty("hoursPerWeek", out var hoursPerWeekElement))
                            {
                                hoursPerWeek = hoursPerWeekElement.GetDouble();
                            }

                            if (medicalShiftsElement.TryGetProperty("description", out var descriptionElement))
                            {
                                medicalShiftsDescription = descriptionElement.GetString();
                            }
                        }

                        int selfEducationDays = 0;

                        if (moduleElement.TryGetProperty("selfEducation", out var selfEducationElement) &&
                            selfEducationElement.ValueKind == JsonValueKind.Object)
                        {
                            if (selfEducationElement.TryGetProperty("totalDays", out var totalDaysElement))
                            {
                                selfEducationDays = totalDaysElement.GetInt32();
                            }
                            else if (selfEducationElement.TryGetProperty("daysPerYear", out var daysPerYearElement))
                            {
                                int daysPerYear = daysPerYearElement.GetInt32();
                                int years = (int)Math.Ceiling(durationMonths / 12.0);
                                selfEducationDays = daysPerYear * years;
                            }
                        }

                        var module = new Module
                        {
                            Name = moduleName,
                            Type = moduleType,
                            StartDate = currentStartDate,
                            EndDate = endDate,
                            Structure = moduleElement.ToString(),
                            SmkVersion = smkVersion,
                            Version = moduleElement.GetProperty("version").GetString(),
                            SpecializationId = specializationId,
                            CompletedInternships = 0,
                            TotalInternships = totalInternships,
                            CompletedCourses = 0,
                            TotalCourses = totalCourses,
                            CompletedProceduresA = 0,
                            TotalProceduresA = totalProceduresA,
                            CompletedProceduresB = 0,
                            TotalProceduresB = totalProceduresB,
                            CompletedShiftHours = 0,
                            RequiredShiftHours = requiredShiftHours,
                            WeeklyShiftHours = hoursPerWeek,
                            CompletedSelfEducationDays = 0,
                            TotalSelfEducationDays = selfEducationDays
                        };

                        if (module.RequiredShiftHours == 0 && module.WeeklyShiftHours > 0)
                        {
                            TimeSpan duration = module.EndDate - module.StartDate;
                            int weeks = Math.Max(1, (int)(duration.TotalDays / 7));
                            module.RequiredShiftHours = (int)Math.Round(module.WeeklyShiftHours * weeks);
                        }
                        else if (module.RequiredShiftHours == 0 && module.WeeklyShiftHours == 0)
                        {
                            double defaultWeeklyHours = 10.083;

                            TimeSpan duration = module.EndDate - module.StartDate;
                            int weeks = Math.Max(1, (int)(duration.TotalDays / 7));

                            module.WeeklyShiftHours = defaultWeeklyHours;
                            module.RequiredShiftHours = (int)Math.Round(defaultWeeklyHours * weeks);
                        }

                        modules.Add(module);

                        currentStartDate = endDate.AddDays(1);
                    }

                    if (modules.Count > 0)
                    {
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

                var specialization = await databaseService.GetSpecializationAsync(specializationId);
                if (specialization == null)
                {
                    return false;
                }

                foreach (var module in modules)
                {
                    module.SpecializationId = specializationId;
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

        public static DateTime CalculateModuleEndDate(Module module, List<Absence> absences)
        {
            if (module == null)
            {
                return DateTime.Now;
            }

            DateTime baseEndDate = module.EndDate;

            var relevantAbsences = absences.Where(a =>
                (a.StartDate >= module.StartDate && a.StartDate <= module.EndDate) ||
                (a.EndDate >= module.StartDate && a.EndDate <= module.EndDate) ||
                (a.StartDate <= module.StartDate && a.EndDate >= module.EndDate))
                .ToList();

            int extensionDays = 0;
            foreach (var absence in relevantAbsences)
            {
                if (absence.Type == AbsenceType.Sick ||
                    absence.Type == AbsenceType.Maternity ||
                    absence.Type == AbsenceType.Paternity)
                {
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

            int reductionDays = relevantAbsences
                .Where(a => a.Type == AbsenceType.Recognition)
                .Sum(a => (a.EndDate - a.StartDate).Days + 1);

            return baseEndDate.AddDays(extensionDays - reductionDays);
        }
    }
}
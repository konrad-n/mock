using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Logging;

namespace SledzSpecke.App.Services.Specialization
{
    public class SpecializationService : BaseService, ISpecializationService
    {
        private readonly IDatabaseService databaseService;
        private readonly IAuthService authService;
        private readonly IDialogService dialogService;
        private readonly ModuleInitializer moduleInitializer;
        private Models.Specialization _cachedSpecialization;
        private List<Module> _cachedModules;

        public SpecializationService(
            IDatabaseService databaseService,
            IAuthService authService,
            IDialogService dialogService,
            IExceptionHandlerService exceptionHandler,
            ILoggingService Logger) : base(exceptionHandler, Logger)
        {
            this.databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            this.authService = authService ?? throw new ArgumentNullException(nameof(authService));
            this.dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            this.moduleInitializer = new ModuleInitializer(databaseService);
        }

        public async Task<Models.Specialization> GetCurrentSpecializationAsync(bool includeModules = true)
        {
            return await SafeExecuteAsync(async () =>
            {
                if (_cachedSpecialization != null)
                {
                    return _cachedSpecialization;
                }

                var user = await authService.GetCurrentUserAsync();
                if (user == null)
                {
                    return null;
                }

                var specialization = await databaseService.GetSpecializationAsync(user.SpecializationId);
                if (specialization != null && includeModules)
                {
                    specialization.Modules = await GetModulesAsync(specialization.SpecializationId, false);
                }

                _cachedSpecialization = specialization;
                return specialization;
            },
            "Nie udało się pobrać aktualnej specjalizacji.",
            new Dictionary<string, object> { { "IncludeModules", includeModules } },
            withRetry: true);
        }

        public async Task<List<Module>> GetModulesAsync(int specializationId, bool initializeIfNeeded = true)
        {
            return await SafeExecuteAsync(async () =>
            {
                if (_cachedModules != null)
                {
                    return _cachedModules;
                }

                var modules = await databaseService.GetModulesAsync(specializationId);

                if (modules.Count == 0 && initializeIfNeeded)
                {
                    await moduleInitializer.InitializeModulesIfNeededAsync(specializationId);
                    modules = await databaseService.GetModulesAsync(specializationId);
                }

                _cachedModules = modules;
                return modules;
            },
            $"Nie udało się pobrać modułów dla specjalizacji (ID: {specializationId}).",
            new Dictionary<string, object> {
                { "SpecializationId", specializationId },
                { "InitializeIfNeeded", initializeIfNeeded }
            },
            withRetry: true);
        }

        public async Task<Module> GetCurrentModuleAsync()
        {
            return await SafeExecuteAsync(async () =>
            {
                var specialization = await GetCurrentSpecializationAsync(false);
                if (specialization?.CurrentModuleId == null)
                {
                    return null;
                }

                return await databaseService.GetModuleAsync(specialization.CurrentModuleId.Value);
            },
            "Nie udało się pobrać aktualnego modułu.",
            withRetry: true);
        }

        public async Task<int> GetInternshipCountAsync(int? moduleId = null)
        {
            return await SafeExecuteAsync(async () =>
            {
                var specialization = await this.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return 0;
                }

                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    return 0;
                }

                // Pobierz aktualny moduł
                Module currentModule = null;
                if (moduleId.HasValue)
                {
                    currentModule = await this.databaseService.GetModuleAsync(moduleId.Value);
                }
                else
                {
                    currentModule = await this.GetCurrentModuleAsync();
                }

                if (currentModule == null)
                {
                    return 0;
                }

                Logger.LogInformation($"GetInternshipCountAsync: Module={currentModule.Name}, ID={currentModule.ModuleId}, Type={currentModule.Type}");

                // Pobierz wymagania stażowe dla danego modułu
                var internshipRequirements = await this.GetInternshipsAsync(currentModule.ModuleId);
                if (internshipRequirements == null || internshipRequirements.Count == 0)
                {
                    return 0;
                }

                Logger.LogInformation($"GetInternshipCountAsync: Znaleziono {internshipRequirements.Count} wymagań stażowych");

                int completedCount = 0;

                // Dla każdego wymagania stażowego
                for (int i = 0; i < internshipRequirements.Count; i++)
                {
                    var requirement = internshipRequirements[i];
                    int requiredDays = requirement.DaysCount;

                    Logger.LogInformation($"GetInternshipCountAsync: Przetwarzanie wymagania {i + 1}/{internshipRequirements.Count}: {requirement.InternshipName}, WymaganeDni={requiredDays}");

                    int introducedDays = 0;

                    if (user.SmkVersion == SmkVersion.New)
                    {
                        // Dla nowego SMK, pobierz realizacje po ID wymagania
                        var realizations = await this.GetRealizedInternshipsNewSMKAsync(
                            moduleId: currentModule.ModuleId,
                            internshipRequirementId: requirement.InternshipId);

                        Logger.LogInformation($"GetInternshipCountAsync: Znaleziono {realizations.Count} realizacji dla nowego SMK");

                        introducedDays = realizations.Sum(r => r.DaysCount);
                    }
                    else
                    {
                        // Dla starego SMK, pobierz wszystkie realizacje dla lat odpowiadających modułowi
                        int startYear = 1;
                        int endYear = 2;

                        if (currentModule.Type == ModuleType.Specialistic)
                        {
                            startYear = 3;
                            endYear = 6;
                        }

                        Logger.LogInformation($"GetInternshipCountAsync: Zakres lat dla modułu: {startYear}-{endYear}");

                        // Pobierz realizacje dla tych lat
                        List<RealizedInternshipOldSMK> allRealizations = new List<RealizedInternshipOldSMK>();
                        for (int year = startYear; year <= endYear; year++)
                        {
                            var yearRealizations = await this.GetRealizedInternshipsOldSMKAsync(year);
                            Logger.LogInformation($"GetInternshipCountAsync: Rok {year}: znaleziono {yearRealizations.Count} realizacji");
                            allRealizations.AddRange(yearRealizations);
                        }

                        // Dodaj również realizacje z year=0 (nieprzypisane do konkretnego roku)
                        var yearZeroRealizations = await this.databaseService.QueryAsync<RealizedInternshipOldSMK>(
                            "SELECT * FROM RealizedInternshipOldSMK WHERE SpecializationId = ? AND Year = 0",
                            specialization.SpecializationId);

                        Logger.LogInformation($"GetInternshipCountAsync: Rok 0 (nieprzypisane): znaleziono {yearZeroRealizations.Count} realizacji");

                        allRealizations.AddRange(yearZeroRealizations);

                        // Filtruj realizacje dla tego konkretnego stażu po nazwie
                        var realizationsForThisRequirement = allRealizations
                            .Where(r => {
                                // Przygotuj nazwy do lepszego porównania
                                string realizationName = r.InternshipName ?? "null";
                                string requirementName = requirement.InternshipName ?? "null";

                                // Jeśli nazwa jest pusta lub "Staż bez nazwy", próbujemy dopasować ją z innymi danymi
                                if (string.IsNullOrEmpty(r.InternshipName)
                                    || r.InternshipName == "Staż bez nazwy")
                                {
                                    bool isFirstRequirement = i == 0;

                                    Logger.LogInformation($"GetInternshipCountAsync: Pusta nazwa realizacji, przypisana do pierwszego wymagania: {isFirstRequirement}");

                                    return isFirstRequirement;
                                }

                                // Standardowe porównanie nazw
                                bool exactMatch = r.InternshipName != null
                                    && r.InternshipName.Equals(requirement.InternshipName, StringComparison.OrdinalIgnoreCase);
                                bool realizationContainsRequirement = r.InternshipName != null
                                    && r.InternshipName.Contains(requirement.InternshipName, StringComparison.OrdinalIgnoreCase);
                                bool requirementContainsRealization = requirement.InternshipName != null
                                    && requirement.InternshipName.Contains(r.InternshipName, StringComparison.OrdinalIgnoreCase);

                                // Dodatkowe porównanie: usuń spacje i znaki specjalne
                                string cleanRealizationName = realizationName
                                    .Replace(" ", string.Empty)
                                    .Replace("-", string.Empty)
                                    .Replace("_", string.Empty)
                                    .ToLowerInvariant();
                                string cleanRequirementName = requirementName
                                    .Replace(" ", string.Empty)
                                    .Replace("-", string.Empty)
                                    .Replace("_", string.Empty)
                                    .ToLowerInvariant();
                                bool fuzzyMatch = cleanRealizationName.Contains(cleanRequirementName)
                                    || cleanRequirementName.Contains(cleanRealizationName);

                                bool result = exactMatch
                                    || realizationContainsRequirement
                                    || requirementContainsRealization
                                    || fuzzyMatch;

                                Logger.LogInformation($"GetInternshipCountAsync: Porównanie '{realizationName}' do '{requirementName}': ExactMatch={exactMatch}, RealizationContainsRequirement={realizationContainsRequirement}, RequirementContainsRealization={requirementContainsRealization}, FuzzyMatch={fuzzyMatch}, Wynik={result}");

                                return result;
                            }).ToList();

                        Logger.LogInformation($"GetInternshipCountAsync: Znaleziono {realizationsForThisRequirement.Count} realizacji dla tego wymagania");

                        foreach (var realization in realizationsForThisRequirement)
                        {
                            Logger.LogInformation($"GetInternshipCountAsync: Realizacja: {realization.InternshipName}, Dni={realization.DaysCount}");
                        }

                        introducedDays = realizationsForThisRequirement.Sum(r => r.DaysCount);
                    }

                    Logger.LogInformation($"GetInternshipCountAsync: WprowadzoneDni={introducedDays}, WymaganeDni={requiredDays}, CzyUkończone={introducedDays >= requiredDays}");

                    // Jeśli liczba wprowadzonych dni >= wymagana liczba dni, zwiększ licznik ukończonych
                    if (introducedDays >= requiredDays)
                    {
                        completedCount++;
                    }
                }

                Logger.LogInformation($"GetInternshipCountAsync: Końcowy wynik completedCount={completedCount}");

                return completedCount;
            },
            "Nie udało się obliczyć liczby ukończonych staży.",
            new Dictionary<string, object> { { "ModuleId", moduleId } });
        }

        public async Task<int> GetProcedureCountAsync(int? moduleId = null)
        {
            return await SafeExecuteAsync(async () =>
            {
                var specialization = await this.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return 0;
                }

                int count = 0;

                if (moduleId.HasValue)
                {
                    var internships = await this.databaseService.GetInternshipsAsync(
                        specializationId: specialization.SpecializationId,
                        moduleId: moduleId);

                    foreach (var internship in internships)
                    {
                        var procedures = await this.databaseService.GetProceduresAsync(internshipId: internship.InternshipId);
                        count += procedures.Count(p => p.OperatorCode == "A");
                    }
                }
                else
                {
                    var internships = await this.databaseService.GetInternshipsAsync(
                        specializationId: specialization.SpecializationId);

                    foreach (var internship in internships)
                    {
                        var procedures = await this.databaseService.GetProceduresAsync(internshipId: internship.InternshipId);
                        count += procedures.Count(p => p.OperatorCode == "A");
                    }
                }

                return count;
            },
            "Nie udało się obliczyć liczby wykonanych procedur.",
            new Dictionary<string, object> { { "ModuleId", moduleId } });
        }

        public async Task<int> GetCourseCountAsync(int? moduleId = null)
        {
            return await SafeExecuteAsync(async () =>
            {
                var specialization = await this.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return 0;
                }

                var courses = await this.databaseService.GetCoursesAsync(
                    specializationId: specialization.SpecializationId,
                    moduleId: moduleId);

                return courses.Count;
            },
            "Nie udało się obliczyć liczby ukończonych kursów.",
            new Dictionary<string, object> { { "ModuleId", moduleId } });
        }

        public async Task<int> GetShiftCountAsync(int? moduleId = null)
        {
            return await SafeExecuteAsync(async () =>
            {
                var specialization = await this.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return 0;
                }

                if (!moduleId.HasValue && specialization.CurrentModuleId.HasValue)
                {
                    moduleId = specialization.CurrentModuleId.Value;
                }

                if (!moduleId.HasValue)
                {
                    var modules = await this.databaseService.GetModulesAsync(specialization.SpecializationId);
                    if (modules.Count > 0)
                    {
                        moduleId = modules[0].ModuleId;
                    }
                }

                if (!moduleId.HasValue)
                {
                    return 0;
                }

                var module = await this.databaseService.GetModuleAsync(moduleId.Value);
                if (module == null)
                {
                    return 0;
                }

                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    return 0;
                }

                double totalHoursDouble = 0;

                if (user.SmkVersion == SmkVersion.New)
                {
                    var query = "SELECT * FROM RealizedMedicalShiftNewSMK WHERE SpecializationId = ? AND ModuleId = ?";
                    var shifts = await this.databaseService.QueryAsync<RealizedMedicalShiftNewSMK>(query, specialization.SpecializationId, moduleId.Value);

                    foreach (var shift in shifts)
                    {
                        totalHoursDouble += shift.Hours + ((double)shift.Minutes / 60.0);
                    }
                }
                else
                {
                    int startYear = 1;
                    int endYear = 6;

                    if (module.Type == ModuleType.Basic)
                    {
                        startYear = 1;
                        endYear = 2;
                    }
                    else if (module.Type == ModuleType.Specialistic)
                    {
                        var modules = await this.databaseService.GetModulesAsync(specialization.SpecializationId);
                        bool hasBasicModule = modules.Any(m => m.Type == ModuleType.Basic);

                        if (hasBasicModule)
                        {
                            startYear = 3;
                            endYear = 6;
                        }
                        else
                        {
                            startYear = 1;
                            endYear = 6;
                        }
                    }

                    for (int year = startYear; year <= endYear; year++)
                    {
                        var yearQuery = "SELECT * FROM RealizedMedicalShiftOldSMK WHERE SpecializationId = ? AND Year = ?";
                        var yearShifts = await this.databaseService.QueryAsync<RealizedMedicalShiftOldSMK>(yearQuery, specialization.SpecializationId, year);

                        foreach (var shift in yearShifts)
                        {
                            totalHoursDouble += shift.Hours + ((double)shift.Minutes / 60.0);
                        }
                    }
                }

                int totalHours = (int)Math.Round(totalHoursDouble);
                return totalHours;
            },
            "Nie udało się obliczyć liczby godzin dyżurów.",
            new Dictionary<string, object> { { "ModuleId", moduleId } });
        }

        public async Task<int> GetSelfEducationCountAsync(int? moduleId = null)
        {
            return await SafeExecuteAsync(async () =>
            {
                var specialization = await this.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return 0;
                }

                var selfEducationItems = await this.databaseService.GetSelfEducationItemsAsync(
                    specializationId: specialization.SpecializationId,
                    moduleId: moduleId);
                return selfEducationItems.Count;
            },
            "Nie udało się obliczyć liczby wykorzystanych dni samokształcenia.",
            new Dictionary<string, object> { { "ModuleId", moduleId } });
        }

        public async Task<int> GetPublicationCountAsync(int? moduleId = null)
        {
            return await SafeExecuteAsync(async () =>
            {
                var specialization = await this.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return 0;
                }

                var publications = await this.databaseService.GetPublicationsAsync(
                    specializationId: specialization.SpecializationId,
                    moduleId: moduleId);

                return publications.Count;
            },
            "Nie udało się obliczyć liczby publikacji.",
            new Dictionary<string, object> { { "ModuleId", moduleId } });
        }

        public async Task<int> GetEducationalActivityCountAsync(int? moduleId = null)
        {
            return await SafeExecuteAsync(async () =>
            {
                var specialization = await this.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return 0;
                }

                var activities = await this.databaseService.GetEducationalActivitiesAsync(
                    specializationId: specialization.SpecializationId,
                    moduleId: moduleId);

                return activities.Count;
            },
            "Nie udało się obliczyć liczby aktywności edukacyjnych.",
            new Dictionary<string, object> { { "ModuleId", moduleId } });
        }

        public async Task<int> GetAbsenceCountAsync()
        {
            return await SafeExecuteAsync(async () =>
            {
                var specialization = await this.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return 0;
                }

                var absences = await this.databaseService.GetAbsencesAsync(specialization.SpecializationId);
                return absences.Count;
            },
            "Nie udało się obliczyć liczby nieobecności.");
        }

        public async Task<int> GetRecognitionCountAsync()
        {
            return await SafeExecuteAsync(async () =>
            {
                var specialization = await this.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return 0;
                }

                var recognitions = await this.databaseService.GetRecognitionsAsync(specialization.SpecializationId);
                return recognitions.Count;
            },
            "Nie udało się obliczyć liczby uznań.");
        }

        public async Task<SpecializationStatistics> GetSpecializationStatisticsAsync(int? moduleId = null)
        {
            return await SafeExecuteAsync(async () =>
            {
                var specialization = await this.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return new SpecializationStatistics();
                }
                var stats = await ProgressCalculator.CalculateFullStatisticsAsync(
                    this.databaseService,
                    specialization.SpecializationId,
                    moduleId);

                if (stats == null)
                {
                    return new SpecializationStatistics();
                }
                return stats;
            },
            "Nie udało się pobrać statystyk specjalizacji.",
            new Dictionary<string, object> { { "ModuleId", moduleId } },
            withRetry: true);
        }

        public async Task UpdateSpecializationProgressAsync(int specializationId)
        {
            await SafeExecuteAsync(async () =>
            {
                await ProgressCalculator.UpdateSpecializationProgressAsync(this.databaseService, specializationId);
            },
            "Nie udało się zaktualizować postępu specjalizacji.",
            new Dictionary<string, object> { { "SpecializationId", specializationId } });
        }

        public async Task UpdateModuleProgressAsync(int moduleId)
        {
            await SafeExecuteAsync(async () =>
            {
                await ProgressCalculator.UpdateModuleProgressAsync(this.databaseService, moduleId);
            },
            "Nie udało się zaktualizować postępu modułu.",
            new Dictionary<string, object> { { "ModuleId", moduleId } });
        }

        public async Task<DateTime> CalculateSpecializationEndDateAsync(int specializationId)
        {
            return await SafeExecuteAsync(async () =>
            {
                var specialization = await this.databaseService.GetSpecializationAsync(specializationId);
                var absences = await this.databaseService.GetAbsencesAsync(specializationId);

                return DateCalculator.CalculateSpecializationEndDate(
                    specialization.StartDate,
                    (specialization.PlannedEndDate - specialization.StartDate).Days + 1,
                    absences);
            },
            "Nie udało się obliczyć daty zakończenia specjalizacji.",
            new Dictionary<string, object> { { "SpecializationId", specializationId } });
        }

        public async Task<List<Module>> GetModulesAsync(int specializationId)
        {
            return await SafeExecuteAsync(async () =>
            {
                return await this.databaseService.GetModulesAsync(specializationId);
            },
            "Nie udało się pobrać modułów specjalizacji.",
            new Dictionary<string, object> { { "SpecializationId", specializationId } },
            withRetry: true);
        }

        public async Task<List<SpecializationProgram>> GetAvailableSpecializationProgramsAsync()
        {
            return await SafeExecuteAsync(async () =>
            {
                return await this.databaseService.GetAllSpecializationProgramsAsync();
            },
            "Nie udało się pobrać dostępnych programów specjalizacji.",
            withRetry: true);
        }

        public async Task<SpecializationProgram> LoadSpecializationProgramAsync(string code, SmkVersion smkVersion)
        {
            return await SafeExecuteAsync(async () =>
            {
                if (string.IsNullOrEmpty(code))
                {
                    throw new InvalidInputException(
                        "Code cannot be null or empty",
                        "Kod specjalizacji nie może być pusty.");
                }

                return await SpecializationLoader.LoadSpecializationProgramAsync(code, smkVersion);
            },
            "Nie udało się załadować programu specjalizacji.",
            new Dictionary<string, object> { { "Code", code }, { "SmkVersion", smkVersion } },
            withRetry: true);
        }

        public async Task<bool> InitializeSpecializationModulesAsync(int specializationId)
        {
            return await SafeExecuteAsync(async () =>
            {
                return await this.moduleInitializer.InitializeModulesIfNeededAsync(specializationId);
            },
            "Nie udało się zainicjować modułów specjalizacji.",
            new Dictionary<string, object> { { "SpecializationId", specializationId } },
            withRetry: true);
        }

        public async Task<List<MedicalShift>> GetMedicalShiftsAsync(int? internshipId = null)
        {
            return await SafeExecuteAsync(async () =>
            {
                return await this.databaseService.GetMedicalShiftsAsync(internshipId);
            },
            "Nie udało się pobrać dyżurów medycznych.",
            new Dictionary<string, object> { { "InternshipId", internshipId } });
        }

        public async Task<MedicalShift> GetMedicalShiftAsync(int shiftId)
        {
            return await SafeExecuteAsync(async () =>
            {
                var shift = await this.databaseService.GetMedicalShiftAsync(shiftId);
                if (shift == null)
                {
                    throw new ResourceNotFoundException(
                        $"Medical shift with ID {shiftId} not found",
                        $"Nie znaleziono dyżuru o ID {shiftId}");
                }
                return shift;
            },
            "Nie udało się pobrać dyżuru medycznego.",
            new Dictionary<string, object> { { "ShiftId", shiftId } });
        }

        public async Task<bool> AddMedicalShiftAsync(MedicalShift shift)
        {
            return await SafeExecuteAsync(async () =>
            {
                if (shift == null)
                {
                    throw new InvalidInputException(
                        "Shift cannot be null",
                        "Dane dyżuru nie mogą być puste.");
                }

                int result = await this.databaseService.SaveMedicalShiftAsync(shift);

                var currentModule = await this.GetCurrentModuleAsync();
                if (currentModule != null)
                {
                    await this.UpdateModuleProgressAsync(currentModule.ModuleId);
                }

                return result > 0;
            },
            "Nie udało się dodać dyżuru medycznego.",
            new Dictionary<string, object> {
                { "ShiftId", shift?.ShiftId },
                { "InternshipId", shift?.InternshipId }
            });
        }

        public async Task<bool> UpdateMedicalShiftAsync(MedicalShift shift)
        {
            return await AddMedicalShiftAsync(shift);
        }

        public async Task<bool> DeleteMedicalShiftAsync(int shiftId)
        {
            return await SafeExecuteAsync(async () =>
            {
                var shift = await this.databaseService.GetMedicalShiftAsync(shiftId);
                if (shift == null)
                {
                    return false;
                }

                int result = await this.databaseService.DeleteMedicalShiftAsync(shift);
                var currentModule = await this.GetCurrentModuleAsync();
                if (currentModule != null)
                {
                    await this.UpdateModuleProgressAsync(currentModule.ModuleId);
                }

                return result > 0;
            },
            "Nie udało się usunąć dyżuru medycznego.",
            new Dictionary<string, object> { { "ShiftId", shiftId } });
        }

        public async Task<Internship> GetInternshipAsync(int internshipId)
        {
            return await SafeExecuteAsync(async () =>
            {
                var internship = await this.databaseService.GetInternshipAsync(internshipId);
                if (internship == null)
                {
                    throw new ResourceNotFoundException(
                        $"Internship with ID {internshipId} not found",
                        $"Nie znaleziono stażu o ID {internshipId}");
                }
                return internship;
            },
            "Nie udało się pobrać stażu.",
            new Dictionary<string, object> { { "InternshipId", internshipId } });
        }

        public async Task<List<Internship>> GetInternshipsAsync(int? moduleId = null)
        {
            return await SafeExecuteAsync(async () =>
            {
                var results = new List<Internship>();
                var currentSpecialization = await this.GetCurrentSpecializationAsync();

                if (currentSpecialization == null)
                {
                    return results;
                }

                if (!moduleId.HasValue && currentSpecialization.CurrentModuleId.HasValue)
                {
                    moduleId = currentSpecialization.CurrentModuleId.Value;
                }

                Module module = null;
                if (moduleId.HasValue)
                {
                    module = await this.databaseService.GetModuleAsync(moduleId.Value);
                }
                else
                {
                    var modules = await this.databaseService.GetModulesAsync(currentSpecialization.SpecializationId);
                    if (modules.Count > 0)
                    {
                        module = modules[0];
                    }
                }

                if (module == null || string.IsNullOrEmpty(module.Structure))
                {
                    return results;
                }

                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip,
                    Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
                };

                ModuleStructure moduleStructure = null;

                moduleStructure = System.Text.Json.JsonSerializer.Deserialize<ModuleStructure>(
                    module.Structure, options);

                if (moduleStructure?.Internships == null)
                {
                    return results;
                }

                var userInternships = await this.databaseService.GetInternshipsAsync(
                    currentSpecialization.SpecializationId,
                    moduleId);

                int id = 1;
                foreach (var requirement in moduleStructure.Internships)
                {
                    var existingInternship = userInternships.FirstOrDefault(
                        i => i.InternshipName == requirement.Name);

                    if (existingInternship != null)
                    {
                        results.Add(existingInternship);
                    }
                    else
                    {
                        results.Add(new Internship
                        {
                            InternshipId = id,
                            SpecializationId = currentSpecialization.SpecializationId,
                            ModuleId = moduleId,
                            InternshipName = requirement.Name,
                            DaysCount = requirement.WorkingDays,
                            StartDate = DateTime.Today,
                            EndDate = DateTime.Today.AddDays(requirement.WorkingDays),
                            Year = 1,
                            IsCompleted = false,
                            IsApproved = false
                        });
                        id++;
                    }
                }

                return results;
            },
            "Nie udało się pobrać listy staży.",
            new Dictionary<string, object> { { "ModuleId", moduleId } });
        }

        public async Task<List<Internship>> GetUserInternshipsAsync(int? moduleId = null)
        {
            return await SafeExecuteAsync(async () =>
            {
                var currentSpecialization = await this.GetCurrentSpecializationAsync();
                if (currentSpecialization == null)
                {
                    return new List<Internship>();
                }

                return await this.databaseService.GetInternshipsAsync(
                    specializationId: currentSpecialization.SpecializationId,
                    moduleId: moduleId);
            },
            "Nie udało się pobrać listy staży użytkownika.",
            new Dictionary<string, object> { { "ModuleId", moduleId } });
        }

        public async Task<Internship> GetCurrentInternshipAsync()
        {
            return await SafeExecuteAsync(async () =>
            {
                var specialization = await this.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return null;
                }

                var internships = await this.databaseService.GetInternshipsAsync(specialization.SpecializationId);
                var today = DateTime.Today;
                var currentInternship = internships.FirstOrDefault(i =>
                    i.StartDate <= today && i.EndDate >= today);
                if (currentInternship == null && internships.Count > 0)
                {
                    currentInternship = internships.OrderByDescending(i => i.InternshipId).First();
                }

                return currentInternship;
            },
            "Nie udało się pobrać aktualnego stażu.");
        }

        public async Task<bool> AddInternshipAsync(Internship internship)
        {
            return await SafeExecuteAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "Internship cannot be null",
                        "Dane stażu nie mogą być puste.");
                }

                if (internship.SpecializationId <= 0)
                {
                    var specialization = await this.GetCurrentSpecializationAsync();
                    if (specialization == null)
                    {
                        return false;
                    }

                    internship.SpecializationId = specialization.SpecializationId;
                }

                int result = await this.databaseService.SaveInternshipAsync(internship);
                if (internship.ModuleId.HasValue)
                {
                    await this.UpdateModuleProgressAsync(internship.ModuleId.Value);
                }

                return result > 0;
            },
            "Nie udało się dodać stażu.",
            new Dictionary<string, object> {
                { "InternshipId", internship?.InternshipId },
                { "ModuleId", internship?.ModuleId }
            });
        }

        public async Task<bool> UpdateInternshipAsync(Internship internship)
        {
            return await SafeExecuteAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "Internship cannot be null",
                        "Dane stażu nie mogą być puste.");
                }

                int result = await this.databaseService.SaveInternshipAsync(internship);

                if (internship.ModuleId.HasValue)
                {
                    await this.UpdateModuleProgressAsync(internship.ModuleId.Value);
                }

                return result > 0;
            },
            "Nie udało się zaktualizować stażu.",
            new Dictionary<string, object> {
                { "InternshipId", internship?.InternshipId },
                { "ModuleId", internship?.ModuleId }
            });
        }

        public async Task<bool> DeleteInternshipAsync(int internshipId)
        {
            return await SafeExecuteAsync(async () =>
            {
                var internship = await this.databaseService.GetInternshipAsync(internshipId);
                if (internship == null)
                {
                    return false;
                }

                var shifts = await this.databaseService.GetMedicalShiftsAsync(internshipId);
                var procedures = await this.databaseService.GetProceduresAsync(internshipId: internshipId);

                if (shifts.Count > 0 || procedures.Count > 0)
                {
                    throw new BusinessRuleViolationException(
                        "Cannot delete internship with associated shifts or procedures",
                        "Nie można usunąć stażu, który ma powiązane dyżury lub procedury.");
                }

                int result = await this.databaseService.DeleteInternshipAsync(internship);

                if (internship.ModuleId.HasValue)
                {
                    await this.UpdateModuleProgressAsync(internship.ModuleId.Value);
                }

                return result > 0;
            },
            "Nie udało się usunąć stażu.",
            new Dictionary<string, object> { { "InternshipId", internshipId } });
        }

        public event EventHandler<int> CurrentModuleChanged;

        public async Task SetCurrentModuleAsync(int moduleId)
        {
            await SafeExecuteAsync(async () =>
            {
                var specialization = await this.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    throw new ResourceNotFoundException(
                        "Current specialization not found",
                        "Nie znaleziono aktualnej specjalizacji.");
                }

                var module = await this.databaseService.GetModuleAsync(moduleId);
                if (module == null || module.SpecializationId != specialization.SpecializationId)
                {
                    throw new ResourceNotFoundException(
                        $"Module with ID {moduleId} not found or belongs to different specialization",
                        "Nie znaleziono modułu o podanym ID lub należy on do innej specjalizacji.");
                }

                specialization.CurrentModuleId = moduleId;
                await this.databaseService.UpdateSpecializationAsync(specialization);
                await SettingsHelper.SetCurrentModuleIdAsync(moduleId);

                this.CurrentModuleChanged?.Invoke(this, moduleId);
            },
            "Nie udało się ustawić aktualnego modułu.",
            new Dictionary<string, object> { { "ModuleId", moduleId } });
        }

        public async Task<List<RealizedInternshipNewSMK>> GetRealizedInternshipsNewSMKAsync(int? moduleId = null, int? internshipRequirementId = null)
        {
            return await SafeExecuteAsync(async () =>
            {
                var currentSpecialization = await this.GetCurrentSpecializationAsync();
                if (currentSpecialization == null)
                {
                    return new List<RealizedInternshipNewSMK>();
                }

                return await this.databaseService.GetRealizedInternshipsNewSMKAsync(
                    currentSpecialization.SpecializationId,
                    moduleId,
                    internshipRequirementId);
            },
            "Nie udało się pobrać listy zrealizowanych staży (Nowy SMK).",
            new Dictionary<string, object> {
                { "ModuleId", moduleId },
                { "InternshipRequirementId", internshipRequirementId }
            });
        }

        public async Task<List<RealizedInternshipOldSMK>> GetRealizedInternshipsOldSMKAsync(int? year = null)
        {
            return await SafeExecuteAsync(async () =>
            {
                var currentSpecialization = await this.GetCurrentSpecializationAsync();
                if (currentSpecialization == null)
                {
                    return new List<RealizedInternshipOldSMK>();
                }

                string query = "SELECT * FROM RealizedInternshipOldSMK WHERE SpecializationId = ?";

                if (year.HasValue)
                {
                    // Uwzględnij również realizacje z Year=0 (nieprzypisane do konkretnego roku)
                    query += " AND (Year = ? OR Year = 0)";
                    return await this.databaseService.QueryAsync<RealizedInternshipOldSMK>(
                        query, currentSpecialization.SpecializationId, year.Value);
                }
                else
                {
                    return await this.databaseService.QueryAsync<RealizedInternshipOldSMK>(
                        query, currentSpecialization.SpecializationId);
                }
            },
            "Nie udało się pobrać listy zrealizowanych staży (Stary SMK).",
            new Dictionary<string, object> { { "Year", year } });
        }

        public async Task<RealizedInternshipNewSMK> GetRealizedInternshipNewSMKAsync(int id)
        {
            return await SafeExecuteAsync(async () =>
            {
                var internship = await this.databaseService.GetRealizedInternshipNewSMKAsync(id);
                if (internship == null)
                {
                    throw new ResourceNotFoundException(
                        $"Realized internship with ID {id} not found",
                        $"Nie znaleziono zrealizowanego stażu o ID {id}");
                }
                return internship;
            },
            "Nie udało się pobrać zrealizowanego stażu (Nowy SMK).",
            new Dictionary<string, object> { { "Id", id } });
        }

        public async Task<RealizedInternshipOldSMK> GetRealizedInternshipOldSMKAsync(int id)
        {
            return await SafeExecuteAsync(async () =>
            {
                var internship = await this.databaseService.GetRealizedInternshipOldSMKAsync(id);
                if (internship == null)
                {
                    throw new ResourceNotFoundException(
                        $"Realized internship with ID {id} not found",
                        $"Nie znaleziono zrealizowanego stażu o ID {id}");
                }
                return internship;
            },
            "Nie udało się pobrać zrealizowanego stażu (Stary SMK).",
            new Dictionary<string, object> { { "Id", id } });
        }

        public async Task<bool> AddRealizedInternshipNewSMKAsync(RealizedInternshipNewSMK internship)
        {
            return await SafeExecuteAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "Internship cannot be null",
                        "Dane stażu nie mogą być puste.");
                }

                if (internship.SpecializationId <= 0)
                {
                    var currentSpecialization = await this.GetCurrentSpecializationAsync();
                    if (currentSpecialization == null)
                    {
                        return false;
                    }

                    internship.SpecializationId = currentSpecialization.SpecializationId;
                }

                int result = await this.databaseService.SaveRealizedInternshipNewSMKAsync(internship);

                if (internship.ModuleId.HasValue)
                {
                    await this.UpdateModuleProgressAsync(internship.ModuleId.Value);
                }

                return result > 0;
            },
            "Nie udało się dodać zrealizowanego stażu (Nowy SMK).",
            new Dictionary<string, object> {
                { "InternshipId", internship?.RealizedInternshipId },
                { "ModuleId", internship?.ModuleId }
            });
        }

        public async Task<bool> AddRealizedInternshipOldSMKAsync(RealizedInternshipOldSMK internship)
        {
            return await SafeExecuteAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "Internship cannot be null",
                        "Dane stażu nie mogą być puste.");
                }

                if (internship.SpecializationId <= 0)
                {
                    var currentSpecialization = await this.GetCurrentSpecializationAsync();
                    if (currentSpecialization == null)
                    {
                        return false;
                    }

                    internship.SpecializationId = currentSpecialization.SpecializationId;
                }

                int result = await this.databaseService.SaveRealizedInternshipOldSMKAsync(internship);

                // Aktualizacja postępu modułu na podstawie roku stażu
                var currentModule = await this.GetCurrentModuleAsync();
                if (currentModule != null)
                {
                    await this.UpdateModuleProgressAsync(currentModule.ModuleId);
                }

                return result > 0;
            },
            "Nie udało się dodać zrealizowanego stażu (Stary SMK).",
            new Dictionary<string, object> {
                { "InternshipId", internship?.RealizedInternshipId },
                { "Year", internship?.Year }
            });
        }

        public async Task<bool> UpdateRealizedInternshipNewSMKAsync(RealizedInternshipNewSMK internship)
        {
            return await SafeExecuteAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "Internship cannot be null",
                        "Dane stażu nie mogą być puste.");
                }

                int result = await this.databaseService.SaveRealizedInternshipNewSMKAsync(internship);

                if (internship.ModuleId.HasValue)
                {
                    await this.UpdateModuleProgressAsync(internship.ModuleId.Value);
                }

                return result > 0;
            },
            "Nie udało się zaktualizować zrealizowanego stażu (Nowy SMK).",
            new Dictionary<string, object> {
                { "InternshipId", internship?.RealizedInternshipId },
                { "ModuleId", internship?.ModuleId }
            });
        }

        public async Task<bool> UpdateRealizedInternshipOldSMKAsync(RealizedInternshipOldSMK internship)
        {
            return await SafeExecuteAsync(async () =>
            {
                if (internship == null)
                {
                    throw new InvalidInputException(
                        "Internship cannot be null",
                        "Dane stażu nie mogą być puste.");
                }

                int result = await this.databaseService.SaveRealizedInternshipOldSMKAsync(internship);

                // Aktualizacja postępu modułu na podstawie roku stażu
                var currentModule = await this.GetCurrentModuleAsync();
                if (currentModule != null)
                {
                    await this.UpdateModuleProgressAsync(currentModule.ModuleId);
                }

                return result > 0;
            },
            "Nie udało się zaktualizować zrealizowanego stażu (Stary SMK).",
            new Dictionary<string, object> {
                { "InternshipId", internship?.RealizedInternshipId },
                { "Year", internship?.Year }
            });
        }

        public async Task<bool> DeleteRealizedInternshipNewSMKAsync(int id)
        {
            return await SafeExecuteAsync(async () =>
            {
                var internship = await this.databaseService.GetRealizedInternshipNewSMKAsync(id);
                if (internship == null)
                {
                    throw new ResourceNotFoundException(
                        $"Realized internship with ID {id} not found",
                        $"Nie znaleziono zrealizowanego stażu o ID {id}");
                }

                int result = await this.databaseService.DeleteRealizedInternshipNewSMKAsync(internship);

                if (internship.ModuleId.HasValue)
                {
                    await this.UpdateModuleProgressAsync(internship.ModuleId.Value);
                }

                return result > 0;
            },
            "Nie udało się usunąć zrealizowanego stażu (Nowy SMK).",
            new Dictionary<string, object> { { "Id", id } });
        }

        public async Task<bool> DeleteRealizedInternshipOldSMKAsync(int id)
        {
            return await SafeExecuteAsync(async () =>
            {
                var internship = await this.databaseService.GetRealizedInternshipOldSMKAsync(id);
                if (internship == null)
                {
                    throw new ResourceNotFoundException(
                        $"Realized internship with ID {id} not found",
                        $"Nie znaleziono zrealizowanego stażu o ID {id}");
                }

                int result = await this.databaseService.DeleteRealizedInternshipOldSMKAsync(internship);

                // Aktualizacja postępu modułu na podstawie roku stażu
                var currentModule = await this.GetCurrentModuleAsync();
                if (currentModule != null)
                {
                    await this.UpdateModuleProgressAsync(currentModule.ModuleId);
                }

                return result > 0;
            },
            "Nie udało się usunąć zrealizowanego stażu (Stary SMK).",
            new Dictionary<string, object> { { "Id", id } });
        }

        public async Task<int?> GetModuleIdForYearAsync(int year)
        {
            return await SafeExecuteAsync(async () =>
            {
                var currentSpecialization = await this.GetCurrentSpecializationAsync();
                if (currentSpecialization == null)
                {
                    return null;
                }

                var modules = await this.databaseService.GetModulesAsync(currentSpecialization.SpecializationId);

                // W starym SMK: lata 1-2 to moduł podstawowy, 3+ to moduł specjalistyczny
                var moduleType = (year <= 2) ? ModuleType.Basic : ModuleType.Specialistic;

                var module = modules.FirstOrDefault(m => m.Type == moduleType);
                return module?.ModuleId;
            },
            "Nie udało się określić modułu dla podanego roku.",
            new Dictionary<string, object> { { "Year", year } });
        }

        public async Task<List<RealizedInternshipOldSMK>> GetFilteredRealizedInternshipsOldSMKAsync(int? moduleId)
        {
            return await SafeExecuteAsync(async () =>
            {
                var currentSpecialization = await this.GetCurrentSpecializationAsync();
                if (currentSpecialization == null)
                {
                    return new List<RealizedInternshipOldSMK>();
                }

                // Jeśli podano moduł, określamy odpowiedni zakres lat
                int startYear = 1;
                int endYear = 6;

                if (moduleId.HasValue)
                {
                    var module = await this.databaseService.GetModuleAsync(moduleId.Value);
                    if (module != null)
                    {
                        if (module.Type == ModuleType.Basic)
                        {
                            startYear = 1;
                            endYear = 2;
                        }
                        else
                        {
                            startYear = 3;
                            endYear = 6;
                        }
                    }
                }

                // Pobieramy wszystkie realizacje dla lat w zakresie
                List<RealizedInternshipOldSMK> allRealizations = new List<RealizedInternshipOldSMK>();

                for (int year = startYear; year <= endYear; year++)
                {
                    var yearRealizations = await this.databaseService.GetRealizedInternshipsOldSMKAsync(
                        currentSpecialization.SpecializationId, year);

                    allRealizations.AddRange(yearRealizations);
                }

                return allRealizations;
            },
            "Nie udało się pobrać filtrowanej listy zrealizowanych staży (Stary SMK).",
            new Dictionary<string, object> { { "ModuleId", moduleId } });
        }

        public void ClearCache()
        {
            _cachedSpecialization = null;
            _cachedModules = null;
            Logger.LogInformation("Cache wyczyszczony");
        }
    }
}
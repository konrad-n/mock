using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;

namespace SledzSpecke.App.Services.Specialization
{
    public class SpecializationService : ISpecializationService
    {
        private readonly IDatabaseService databaseService;
        private readonly IAuthService authService;
        private readonly IDialogService dialogService;
        private readonly ModuleInitializer moduleInitializer;
        private Models.Specialization _cachedSpecialization;
        private List<Module> _cachedModules;

        public SpecializationService(IDatabaseService databaseService, IAuthService authService, IDialogService dialogService)
        {
            this.databaseService = databaseService;
            this.authService = authService;
            this.dialogService = dialogService;
            this.moduleInitializer = new ModuleInitializer(databaseService);
        }

        public async Task<Models.Specialization> GetCurrentSpecializationAsync(bool includeModules = true)
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
        }

        public async Task<List<Module>> GetModulesAsync(int specializationId, bool initializeIfNeeded)
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
        }


        public async Task<Module> GetCurrentModuleAsync()
        {
            var specialization = await GetCurrentSpecializationAsync(false);
            if (specialization?.CurrentModuleId == null)
            {
                return null;
            }

            return await databaseService.GetModuleAsync(specialization.CurrentModuleId.Value);
        }

        public async Task<int> GetInternshipCountAsync(int? moduleId = null)
        {
            var specialization = await this.GetCurrentSpecializationAsync();
            if (specialization == null)
            {
                return 0;
            }

            var internships = await this.databaseService.GetInternshipsAsync(
                specializationId: specialization.SpecializationId,
                moduleId: moduleId);

            int completedCount = internships.Count(i => i.IsCompleted);
            return completedCount;
        }

        public async Task<int> GetProcedureCountAsync(int? moduleId = null)
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
        }

        public async Task<int> GetCourseCountAsync(int? moduleId = null)
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
        }

        public async Task<int> GetShiftCountAsync(int? moduleId = null)
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
        }

        public async Task<int> GetSelfEducationCountAsync(int? moduleId = null)
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
        }

        public async Task<int> GetPublicationCountAsync(int? moduleId = null)
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
        }

        public async Task<int> GetEducationalActivityCountAsync(int? moduleId = null)
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
        }

        public async Task<int> GetAbsenceCountAsync()
        {
            var specialization = await this.GetCurrentSpecializationAsync();
            if (specialization == null)
            {
                return 0;
            }

            var absences = await this.databaseService.GetAbsencesAsync(specialization.SpecializationId);
            return absences.Count;
        }

        public async Task<int> GetRecognitionCountAsync()
        {
            var specialization = await this.GetCurrentSpecializationAsync();
            if (specialization == null)
            {
                return 0;
            }

            var recognitions = await this.databaseService.GetRecognitionsAsync(specialization.SpecializationId);
            return recognitions.Count;
        }

        public async Task<SpecializationStatistics> GetSpecializationStatisticsAsync(int? moduleId = null)
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
        }

        public async Task UpdateSpecializationProgressAsync(int specializationId)
        {
            await ProgressCalculator.UpdateSpecializationProgressAsync(this.databaseService, specializationId);
        }

        public async Task UpdateModuleProgressAsync(int moduleId)
        {
            await ProgressCalculator.UpdateModuleProgressAsync(this.databaseService, moduleId);
        }

        public async Task<DateTime> CalculateSpecializationEndDateAsync(int specializationId)
        {
                var specialization = await this.databaseService.GetSpecializationAsync(specializationId);
                var absences = await this.databaseService.GetAbsencesAsync(specializationId);

                return DateCalculator.CalculateSpecializationEndDate(
                    specialization.StartDate,
                    (specialization.PlannedEndDate - specialization.StartDate).Days + 1,
                    absences);
        }

        public async Task<List<Module>> GetModulesAsync(int specializationId)
        {
            return await this.databaseService.GetModulesAsync(specializationId);
        }

        public async Task<List<SpecializationProgram>> GetAvailableSpecializationProgramsAsync()
        {
            return await this.databaseService.GetAllSpecializationProgramsAsync();
        }

        public async Task<SpecializationProgram> LoadSpecializationProgramAsync(string code, SmkVersion smkVersion)
        {
            return await SpecializationLoader.LoadSpecializationProgramAsync(code, smkVersion);
        }

        public async Task<bool> InitializeSpecializationModulesAsync(int specializationId)
        {
            return await this.moduleInitializer.InitializeModulesIfNeededAsync(specializationId);
        }

        public async Task<List<MedicalShift>> GetMedicalShiftsAsync(int? internshipId = null)
        {
            return await this.databaseService.GetMedicalShiftsAsync(internshipId);
        }

        public async Task<MedicalShift> GetMedicalShiftAsync(int shiftId)
        {
            return await this.databaseService.GetMedicalShiftAsync(shiftId);
        }

        public async Task<bool> AddMedicalShiftAsync(MedicalShift shift)
        {
            int result = await this.databaseService.SaveMedicalShiftAsync(shift);

            var currentModule = await this.GetCurrentModuleAsync();
            if (currentModule != null)
            {
                await this.UpdateModuleProgressAsync(currentModule.ModuleId);
            }

            return result > 0;
        }

        public async Task<bool> UpdateMedicalShiftAsync(MedicalShift shift)
        {
            return await AddMedicalShiftAsync(shift);
        }

        public async Task<bool> DeleteMedicalShiftAsync(int shiftId)
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
        }

        public async Task<Internship> GetInternshipAsync(int internshipId)
        {
            return await this.databaseService.GetInternshipAsync(internshipId);
        }

        public async Task<List<Internship>> GetInternshipsAsync(int? moduleId = null)
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
                        InternshipId = -id,
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
        }

        public async Task<List<Internship>> GetUserInternshipsAsync(int? moduleId = null)
        {
            var currentSpecialization = await this.GetCurrentSpecializationAsync();
            if (currentSpecialization == null)
            {
                return new List<Internship>();
            }

            return await this.databaseService.GetInternshipsAsync(
                specializationId: currentSpecialization.SpecializationId,
                moduleId: moduleId);
        }

        public async Task<Internship> GetCurrentInternshipAsync()
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
        }

        public async Task<bool> AddInternshipAsync(Internship internship)
        {
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
        }

        public async Task<bool> UpdateInternshipAsync(Internship internship)
        {
            int result = await this.databaseService.SaveInternshipAsync(internship);

            if (internship.ModuleId.HasValue)
            {
                await this.UpdateModuleProgressAsync(internship.ModuleId.Value);
            }

            return result > 0;
        }

        public async Task<bool> DeleteInternshipAsync(int internshipId)
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
                return false;
            }

            int result = await this.databaseService.DeleteInternshipAsync(internship);

            if (internship.ModuleId.HasValue)
            {
                await this.UpdateModuleProgressAsync(internship.ModuleId.Value);
            }

            return result > 0;
        }

        public event EventHandler<int> CurrentModuleChanged;

        public async Task SetCurrentModuleAsync(int moduleId)
        {
            var specialization = await this.GetCurrentSpecializationAsync();
            if (specialization == null)
            {
                return;
            }

            var module = await this.databaseService.GetModuleAsync(moduleId);
            if (module == null || module.SpecializationId != specialization.SpecializationId)
            {
                return;
            }

            specialization.CurrentModuleId = moduleId;
            await this.databaseService.UpdateSpecializationAsync(specialization);
            await SettingsHelper.SetCurrentModuleIdAsync(moduleId);

            this.CurrentModuleChanged?.Invoke(this, moduleId);
        }

        public async Task<List<RealizedInternshipNewSMK>> GetRealizedInternshipsNewSMKAsync(int? moduleId = null, int? internshipRequirementId = null)
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
        }

        public async Task<List<RealizedInternshipOldSMK>> GetRealizedInternshipsOldSMKAsync(int? year = null)
        {
            var currentSpecialization = await this.GetCurrentSpecializationAsync();
            if (currentSpecialization == null)
            {
                return new List<RealizedInternshipOldSMK>();
            }

            string query = "SELECT * FROM RealizedInternshipOldSMK WHERE SpecializationId = ?";

            if (year.HasValue)
            {
                query += " AND Year = ?";
                return await this.databaseService.QueryAsync<RealizedInternshipOldSMK>(
                    query, currentSpecialization.SpecializationId, year.Value);
            }
            else
            {
                return await this.databaseService.QueryAsync<RealizedInternshipOldSMK>(
                    query, currentSpecialization.SpecializationId);
            }
        }

        public async Task<RealizedInternshipNewSMK> GetRealizedInternshipNewSMKAsync(int id)
        {
            return await this.databaseService.GetRealizedInternshipNewSMKAsync(id);
        }

        public async Task<RealizedInternshipOldSMK> GetRealizedInternshipOldSMKAsync(int id)
        {
            return await this.databaseService.GetRealizedInternshipOldSMKAsync(id);
        }

        public async Task<bool> AddRealizedInternshipNewSMKAsync(RealizedInternshipNewSMK internship)
        {
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
        }

        public async Task<bool> AddRealizedInternshipOldSMKAsync(RealizedInternshipOldSMK internship)
        {
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
        }

        public async Task<bool> UpdateRealizedInternshipNewSMKAsync(RealizedInternshipNewSMK internship)
        {
            int result = await this.databaseService.SaveRealizedInternshipNewSMKAsync(internship);

            if (internship.ModuleId.HasValue)
            {
                await this.UpdateModuleProgressAsync(internship.ModuleId.Value);
            }

            return result > 0;
        }

        public async Task<bool> UpdateRealizedInternshipOldSMKAsync(RealizedInternshipOldSMK internship)
        {
            int result = await this.databaseService.SaveRealizedInternshipOldSMKAsync(internship);

            // Aktualizacja postępu modułu na podstawie roku stażu
            var currentModule = await this.GetCurrentModuleAsync();
            if (currentModule != null)
            {
                await this.UpdateModuleProgressAsync(currentModule.ModuleId);
            }

            return result > 0;
        }

        public async Task<bool> DeleteRealizedInternshipNewSMKAsync(int id)
        {
            var internship = await this.databaseService.GetRealizedInternshipNewSMKAsync(id);
            if (internship == null)
            {
                return false;
            }

            int result = await this.databaseService.DeleteRealizedInternshipNewSMKAsync(internship);

            if (internship.ModuleId.HasValue)
            {
                await this.UpdateModuleProgressAsync(internship.ModuleId.Value);
            }

            return result > 0;
        }

        public async Task<bool> DeleteRealizedInternshipOldSMKAsync(int id)
        {
            var internship = await this.databaseService.GetRealizedInternshipOldSMKAsync(id);
            if (internship == null)
            {
                return false;
            }

            int result = await this.databaseService.DeleteRealizedInternshipOldSMKAsync(internship);

            // Aktualizacja postępu modułu na podstawie roku stażu
            var currentModule = await this.GetCurrentModuleAsync();
            if (currentModule != null)
            {
                await this.UpdateModuleProgressAsync(currentModule.ModuleId);
            }

            return result > 0;
        }

        public async Task<int?> GetModuleIdForYearAsync(int year)
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
        }

        public async Task<List<RealizedInternshipOldSMK>> GetFilteredRealizedInternshipsOldSMKAsync(int? moduleId)
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
        }

        public void ClearCache()
        {
            _cachedSpecialization = null;
            _cachedModules = null;
        }
    }
}

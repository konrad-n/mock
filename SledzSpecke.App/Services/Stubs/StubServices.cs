using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Services.Stubs
{
    public class StubProcedureService : IProcedureService
    {
        public Task<ProcedureExecution> AddProcedureAsync(ProcedureExecution procedure)
            => Task.FromResult(procedure);

        public Task<bool> DeleteProcedureAsync(int id)
            => Task.FromResult(true);

        public Task<List<string>> GetAvailableCategoriesAsync()
            => Task.FromResult(new List<string>());

        public Task<List<string>> GetAvailableStagesAsync()
            => Task.FromResult(new List<string>());

        public Task<ProcedureExecution> GetProcedureAsync(int id)
            => Task.FromResult(new ProcedureExecution());

        public Task<double> GetProcedureCompletionPercentageAsync()
            => Task.FromResult(0.25);

        public Task<Dictionary<string, (int Required, int Completed, int Assisted)>> GetProcedureProgressByCategoryAsync()
            => Task.FromResult(new Dictionary<string, (int Required, int Completed, int Assisted)>());

        public Task<Dictionary<string, (int Required, int Completed, int Assisted)>> GetProcedureProgressByStageAsync()
            => Task.FromResult(new Dictionary<string, (int Required, int Completed, int Assisted)>());

        public Task<List<ProcedureRequirement>> GetRequirementsByCategoryAsync(string category)
            => Task.FromResult(new List<ProcedureRequirement>());

        public Task<List<ProcedureRequirement>> GetRequirementsByStageAsync(string stage)
            => Task.FromResult(new List<ProcedureRequirement>());

        public Task<List<ProcedureRequirement>> GetRequirementsForSpecializationAsync()
            => Task.FromResult(new List<ProcedureRequirement>());

        public Task<List<ProcedureExecution>> GetUserProceduresAsync()
            => Task.FromResult(new List<ProcedureExecution>());

        public Task<bool> UpdateProcedureAsync(ProcedureExecution procedure)
            => Task.FromResult(true);

        public Task<bool> ValidateProcedureRequirementsAsync(ProcedureExecution procedure)
            => Task.FromResult(true);
    }

    public class StubCourseService : ICourseService
    {
        public Task<bool> CompleteCourseAsync(int courseId, CourseDocument certificate)
            => Task.FromResult(true);

        public Task<Course> GetCourseAsync(int id)
            => Task.FromResult(new Course());

        public Task<List<CourseDefinition>> GetCoursesByYearAsync(int year)
            => Task.FromResult(new List<CourseDefinition>());

        public Task<double> GetCourseProgressAsync()
            => Task.FromResult(0.4);

        public Task<Dictionary<string, (int Required, int Completed)>> GetCourseProgressByYearAsync()
            => Task.FromResult(new Dictionary<string, (int Required, int Completed)>());

        public Task<List<string>> GetCourseTopicsAsync(int courseDefinitionId)
            => Task.FromResult(new List<string>());

        public Task<List<CourseDefinition>> GetRecommendedCoursesForCurrentYearAsync()
            => Task.FromResult(new List<CourseDefinition>());

        public Task<List<CourseDefinition>> GetRequiredCoursesAsync()
            => Task.FromResult(new List<CourseDefinition>());

        public Task<List<Course>> GetUserCoursesAsync()
            => Task.FromResult(new List<Course>());

        public Task<Course> RegisterForCourseAsync(Course course)
            => Task.FromResult(course);
    }

    public class StubInternshipService : IInternshipService
    {
        public Task<bool> CompleteInternshipAsync(int internshipId, InternshipDocument completion)
            => Task.FromResult(true);

        public Task<Internship> GetInternshipAsync(int id)
            => Task.FromResult(new Internship());

        public Task<double> GetInternshipProgressAsync()
            => Task.FromResult(0.3);

        public Task<Dictionary<string, (int Required, int Completed)>> GetInternshipProgressByYearAsync()
            => Task.FromResult(new Dictionary<string, (int Required, int Completed)>());

        public Task<List<InternshipModule>> GetModulesForInternshipAsync(int internshipDefinitionId)
            => Task.FromResult(new List<InternshipModule>());

        public Task<List<InternshipDefinition>> GetRecommendedInternshipsForCurrentYearAsync()
            => Task.FromResult(new List<InternshipDefinition>());

        public Task<Dictionary<string, Dictionary<string, int>>> GetRequiredProceduresByInternshipAsync(int internshipDefinitionId)
            => Task.FromResult(new Dictionary<string, Dictionary<string, int>>());

        public Task<List<InternshipDefinition>> GetRequiredInternshipsAsync()
            => Task.FromResult(new List<InternshipDefinition>());

        public Task<Dictionary<string, List<string>>> GetRequiredSkillsByInternshipAsync(int internshipDefinitionId)
            => Task.FromResult(new Dictionary<string, List<string>>());

        public Task<List<Internship>> GetUserInternshipsAsync()
            => Task.FromResult(new List<Internship>());

        public Task<Internship> StartInternshipAsync(Internship internship)
            => Task.FromResult(internship);
    }

    public class StubDutyService : IDutyService
    {
        public Task<Duty> AddDutyAsync(Duty duty)
            => Task.FromResult(duty);

        public Task<bool> DeleteDutyAsync(int id)
            => Task.FromResult(true);

        public Task<Duty> GetDutyAsync(int id)
            => Task.FromResult(new Duty());

        public Task<DutyStatistics> GetDutyStatisticsAsync()
            => Task.FromResult(new DutyStatistics
            {
                TotalHours = 120,
                MonthlyHours = 24,
                RemainingHours = 280,
                DutiesByType = new Dictionary<DutyType, int>()
            });

        public Task<List<Duty>> GetUpcomingDutiesAsync(int daysAhead = 7)
            => Task.FromResult(new List<Duty>());

        public Task<List<Duty>> GetUserDutiesAsync(DateTime? fromDate = null)
            => Task.FromResult(new List<Duty>());

        public Task<bool> UpdateDutyAsync(Duty duty)
            => Task.FromResult(true);
    }

    public class StubUserService : IUserService
    {
        public Task<bool> ExportUserDataAsync()
            => Task.FromResult(true);

        public Task<int> GetCurrentUserIdAsync()
            => Task.FromResult(1);

        public Task<User> GetCurrentUserAsync()
            => Task.FromResult(new User
            {
                Id = 1,
                Name = "Test User",
                PWZ = "123456",
                ExpectedEndDate = DateTime.Now.AddYears(2),
                CurrentSpecializationId = 1
            });

        public Task<bool> LogoutAsync()
            => Task.FromResult(true);
    }

    public class StubSpecializationService : ISpecializationService
    {
        public Task<bool> ChangeSpecializationAsync(int specializationId)
            => Task.FromResult(true);

        public Task<List<Specialization>> GetAvailableSpecializationsAsync()
            => Task.FromResult(new List<Specialization>
            {
            new Specialization { Id = 1, Name = "Psychiatria", DurationInWeeks = 260 }
            });

        public Task<Specialization> GetCurrentSpecializationAsync()
            => Task.FromResult(new Specialization
            {
                Id = 1,
                Name = "Psychiatria",
                DurationInWeeks = 260,
                Description = "Specjalizacja z psychiatrii"
            });

        public Task<SpecializationProgress> GetProgressStatisticsAsync(int specializationId)
            => Task.FromResult(new SpecializationProgress
            {
                UserId = 1,
                SpecializationId = specializationId,
                ProceduresProgress = 0.25,
                CoursesProgress = 0.4,
                InternshipsProgress = 0.3,
                DutiesProgress = 0.45,
                OverallProgress = 0.35,
                LastCalculated = DateTime.Now
            });

        public Task<Dictionary<string, double>> GetRequirementsProgressAsync(int specializationId)
            => Task.FromResult(new Dictionary<string, double>
            {
            { "Procedury", 0.25 },
            { "Kursy", 0.4 },
            { "Staże", 0.3 },
            { "Dyżury", 0.45 }
            });

        public Task<List<CourseDefinition>> GetRequiredCoursesAsync(int specializationId)
            => Task.FromResult(new List<CourseDefinition>());

        public Task<List<DutyRequirement>> GetRequiredDutiesAsync(int specializationId)
            => Task.FromResult(new List<DutyRequirement>());

        public Task<List<InternshipDefinition>> GetRequiredInternshipsAsync(int specializationId)
            => Task.FromResult(new List<InternshipDefinition>());

        public Task<List<ProcedureRequirement>> GetRequiredProceduresAsync(int specializationId)
            => Task.FromResult(new List<ProcedureRequirement>());

        public Task<Specialization> GetSpecializationAsync(int id)
            => Task.FromResult(new Specialization
            {
                Id = id,
                Name = "Psychiatria",
                DurationInWeeks = 260
            });
    }

    public class StubSettingsService : ISettingsService
    {
        private readonly Dictionary<string, object> _settings = new Dictionary<string, object>();

        public Task<bool> ClearAllSettingsAsync()
        {
            _settings.Clear();
            return Task.FromResult(true);
        }

        public T GetSetting<T>(string key, T defaultValue)
        {
            if (_settings.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return defaultValue;
        }

        public void SetSetting<T>(string key, T value)
        {
            _settings[key] = value;
        }
    }

    public class StubDataSyncService : IDataSyncService
    {
        public Task<bool> ClearAllDataAsync()
            => Task.FromResult(true);

        public Task<bool> CreateBackupAsync()
            => Task.FromResult(true);

        public Task<bool> RestoreFromBackupAsync()
            => Task.FromResult(true);

        public Task<bool> SyncAllDataAsync()
            => Task.FromResult(true);
    }
}

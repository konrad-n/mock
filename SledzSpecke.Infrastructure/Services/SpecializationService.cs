using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Core.Models.Monitoring;
using SledzSpecke.Infrastructure.Database.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Services
{
    public class SpecializationService : ISpecializationService
    {
        private readonly ISpecializationRepository _specializationRepository;
        private readonly IUserService _userService;
        private readonly IProcedureRepository _procedureRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IInternshipRepository _internshipRepository;
        private readonly IDutyRepository _dutyRepository;
        private readonly ISpecializationRequirementsProvider _requirementsProvider;
        private readonly ILogger<SpecializationService> _logger;

        public SpecializationService(
            ISpecializationRepository specializationRepository,
            IUserService userService,
            IProcedureRepository procedureRepository,
            ICourseRepository courseRepository,
            IInternshipRepository internshipRepository,
            IDutyRepository dutyRepository,
            ISpecializationRequirementsProvider requirementsProvider,
            ILogger<SpecializationService> logger)
        {
            _specializationRepository = specializationRepository;
            _userService = userService;
            _procedureRepository = procedureRepository;
            _courseRepository = courseRepository;
            _internshipRepository = internshipRepository;
            _dutyRepository = dutyRepository;
            _requirementsProvider = requirementsProvider;
            _logger = logger;
        }

        public async Task<Specialization> GetCurrentSpecializationAsync()
        {
            try
            {
                var user = await _userService.GetCurrentUserAsync();
                if (user?.CurrentSpecializationId == null)
                {
                    return null;
                }

                return await _specializationRepository.GetByIdAsync(user.CurrentSpecializationId.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current specialization");
                return null;
            }
        }

        public async Task<Specialization> GetSpecializationAsync(int id)
        {
            try
            {
                return await _specializationRepository.GetWithRequirementsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting specialization {SpecializationId}", id);
                return null;
            }
        }

        public async Task<List<Specialization>> GetAllSpecializationsAsync()
        {
            try
            {
                return await _specializationRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all specializations");
                return new List<Specialization>();
            }
        }


        public async Task<SpecializationProgress> GetProgressStatisticsAsync(int specializationId)
        {
            try
            {
                var userId = await _userService.GetCurrentUserIdAsync();
                double proceduresProgress = await CalculateProceduresProgressAsync(userId, specializationId);
                var dutyStats = await _dutyRepository.GetDutyStatisticsAsync(userId);
                double dutiesProgress = CalculateDutiesProgress(dutyStats);
                double coursesProgress = await CalculateCoursesProgressAsync(userId, specializationId);
                double internshipsProgress = await CalculateInternshipsProgressAsync(userId, specializationId);
                double overallProgress = (proceduresProgress + dutiesProgress + coursesProgress + internshipsProgress) / 4.0;

                var progress = new SpecializationProgress
                {
                    UserId = userId,
                    SpecializationId = specializationId,
                    ProceduresProgress = proceduresProgress,
                    CoursesProgress = coursesProgress,
                    InternshipsProgress = internshipsProgress,
                    DutiesProgress = dutiesProgress,
                    OverallProgress = overallProgress,
                    TotalProgress = overallProgress,
                    LastCalculated = DateTime.Now,
                    RemainingRequirements = GenerateRemainingRequirementsText(proceduresProgress, dutiesProgress, coursesProgress, internshipsProgress)
                };

                return progress;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting progress statistics");
                return new SpecializationProgress
                {
                    UserId = await _userService.GetCurrentUserIdAsync(),
                    SpecializationId = specializationId,
                    OverallProgress = 0,
                    TotalProgress = 0
                };
            }
        }

        private string GenerateRemainingRequirementsText(double proceduresProgress, double dutiesProgress,
                                                      double coursesProgress, double internshipsProgress)
        {
            var remainingItems = new List<string>();

            if (proceduresProgress < 1.0)
                remainingItems.Add($"Procedury: {proceduresProgress:P0} ukończone");

            if (dutiesProgress < 1.0)
                remainingItems.Add($"Dyżury: {dutiesProgress:P0} ukończone");

            if (coursesProgress < 1.0)
                remainingItems.Add($"Kursy: {coursesProgress:P0} ukończone");

            if (internshipsProgress < 1.0)
                remainingItems.Add($"Staże: {internshipsProgress:P0} ukończone");

            return string.Join("\n", remainingItems);
        }

        private async Task<double> CalculateProceduresProgressAsync(int userId, int specializationId)
        {
            try
            {
                var requiredProcedures = _requirementsProvider.GetRequiredProceduresBySpecialization(specializationId);
                var userProcedures = await _procedureRepository.GetUserProceduresAsync(userId);
                var completedProcedures = new Dictionary<string, ProcedureMonitoring.ProcedureProgress>();

                foreach (var procedure in userProcedures)
                {
                    if (!completedProcedures.ContainsKey(procedure.Name))
                    {
                        completedProcedures[procedure.Name] = new ProcedureMonitoring.ProcedureProgress
                        {
                            ProcedureName = procedure.Name,
                            CompletedCount = 0,
                            AssistanceCount = 0,
                            SimulationCount = 0,
                            Executions = new List<ProcedureMonitoring.ProcedureExecution>()
                        };
                    }

                    var progress = completedProcedures[procedure.Name];

                    var execution = new ProcedureMonitoring.ProcedureExecution
                    {
                        ExecutionDate = procedure.ExecutionDate,
                        Type = procedure.Type.ToString(),
                        Location = procedure.Location,
                        Notes = procedure.Notes
                    };

                    progress.Executions.Add(execution);

                    if (procedure.Type == Core.Models.Enums.ProcedureType.Execution)
                    {
                        if (procedure.IsSimulation)
                            progress.SimulationCount++;
                        else
                            progress.CompletedCount++;
                    }
                    else
                    {
                        progress.AssistanceCount++;
                    }
                }

                var verifier = new ProcedureMonitoring.ProgressVerification(requiredProcedures);
                var summary = verifier.GenerateProgressSummary(completedProcedures);

                return summary.OverallCompletionPercentage / 100.0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating procedures progress");
                return 0;
            }
        }

        private double CalculateDutiesProgress(DutyStatistics dutyStats)
        {
            if (dutyStats == null || dutyStats.TotalHours + dutyStats.RemainingHours <= 0)
            {
                return 0;
            }

            return Math.Min(1.0, (double)dutyStats.TotalHours / (double)(dutyStats.TotalHours + dutyStats.RemainingHours));
        }

        private async Task<double> CalculateCoursesProgressAsync(int userId, int specializationId)
        {
            try
            {
                return await _courseRepository.GetCourseProgressAsync(userId, specializationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating courses progress");
                return 0;
            }
        }

        private async Task<double> CalculateInternshipsProgressAsync(int userId, int specializationId)
        {
            try
            {
                return await _internshipRepository.GetInternshipProgressAsync(userId, specializationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating internships progress");
                return 0;
            }
        }

        public async Task<List<ProcedureRequirement>> GetRequiredProceduresAsync(int specializationId)
        {
            try
            {
                return await _procedureRepository.GetRequirementsForSpecializationAsync(specializationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting required procedures");
                return new List<ProcedureRequirement>();
            }
        }

        public async Task<List<CourseDefinition>> GetRequiredCoursesAsync(int specializationId)
        {
            try
            {
                return await _courseRepository.GetRequiredCoursesAsync(specializationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting required courses");
                return new List<CourseDefinition>();
            }
        }

        public async Task<List<InternshipDefinition>> GetRequiredInternshipsAsync(int specializationId)
        {
            try
            {
                return await _internshipRepository.GetRequiredInternshipsAsync(specializationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting required internships");
                return new List<InternshipDefinition>();
            }
        }

        public async Task<List<DutyRequirement>> GetRequiredDutiesAsync(int specializationId)
        {
            try
            {
                return new List<DutyRequirement>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting required duties");
                return new List<DutyRequirement>();
            }
        }

        public async Task<Dictionary<string, double>> GetRequirementsProgressAsync(int specializationId)
        {
            try
            {
                var userId = await _userService.GetCurrentUserIdAsync();
                var coursesProgress = await _courseRepository.GetCourseProgressByYearAsync(userId, specializationId);
                var internshipsProgress = await _internshipRepository.GetInternshipProgressByYearAsync(userId, specializationId);
                var requirementsProgress = new Dictionary<string, double>();

                foreach (var year in coursesProgress)
                {
                    if (year.Value.Required > 0)
                    {
                        requirementsProgress[$"Kursy: {year.Key}"] =
                            Math.Min(1.0, (double)year.Value.Completed / year.Value.Required);
                    }
                }

                foreach (var year in internshipsProgress)
                {
                    if (year.Value.Required > 0)
                    {
                        requirementsProgress[$"Staże: {year.Key}"] =
                            Math.Min(1.0, (double)year.Value.Completed / year.Value.Required);
                    }
                }

                return requirementsProgress;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting requirements progress");
                return new Dictionary<string, double>();
            }
        }
    }
}

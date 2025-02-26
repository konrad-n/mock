using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
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
        private readonly ILogger<SpecializationService> _logger;

        public SpecializationService(
            ISpecializationRepository specializationRepository,
            IUserService userService,
            IProcedureRepository procedureRepository,
            ICourseRepository courseRepository,
            IInternshipRepository internshipRepository,
            IDutyRepository dutyRepository,
            ILogger<SpecializationService> logger)
        {
            _specializationRepository = specializationRepository;
            _userService = userService;
            _procedureRepository = procedureRepository;
            _courseRepository = courseRepository;
            _internshipRepository = internshipRepository;
            _dutyRepository = dutyRepository;
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

        public async Task<SpecializationProgress> GetProgressStatisticsAsync(int specializationId)
        {
            try
            {
                var userId = await _userService.GetCurrentUserIdAsync();
                
                // Pobierz odpowiednie dane
                var procedureStats = await _procedureRepository.GetProcedureStatsAsync(userId);
                var dutyStats = await _dutyRepository.GetDutyStatisticsAsync(userId);
                
                // Oblicz postęp
                double proceduresProgress = CalculateProceduresProgress(procedureStats);
                double dutiesProgress = CalculateDutiesProgress(dutyStats);
                double coursesProgress = await CalculateCoursesProgressAsync(userId, specializationId);
                double internshipsProgress = await CalculateInternshipsProgressAsync(userId, specializationId);
                
                // Oblicz ogólny postęp
                double overallProgress = (proceduresProgress + dutiesProgress + coursesProgress + internshipsProgress) / 4.0;
                
                // Utwórz obiekt postępu specjalizacji
                var progress = new SpecializationProgress
                {
                    UserId = userId,
                    SpecializationId = specializationId,
                    ProceduresProgress = proceduresProgress,
                    CoursesProgress = coursesProgress,
                    InternshipsProgress = internshipsProgress,
                    DutiesProgress = dutiesProgress,
                    OverallProgress = overallProgress,
                    LastCalculated = DateTime.Now
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
                    OverallProgress = 0
                };
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
                // Tutaj należałoby zaimplementować pobranie wymagań dotyczących dyżurów
                // z odpowiedniego repozytorium
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
                
                // Pobierz postęp dla różnych kategorii wymagań
                var procedureProgress = await _procedureRepository.GetProcedureProgressByCategoryAsync(userId, specializationId);
                var coursesProgress = await _courseRepository.GetCourseProgressByYearAsync(userId, specializationId);
                var internshipsProgress = await _internshipRepository.GetInternshipProgressByYearAsync(userId, specializationId);
                
                // Oblicz postęp dla każdej kategorii
                var requirementsProgress = new Dictionary<string, double>();
                
                // Dodaj postęp procedur
                foreach (var category in procedureProgress)
                {
                    if (category.Value.Required > 0)
                    {
                        requirementsProgress[$"Procedury: {category.Key}"] = 
                            (double)category.Value.Completed / category.Value.Required;
                    }
                }
                
                // Dodaj postęp kursów
                foreach (var year in coursesProgress)
                {
                    if (year.Value.Required > 0)
                    {
                        requirementsProgress[$"Kursy: {year.Key}"] = 
                            (double)year.Value.Completed / year.Value.Required;
                    }
                }
                
                // Dodaj postęp staży
                foreach (var year in internshipsProgress)
                {
                    if (year.Value.Required > 0)
                    {
                        requirementsProgress[$"Staże: {year.Key}"] = 
                            (double)year.Value.Completed / year.Value.Required;
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

        // Metody pomocnicze do obliczania postępu
        private double CalculateProceduresProgress(Dictionary<string, int> procedureStats)
        {
            // W rzeczywistej implementacji należałoby porównać liczbę wykonanych 
            // procedur z wymaganiami specjalizacji
            return 0.5; // Domyślna wartość dla uproszczenia
        }

        private double CalculateDutiesProgress(DutyStatistics dutyStats)
        {
            if (dutyStats.TotalHours + dutyStats.RemainingHours <= 0)
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
            catch
            {
                return 0;
            }
        }

        private async Task<double> CalculateInternshipsProgressAsync(int userId, int specializationId)
        {
            try
            {
                // W rzeczywistej implementacji należałoby pobierać tę wartość z repozytorium
                return 0.3; // Domyślna wartość dla uproszczenia
            }
            catch
            {
                return 0;
            }
        }
    }
}

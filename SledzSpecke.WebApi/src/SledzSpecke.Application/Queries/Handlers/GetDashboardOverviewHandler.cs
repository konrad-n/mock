using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

internal sealed class GetDashboardOverviewHandler : IQueryHandler<GetDashboardOverview, DashboardOverviewDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IModuleRepository _moduleRepository;
    private readonly IProgressCalculationService _progressCalculationService;
    private readonly IPublicationRepository _publicationRepository;
    private readonly ISelfEducationRepository _selfEducationRepository;
    private readonly IUserContextService _userContextService;

    public GetDashboardOverviewHandler(
        IUserRepository userRepository,
        ISpecializationRepository specializationRepository,
        IModuleRepository moduleRepository,
        IProgressCalculationService progressCalculationService,
        IPublicationRepository publicationRepository,
        ISelfEducationRepository selfEducationRepository,
        IUserContextService userContextService)
    {
        _userRepository = userRepository;
        _specializationRepository = specializationRepository;
        _moduleRepository = moduleRepository;
        _progressCalculationService = progressCalculationService;
        _publicationRepository = publicationRepository;
        _selfEducationRepository = selfEducationRepository;
        _userContextService = userContextService;
    }

    public async Task<DashboardOverviewDto> HandleAsync(GetDashboardOverview query)
    {
        // Get user and their current specialization
        var userId = _userContextService.GetUserId();
        var user = await _userRepository.GetByIdAsync(new UserId(userId));
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        var specialization = await _specializationRepository.GetByIdAsync(user.SpecializationId);
        if (specialization == null)
        {
            throw new InvalidOperationException("User has no active specialization");
        }

        // Get current module
        var currentModule = specialization.CurrentModuleId != null 
            ? await _moduleRepository.GetByIdAsync(specialization.CurrentModuleId)
            : specialization.Modules.FirstOrDefault();

        if (currentModule == null)
        {
            throw new InvalidOperationException("No module found for specialization");
        }

        // Calculate statistics for current module
        var statistics = await _progressCalculationService.CalculateFullStatisticsAsync(
            specialization.Id.Value, 
            currentModule.Id.Value);

        // Get additional counts
        var publications = await _publicationRepository.GetByUserIdAsync(new UserId(userId));
        var selfEducation = await _selfEducationRepository.GetByUserIdAsync(new UserId(userId));

        // Build dashboard overview
        var dashboard = new DashboardOverviewDto
        {
            OverallProgress = (decimal)statistics.OverallProgress,
            CurrentModuleId = currentModule.Id.Value,
            CurrentModuleName = currentModule.Name,
            ModuleType = currentModule.Type == Core.ValueObjects.ModuleType.Basic ? DashboardModuleType.Basic : DashboardModuleType.Specialistic,
            Specialization = new SpecializationInfoDto
            {
                Id = specialization.Id.Value,
                Name = specialization.Name,
                ProgramCode = specialization.ProgramCode,
                StartDate = specialization.StartDate,
                PlannedEndDate = specialization.PlannedEndDate,
                DurationYears = specialization.DurationYears,
                SmkVersion = specialization.SmkVersion.Value
            },
            ModuleProgress = new ModuleProgressDto
            {
                Internships = new DashboardInternshipProgressDto
                {
                    Completed = statistics.InternshipProgress.Completed,
                    Required = statistics.InternshipProgress.Total,
                    CompletedDays = 0, // TODO: Calculate from internships
                    RequiredDays = 0, // TODO: Get from template
                    ProgressPercentage = (decimal)statistics.InternshipProgress.PercentageComplete
                },
                Courses = new DashboardCourseProgressDto
                {
                    Completed = statistics.CourseProgress.Completed,
                    Required = statistics.CourseProgress.Total,
                    ProgressPercentage = (decimal)statistics.CourseProgress.PercentageComplete
                },
                Procedures = new DashboardProcedureProgressDto
                {
                    CompletedTypeA = statistics.ProcedureProgress.CompletedTypeA,
                    RequiredTypeA = statistics.ProcedureProgress.TotalTypeA,
                    CompletedTypeB = statistics.ProcedureProgress.CompletedTypeB,
                    RequiredTypeB = statistics.ProcedureProgress.TotalTypeB,
                    ProgressPercentage = (decimal)statistics.ProcedureProgress.PercentageComplete
                },
                MedicalShifts = new DashboardMedicalShiftProgressDto
                {
                    CompletedHours = statistics.MedicalShiftProgress.CompletedHours,
                    RequiredHours = statistics.MedicalShiftProgress.RequiredHours,
                    ProgressPercentage = (decimal)statistics.MedicalShiftProgress.PercentageComplete
                }
            },
            SelfEducationCount = selfEducation.Count(s => s.IsCompleted),
            PublicationsCount = publications.Count()
        };

        return dashboard;
    }
}
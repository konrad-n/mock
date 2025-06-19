using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

public sealed class GetDashboardOverviewHandler : IQueryHandler<GetDashboardOverview, DashboardOverviewDto>
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

        // Get user's specializations
        var specializations = await _specializationRepository.GetByUserIdAsync(new UserId(userId));
        if (!specializations.Any())
        {
            // Return empty dashboard if no specialization
            return new DashboardOverviewDto
            {
                OverallProgress = 0,
                CurrentModuleId = 0,
                CurrentModuleName = "No module",
                ModuleType = DashboardModuleType.Basic,
                Specialization = new SpecializationInfoDto
                {
                    Id = 0,
                    Name = "No specialization selected",
                    ProgramCode = "N/A",
                    StartDate = DateTime.UtcNow,
                    PlannedEndDate = DateTime.UtcNow.AddYears(5),
                    DurationYears = 5,
                    SmkVersion = "old"
                },
                ModuleProgress = new ModuleProgressDto(),
                SelfEducationCount = 0,
                PublicationsCount = 0
            };
        }

        // Use first specialization for now
        var specialization = specializations.First();

        // Get current module
        var currentModule = specialization.CurrentModuleId != null 
            ? await _moduleRepository.GetByIdAsync(specialization.CurrentModuleId)
            : null;

        // Calculate progress using the service
        var progress = await _progressCalculationService.CalculateProgressAsync(userId);

        // Get publications and self-education data
        var publications = await _publicationRepository.GetByUserIdAsync(new UserId(userId));
        var selfEducationRecords = await _selfEducationRepository.GetByUserIdAsync(new UserId(userId));

        return new DashboardOverviewDto
        {
            OverallProgress = (decimal)progress.CompletionPercentage,
            CurrentModuleId = currentModule?.Id.Value ?? 0,
            CurrentModuleName = currentModule?.Name ?? "Module not started",
            ModuleType = currentModule?.ModuleType == Core.Enums.ModuleType.Basic 
                ? DashboardModuleType.Basic 
                : DashboardModuleType.Specialistic,
            Specialization = new SpecializationInfoDto
            {
                Id = specialization.Id.Value,
                Name = specialization.Name,
                ProgramCode = specialization.ProgramCode,
                StartDate = DateTime.UtcNow, // Would need to track this
                PlannedEndDate = DateTime.UtcNow.AddYears(5),
                DurationYears = 5,
                SmkVersion = specialization.SmkVersion.Value
            },
            ModuleProgress = new ModuleProgressDto
            {
                Internships = new DashboardInternshipProgressDto
                {
                    Completed = progress.InternshipsCompleted,
                    Required = progress.InternshipsRequired,
                    Progress = progress.InternshipsRequired > 0 
                        ? (decimal)progress.InternshipsCompleted / progress.InternshipsRequired * 100 
                        : 0
                },
                Courses = new DashboardCourseProgressDto
                {
                    Completed = progress.CoursesCompleted,
                    Required = progress.CoursesRequired,
                    Progress = progress.CoursesRequired > 0 
                        ? (decimal)progress.CoursesCompleted / progress.CoursesRequired * 100 
                        : 0
                },
                Procedures = new DashboardProcedureProgressDto
                {
                    Completed = progress.ProceduresCompleted,
                    Required = progress.ProceduresRequired,
                    Progress = progress.ProceduresRequired > 0 
                        ? (decimal)progress.ProceduresCompleted / progress.ProceduresRequired * 100 
                        : 0
                },
                MedicalShifts = new DashboardMedicalShiftProgressDto
                {
                    TotalHours = progress.TotalHoursCompleted,
                    RequiredHours = progress.TotalHoursRequired,
                    MonthlyHours = progress.MonthlyHoursCompleted,
                    Progress = progress.TotalHoursRequired > 0 
                        ? (decimal)progress.TotalHoursCompleted / progress.TotalHoursRequired * 100 
                        : 0
                }
            },
            SelfEducationCount = selfEducationRecords.Count(),
            PublicationsCount = publications.Count()
        };
    }
}
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

        // Calculate progress (simplified for now - use default values)
        // TODO: Implement actual progress calculation when IProgressCalculationService is available
        var overallProgress = 25.5m; // Placeholder

        // Get publications and self-education data
        var publications = await _publicationRepository.GetByUserIdAsync(new UserId(userId));
        var selfEducationRecords = await _selfEducationRepository.GetByUserIdAsync(new UserId(userId));

        return new DashboardOverviewDto
        {
            OverallProgress = overallProgress,
            CurrentModuleId = currentModule?.Id.Value ?? 0,
            CurrentModuleName = currentModule?.Name ?? "Module not started",
            ModuleType = currentModule != null && currentModule.Name.Contains("podstawowy", StringComparison.OrdinalIgnoreCase)
                ? DashboardModuleType.Basic 
                : DashboardModuleType.Specialist,
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
                    Completed = 2, // Placeholder values
                    Required = 5,
                    CompletedDays = 60,
                    RequiredDays = 150,
                    ProgressPercentage = 40
                },
                Courses = new DashboardCourseProgressDto
                {
                    Completed = 3,
                    Required = 8,
                    ProgressPercentage = 37.5m
                },
                Procedures = new DashboardProcedureProgressDto
                {
                    CompletedTypeA = 15,
                    RequiredTypeA = 30,
                    CompletedTypeB = 8,
                    RequiredTypeB = 20,
                    ProgressPercentage = 46
                },
                MedicalShifts = new DashboardMedicalShiftProgressDto
                {
                    CompletedHours = 320,
                    RequiredHours = 800,
                    ProgressPercentage = 40
                }
            },
            SelfEducationCount = selfEducationRecords.Count(),
            PublicationsCount = publications.Count()
        };
    }
}
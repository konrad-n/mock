using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

public sealed class GetDashboardOverviewHandlerSimple : IQueryHandler<GetDashboardOverview, DashboardOverviewDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserContextService _userContextService;

    public GetDashboardOverviewHandlerSimple(
        IUserRepository userRepository,
        IUserContextService userContextService)
    {
        _userRepository = userRepository;
        _userContextService = userContextService;
    }

    public async Task<DashboardOverviewDto> HandleAsync(GetDashboardOverview query)
    {
        var userId = _userContextService.GetUserId();
        var user = await _userRepository.GetByIdAsync(new UserId(userId));
        
        // Return a simple dashboard with default data
        return new DashboardOverviewDto
        {
            OverallProgress = 25.5m,
            CurrentModuleId = 101,
            CurrentModuleName = "Moduł podstawowy",
            ModuleType = DashboardModuleType.Basic,
            Specialization = new SpecializationInfoDto
            {
                Id = 1,
                Name = "Kardiologia",
                ProgramCode = "KARD-2023",
                StartDate = DateTime.UtcNow.AddMonths(-6),
                PlannedEndDate = DateTime.UtcNow.AddYears(4).AddMonths(6),
                DurationYears = 5,
                SmkVersion = "old"
            },
            ModuleProgress = new ModuleProgressDto
            {
                Internships = new DashboardInternshipProgressDto
                {
                    Completed = 2,
                    Required = 8,
                    CompletedDays = 60,
                    RequiredDays = 240,
                    ProgressPercentage = 25.0m
                },
                Courses = new DashboardCourseProgressDto
                {
                    Completed = 3,
                    Required = 10,
                    ProgressPercentage = 30.0m
                },
                Procedures = new DashboardProcedureProgressDto
                {
                    CompletedTypeA = 25,
                    RequiredTypeA = 50,
                    CompletedTypeB = 20,
                    RequiredTypeB = 30,
                    ProgressPercentage = 56.25m
                },
                MedicalShifts = new DashboardMedicalShiftProgressDto
                {
                    CompletedHours = 800,
                    RequiredHours = 3200,
                    ProgressPercentage = 25.0m
                }
            },
            SelfEducationCount = 5,
            PublicationsCount = 2
        };
    }
}
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

        // TODO: User-Specialization relationship needs to be redesigned
        // var specialization = await _specializationRepository.GetByIdAsync(user.SpecializationId);
        // For now, return empty dashboard as we cannot access user's specialization
        throw new InvalidOperationException("User-Specialization relationship needs to be redesigned. Cannot retrieve dashboard data.");
    }
}
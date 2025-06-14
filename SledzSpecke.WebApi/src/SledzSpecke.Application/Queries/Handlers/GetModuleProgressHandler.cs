using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

public sealed class GetModuleProgressHandler : IQueryHandler<GetModuleProgress, SpecializationStatisticsDto>
{
    private readonly IProgressCalculationService _progressCalculationService;
    private readonly IUserContextService _userContextService;
    private readonly IUserRepository _userRepository;
    private readonly ISpecializationRepository _specializationRepository;

    public GetModuleProgressHandler(
        IProgressCalculationService progressCalculationService,
        IUserContextService userContextService,
        IUserRepository userRepository,
        ISpecializationRepository specializationRepository)
    {
        _progressCalculationService = progressCalculationService;
        _userContextService = userContextService;
        _userRepository = userRepository;
        _specializationRepository = specializationRepository;
    }

    public async Task<SpecializationStatisticsDto> HandleAsync(GetModuleProgress query)
    {
        // Verify user has access to this specialization
        var userId = _userContextService.GetUserId();
        var user = await _userRepository.GetByIdAsync(new UserId(userId));
        
        if (user == null || user.SpecializationId.Value != query.SpecializationId)
        {
            throw new UnauthorizedAccessException("You can only view progress for your own specialization");
        }

        // Calculate and return statistics
        return await _progressCalculationService.CalculateFullStatisticsAsync(query.SpecializationId, query.ModuleId);
    }
}
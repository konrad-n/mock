using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

internal sealed class GetTotalReductionDaysHandler : IQueryHandler<GetTotalReductionDays, int>
{
    private readonly IRecognitionRepository _recognitionRepository;
    private readonly IUserContextService _userContextService;

    public GetTotalReductionDaysHandler(
        IRecognitionRepository recognitionRepository,
        IUserContextService userContextService)
    {
        _recognitionRepository = recognitionRepository;
        _userContextService = userContextService;
    }

    public async Task<int> HandleAsync(GetTotalReductionDays query)
    {
        var currentUserId = _userContextService.GetUserId();
        if (query.UserId != (int)currentUserId)
        {
            throw new UnauthorizedAccessException("You can only view your own recognition data.");
        }

        var recognitions = await _recognitionRepository.GetByUserIdAsync(new UserId(query.UserId));
        
        // Filter by specialization and only count approved recognitions
        var totalReductionDays = recognitions
            .Where(r => r.SpecializationId.Value == query.SpecializationId && r.IsApproved)
            .Sum(r => r.DaysReduction);

        return totalReductionDays;
    }
}
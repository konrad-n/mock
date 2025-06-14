using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

internal sealed class GetTotalCreditHoursHandler : IQueryHandler<GetTotalCreditHours, int>
{
    private readonly ISelfEducationRepository _selfEducationRepository;
    private readonly IUserContextService _userContextService;

    public GetTotalCreditHoursHandler(
        ISelfEducationRepository selfEducationRepository,
        IUserContextService userContextService)
    {
        _selfEducationRepository = selfEducationRepository;
        _userContextService = userContextService;
    }

    public async Task<int> HandleAsync(GetTotalCreditHours query)
    {
        var currentUserId = _userContextService.GetUserId();
        if (query.UserId != (int)currentUserId)
        {
            throw new UnauthorizedAccessException("You can only view your own self-education data.");
        }

        var activities = await _selfEducationRepository.GetByUserIdAsync(new UserId(query.UserId));
        
        // Only count completed activities for the specialization
        var totalCreditHours = activities
            .Where(a => a.SpecializationId.Value == query.SpecializationId && a.IsCompleted)
            .Sum(a => a.CreditHours);

        return totalCreditHours;
    }
}
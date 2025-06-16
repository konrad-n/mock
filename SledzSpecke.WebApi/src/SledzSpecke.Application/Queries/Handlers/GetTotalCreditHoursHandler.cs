using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

public sealed class GetTotalCreditHoursHandler : IQueryHandler<GetTotalCreditHours, int>
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
        
        // In the new model, all recorded activities are considered complete
        // Convert hours to credit hours (1:1 ratio)
        var totalCreditHours = activities
            .Sum(a => a.Hours);

        return totalCreditHours;
    }
}
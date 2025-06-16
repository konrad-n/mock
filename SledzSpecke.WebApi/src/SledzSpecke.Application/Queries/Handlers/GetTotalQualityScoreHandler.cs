using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

public sealed class GetTotalQualityScoreHandler : IQueryHandler<GetTotalQualityScore, decimal>
{
    private readonly ISelfEducationRepository _selfEducationRepository;
    private readonly IUserContextService _userContextService;

    public GetTotalQualityScoreHandler(
        ISelfEducationRepository selfEducationRepository,
        IUserContextService userContextService)
    {
        _selfEducationRepository = selfEducationRepository;
        _userContextService = userContextService;
    }

    public async Task<decimal> HandleAsync(GetTotalQualityScore query)
    {
        var currentUserId = _userContextService.GetUserId();
        if (query.UserId != (int)currentUserId)
        {
            throw new UnauthorizedAccessException("You can only view your own self-education data.");
        }

        var activities = await _selfEducationRepository.GetByUserIdAsync(new UserId(query.UserId));
        
        // In the new model, use GetEducationPoints() method
        decimal totalQualityScore = 0;

        foreach (var activity in activities)
        {
            // Get education points from the activity
            var points = activity.GetEducationPoints();
            
            // Convert to decimal for quality score
            totalQualityScore += points;
        }

        return totalQualityScore;
    }
}
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
        
        var specializationActivities = activities
            .Where(a => a.SpecializationId.Value == query.SpecializationId && a.IsCompleted);

        decimal totalQualityScore = 0;

        foreach (var activity in specializationActivities)
        {
            // Calculate quality score based on type and credit hours
            decimal baseScore = activity.QualityScore ?? 0;
            
            // Add bonus for accredited providers
            if (!string.IsNullOrEmpty(activity.Provider) && activity.Provider.Contains("Accredited"))
            {
                baseScore *= 1.2m;
            }
            
            // Weight by credit hours
            decimal weightedScore = baseScore * (activity.CreditHours / 10m);
            
            totalQualityScore += weightedScore;
        }

        return totalQualityScore;
    }
}
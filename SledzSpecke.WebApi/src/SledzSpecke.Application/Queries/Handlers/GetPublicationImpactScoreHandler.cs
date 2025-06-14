using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

internal sealed class GetPublicationImpactScoreHandler : IQueryHandler<GetPublicationImpactScore, decimal>
{
    private readonly IPublicationRepository _publicationRepository;
    private readonly IUserContextService _userContextService;

    public GetPublicationImpactScoreHandler(
        IPublicationRepository publicationRepository,
        IUserContextService userContextService)
    {
        _publicationRepository = publicationRepository;
        _userContextService = userContextService;
    }

    public async Task<decimal> HandleAsync(GetPublicationImpactScore query)
    {
        var currentUserId = _userContextService.GetUserId();
        if (query.UserId != (int)currentUserId)
        {
            throw new UnauthorizedAccessException("You can only view your own publication scores.");
        }

        var publications = await _publicationRepository.GetByUserIdAsync(new UserId(query.UserId));
        
        var specializationPublications = publications
            .Where(p => p.SpecializationId.Value == query.SpecializationId);

        decimal totalImpactScore = 0;

        foreach (var publication in specializationPublications)
        {
            decimal score = publication.ImpactFactor ?? 0;
            
            // Bonus points for being first author
            if (publication.IsFirstAuthor)
            {
                score *= 1.5m;
            }
            
            // Bonus points for peer-reviewed publications
            if (publication.IsPeerReviewed)
            {
                score *= 1.2m;
            }
            
            totalImpactScore += score;
        }

        return totalImpactScore;
    }
}
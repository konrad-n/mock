using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

internal sealed class GetFirstAuthorPublicationsHandler : IQueryHandler<GetFirstAuthorPublications, IEnumerable<PublicationDto>>
{
    private readonly IPublicationRepository _publicationRepository;
    private readonly IUserContextService _userContextService;

    public GetFirstAuthorPublicationsHandler(
        IPublicationRepository publicationRepository,
        IUserContextService userContextService)
    {
        _publicationRepository = publicationRepository;
        _userContextService = userContextService;
    }

    public async Task<IEnumerable<PublicationDto>> HandleAsync(GetFirstAuthorPublications query)
    {
        var currentUserId = _userContextService.GetUserId();
        if (query.UserId != (int)currentUserId)
        {
            throw new UnauthorizedAccessException("You can only view your own publications.");
        }

        var publications = await _publicationRepository.GetByUserIdAsync(new UserId(query.UserId));
        
        return publications
            .Where(p => p.IsFirstAuthor)
            .Select(p => new PublicationDto
            {
                Id = p.Id.Value,
                SpecializationId = p.SpecializationId.Value,
                UserId = p.UserId.Value,
                Type = p.Type.ToString(),
                Title = p.Title,
                PublicationDate = p.PublicationDate,
                Authors = p.Authors,
                Journal = p.Journal,
                Publisher = p.Publisher,
                Abstract = p.Abstract,
                Keywords = p.Keywords,
                IsFirstAuthor = p.IsFirstAuthor,
                IsCorrespondingAuthor = p.IsCorrespondingAuthor,
                IsPeerReviewed = p.IsPeerReviewed,
                ImpactFactor = p.ImpactFactor,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .OrderByDescending(p => p.PublicationDate)
            .ToList();
    }
}
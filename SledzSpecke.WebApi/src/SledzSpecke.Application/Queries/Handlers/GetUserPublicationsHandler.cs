using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

public sealed class GetUserPublicationsHandler : IQueryHandler<GetUserPublications, IEnumerable<PublicationDto>>
{
    private readonly IPublicationRepository _publicationRepository;

    public GetUserPublicationsHandler(IPublicationRepository publicationRepository)
    {
        _publicationRepository = publicationRepository;
    }

    public async Task<IEnumerable<PublicationDto>> HandleAsync(GetUserPublications query)
    {
        var userId = new UserId(query.UserId);
        var publications = await _publicationRepository.GetByUserIdAsync(userId);

        if (query.SpecializationId.HasValue)
        {
            publications = publications.Where(p => p.SpecializationId.Value == query.SpecializationId.Value);
        }

        return publications
            .OrderByDescending(p => p.PublicationDate)
            .Select(p => new PublicationDto
            {
                Id = p.Id.Value,
                SpecializationId = p.SpecializationId.Value,
                UserId = p.UserId.Value,
                Type = p.Type.ToString(),
                Title = p.Title,
                Authors = p.Authors,
                Journal = p.Journal,
                Publisher = p.Publisher,
                PublicationDate = p.PublicationDate,
                Volume = p.Volume,
                Issue = p.Issue,
                Pages = p.Pages,
                DOI = p.DOI,
                PMID = p.PMID,
                ISBN = p.ISBN,
                URL = p.URL,
                Abstract = p.Abstract,
                Keywords = p.Keywords,
                FilePath = p.FilePath,
                IsFirstAuthor = p.IsFirstAuthor,
                IsCorrespondingAuthor = p.IsCorrespondingAuthor,
                IsPeerReviewed = p.IsPeerReviewed,
                ImpactScore = p.CalculateImpactScore(),
                SyncStatus = p.SyncStatus.ToString(),
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            });
    }
}
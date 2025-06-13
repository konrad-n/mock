using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Repositories;

public interface IPublicationRepository
{
    Task<Publication?> GetByIdAsync(PublicationId id);
    Task<IEnumerable<Publication>> GetByUserIdAsync(UserId userId);
    Task<IEnumerable<Publication>> GetBySpecializationIdAsync(SpecializationId specializationId);
    Task<IEnumerable<Publication>> GetByUserAndSpecializationAsync(UserId userId, SpecializationId specializationId);
    Task<IEnumerable<Publication>> GetByTypeAsync(PublicationType type);
    Task<IEnumerable<Publication>> GetPeerReviewedPublicationsAsync(UserId userId);
    Task<IEnumerable<Publication>> GetFirstAuthorPublicationsAsync(UserId userId);
    Task<IEnumerable<Publication>> GetRecentPublicationsAsync(UserId userId, int years = 5);
    Task<int> GetTotalImpactScoreAsync(UserId userId, SpecializationId specializationId);
    Task<int> GetPublicationCountByTypeAsync(UserId userId, PublicationType type);
    Task AddAsync(Publication publication);
    Task UpdateAsync(Publication publication);
    Task DeleteAsync(PublicationId id);
}
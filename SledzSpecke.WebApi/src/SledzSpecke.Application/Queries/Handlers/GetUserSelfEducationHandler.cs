using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

public sealed class GetUserSelfEducationHandler : IQueryHandler<GetUserSelfEducation, IEnumerable<SelfEducationDto>>
{
    private readonly ISelfEducationRepository _selfEducationRepository;

    public GetUserSelfEducationHandler(ISelfEducationRepository selfEducationRepository)
    {
        _selfEducationRepository = selfEducationRepository;
    }

    public async Task<IEnumerable<SelfEducationDto>> HandleAsync(GetUserSelfEducation query)
    {
        var userId = new UserId(query.UserId);
        var selfEducations = await _selfEducationRepository.GetByUserIdAsync(userId);

        if (query.SpecializationId.HasValue)
        {
            selfEducations = selfEducations.Where(s => s.SpecializationId.Value == query.SpecializationId.Value);
        }

        return selfEducations
            .OrderByDescending(s => s.Year)
            .ThenByDescending(s => s.StartDate)
            .Select(s => new SelfEducationDto
            {
                Id = s.Id.Value,
                SpecializationId = s.SpecializationId.Value,
                UserId = s.UserId.Value,
                Type = s.Type.ToString(),
                Year = s.Year,
                Title = s.Title,
                Description = s.Description,
                Provider = s.Provider,
                Publisher = s.Publisher,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                DurationHours = s.DurationHours,
                IsCompleted = s.IsCompleted,
                CompletedAt = s.CompletedAt,
                CertificatePath = s.CertificatePath,
                URL = s.URL,
                ISBN = s.ISBN,
                DOI = s.DOI,
                CreditHours = s.CreditHours,
                QualityScore = s.CalculateQualityScore(),
                SyncStatus = s.SyncStatus.ToString(),
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            });
    }
}
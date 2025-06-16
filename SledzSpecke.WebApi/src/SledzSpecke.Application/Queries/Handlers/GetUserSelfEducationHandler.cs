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

        // Note: Can't filter by SpecializationId in new model (it's linked through Module)
        // Skip filtering for now
        
        return selfEducations
            .OrderByDescending(s => s.Date)
            .Select(s => new SelfEducationDto
            {
                Id = s.Id.Value,
                SpecializationId = query.SpecializationId ?? 0,
                UserId = query.UserId,
                Type = s.Type.ToString(),
                Year = s.Date.Year,
                Title = s.Description,
                Description = s.Description,
                Provider = null,
                Publisher = null,
                StartDate = s.Date,
                EndDate = s.Date,
                DurationHours = s.Hours,
                IsCompleted = true,
                CompletedAt = s.Date,
                CertificatePath = null,
                URL = null,
                ISBN = null,
                DOI = null,
                CreditHours = s.Hours,
                QualityScore = s.GetEducationPoints(),
                SyncStatus = s.SyncStatus.ToString(),
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            });
    }
}
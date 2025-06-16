using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

public sealed class GetCompletedSelfEducationHandler : IQueryHandler<GetCompletedSelfEducation, IEnumerable<SelfEducationDto>>
{
    private readonly ISelfEducationRepository _selfEducationRepository;
    private readonly IUserContextService _userContextService;

    public GetCompletedSelfEducationHandler(
        ISelfEducationRepository selfEducationRepository,
        IUserContextService userContextService)
    {
        _selfEducationRepository = selfEducationRepository;
        _userContextService = userContextService;
    }

    public async Task<IEnumerable<SelfEducationDto>> HandleAsync(GetCompletedSelfEducation query)
    {
        var currentUserId = _userContextService.GetUserId();
        if (query.UserId != (int)currentUserId)
        {
            throw new UnauthorizedAccessException("You can only view your own self-education activities.");
        }

        var activities = await _selfEducationRepository.GetByUserIdAsync(new UserId(query.UserId));
        
        // In the new model, all recorded activities are considered complete
        return activities
            .Select(a => new SelfEducationDto
            {
                Id = a.Id.Value,
                SpecializationId = query.SpecializationId, // Use from query
                UserId = query.UserId, // Use from query
                Type = a.Type.ToString(),
                Year = a.Date.Year,
                Title = a.Description, // Use description as title
                Description = a.Description,
                Provider = null, // Not available in new model
                StartDate = a.Date,
                EndDate = a.Date,
                CreditHours = a.Hours,
                DurationHours = a.Hours,
                IsCompleted = true, // All recorded activities are complete
                CompletedAt = a.Date,
                CertificatePath = null, // Not available in new model
                QualityScore = (double)a.GetEducationPoints(),
                SyncStatus = a.SyncStatus.ToString(),
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            })
            .OrderByDescending(a => a.CompletedAt)
            .ToList();
    }
}
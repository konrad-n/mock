using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

public sealed class GetSelfEducationByYearHandler : IQueryHandler<GetSelfEducationByYear, IEnumerable<SelfEducationDto>>
{
    private readonly ISelfEducationRepository _selfEducationRepository;
    private readonly IUserContextService _userContextService;

    public GetSelfEducationByYearHandler(
        ISelfEducationRepository selfEducationRepository,
        IUserContextService userContextService)
    {
        _selfEducationRepository = selfEducationRepository;
        _userContextService = userContextService;
    }

    public async Task<IEnumerable<SelfEducationDto>> HandleAsync(GetSelfEducationByYear query)
    {
        var currentUserId = _userContextService.GetUserId();
        if (query.UserId != (int)currentUserId)
        {
            throw new UnauthorizedAccessException("You can only view your own self-education activities.");
        }

        var activities = await _selfEducationRepository.GetByUserIdAsync(new UserId(query.UserId));
        
        // Filter by year based on the Date field
        return activities
            .Where(a => a.Date.Year == query.Year)
            .Select(a => new SelfEducationDto
            {
                Id = a.Id.Value,
                SpecializationId = 0, // Not available in new model
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
            .OrderByDescending(a => a.StartDate)
            .ToList();
    }
}
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

internal sealed class GetSelfEducationByYearHandler : IQueryHandler<GetSelfEducationByYear, IEnumerable<SelfEducationDto>>
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
        
        return activities
            .Where(a => a.Year == query.Year)
            .Select(a => new SelfEducationDto
            {
                Id = a.Id.Value,
                SpecializationId = a.SpecializationId.Value,
                UserId = a.UserId.Value,
                Type = a.Type.ToString(),
                Year = a.Year,
                Title = a.Title,
                Description = a.Description,
                Provider = a.Provider,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                CreditHours = a.CreditHours,
                DurationHours = a.DurationHours,
                IsCompleted = a.IsCompleted,
                CompletedAt = a.CompletedAt,
                CertificatePath = a.CertificatePath,
                QualityScore = (double)(a.QualityScore ?? 0),
                SyncStatus = a.SyncStatus.ToString(),
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            })
            .OrderByDescending(a => a.StartDate)
            .ToList();
    }
}
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

public sealed class GetEducationalActivitiesHandler : IQueryHandler<GetEducationalActivities, IEnumerable<EducationalActivityDto>>
{
    private readonly IEducationalActivityRepository _repository;

    public GetEducationalActivitiesHandler(IEducationalActivityRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<EducationalActivityDto>> HandleAsync(GetEducationalActivities query)
    {
        var activities = await _repository.GetBySpecializationIdAsync(new SpecializationId(query.SpecializationId));

        return activities.Select(a => new EducationalActivityDto
        {
            Id = a.Id.Value,
            SpecializationId = a.SpecializationId.Value,
            ModuleId = a.ModuleId?.Value,
            Type = a.Type.ToString(),
            Title = a.Title.Value,
            Description = a.Description?.Value,
            StartDate = a.StartDate,
            EndDate = a.EndDate,
            SyncStatus = a.SyncStatus.ToString(),
            IsOngoing = a.IsOngoing(),
            IsCompleted = a.IsCompleted(),
            IsUpcoming = a.IsUpcoming(),
            DurationDays = a.GetDuration().TotalDays
        }).OrderByDescending(a => a.StartDate);
    }
}
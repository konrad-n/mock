using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

internal sealed class GetEducationalActivityByIdHandler : IQueryHandler<GetEducationalActivityById, EducationalActivityDto?>
{
    private readonly IEducationalActivityRepository _repository;

    public GetEducationalActivityByIdHandler(IEducationalActivityRepository repository)
    {
        _repository = repository;
    }

    public async Task<EducationalActivityDto?> HandleAsync(GetEducationalActivityById query)
    {
        var activity = await _repository.GetByIdAsync(new EducationalActivityId(query.Id));
        
        if (activity is null)
            return null;

        return new EducationalActivityDto
        {
            Id = activity.Id.Value,
            SpecializationId = activity.SpecializationId.Value,
            ModuleId = activity.ModuleId?.Value,
            Type = activity.Type.ToString(),
            Title = activity.Title.Value,
            Description = activity.Description?.Value,
            StartDate = activity.StartDate,
            EndDate = activity.EndDate,
            SyncStatus = activity.SyncStatus.ToString(),
            IsOngoing = activity.IsOngoing(),
            IsCompleted = activity.IsCompleted(),
            IsUpcoming = activity.IsUpcoming(),
            DurationDays = activity.GetDuration().TotalDays
        };
    }
}
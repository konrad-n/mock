using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers.AdditionalSelfEducationDays;

public sealed class GetAdditionalSelfEducationDaysByModuleHandler : IQueryHandler<GetAdditionalSelfEducationDaysByModule, IEnumerable<AdditionalSelfEducationDaysDto>>
{
    private readonly IAdditionalSelfEducationDaysRepository _repository;

    public GetAdditionalSelfEducationDaysByModuleHandler(IAdditionalSelfEducationDaysRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AdditionalSelfEducationDaysDto>> HandleAsync(GetAdditionalSelfEducationDaysByModule query)
    {
        var days = await _repository.GetByModuleIdAsync(query.ModuleId);
        
        return days.Select(d => new AdditionalSelfEducationDaysDto(
            Id: d.Id,
            SpecializationId: 0, // Not available at this level, would need to query module
            Year: d.StartDate.Year,
            DaysUsed: d.NumberOfDays,
            Comment: d.Purpose
        ));
    }
}
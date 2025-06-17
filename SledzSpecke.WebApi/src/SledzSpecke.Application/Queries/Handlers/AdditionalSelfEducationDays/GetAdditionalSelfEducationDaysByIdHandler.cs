using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Queries;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Application.Queries.Handlers.AdditionalSelfEducationDays;

public sealed class GetAdditionalSelfEducationDaysByIdHandler : IQueryHandler<GetAdditionalSelfEducationDaysById, AdditionalSelfEducationDaysDto>
{
    private readonly IAdditionalSelfEducationDaysRepository _repository;

    public GetAdditionalSelfEducationDaysByIdHandler(IAdditionalSelfEducationDaysRepository repository)
    {
        _repository = repository;
    }

    public async Task<AdditionalSelfEducationDaysDto> HandleAsync(GetAdditionalSelfEducationDaysById query)
    {
        var day = await _repository.GetByIdAsync(query.Id);
        
        if (day == null)
        {
            return null;
        }

        return new AdditionalSelfEducationDaysDto(
            Id: day.Id,
            SpecializationId: 0, // Not available at this level, would need to query module
            Year: day.StartDate.Year,
            DaysUsed: day.NumberOfDays,
            Comment: day.Purpose
        );
    }
}
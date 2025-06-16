using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public record GetAdditionalSelfEducationDaysBySpecialization(int SpecializationId) 
    : IQuery<IEnumerable<AdditionalSelfEducationDaysDto>>;
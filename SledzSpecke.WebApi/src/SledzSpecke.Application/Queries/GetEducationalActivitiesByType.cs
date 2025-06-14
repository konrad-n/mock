using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public sealed record GetEducationalActivitiesByType(
    int SpecializationId,
    string Type) : IQuery<IEnumerable<EducationalActivityDto>>;
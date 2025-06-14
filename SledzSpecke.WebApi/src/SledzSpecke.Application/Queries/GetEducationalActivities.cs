using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public sealed record GetEducationalActivities(int SpecializationId) : IQuery<IEnumerable<EducationalActivityDto>>;
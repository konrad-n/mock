using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public record GetCompletedSelfEducation(int UserId, int SpecializationId) : IQuery<IEnumerable<SelfEducationDto>>;
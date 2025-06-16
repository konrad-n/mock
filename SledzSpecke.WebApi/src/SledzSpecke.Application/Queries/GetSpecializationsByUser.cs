using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public record GetSpecializationsByUser(int UserId) : IQuery<IEnumerable<SpecializationSmkDto>>;
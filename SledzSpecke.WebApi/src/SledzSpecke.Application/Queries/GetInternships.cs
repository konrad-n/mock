using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public record GetInternships(int SpecializationId, int? ModuleId = null) : IQuery<IEnumerable<InternshipDto>>;
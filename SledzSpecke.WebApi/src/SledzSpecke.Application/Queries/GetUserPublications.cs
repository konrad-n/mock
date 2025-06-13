using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public record GetUserPublications(int UserId, int? SpecializationId = null) : IQuery<IEnumerable<PublicationDto>>;
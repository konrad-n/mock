using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public record GetFirstAuthorPublications(int UserId) : IQuery<IEnumerable<PublicationDto>>;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public sealed record GetEntityFiles : IQuery<IEnumerable<FileMetadataDto>>
{
    public string EntityType { get; init; } = null!;
    public int EntityId { get; init; }
}
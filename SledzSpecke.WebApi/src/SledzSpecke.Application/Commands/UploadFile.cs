using Microsoft.AspNetCore.Http;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Commands;

public sealed record UploadFile : ICommand<FileMetadataDto>
{
    public IFormFile File { get; init; } = null!;
    public string EntityType { get; init; } = null!;
    public int EntityId { get; init; }
    public string? Description { get; init; }
}
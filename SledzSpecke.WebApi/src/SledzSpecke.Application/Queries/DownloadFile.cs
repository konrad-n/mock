using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public sealed record DownloadFile : IQuery<FileDownloadResult>
{
    public int FileId { get; init; }
}
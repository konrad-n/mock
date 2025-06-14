using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public sealed record DeleteFile : ICommand
{
    public int FileId { get; init; }
}
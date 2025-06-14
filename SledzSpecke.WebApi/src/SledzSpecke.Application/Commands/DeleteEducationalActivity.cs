using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public sealed record DeleteEducationalActivity(int Id) : ICommand;
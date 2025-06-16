using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public record CompleteModule(int ModuleId) : ICommand;
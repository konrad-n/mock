using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public record SwitchModule(int SpecializationId, int ModuleId) : ICommand;
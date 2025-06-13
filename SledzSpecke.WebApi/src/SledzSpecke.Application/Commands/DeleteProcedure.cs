using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public record DeleteProcedure(int ProcedureId) : ICommand;
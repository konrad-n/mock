using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands;

public sealed record DeleteProcedureRealizationCommand(
    ProcedureRealizationId Id
) : ICommand;
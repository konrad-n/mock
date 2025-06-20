using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands;

public sealed record UpdateProcedureRealizationCommand(
    ProcedureRealizationId Id,
    DateTime Date,
    string Location,
    ProcedureRole Role
) : ICommand;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands;

public sealed record AddProcedureRealizationCommand(
    ProcedureRequirementId RequirementId,
    UserId UserId,
    DateTime Date,
    string Location,
    ProcedureRole Role,
    int? Year = null
) : ICommand;
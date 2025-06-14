using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands;

public record UpdateAbsence(
    AbsenceId AbsenceId,
    AbsenceType Type,
    DateTime StartDate,
    DateTime EndDate,
    string? Description) : ICommand;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands;

public record CreateAbsence(
    SpecializationId SpecializationId,
    UserId UserId,
    AbsenceType Type,
    DateTime StartDate,
    DateTime EndDate,
    string? Description = null) : ICommand;
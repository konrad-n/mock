using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands;

public record CreateSpecialization(
    string Name,
    string ProgramCode,
    SmkVersion SmkVersion,
    DateTime StartDate,
    DateTime PlannedEndDate,
    string ProgramStructure,
    int DurationYears
) : ICommand<int>;
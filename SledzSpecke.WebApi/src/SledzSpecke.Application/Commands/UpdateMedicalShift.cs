using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public record UpdateMedicalShift(
    int ShiftId,
    DateTime? Date,
    int? Hours,
    int? Minutes,
    string? Location
) : ICommand;
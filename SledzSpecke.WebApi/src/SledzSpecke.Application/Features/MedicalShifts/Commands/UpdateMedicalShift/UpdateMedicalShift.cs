using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Features.MedicalShifts.Commands.UpdateMedicalShift;

public record UpdateMedicalShift(
    int Id,
    DateTime? Date,
    int? Hours,
    int? Minutes,
    string? Location
) : ICommand;
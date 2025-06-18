using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Features.MedicalShifts.Commands.DeleteMedicalShift;

public record DeleteMedicalShift(int Id) : ICommand;
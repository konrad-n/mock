using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public record DeleteMedicalShift(int ShiftId) : ICommand;
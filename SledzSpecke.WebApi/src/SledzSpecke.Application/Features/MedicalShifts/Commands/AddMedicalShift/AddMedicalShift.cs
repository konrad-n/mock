using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Features.MedicalShifts.Commands.AddMedicalShift;

public record AddMedicalShift(
    int InternshipId,
    DateTime Date,
    int Hours,
    int Minutes,
    string Location,
    int Year
) : ICommand<int>;
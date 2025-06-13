using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public record AddMedicalShift(
    int InternshipId,
    DateTime Date,
    int Hours,
    int Minutes,
    string Location,
    int Year
) : ICommand<int>;
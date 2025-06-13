using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public record AddProcedure(
    int InternshipId,
    DateTime Date,
    int Year,
    string Code,
    string Location,
    string Status,
    string? OperatorCode = null,
    string? PerformingPerson = null,
    string? PatientInitials = null,
    char? PatientGender = null
) : ICommand<int>;
using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public record UpdateProcedure(
    int ProcedureId,
    DateTime? Date = null,
    string? Code = null,
    string? Location = null,
    string? Status = null,
    string? OperatorCode = null,
    string? PerformingPerson = null,
    string? PatientInitials = null,
    char? PatientGender = null
) : ICommand;
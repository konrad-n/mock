using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public record UpdateProcedure(
    int ProcedureId,
    DateTime? Date = null,
    string? Code = null,
    string? Location = null,
    string? Status = null,
    string? ExecutionType = null,
    string? PerformingPerson = null,
    string? PatientInfo = null,
    string? PatientInitials = null,
    char? PatientGender = null,
    // Old SMK specific fields
    int? ProcedureRequirementId = null,
    string? ProcedureGroup = null,
    string? AssistantData = null,
    string? InternshipName = null,
    // New SMK specific fields
    int? CountA = null,
    int? CountB = null,
    string? Supervisor = null,
    string? Institution = null,
    string? Comments = null
) : ICommand;
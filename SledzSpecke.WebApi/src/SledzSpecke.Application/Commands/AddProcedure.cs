using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Commands;

public record AddProcedure(
    int InternshipId,
    DateTime Date,
    int Year,
    string Code,
    string Name,
    string Location,
    string Status,
    string ExecutionType,
    string SupervisorName,
    string? SupervisorPwz = null,
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
    int? ModuleId = null,
    string? ProcedureName = null,
    int? CountA = null,
    int? CountB = null,
    string? Supervisor = null,
    string? Institution = null,
    string? Comments = null
) : ICommand<int>;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.DTO;

public record ProcedureDto(
    int Id,
    int InternshipId,
    DateTime Date,
    int Year,
    string Code,
    string? OperatorCode,
    string? PerformingPerson,
    string Location,
    string? PatientInitials,
    char? PatientGender,
    string? AssistantData,
    string? ProcedureGroup,
    string Status,
    SyncStatus SyncStatus,
    string? AdditionalFields,
    bool IsCompleted,
    bool CanBeModified
);
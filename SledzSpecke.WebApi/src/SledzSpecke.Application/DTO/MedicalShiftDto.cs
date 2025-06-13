using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.DTO;

public record MedicalShiftDto(
    int Id,
    int InternshipId,
    DateTime Date,
    int Hours,
    int Minutes,
    string Location,
    int Year,
    SyncStatus SyncStatus,
    string? AdditionalFields,
    DateTime? ApprovalDate,
    string? ApproverName,
    string? ApproverRole,
    bool IsApproved,
    bool CanBeDeleted,
    TimeSpan Duration
);
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.DTO;

public record ModuleDto(
    int Id,
    int SpecializationId,
    ModuleType Type,
    SmkVersion SmkVersion,
    string Version,
    string Name,
    DateTime StartDate,
    DateTime EndDate,
    string Structure,
    int CompletedInternships,
    int TotalInternships,
    int CompletedCourses,
    int TotalCourses,
    int CompletedProceduresA,
    int TotalProceduresA,
    int CompletedProceduresB,
    int TotalProceduresB,
    int CompletedShiftHours,
    int RequiredShiftHours,
    double WeeklyShiftHours,
    int CompletedSelfEducationDays,
    int TotalSelfEducationDays,
    bool IsCompleted,
    double OverallProgress
);
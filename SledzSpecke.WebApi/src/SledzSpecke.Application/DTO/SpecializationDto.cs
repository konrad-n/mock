using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.DTO;

public record SpecializationDto(
    int Id,
    string Name,
    string ProgramCode,
    SmkVersion SmkVersion,
    DateTime StartDate,
    DateTime PlannedEndDate,
    DateTime CalculatedEndDate,
    string ProgramStructure,
    int? CurrentModuleId,
    int DurationYears,
    int CompletedInternships,
    int TotalInternships,
    int CompletedCourses,
    int TotalCourses,
    List<ModuleDto> Modules
);
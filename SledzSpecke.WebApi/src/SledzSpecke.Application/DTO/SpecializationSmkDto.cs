using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.DTO;

public record SpecializationSmkDto(
    int Id,
    int UserId,
    string Name,
    string ProgramCode,
    string SmkVersion,
    string ProgramVariant,
    DateTime StartDate,
    DateTime PlannedEndDate,
    DateTime? ActualEndDate,
    int PlannedPesYear,
    string Status,
    string ProgramStructure,
    int? CurrentModuleId,
    int DurationYears,
    List<ModuleDto> Modules
);
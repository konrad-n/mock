using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public record GetModuleRequirements(int ModuleId) : IQuery<ModuleRequirementsDto>;

public record ModuleRequirementsDto(
    int ModuleId,
    string ModuleName,
    int RequiredWeeks,
    int RequiredProcedures,
    List<ProcedureRequirementDto> ProcedureRequirements,
    List<string> AvailableLocations);

public record ProcedureRequirementDto(
    string Code,
    string Name,
    int RequiredCount,
    string Category);
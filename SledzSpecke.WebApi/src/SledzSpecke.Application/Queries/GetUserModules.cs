using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public record GetUserModules(int UserId, bool IncludeCompleted = true) : IQuery<IEnumerable<ModuleWithProgressDto>>;

public record ModuleWithProgressDto(
    int ModuleId,
    string ModuleName,
    string ModuleType,
    int OrderNumber,
    int RequiredDurationWeeks,
    ModuleProgressDto Progress);
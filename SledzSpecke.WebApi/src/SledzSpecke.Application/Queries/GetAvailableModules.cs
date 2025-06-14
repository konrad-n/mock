using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public record GetAvailableModules(int SpecializationId) : IQuery<IEnumerable<ModuleListDto>>;
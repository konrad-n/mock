using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public record GetModuleProgress(int SpecializationId, int? ModuleId = null) : IQuery<SpecializationStatisticsDto>;
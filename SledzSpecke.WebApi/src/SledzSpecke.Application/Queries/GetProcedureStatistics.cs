using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public record GetProcedureStatistics(int? ModuleId = null, int? ProcedureRequirementId = null) 
    : IQuery<ProcedureSummaryDto>;
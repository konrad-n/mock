using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Queries;

public record GetProcedureById(int ProcedureId) : IQuery<ProcedureDto>;
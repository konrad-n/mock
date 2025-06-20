using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries;

public sealed record GetModuleProceduresQuery(
    UserId UserId,
    ModuleId ModuleId
) : IQuery<ModuleProceduresDto>;
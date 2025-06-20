using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries;

public sealed record GetUserProceduresQuery(
    UserId UserId,
    int? SpecializationId = null
) : IQuery<UserProceduresDto>;
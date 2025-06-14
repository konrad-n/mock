using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Queries;

public record GetTotalReductionDays(int UserId, int SpecializationId) : IQuery<int>;
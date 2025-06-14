using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Queries;

public record GetTotalQualityScore(int UserId, int SpecializationId) : IQuery<decimal>;
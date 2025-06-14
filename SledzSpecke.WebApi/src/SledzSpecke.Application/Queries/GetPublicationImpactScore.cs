using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Queries;

public record GetPublicationImpactScore(int UserId, int SpecializationId) : IQuery<decimal>;
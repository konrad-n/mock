using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Queries;

public record GetTotalCreditHours(int UserId, int SpecializationId) : IQuery<int>;
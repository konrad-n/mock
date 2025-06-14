using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Application.Queries;

public record CalculateInternshipDays(DateTime StartDate, DateTime EndDate) : IQuery<int>;
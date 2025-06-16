using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Abstractions;

public interface IPdfGenerationService
{
    Task<string> GenerateMonthlyReportAsync(
        InternshipId internshipId,
        DateTime month,
        CancellationToken cancellationToken = default);
}
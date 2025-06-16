using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Abstractions;

public interface IMilestoneService
{
    Task<IEnumerable<Milestone>> CheckMilestonesAsync(
        InternshipId internshipId,
        string procedureCode,
        int procedureCount,
        CancellationToken cancellationToken = default);
}
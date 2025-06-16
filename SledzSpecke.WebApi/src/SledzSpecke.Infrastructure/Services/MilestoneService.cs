using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Infrastructure.Services;

public class MilestoneService : IMilestoneService
{
    private readonly ILogger<MilestoneService> _logger;

    public MilestoneService(ILogger<MilestoneService> logger)
    {
        _logger = logger;
    }

    public Task<IEnumerable<Milestone>> CheckMilestonesAsync(InternshipId internshipId, string procedureCode, int procedureCount, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Checking milestones: InternshipId={InternshipId}, Code={Code}, Count={Count}", 
            internshipId.Value, procedureCode, procedureCount);

        var milestones = new List<Milestone>();

        // Example milestone logic
        if (procedureCount == 10)
        {
            milestones.Add(new Milestone
            {
                Type = "ProcedureCount",
                Name = $"First 10 {procedureCode} Procedures",
                Description = $"Completed 10 procedures of type {procedureCode}",
                AchievedAt = DateTime.UtcNow
            });
        }

        if (procedureCount == 50)
        {
            milestones.Add(new Milestone
            {
                Type = "ProcedureCount",
                Name = $"50 {procedureCode} Procedures",
                Description = $"Completed 50 procedures of type {procedureCode}",
                AchievedAt = DateTime.UtcNow
            });
        }

        return Task.FromResult<IEnumerable<Milestone>>(milestones);
    }
}
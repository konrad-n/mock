using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Models.Statistics;

/// <summary>
/// Represents procedure statistics for tracking medical training progress
/// </summary>
public class ProcedureStatistics
{
    public InternshipId InternshipId { get; init; }
    public string ProcedureCode { get; init; } = string.Empty;
    public string ProcedureName { get; init; } = string.Empty;
    public int TotalPerformed { get; init; }
    public int RequiredCount { get; init; }
    public decimal CompletionPercentage { get; init; }
    public DateTime FirstPerformed { get; init; }
    public DateTime LastPerformed { get; init; }
    public int UniqueLocations { get; init; }
    public Dictionary<string, int> LocationBreakdown { get; init; } = new();
    public List<DateTime> PerformanceDates { get; init; } = new();
    public bool IsCompleted => TotalPerformed >= RequiredCount;
    
    // Advanced metrics
    public double AverageDailyRate { get; init; }
    public int DaysToCompletion { get; init; }
    public string MostFrequentLocation { get; init; } = string.Empty;
}

public class DailyProcedureStatistics
{
    public InternshipId InternshipId { get; init; }
    public DateTime Date { get; init; }
    public int TotalProcedures { get; init; }
    public int UniqueProcedureTypes { get; init; }
    public Dictionary<string, int> ProcedureCounts { get; init; } = new();
    public List<string> Locations { get; init; } = new();
    public bool HasDuplicates { get; init; }
    public List<string> DuplicatedProcedures { get; init; } = new();
}
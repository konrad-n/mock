using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Models.Statistics;

/// <summary>
/// Represents monthly shift statistics for an internship
/// </summary>
public class MonthlyShiftStatistics
{
    public InternshipId InternshipId { get; init; }
    public int Year { get; init; }
    public int Month { get; init; }
    public int TotalHours { get; init; }
    public int ApprovedHours { get; init; }
    public int PendingHours { get; init; }
    public int RejectedHours { get; init; }
    public int ShiftCount { get; init; }
    public decimal AverageHoursPerShift { get; init; }
    public int WeekendShifts { get; init; }
    public int NightShifts { get; init; }
    public DateTime LastUpdated { get; init; }
    
    // SMK Compliance
    public bool MeetsMonthlyMinimum { get; init; }
    public int RequiredHours { get; init; }
    public int HoursDeficit { get; init; }
    
    // Weekly statistics
    public Dictionary<int, WeeklyStatistics> WeeklyBreakdown { get; init; } = new();
}

public class WeeklyStatistics
{
    public int WeekNumber { get; init; }
    public int Hours { get; init; }
    public bool ExceedsWeeklyLimit { get; init; }
    public int MaxAllowedHours { get; init; }
}
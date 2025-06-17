namespace SledzSpecke.Application.Models.Statistics;

public class ProcedureDepartmentStats
{
    public string ProcedureCode { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public DateTime FirstRecorded { get; set; }
    public DateTime LastUpdated { get; set; }
    public double DailyAverage { get; set; }
}
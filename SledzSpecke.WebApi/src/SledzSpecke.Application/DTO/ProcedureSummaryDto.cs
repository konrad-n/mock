namespace SledzSpecke.Application.DTO;

public class ProcedureSummaryDto
{
    public int RequiredCountA { get; set; }
    public int RequiredCountB { get; set; }
    public int CompletedCountA { get; set; }
    public int CompletedCountB { get; set; }
    public int ApprovedCountA { get; set; }
    public int ApprovedCountB { get; set; }
    public int RemainingCountA => RequiredCountA - CompletedCountA;
    public int RemainingCountB => RequiredCountB - CompletedCountB;
}
namespace SledzSpecke.Application.DTO;

public sealed class UserProceduresDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public List<ModuleProceduresDto> Modules { get; set; } = new();
    public UserProcedureSummaryDto OverallSummary { get; set; } = new();
}

public sealed class UserProcedureSummaryDto
{
    public int TotalModules { get; set; }
    public int TotalProcedures { get; set; }
    public int CompletedProcedures { get; set; }
    public int TotalRealizationsAsOperator { get; set; }
    public int TotalRealizationsAsAssistant { get; set; }
    public decimal OverallCompletionPercentage { get; set; }
    public DateTime? LastRealizationDate { get; set; }
}
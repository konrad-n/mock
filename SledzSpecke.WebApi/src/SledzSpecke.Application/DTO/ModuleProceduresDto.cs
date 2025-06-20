namespace SledzSpecke.Application.DTO;

public sealed class ModuleProceduresDto
{
    public int ModuleId { get; set; }
    public string ModuleName { get; set; } = string.Empty;
    public string ModuleType { get; set; } = string.Empty;
    public List<ProcedureDetailsDto> Procedures { get; set; } = new();
    public ModuleProcedureSummaryDto Summary { get; set; } = new();
}

public sealed class ProcedureDetailsDto
{
    public int RequirementId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int RequiredAsOperator { get; set; }
    public int RequiredAsAssistant { get; set; }
    public int CompletedAsOperator { get; set; }
    public int CompletedAsAssistant { get; set; }
    public bool IsCompleted { get; set; }
    public List<ProcedureRealizationDto> Realizations { get; set; } = new();
}

public sealed class ProcedureRealizationDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int? Year { get; set; }
}

public sealed class ModuleProcedureSummaryDto
{
    public int TotalProcedures { get; set; }
    public int CompletedProcedures { get; set; }
    public int TotalRequiredAsOperator { get; set; }
    public int TotalRequiredAsAssistant { get; set; }
    public int TotalCompletedAsOperator { get; set; }
    public int TotalCompletedAsAssistant { get; set; }
    public decimal CompletionPercentage { get; set; }
}
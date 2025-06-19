using SledzSpecke.Core.Entities.Base;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

public sealed class ProcedureRequirement : Entity
{
    public ProcedureRequirementId Id { get; private set; }
    public ModuleId ModuleId { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public int RequiredAsOperator { get; private set; }  // "A" - samodzielnie
    public int RequiredAsAssistant { get; private set; } // "B" - asysta
    public int DisplayOrder { get; private set; }
    
    // Navigation
    public Module? Module { get; private set; }
    private readonly List<ProcedureRealization> _realizations = new();
    public IReadOnlyList<ProcedureRealization> Realizations => _realizations.AsReadOnly();

    private ProcedureRequirement() { }

    public ProcedureRequirement(
        ModuleId moduleId,
        string code,
        string name,
        int requiredAsOperator,
        int requiredAsAssistant,
        int displayOrder)
    {
        ModuleId = moduleId;
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        RequiredAsOperator = requiredAsOperator;
        RequiredAsAssistant = requiredAsAssistant;
        DisplayOrder = displayOrder;
    }
}
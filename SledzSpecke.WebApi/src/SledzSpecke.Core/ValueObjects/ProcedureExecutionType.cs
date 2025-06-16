namespace SledzSpecke.Core.ValueObjects;

/// <summary>
/// Represents the type of procedure execution in SMK system.
/// </summary>
public enum ProcedureExecutionType
{
    /// <summary>
    /// Code A - Procedure performed independently with assistance/supervision
    /// The resident acts as the primary operator
    /// </summary>
    CodeA,
    
    /// <summary>
    /// Code B - Procedure where resident assisted as first assistant
    /// The resident assists another physician who is the primary operator
    /// </summary>
    CodeB
}
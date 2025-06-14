using SledzSpecke.Core.Exceptions;
using System.Text.RegularExpressions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record ProcedureCode
{
    // Medical procedure codes typically follow specific formats
    // e.g., ICD-10-PCS format or institution-specific formats
    private static readonly Regex CodePattern = new(@"^[A-Z0-9\.\-]{2,20}$", RegexOptions.Compiled);
    
    public string Value { get; }

    public ProcedureCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidProcedureCodeException("Procedure code cannot be empty.");
        }

        value = value.Trim().ToUpperInvariant();

        if (!CodePattern.IsMatch(value))
        {
            throw new InvalidProcedureCodeException(
                "Procedure code must be 2-20 characters long and contain only uppercase letters, numbers, dots, and hyphens.");
        }

        Value = value;
    }

    public static implicit operator string(ProcedureCode code) => code.Value;
    public static implicit operator ProcedureCode(string code) => new(code);
    
    public override string ToString() => Value;
}
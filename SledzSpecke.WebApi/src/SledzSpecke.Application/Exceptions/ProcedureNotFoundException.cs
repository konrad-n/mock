using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Application.Exceptions;

public class ProcedureNotFoundException : CustomException
{
    public int ProcedureId { get; }

    public ProcedureNotFoundException(int procedureId)
        : base($"Procedure with ID '{procedureId}' was not found.")
    {
        ProcedureId = procedureId;
    }
}
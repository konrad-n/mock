using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

public class ProcedureNewSmk : ProcedureBase
{
    public string? Supervisor { get; private set; }
    public string? Institution { get; private set; }
    public string? Comments { get; private set; }
    public bool RequiresOperatorCode { get; private set; }

    private ProcedureNewSmk(ProcedureId id, InternshipId internshipId, DateTime date, int year,
        string code, string location, ProcedureStatus status)
        : base(id, internshipId, date, year, code, location, status, SmkVersion.New)
    {
        RequiresOperatorCode = DetermineIfOperatorCodeRequired(code);
    }

    public static ProcedureNewSmk Create(ProcedureId id, InternshipId internshipId, DateTime date,
        string code, string location)
    {
        ValidateInput(code, location, date);
        return new ProcedureNewSmk(id, internshipId, date, date.Year, code, location, ProcedureStatus.Pending);
    }

    public void SetSupervisor(string? supervisor)
    {
        EnsureCanModify();
        Supervisor = supervisor;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetInstitution(string? institution)
    {
        EnsureCanModify();
        Institution = institution;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetComments(string? comments)
    {
        EnsureCanModify();
        Comments = comments;
        UpdatedAt = DateTime.UtcNow;
    }

    public override bool IsValidForSmkVersion()
    {
        if (RequiresOperatorCode && string.IsNullOrEmpty(OperatorCode))
            return false;

        return !string.IsNullOrEmpty(Code) && !string.IsNullOrEmpty(Location);
    }

    public override void ValidateSmkSpecificRules()
    {
        if (RequiresOperatorCode && string.IsNullOrEmpty(OperatorCode))
            throw new InvalidOperationException("Operator code is required for this procedure type in New SMK.");

        if (Status == ProcedureStatus.Completed && string.IsNullOrEmpty(Supervisor))
            throw new InvalidOperationException("Supervisor is required for completed procedures in New SMK.");
    }

    public override void Complete()
    {
        ValidateSmkSpecificRules();
        base.Complete();
    }

    private static bool DetermineIfOperatorCodeRequired(string code)
    {
        return code.StartsWith("A") || code.Contains("OPER") || code.Contains("SURG");
    }

    private static void ValidateInput(string code, string location, DateTime date)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Procedure code cannot be empty.", nameof(code));

        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be empty.", nameof(location));

        if (date > DateTime.UtcNow.Date)
            throw new ArgumentException("Procedure date cannot be in the future.", nameof(date));
    }
}
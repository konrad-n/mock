using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

public class ProcedureOldSmk : ProcedureBase
{
    public string? LegacyFields { get; private set; }
    public bool IsLegacyFormat { get; private set; }
    public string? OldSmkCategory { get; private set; }

    private ProcedureOldSmk(ProcedureId id, InternshipId internshipId, DateTime date, int year,
        string code, string location, ProcedureStatus status)
        : base(id, internshipId, date, year, code, location, status, SmkVersion.Old)
    {
        IsLegacyFormat = true;
        OldSmkCategory = DetermineOldSmkCategory(code);
    }

    public static ProcedureOldSmk Create(ProcedureId id, InternshipId internshipId, DateTime date,
        string code, string location)
    {
        ValidateInput(code, location, date);
        return new ProcedureOldSmk(id, internshipId, date, date.Year, code, location, ProcedureStatus.Pending);
    }

    public void SetLegacyFields(string? legacyFields)
    {
        EnsureCanModify();
        LegacyFields = legacyFields;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetOldSmkCategory(string? category)
    {
        EnsureCanModify();
        OldSmkCategory = category;
        UpdatedAt = DateTime.UtcNow;
    }

    public override bool IsValidForSmkVersion()
    {
        return !string.IsNullOrEmpty(Code) && 
               !string.IsNullOrEmpty(Location) && 
               !string.IsNullOrEmpty(OldSmkCategory);
    }

    public override void ValidateSmkSpecificRules()
    {
        if (string.IsNullOrEmpty(OldSmkCategory))
            throw new InvalidOperationException("Old SMK category is required for Old SMK procedures.");

        if (Status == ProcedureStatus.Completed && string.IsNullOrEmpty(PerformingPerson))
            throw new InvalidOperationException("Performing person is required for completed procedures in Old SMK.");
    }

    public override void Complete()
    {
        ValidateSmkSpecificRules();
        base.Complete();
    }

    public override void UpdateProcedureDetails(string? operatorCode, string? performingPerson,
        string? patientInitials, char? patientGender)
    {
        base.UpdateProcedureDetails(operatorCode, performingPerson, patientInitials, patientGender);
        
        if (!string.IsNullOrEmpty(performingPerson))
        {
            OldSmkCategory ??= DetermineOldSmkCategory(Code);
        }
    }

    private static string DetermineOldSmkCategory(string code)
    {
        if (code.StartsWith("1") || code.StartsWith("2"))
            return "Basic";
        if (code.StartsWith("3") || code.StartsWith("4"))
            return "Intermediate";
        if (code.StartsWith("5") || code.StartsWith("6"))
            return "Advanced";
        
        return "General";
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
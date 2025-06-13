using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

public class Procedure : ProcedureBase
{
    private Procedure(ProcedureId id, InternshipId internshipId, DateTime date, int year, string code,
        string location, ProcedureStatus status, SmkVersion smkVersion = SmkVersion.New)
        : base(id, internshipId, date, year, code, location, status, smkVersion)
    {
    }

    public static Procedure Create(ProcedureId id, InternshipId internshipId, DateTime date, 
        string code, string location, SmkVersion smkVersion = SmkVersion.New)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new InvalidProcedureCodeException(code ?? string.Empty);
        
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be empty.", nameof(location));
        
        if (date > DateTime.UtcNow.Date)
            throw new ArgumentException("Procedure date cannot be in the future.", nameof(date));

        return new Procedure(id, internshipId, date, date.Year, code, location, ProcedureStatus.Pending, smkVersion);
    }

    public override bool IsValidForSmkVersion()
    {
        return !string.IsNullOrEmpty(Code) && !string.IsNullOrEmpty(Location);
    }

    public override void ValidateSmkSpecificRules()
    {
        // Generic validation for backward compatibility
        if (string.IsNullOrEmpty(Code))
            throw new InvalidOperationException("Procedure code is required.");
        
        if (string.IsNullOrEmpty(Location))
            throw new InvalidOperationException("Location is required.");
    }
}
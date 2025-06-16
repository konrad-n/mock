using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

public class Procedure : ProcedureBase
{
    private Procedure(ProcedureId id, ModuleId moduleId, InternshipId internshipId, DateTime date, int year, string code,
        string name, string location, ProcedureExecutionType executionType, string supervisorName,
        ProcedureStatus status, SmkVersion smkVersion)
        : base(id, moduleId, internshipId, date, year, code, name, location, executionType, 
               supervisorName, status, smkVersion)
    {
    }

    public static Procedure Create(ProcedureId id, InternshipId internshipId, DateTime date,
        string code, string location, int year, SmkVersion smkVersion)
    {
        // This overload is for backward compatibility with tests
        // Using reasonable defaults for new required fields
        var moduleId = new ModuleId(1); // Default module ID for tests
        var name = code; // Use code as name for backward compatibility
        var executionType = ProcedureExecutionType.CodeA; // Default to Code A
        var supervisorName = "Test Supervisor"; // Default supervisor for tests
        
        return Create(id, moduleId, internshipId, date, code, name, location, 
                     executionType, supervisorName, year, smkVersion);
    }
    
    public static Procedure Create(ProcedureId id, ModuleId moduleId, InternshipId internshipId, DateTime date,
        string code, string name, string location, ProcedureExecutionType executionType, 
        string supervisorName, int year, SmkVersion smkVersion)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new InvalidProcedureCodeException(code ?? string.Empty);

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Procedure name cannot be empty.", nameof(name));
            
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be empty.", nameof(location));

        if (date > DateTime.UtcNow.Date)
            throw new ArgumentException("Procedure date cannot be in the future.", nameof(date));

        return new Procedure(id, moduleId, internshipId, date, year, code, name, location, 
                           executionType, supervisorName, ProcedureStatus.Pending, smkVersion);
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
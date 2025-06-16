using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

/// <summary>
/// Represents a procedure in the New SMK system.
/// New SMK tracks aggregated procedure counts by module with date ranges.
/// New SMK tracks both Code A (performed) and Code B (assisted) separately.
/// </summary>
public class ProcedureNewSmk : ProcedureBase
{

    /// <summary>
    /// Reference to procedure requirement from template
    /// </summary>
    public int ProcedureRequirementId { get; private set; }

    /// <summary>
    /// Count of procedures performed as operator (Code A)
    /// </summary>
    public int CountA { get; private set; }

    /// <summary>
    /// Count of procedures performed as assistant (Code B)
    /// </summary>
    public int CountB { get; private set; }
    
    /// <summary>
    /// Required count for Code A procedures
    /// </summary>
    public int RequiredCountCodeA { get; private set; }
    
    /// <summary>
    /// Required count for Code B procedures
    /// </summary>
    public int RequiredCountCodeB { get; private set; }

    /// <summary>
    /// Name of the procedure from requirement template
    /// </summary>
    public string ProcedureName { get; private set; }

    /// <summary>
    /// Supervisor overseeing the procedures
    /// </summary>
    public string? Supervisor { get; private set; }

    /// <summary>
    /// Institution where procedures were performed
    /// </summary>
    public string? Institution { get; private set; }

    /// <summary>
    /// Additional comments about the procedures
    /// </summary>
    public string? Comments { get; private set; }

    private ProcedureNewSmk(ProcedureId id, ModuleId moduleId, InternshipId internshipId, DateTime date,
        string code, string name, string location, ProcedureExecutionType executionType,
        string supervisorName, ProcedureStatus status, int procedureRequirementId)
        : base(id, moduleId, internshipId, date, date.Year, code, name, location, executionType,
               supervisorName, status, SmkVersion.New)
    {
        ProcedureRequirementId = procedureRequirementId;
        ProcedureName = name;
        CountA = 0;
        CountB = 0;
    }

    public static ProcedureNewSmk Create(ProcedureId id, ModuleId moduleId, InternshipId internshipId, DateTime date,
        string code, string procedureName, string location, ProcedureExecutionType executionType,
        string supervisorName, int procedureRequirementId)
    {
        ValidateInput(code, location, date, procedureName);
        return new ProcedureNewSmk(id, moduleId, internshipId, date, code, procedureName, location,
            executionType, supervisorName, ProcedureStatus.Pending, procedureRequirementId);
    }

    public void UpdateCounts(int countA, int countB)
    {
        EnsureCanModify();

        if (countA < 0)
            throw new ArgumentException("Count A cannot be negative.", nameof(countA));
        if (countB < 0)
            throw new ArgumentException("Count B cannot be negative.", nameof(countB));

        CountA = countA;
        CountB = countB;
        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public void IncrementCounts(int deltaA, int deltaB)
    {
        EnsureCanModify();

        if (CountA + deltaA < 0)
            throw new ArgumentException("Resulting Count A cannot be negative.", nameof(deltaA));
        if (CountB + deltaB < 0)
            throw new ArgumentException("Resulting Count B cannot be negative.", nameof(deltaB));

        CountA += deltaA;
        CountB += deltaB;
        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public void SetSupervisor(string? supervisor)
    {
        EnsureCanModify();
        Supervisor = supervisor;
        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public void SetInstitution(string? institution)
    {
        EnsureCanModify();
        Institution = institution;
        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public void SetComments(string? comments)
    {
        EnsureCanModify();
        Comments = comments;
        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public override bool IsValidForSmkVersion()
    {
        // New SMK requires module, procedure requirement, and at least one count
        return SmkVersion == SmkVersion.New &&
               ModuleId != null &&
               ProcedureRequirementId > 0 &&
               !string.IsNullOrEmpty(ProcedureName) &&
               (CountA > 0 || CountB > 0) &&
               !string.IsNullOrEmpty(Location) &&
               !string.IsNullOrEmpty(SupervisorName);
    }

    public override void ValidateSmkSpecificRules()
    {
        // Validate module and requirement
        if (ModuleId == null)
            throw new InvalidOperationException("Module ID is required for New SMK procedures.");

        if (ProcedureRequirementId <= 0)
            throw new InvalidOperationException("Procedure requirement ID is required for New SMK procedures.");

        // Validate counts
        if (CountA == 0 && CountB == 0)
            throw new InvalidOperationException("At least one procedure count (A or B) must be greater than zero.");

        // For completed procedures in New SMK, supervisor is required
        if (Status == ProcedureStatus.Completed && string.IsNullOrEmpty(Supervisor))
            throw new InvalidOperationException("Supervisor is required for completed procedures in New SMK.");
    }

    public override void Complete()
    {
        ValidateSmkSpecificRules();
        base.Complete();
    }

    public override void UpdateProcedureDetails(ProcedureExecutionType executionType, string? performingPerson,
        string? patientInfo, string? patientInitials, char? patientGender)
    {
        // New SMK doesn't track individual patient data
        // These are aggregated counts, not individual procedures
        EnsureCanModify();

        ExecutionType = executionType;
        
        // Only update supervisor if provided as performing person
        if (!string.IsNullOrEmpty(performingPerson) && string.IsNullOrEmpty(Supervisor))
        {
            Supervisor = performingPerson;
        }

        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    private static void ValidateInput(string code, string location, DateTime date, string procedureName)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new InvalidProcedureCodeException(code ?? string.Empty);

        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be empty.", nameof(location));

        // No future date validation - MAUI app allows future dates

        if (string.IsNullOrWhiteSpace(procedureName))
            throw new ArgumentException("Procedure name cannot be empty.", nameof(procedureName));
    }
}
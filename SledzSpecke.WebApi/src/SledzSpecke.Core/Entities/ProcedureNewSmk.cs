using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

/// <summary>
/// Represents a procedure in the New SMK system.
/// New SMK tracks aggregated procedure counts by module with date ranges.
/// </summary>
public class ProcedureNewSmk : ProcedureBase
{
    /// <summary>
    /// Module ID this procedure entry belongs to
    /// </summary>
    public ModuleId ModuleId { get; private set; }

    /// <summary>
    /// Reference to procedure requirement from template
    /// </summary>
    public int ProcedureRequirementId { get; private set; }

    /// <summary>
    /// Count of procedures performed as operator (A)
    /// </summary>
    public int CountA { get; private set; }

    /// <summary>
    /// Count of procedures performed as assistant (B)
    /// </summary>
    public int CountB { get; private set; }

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

    private ProcedureNewSmk(ProcedureId id, InternshipId internshipId, DateTime date,
        string code, string location, ProcedureStatus status, ModuleId moduleId,
        int procedureRequirementId, string procedureName)
        : base(id, internshipId, date, date.Year, code, location, status, SmkVersion.New)
    {
        ModuleId = moduleId;
        ProcedureRequirementId = procedureRequirementId;
        ProcedureName = procedureName;
        CountA = 0;
        CountB = 0;
    }

    public static ProcedureNewSmk Create(ProcedureId id, InternshipId internshipId, DateTime date,
        string code, string location, ModuleId moduleId, int procedureRequirementId, string procedureName)
    {
        ValidateInput(code, location, date, procedureName);
        return new ProcedureNewSmk(id, internshipId, date, code, location, ProcedureStatus.Pending,
            moduleId, procedureRequirementId, procedureName);
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
               !string.IsNullOrEmpty(Location);
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

    public override void UpdateProcedureDetails(string? operatorCode, string? performingPerson,
        string? patientInitials, char? patientGender)
    {
        // New SMK doesn't track individual patient data or operator codes
        // These are aggregated counts, not individual procedures
        EnsureCanModify();

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
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

/// <summary>
/// Represents a procedure in the Old SMK system.
/// Old SMK tracks individual procedure instances with specific patient data and year-based progression.
/// In Old SMK, procedures only count Code A (performed with supervision).
/// </summary>
public class ProcedureOldSmk : ProcedureBase
{
    /// <summary>
    /// Reference to procedure requirement from template (optional)
    /// </summary>
    public int? ProcedureRequirementId { get; private set; }
    
    /// <summary>
    /// Required count for Code A procedures (Old SMK only tracks Code A)
    /// </summary>
    public int RequiredCountCodeA { get; private set; }

    // Note: ProcedureGroup and AssistantData are inherited from ProcedureBase

    /// <summary>
    /// Name of the internship during which procedure was performed
    /// </summary>
    public string? InternshipName { get; private set; }

    private ProcedureOldSmk(ProcedureId id, int moduleId, int internshipId, DateTime date, int year,
        string code, string name, string location, ProcedureExecutionType executionType,
        string supervisorName, ProcedureStatus status)
        : base(id, moduleId, internshipId, date, year, code, name, location, executionType, 
               supervisorName, status, SmkVersion.Old)
    {
    }

    public static ProcedureOldSmk Create(ProcedureId id, int moduleId, int internshipId, DateTime date,
        int year, string code, string name, string location, ProcedureExecutionType executionType,
        string supervisorName)
    {
        ValidateInput(code, name, location, date, year);
        return new ProcedureOldSmk(id, moduleId, internshipId, date, year, code, name, location, 
                                   executionType, supervisorName, ProcedureStatus.Pending);
    }

    public void SetProcedureRequirement(int requirementId)
    {
        EnsureCanModify();
        ProcedureRequirementId = requirementId;
        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    // Note: SetProcedureGroup and SetAssistantData are inherited from ProcedureBase

    public void SetInternshipName(string internshipName)
    {
        EnsureCanModify();
        InternshipName = internshipName;
        UpdatedAt = DateTime.UtcNow;

        // Automatically transition from Synced to Modified
        if (SyncStatus == SyncStatus.Synced)
        {
            SyncStatus = SyncStatus.Modified;
        }
    }

    public override bool IsValidForSmkVersion()
    {
        // Old SMK requires valid execution type
        return SmkVersion == SmkVersion.Old &&
               !string.IsNullOrEmpty(Code) &&
               !string.IsNullOrEmpty(Name) &&
               !string.IsNullOrEmpty(Location) &&
               !string.IsNullOrEmpty(SupervisorName) &&
               Year >= 0 && Year <= 6;
    }

    public override void ValidateSmkSpecificRules()
    {
        // Validate year range
        if (Year < 0 || Year > 6)
            throw new InvalidOperationException("Year must be between 0 (unassigned) and 6 for Old SMK procedures.");

        // For completed procedures in Old SMK, performing person is required
        if (Status == ProcedureStatus.Completed && string.IsNullOrEmpty(PerformingPerson))
            throw new InvalidOperationException("Performing person is required for completed procedures in Old SMK.");

        // For completed procedures, patient data should be complete
        if (Status == ProcedureStatus.Completed)
        {
            if (string.IsNullOrEmpty(PatientInitials))
                throw new InvalidOperationException("Patient initials are required for completed procedures.");

            if (!PatientGender.HasValue)
                throw new InvalidOperationException("Patient gender is required for completed procedures.");
        }

        // Supervisor name is required
        if (string.IsNullOrEmpty(SupervisorName))
            throw new InvalidOperationException("Supervisor name is required for Old SMK procedures.");
    }

    public override void Complete()
    {
        ValidateSmkSpecificRules();
        base.Complete();
    }

    public override void UpdateProcedureDetails(ProcedureExecutionType executionType, string? performingPerson,
        string? patientInfo, string? patientInitials, char? patientGender)
    {
        base.UpdateProcedureDetails(executionType, performingPerson, patientInfo, patientInitials, patientGender);

        // Additional Old SMK specific logic could go here if needed
    }

    private static void ValidateInput(string code, string name, string location, DateTime date, int year)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new InvalidProcedureCodeException(code ?? string.Empty);

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Procedure name cannot be empty.", nameof(name));

        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be empty.", nameof(location));

        // No future date validation - MAUI app allows future dates

        // Validate year for Old SMK (0-6)
        if (year < 0 || year > 6)
            throw new ArgumentException("Year must be between 0 (unassigned) and 6 for Old SMK.", nameof(year));
    }
}
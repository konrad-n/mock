using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Entities;

/// <summary>
/// Represents a procedure in the Old SMK system.
/// Old SMK tracks individual procedure instances with specific patient data and year-based progression.
/// </summary>
public class ProcedureOldSmk : ProcedureBase
{
    /// <summary>
    /// Reference to procedure requirement from template (optional)
    /// </summary>
    public int? ProcedureRequirementId { get; private set; }

    // Note: ProcedureGroup and AssistantData are inherited from ProcedureBase

    /// <summary>
    /// Name of the internship during which procedure was performed
    /// </summary>
    public string? InternshipName { get; private set; }

    private ProcedureOldSmk(ProcedureId id, InternshipId internshipId, DateTime date, int year,
        string code, string location, ProcedureStatus status)
        : base(id, internshipId, date, year, code, location, status, SmkVersion.Old)
    {
    }

    public static ProcedureOldSmk Create(ProcedureId id, InternshipId internshipId, DateTime date,
        int year, string code, string location)
    {
        ValidateInput(code, location, date, year);
        return new ProcedureOldSmk(id, internshipId, date, year, code, location, ProcedureStatus.Pending);
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
        // Old SMK requires operator code to be A or B
        return SmkVersion == SmkVersion.Old &&
               !string.IsNullOrEmpty(Code) &&
               !string.IsNullOrEmpty(Location) &&
               (string.IsNullOrEmpty(OperatorCode) || OperatorCode == "A" || OperatorCode == "B") &&
               Year >= 0 && Year <= 6;
    }

    public override void ValidateSmkSpecificRules()
    {
        // Validate operator code for Old SMK
        if (!string.IsNullOrEmpty(OperatorCode) && OperatorCode != "A" && OperatorCode != "B")
            throw new InvalidOperationException("Operator code must be 'A' or 'B' for Old SMK procedures.");

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

        // Additional Old SMK specific logic could go here if needed
    }

    private static void ValidateInput(string code, string location, DateTime date, int year)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new InvalidProcedureCodeException(code ?? string.Empty);

        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be empty.", nameof(location));

        // No future date validation - MAUI app allows future dates

        // Validate year for Old SMK (0-6)
        if (year < 0 || year > 6)
            throw new ArgumentException("Year must be between 0 (unassigned) and 6 for Old SMK.", nameof(year));
    }
}
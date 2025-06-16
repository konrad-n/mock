using SledzSpecke.Application.DTO;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Procedures.Extensions;

internal static class ProcedureMappingExtensions
{
    public static ProcedureDto ToDto(this ProcedureBase procedure)
    {
        // Extract SMK-specific fields based on procedure type
        int? procedureRequirementId = null;
        string? internshipName = null;
        string? oldSmkCategory = null;
        int? moduleId = null;
        string? procedureName = null;
        int? countA = null;
        int? countB = null;
        string? supervisor = null;
        string? institution = null;
        string? comments = null;

        // Handle specific procedure types
        if (procedure is ProcedureOldSmk oldSmk)
        {
            procedureRequirementId = oldSmk.ProcedureRequirementId;
            internshipName = oldSmk.InternshipName;
        }
        else if (procedure is ProcedureNewSmk newSmk)
        {
            moduleId = newSmk.ModuleId?.Value;
            procedureName = newSmk.ProcedureName;
            countA = newSmk.CountA;
            countB = newSmk.CountB;
            supervisor = newSmk.Supervisor;
            institution = newSmk.Institution;
            comments = newSmk.Comments;
        }

        return new ProcedureDto(
            procedure.Id.Value,
            procedure.InternshipId.Value,
            procedure.Date,
            procedure.Year,
            procedure.Code,
            procedure.ExecutionType.ToString(),
            procedure.PerformingPerson,
            procedure.Location,
            procedure.PatientInitials,
            procedure.PatientGender,
            procedure.AssistantData,
            procedure.ProcedureGroup,
            procedure.Status.ToString(),
            procedure.SyncStatus,
            procedure.AdditionalFields,
            procedure.IsCompleted,
            procedure.CanBeModified,
            procedure.SmkVersion.ToString(),
            // Old SMK specific
            procedureRequirementId,
            internshipName,
            oldSmkCategory,
            // New SMK specific
            moduleId,
            procedureName,
            countA,
            countB,
            supervisor,
            institution,
            comments
        );
    }
}
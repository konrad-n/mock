using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Policies.Procedure;

public class OldSmkProcedurePolicy : ISmkPolicy<ProcedureBase>
{
    public SmkVersion ApplicableVersion => SmkVersion.Old;

    public Result Validate(ProcedureBase procedure, SpecializationContext context)
    {
        if (procedure.SmkVersion != SmkVersion.Old)
        {
            return Result.Failure("Procedura nie jest zgodna z wersją Old SMK");
        }

        // Validate operator code
        if (!string.IsNullOrEmpty(procedure.OperatorCode) && 
            procedure.OperatorCode != "A" && 
            procedure.OperatorCode != "B")
        {
            return Result.Failure("Kod operatora musi być 'A' (operator) lub 'B' (asystent)");
        }

        // Validate year range (0 = unassigned, 1-6 = year)
        if (procedure.Year < 0 || procedure.Year > 6)
        {
            return Result.Failure("Rok musi być w zakresie 0-6 dla Old SMK");
        }

        // For completed procedures, validate required fields
        if (procedure.Status == ProcedureStatus.Completed)
        {
            if (string.IsNullOrWhiteSpace(procedure.PerformingPerson))
            {
                return Result.Failure("Osoba wykonująca jest wymagana dla zakończonych procedur");
            }

            if (string.IsNullOrWhiteSpace(procedure.PatientInitials))
            {
                return Result.Failure("Inicjały pacjenta są wymagane dla zakończonych procedur");
            }

            if (!procedure.PatientGender.HasValue)
            {
                return Result.Failure("Płeć pacjenta jest wymagana dla zakończonych procedur");
            }

            if (procedure.PatientGender.HasValue && 
                procedure.PatientGender.Value != 'M' && 
                procedure.PatientGender.Value != 'K')
            {
                return Result.Failure("Płeć pacjenta musi być 'M' lub 'K'");
            }
        }

        // Validate procedure can be performed by user in their current year
        if (context.ContextDate < context.ContextDate.AddYears(-procedure.Year))
        {
            return Result.Failure($"Procedura nie może być wykonana w roku {procedure.Year}");
        }

        return Result.Success();
    }

    public static class ErrorCodes
    {
        public const string InvalidOperatorCode = "SMK_006";
        public const string InvalidYear = "SMK_007";
        public const string MissingPerformingPerson = "SMK_008";
        public const string MissingPatientData = "SMK_009";
        public const string InvalidGender = "SMK_010";
    }
}
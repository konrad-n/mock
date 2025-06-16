using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Policies.Procedure;

public class NewSmkProcedurePolicy : ISmkPolicy<ProcedureBase>
{
    public SmkVersion ApplicableVersion => SmkVersion.New;

    public Result Validate(ProcedureBase procedure, SpecializationContext context)
    {
        if (procedure.SmkVersion != SmkVersion.New)
        {
            return Result.Failure("Procedura nie jest zgodna z wersją New SMK");
        }

        if (procedure is not ProcedureNewSmk newSmkProcedure)
        {
            return Result.Failure("Nieprawidłowy typ procedury dla New SMK");
        }

        // Validate module context
        if (context.CurrentModuleId == null)
        {
            return Result.Failure("Brak aktywnego modułu dla nowego systemu SMK");
        }

        // Validate procedure belongs to current module
        if (newSmkProcedure.ModuleId != context.CurrentModuleId)
        {
            return Result.Failure("Procedura musi należeć do aktualnego modułu");
        }

        // Validate at least one count is greater than zero
        if (newSmkProcedure.CountA == 0 && newSmkProcedure.CountB == 0)
        {
            return Result.Failure("Co najmniej jedna liczba procedur (A lub B) musi być większa od zera");
        }

        // Validate counts are non-negative
        if (newSmkProcedure.CountA < 0 || newSmkProcedure.CountB < 0)
        {
            return Result.Failure("Liczba procedur nie może być ujemna");
        }

        // For completed procedures, validate supervisor
        if (procedure.Status == ProcedureStatus.Completed)
        {
            if (string.IsNullOrWhiteSpace(newSmkProcedure.Supervisor))
            {
                return Result.Failure("Opiekun jest wymagany dla zakończonych procedur w New SMK");
            }
        }

        // Validate procedure requirement exists
        if (newSmkProcedure.ProcedureRequirementId <= 0)
        {
            return Result.Failure("Wymaganie procedury jest nieprawidłowe");
        }

        // Validate institution if provided
        if (!string.IsNullOrWhiteSpace(newSmkProcedure.Institution) && 
            newSmkProcedure.Institution.Length > 200)
        {
            return Result.Failure("Nazwa instytucji jest zbyt długa (maksymalnie 200 znaków)");
        }

        return Result.Success();
    }

    public static class ErrorCodes
    {
        public const string InvalidProcedureType = "SMK_011";
        public const string ModuleNotActive = "SMK_004";
        public const string WrongModule = "SMK_012";
        public const string ZeroCounts = "SMK_013";
        public const string NegativeCounts = "SMK_014";
        public const string SupervisorRequired = "SMK_005";
        public const string InvalidRequirement = "SMK_015";
        public const string InstitutionTooLong = "SMK_016";
    }
}
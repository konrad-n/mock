namespace SledzSpecke.Application.Abstractions;

public interface IValidationService
{
    Task<ValidationResult> ValidateProcedureCodeAsync(
        string procedureCode,
        string smkVersion,
        CancellationToken cancellationToken = default);

    Task<int> GetDailyProcedureLimitAsync(
        string procedureCode,
        CancellationToken cancellationToken = default);
}
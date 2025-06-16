using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Abstractions;

namespace SledzSpecke.Infrastructure.Services;

public class ValidationService : IValidationService
{
    private readonly ILogger<ValidationService> _logger;
    
    // Example SMK procedure codes for validation
    private readonly HashSet<string> _validProcedureCodes = new()
    {
        "A.1", "A.2", "A.3", "A.4", "A.5",
        "B.1", "B.2", "B.3", "B.4", "B.5",
        "C.1", "C.2", "C.3", "C.4", "C.5",
        "D.1", "D.2", "D.3", "D.4", "D.5"
    };

    private readonly Dictionary<string, int> _dailyLimits = new()
    {
        { "A.1", 5 },
        { "B.1", 3 },
        { "C.1", 10 }
    };

    public ValidationService(ILogger<ValidationService> logger)
    {
        _logger = logger;
    }

    public Task<ValidationResult> ValidateProcedureCodeAsync(string procedureCode, string smkVersion, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Validating procedure code: Code={Code}, SMKVersion={Version}", procedureCode, smkVersion);

        if (string.IsNullOrWhiteSpace(procedureCode))
        {
            return Task.FromResult(ValidationResult.Failure("Procedure code cannot be empty"));
        }

        // In a real implementation, this would check against the actual SMK specification
        var isValid = _validProcedureCodes.Contains(procedureCode.ToUpper());
        
        return Task.FromResult(isValid 
            ? ValidationResult.Success() 
            : ValidationResult.Failure($"Invalid procedure code: {procedureCode}"));
    }

    public Task<int> GetDailyProcedureLimitAsync(string procedureCode, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting daily limit for procedure: Code={Code}", procedureCode);

        // Return the limit if defined, otherwise 0 (no limit)
        return Task.FromResult(_dailyLimits.TryGetValue(procedureCode.ToUpper(), out var limit) ? limit : 0);
    }
}
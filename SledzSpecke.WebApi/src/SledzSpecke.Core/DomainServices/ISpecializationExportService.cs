using System.Threading.Tasks;
using SledzSpecke.Core.Abstractions;

namespace SledzSpecke.Core.DomainServices;

public interface ISpecializationExportService
{
    Task<Result<byte[]>> ExportToXlsxAsync(int specializationId);
    Task<Result<ExportPreview>> PreviewExportAsync(int specializationId);
    Task<Result<ValidationReport>> ValidateForExportAsync(int specializationId);
}

public sealed class ExportPreview
{
    public int SpecializationId { get; init; }
    public string UserName { get; init; }
    public string Pesel { get; init; }
    public string PwzNumber { get; init; }
    public string SpecializationName { get; init; }
    public string SmkVersion { get; init; }
    public int TotalInternships { get; init; }
    public int TotalCourses { get; init; }
    public int TotalMedicalShifts { get; init; }
    public int TotalProcedures { get; init; }
    public int TotalSelfEducationDays { get; init; }
    public ExportValidationStatus ValidationStatus { get; init; }
    public List<string> ValidationWarnings { get; init; } = new();
}

public sealed class ValidationReport
{
    public bool IsValid { get; init; }
    public List<ValidationError> Errors { get; init; } = new();
    public List<ValidationWarning> Warnings { get; init; } = new();
    public DateTime ValidationDate { get; init; }
    public string SmkVersion { get; init; }
}

public sealed class ValidationError
{
    public string Field { get; init; }
    public string Message { get; init; }
    public string ErrorCode { get; init; }
    public ValidationSeverity Severity { get; init; }
}

public sealed class ValidationWarning
{
    public string Field { get; init; }
    public string Message { get; init; }
    public string WarningCode { get; init; }
}

public enum ExportValidationStatus
{
    Valid,
    HasWarnings,
    HasErrors
}

public enum ValidationSeverity
{
    Warning,
    Error,
    Critical
}
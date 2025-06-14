namespace SledzSpecke.Application.Validation;

/// <summary>
/// Represents the result of a validation operation with detailed error information
/// </summary>
public class ValidationResult
{
    private readonly List<ValidationError> _errors = new();

    public bool IsValid => _errors.Count == 0;
    public IReadOnlyList<ValidationError> Errors => _errors.AsReadOnly();

    public void AddError(string propertyName, string errorMessage)
    {
        _errors.Add(new ValidationError(propertyName, errorMessage));
    }

    public void AddError(ValidationError error)
    {
        _errors.Add(error);
    }

    public void Merge(ValidationResult other)
    {
        foreach (var error in other.Errors)
        {
            _errors.Add(error);
        }
    }

    public string GetErrorMessage()
    {
        if (IsValid)
            return string.Empty;

        return string.Join("; ", _errors.Select(e => e.ToString()));
    }

    public static ValidationResult Success() => new();

    public static ValidationResult Failure(string propertyName, string errorMessage)
    {
        var result = new ValidationResult();
        result.AddError(propertyName, errorMessage);
        return result;
    }
}

/// <summary>
/// Represents a single validation error
/// </summary>
public record ValidationError(string PropertyName, string ErrorMessage)
{
    public override string ToString() => $"{PropertyName}: {ErrorMessage}";
}
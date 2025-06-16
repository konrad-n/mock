namespace SledzSpecke.Application.Exceptions;

public abstract class SmkException : ApplicationException
{
    protected SmkException(string message) : base(message) { }
}

public class SmkValidationException : SmkException
{
    public string Field { get; }
    public object? AttemptedValue { get; }

    public SmkValidationException(string field, object? attemptedValue, string message) 
        : base(message)
    {
        Field = field;
        AttemptedValue = attemptedValue;
    }
}

public class SmkExportException : SmkException
{
    public int SpecializationId { get; }
    public string? SmkVersion { get; }

    public SmkExportException(int specializationId, string? smkVersion, string message) 
        : base(message)
    {
        SpecializationId = specializationId;
        SmkVersion = smkVersion;
    }
}

public class SmkDataIncompleteException : SmkException
{
    public string[] MissingFields { get; }

    public SmkDataIncompleteException(string[] missingFields) 
        : base($"SMK export failed - missing required fields: {string.Join(", ", missingFields)}")
    {
        MissingFields = missingFields;
    }
}

public class SmkBusinessRuleViolationException : SmkException
{
    public string RuleCode { get; }

    public SmkBusinessRuleViolationException(string ruleCode, string message) 
        : base(message)
    {
        RuleCode = ruleCode;
    }
}
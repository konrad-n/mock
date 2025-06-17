namespace SledzSpecke.Application;

/// <summary>
/// Standard error codes used throughout the application
/// </summary>
public static class ErrorCodes
{
    public const string NOT_FOUND = "NOT_FOUND";
    public const string UNAUTHORIZED = "UNAUTHORIZED";
    public const string FORBIDDEN = "FORBIDDEN";
    public const string CONFLICT = "CONFLICT";
    public const string VALIDATION_ERROR = "VALIDATION_ERROR";
    public const string DOMAIN_ERROR = "DOMAIN_ERROR";
    public const string BUSINESS_RULE_VIOLATION = "BUSINESS_RULE_VIOLATION";
    public const string EXTERNAL_SERVICE_ERROR = "EXTERNAL_SERVICE_ERROR";
    public const string TIMEOUT = "TIMEOUT";
    public const string INTERNAL_ERROR = "INTERNAL_ERROR";
    
    // Domain-specific error codes
    public const string USER_ALREADY_EXISTS = "USER_ALREADY_EXISTS";
    public const string INVALID_CREDENTIALS = "INVALID_CREDENTIALS";
    public const string EMAIL_ALREADY_IN_USE = "EMAIL_ALREADY_IN_USE";
    public const string PESEL_ALREADY_IN_USE = "PESEL_ALREADY_IN_USE";
    public const string PWZ_ALREADY_IN_USE = "PWZ_ALREADY_IN_USE";
    public const string SHIFT_OUTSIDE_INTERNSHIP = "SHIFT_OUTSIDE_INTERNSHIP";
    public const string WEEKLY_HOURS_EXCEEDED = "WEEKLY_HOURS_EXCEEDED";
    public const string MONTHLY_HOURS_INSUFFICIENT = "MONTHLY_HOURS_INSUFFICIENT";
    public const string DUPLICATE_PROCEDURE = "DUPLICATE_PROCEDURE";
    public const string INVALID_MODULE_PROGRESSION = "INVALID_MODULE_PROGRESSION";
    public const string SPECIALIZATION_NOT_ACTIVE = "SPECIALIZATION_NOT_ACTIVE";
    public const string COURSE_NOT_APPROVED = "COURSE_NOT_APPROVED";
    public const string INVALID_SMK_VERSION = "INVALID_SMK_VERSION";
}
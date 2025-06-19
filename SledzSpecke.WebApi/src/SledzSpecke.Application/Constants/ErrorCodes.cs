namespace SledzSpecke.Application.Constants;

/// <summary>
/// Centralized error codes for the application
/// </summary>
public static class ErrorCodes
{
    // Authentication & Authorization
    public const string INVALID_CREDENTIALS = "AUTH_INVALID_CREDENTIALS";
    public const string TOKEN_EXPIRED = "AUTH_TOKEN_EXPIRED";
    public const string UNAUTHORIZED_ACCESS = "AUTH_UNAUTHORIZED";
    public const string INSUFFICIENT_PERMISSIONS = "AUTH_INSUFFICIENT_PERMISSIONS";
    
    // User Management
    public const string USER_NOT_FOUND = "USER_NOT_FOUND";
    public const string USER_ALREADY_EXISTS = "USER_ALREADY_EXISTS";
    public const string EMAIL_ALREADY_IN_USE = "USER_EMAIL_IN_USE";
    public const string USER_REGISTRATION_FAILED = "USER_REGISTRATION_FAILED";
    public const string USER_UPDATE_FAILED = "USER_UPDATE_FAILED";
    public const string INVALID_PASSWORD = "USER_INVALID_PASSWORD";
    
    // Specialization
    public const string SPECIALIZATION_NOT_FOUND = "SPEC_NOT_FOUND";
    public const string INVALID_SPECIALIZATION = "SPEC_INVALID";
    public const string MODULE_NOT_FOUND = "SPEC_MODULE_NOT_FOUND";
    public const string CANNOT_SWITCH_MODULE = "SPEC_CANNOT_SWITCH_MODULE";
    
    // Internships
    public const string INTERNSHIP_NOT_FOUND = "INTERN_NOT_FOUND";
    public const string INTERNSHIP_OVERLAP = "INTERN_OVERLAP";
    public const string INTERNSHIP_ALREADY_COMPLETED = "INTERN_ALREADY_COMPLETED";
    public const string INTERNSHIP_CANNOT_MODIFY = "INTERN_CANNOT_MODIFY";
    public const string INTERNSHIP_INVALID_DATES = "INTERN_INVALID_DATES";
    
    // Medical Shifts
    public const string SHIFT_NOT_FOUND = "SHIFT_NOT_FOUND";
    public const string SHIFT_INVALID_DURATION = "SHIFT_INVALID_DURATION";
    public const string SHIFT_OUTSIDE_INTERNSHIP = "SHIFT_OUTSIDE_INTERNSHIP";
    public const string SHIFT_CANNOT_MODIFY = "SHIFT_CANNOT_MODIFY";
    public const string SHIFT_DUPLICATE = "SHIFT_DUPLICATE";
    
    // Procedures
    public const string PROCEDURE_NOT_FOUND = "PROC_NOT_FOUND";
    public const string PROCEDURE_INVALID_CODE = "PROC_INVALID_CODE";
    public const string PROCEDURE_DUPLICATE = "PROC_DUPLICATE";
    public const string PROCEDURE_CANNOT_MODIFY = "PROC_CANNOT_MODIFY";
    public const string PROCEDURE_INVALID_OPERATOR = "PROC_INVALID_OPERATOR";
    
    // Courses
    public const string COURSE_NOT_FOUND = "COURSE_NOT_FOUND";
    public const string COURSE_ALREADY_COMPLETED = "COURSE_ALREADY_COMPLETED";
    public const string COURSE_INVALID_DATES = "COURSE_INVALID_DATES";
    public const string COURSE_CANNOT_MODIFY = "COURSE_CANNOT_MODIFY";
    
    // Absences
    public const string ABSENCE_NOT_FOUND = "ABSENCE_NOT_FOUND";
    public const string ABSENCE_OVERLAP = "ABSENCE_OVERLAP";
    public const string ABSENCE_INVALID_DATES = "ABSENCE_INVALID_DATES";
    public const string ABSENCE_CANNOT_MODIFY = "ABSENCE_CANNOT_MODIFY";
    
    // Sync Status
    public const string CANNOT_MODIFY_SYNCED = "SYNC_CANNOT_MODIFY_SYNCED";
    public const string CANNOT_MODIFY_APPROVED = "SYNC_CANNOT_MODIFY_APPROVED";
    public const string SYNC_FAILED = "SYNC_FAILED";
    public const string SYNC_CONFLICT = "SYNC_CONFLICT";
    
    // Validation
    public const string VALIDATION_FAILED = "VAL_FAILED";
    public const string INVALID_EMAIL_FORMAT = "VAL_INVALID_EMAIL";
    public const string INVALID_PHONE_FORMAT = "VAL_INVALID_PHONE";
    public const string INVALID_DATE_RANGE = "VAL_INVALID_DATE_RANGE";
    public const string REQUIRED_FIELD_MISSING = "VAL_REQUIRED_FIELD";
    public const string VALUE_OUT_OF_RANGE = "VAL_OUT_OF_RANGE";
    
    // File Operations
    public const string FILE_NOT_FOUND = "FILE_NOT_FOUND";
    public const string FILE_TOO_LARGE = "FILE_TOO_LARGE";
    public const string FILE_INVALID_FORMAT = "FILE_INVALID_FORMAT";
    public const string FILE_UPLOAD_FAILED = "FILE_UPLOAD_FAILED";
    
    // General
    public const string INTERNAL_ERROR = "INTERNAL_ERROR";
    public const string OPERATION_TIMEOUT = "OPERATION_TIMEOUT";
    public const string INVALID_OPERATION = "INVALID_OPERATION";
    public const string RESOURCE_NOT_FOUND = "RESOURCE_NOT_FOUND";
    public const string CONCURRENCY_CONFLICT = "CONCURRENCY_CONFLICT";
}
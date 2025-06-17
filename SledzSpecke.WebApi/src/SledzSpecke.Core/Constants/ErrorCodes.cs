namespace SledzSpecke.Core.Constants;

public static class ErrorCodes
{
    // General Errors
    public const string VALIDATION_ERROR = "VALIDATION_ERROR";
    public const string INTERNAL_ERROR = "INTERNAL_ERROR";
    public const string NOT_FOUND = "NOT_FOUND";
    public const string ALREADY_EXISTS = "ALREADY_EXISTS";
    public const string UNAUTHORIZED = "UNAUTHORIZED";
    public const string FORBIDDEN = "FORBIDDEN";
    public const string INVALID_OPERATION = "INVALID_OPERATION";
    
    // Entity Specific Errors
    public const string USER_NOT_FOUND = "USER_NOT_FOUND";
    public const string USER_ALREADY_EXISTS = "USER_ALREADY_EXISTS";
    public const string INVALID_CREDENTIALS = "INVALID_CREDENTIALS";
    
    // Medical Shifts
    public const string SHIFT_NOT_FOUND = "SHIFT_NOT_FOUND";
    public const string SHIFT_ALREADY_EXISTS = "SHIFT_ALREADY_EXISTS";
    public const string INVALID_SHIFT_DURATION = "INVALID_SHIFT_DURATION";
    public const string SHIFT_OUTSIDE_INTERNSHIP = "SHIFT_OUTSIDE_INTERNSHIP";
    
    // Internships
    public const string INTERNSHIP_NOT_FOUND = "INTERNSHIP_NOT_FOUND";
    public const string INTERNSHIP_ALREADY_COMPLETED = "INTERNSHIP_ALREADY_COMPLETED";
    public const string INTERNSHIP_OVERLAP = "INTERNSHIP_OVERLAP";
    
    // Specializations
    public const string SPECIALIZATION_NOT_FOUND = "SPECIALIZATION_NOT_FOUND";
    public const string INVALID_SPECIALIZATION = "INVALID_SPECIALIZATION";
    
    // SMK Integration
    public const string SMK_SYNC_FAILED = "SMK_SYNC_FAILED";
    public const string SMK_INVALID_VERSION = "SMK_INVALID_VERSION";
    public const string SMK_MODULE_NOT_FOUND = "SMK_MODULE_NOT_FOUND";
    
    // Publications
    public const string PUBLICATION_NOT_FOUND = "PUBLICATION_NOT_FOUND";
    public const string INVALID_PUBLICATION_TYPE = "INVALID_PUBLICATION_TYPE";
}
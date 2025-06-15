namespace SledzSpecke.Api.Models;

/// <summary>
/// Detailed error response for improved error tracking and monitoring
/// </summary>
public class DetailedErrorResponse
{
    /// <summary>
    /// Machine-readable error code (e.g., "USER_REGISTRATION_FAILED")
    /// </summary>
    public string ErrorCode { get; set; } = string.Empty;
    
    /// <summary>
    /// User-friendly error message in the requested language
    /// </summary>
    public string UserMessage { get; set; } = string.Empty;
    
    /// <summary>
    /// Technical error details for developers (only in non-production)
    /// </summary>
    public string? TechnicalMessage { get; set; }
    
    /// <summary>
    /// Unique correlation ID for tracking this error across systems
    /// </summary>
    public string CorrelationId { get; set; } = string.Empty;
    
    /// <summary>
    /// Timestamp when the error occurred
    /// </summary>
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    
    /// <summary>
    /// Additional context information
    /// </summary>
    public Dictionary<string, object>? Details { get; set; }
    
    /// <summary>
    /// Stack trace (only in development)
    /// </summary>
    public string? StackTrace { get; set; }
    
    /// <summary>
    /// The request path that caused the error
    /// </summary>
    public string? Path { get; set; }
    
    /// <summary>
    /// HTTP method used
    /// </summary>
    public string? Method { get; set; }
    
    /// <summary>
    /// Creates a detailed error response
    /// </summary>
    public static DetailedErrorResponse Create(
        string errorCode,
        string userMessage,
        string correlationId,
        string? technicalMessage = null,
        Dictionary<string, object>? details = null)
    {
        return new DetailedErrorResponse
        {
            ErrorCode = errorCode,
            UserMessage = userMessage,
            TechnicalMessage = technicalMessage,
            CorrelationId = correlationId,
            Details = details,
            Timestamp = DateTimeOffset.UtcNow
        };
    }
}
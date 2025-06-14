using SledzSpecke.Application.Helpers;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.DTO;

public record MedicalShiftDto(
    int Id,
    int InternshipId,
    DateTime Date,
    int Hours,
    int Minutes,
    string Location,
    int Year,
    SyncStatus SyncStatus,
    string? AdditionalFields,
    DateTime? ApprovalDate,
    string? ApproverName,
    string? ApproverRole,
    bool IsApproved,
    bool CanBeDeleted,
    TimeSpan Duration
)
{
    /// <summary>
    /// Formatted time display similar to MAUI's FormattedTime property.
    /// Shows as "X godz. Y min." with normalized time.
    /// </summary>
    public string FormattedTime => TimeNormalizationHelper.FormatTime(Hours, Minutes, normalize: true);
    
    /// <summary>
    /// Total duration in decimal hours (e.g., 2.5 for 2h 30m).
    /// Used for statistics and summaries.
    /// </summary>
    public double TotalHours => TimeNormalizationHelper.CalculateTotalHours(Hours, Minutes);
}
namespace SledzSpecke.Application.Helpers;

/// <summary>
/// Helper class for time normalization, similar to MAUI's MedicalShiftsSummary.NormalizeTime()
/// Converts excess minutes (>= 60) to hours for display and summary purposes.
/// Individual shift records retain their original values.
/// </summary>
public static class TimeNormalizationHelper
{
    /// <summary>
    /// Normalizes time by converting excess minutes to hours.
    /// For example: 2 hours 90 minutes becomes 3 hours 30 minutes.
    /// </summary>
    /// <param name="hours">Original hours</param>
    /// <param name="minutes">Original minutes (can be > 59)</param>
    /// <returns>Tuple of normalized (hours, minutes)</returns>
    public static (int hours, int minutes) NormalizeTime(int hours, int minutes)
    {
        if (minutes >= 60)
        {
            hours += minutes / 60;
            minutes = minutes % 60;
        }
        
        return (hours, minutes);
    }
    
    /// <summary>
    /// Calculates total hours as a decimal value.
    /// For example: 2 hours 30 minutes = 2.5 hours
    /// </summary>
    /// <param name="hours">Hours component</param>
    /// <param name="minutes">Minutes component</param>
    /// <returns>Total hours as decimal</returns>
    public static double CalculateTotalHours(int hours, int minutes)
    {
        return hours + (minutes / 60.0);
    }
    
    /// <summary>
    /// Formats time for display similar to MAUI's FormattedTime property.
    /// </summary>
    /// <param name="hours">Hours component</param>
    /// <param name="minutes">Minutes component</param>
    /// <param name="normalize">Whether to normalize time before formatting</param>
    /// <returns>Formatted string like "2 godz. 30 min."</returns>
    public static string FormatTime(int hours, int minutes, bool normalize = false)
    {
        if (normalize)
        {
            (hours, minutes) = NormalizeTime(hours, minutes);
        }
        
        return $"{hours} godz. {minutes} min.";
    }
}
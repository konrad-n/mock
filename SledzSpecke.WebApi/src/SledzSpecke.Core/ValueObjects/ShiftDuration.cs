using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

/// <summary>
/// Represents the duration of a medical shift.
/// Stores total minutes internally to support SMK requirements where minutes can exceed 59.
/// </summary>
public sealed record ShiftDuration
{
    private const int MinimumDurationMinutes = 60;
    
    public int TotalMinutes { get; }
    
    /// <summary>
    /// Creates a ShiftDuration from total minutes.
    /// This is the primary constructor that ensures minimum duration validation.
    /// </summary>
    private ShiftDuration(int totalMinutes)
    {
        if (totalMinutes < MinimumDurationMinutes)
        {
            throw new InvalidShiftDurationException(
                $"Minimum shift duration is {MinimumDurationMinutes} minutes. Provided: {totalMinutes} minutes");
        }
        
        TotalMinutes = totalMinutes;
    }
    
    /// <summary>
    /// Creates a ShiftDuration from hours and minutes.
    /// Allows minutes > 59 as per SMK requirements.
    /// </summary>
    public ShiftDuration(int hours, int minutes)
    {
        if (hours < 0)
        {
            throw new InvalidShiftDurationException("Hours cannot be negative.");
        }

        if (minutes < 0)
        {
            throw new InvalidShiftDurationException("Minutes cannot be negative.");
        }

        var totalMinutes = hours * 60 + minutes;
        if (totalMinutes < MinimumDurationMinutes)
        {
            throw new InvalidShiftDurationException(
                $"Minimum shift duration is {MinimumDurationMinutes} minutes. Provided: {hours}h {minutes}min = {totalMinutes} minutes");
        }
        
        TotalMinutes = totalMinutes;
    }
    
    /// <summary>
    /// Gets the hours component for display purposes.
    /// </summary>
    public int Hours => TotalMinutes / 60;
    
    /// <summary>
    /// Gets the minutes component for display purposes.
    /// Note: This can be > 59 if the shift was entered that way.
    /// </summary>
    public int Minutes => TotalMinutes % 60;
    
    public TimeSpan ToTimeSpan() => new(Hours, Minutes, 0);
    
    public ShiftDuration Normalize()
    {
        var totalMinutes = TotalMinutes;
        var normalizedHours = totalMinutes / 60;
        var normalizedMinutes = totalMinutes % 60;
        return new ShiftDuration(normalizedHours, normalizedMinutes);
    }
    
    public override string ToString() => $"{Hours:D2}:{Minutes:D2}";
    
    public string ToNormalizedString()
    {
        var normalized = Normalize();
        return $"{normalized.Hours:D2}:{normalized.Minutes:D2}";
    }
    
    /// <summary>
    /// Converts duration to SMK export format (HH:MM).
    /// </summary>
    public string ToSmkFormat() => $"{Hours:D2}:{Minutes:D2}";
    
    /// <summary>
    /// Converts duration to display format.
    /// Example: "2h 15min" or "3h 0min"
    /// </summary>
    public string ToDisplayFormat() => $"{Hours}h {Minutes}min";
    
    /// <summary>
    /// Creates a ShiftDuration from total minutes.
    /// </summary>
    public static ShiftDuration FromMinutes(int totalMinutes)
    {
        return new ShiftDuration(totalMinutes);
    }
    
    /// <summary>
    /// Creates a ShiftDuration from a TimeSpan.
    /// </summary>
    public static ShiftDuration FromTimeSpan(TimeSpan timeSpan)
    {
        var totalMinutes = (int)timeSpan.TotalMinutes;
        return new ShiftDuration(totalMinutes);
    }
    
    /// <summary>
    /// Adds two shift durations together.
    /// </summary>
    public static ShiftDuration operator +(ShiftDuration left, ShiftDuration right)
    {
        return new ShiftDuration(left.TotalMinutes + right.TotalMinutes);
    }
    
    /// <summary>
    /// Subtracts one shift duration from another.
    /// </summary>
    public static ShiftDuration operator -(ShiftDuration left, ShiftDuration right)
    {
        return new ShiftDuration(left.TotalMinutes - right.TotalMinutes);
    }
    
    // Comparison operators
    public static bool operator <(ShiftDuration left, ShiftDuration right) 
        => left.TotalMinutes < right.TotalMinutes;
        
    public static bool operator >(ShiftDuration left, ShiftDuration right) 
        => left.TotalMinutes > right.TotalMinutes;
        
    public static bool operator <=(ShiftDuration left, ShiftDuration right) 
        => left.TotalMinutes <= right.TotalMinutes;
        
    public static bool operator >=(ShiftDuration left, ShiftDuration right) 
        => left.TotalMinutes >= right.TotalMinutes;
}
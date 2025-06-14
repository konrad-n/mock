using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Core.ValueObjects;

public sealed record ShiftDuration
{
    public int Hours { get; }
    public int Minutes { get; }
    
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

        // Allow minutes > 59 for special cases (will be normalized at display level)
        Hours = hours;
        Minutes = minutes;
    }

    public int TotalMinutes => Hours * 60 + Minutes;
    
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
    
    public static ShiftDuration Zero => new(0, 0);
    
    public static ShiftDuration FromTimeSpan(TimeSpan timeSpan)
    {
        return new ShiftDuration((int)timeSpan.TotalHours, timeSpan.Minutes);
    }
    
    public static ShiftDuration FromMinutes(int totalMinutes)
    {
        if (totalMinutes < 0)
        {
            throw new InvalidShiftDurationException("Total minutes cannot be negative.");
        }
        
        return new ShiftDuration(totalMinutes / 60, totalMinutes % 60);
    }
}
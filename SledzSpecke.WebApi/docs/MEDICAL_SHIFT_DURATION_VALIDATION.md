# Medical Shift Duration Validation

## Overview

The medical shift duration validation has been aligned with the MAUI implementation to provide consistent behavior across platforms.

## Key Principles

### 1. Minimal Validation
- Only ensures that the total duration (hours + minutes) is greater than zero
- No maximum duration limits are enforced
- Negative values are not allowed

### 2. Flexible Minutes Input
- Minutes are NOT restricted to 0-59
- Users can enter values like 90 minutes, 120 minutes, etc.
- This flexibility allows for easier data entry

### 3. Time Normalization
- Normalization (converting excess minutes to hours) happens only at the display/summary level
- Individual shift records retain their original values
- Example: A shift with 2 hours 90 minutes is stored as-is, but displayed as 3 hours 30 minutes

## Implementation Details

### Validation Rules

#### Common Rules (Both SMK Versions)
- Hours >= 0
- Minutes >= 0  
- Total duration (hours + minutes) > 0
- Location is required and <= 100 characters

#### Old SMK Specific
- Year must be between 1 and 5
- Default initialization: 24 hours, 0 minutes (matching MAUI)

#### New SMK Specific
- Year must be provided (> 0)
- Default initialization: 10 hours, 5 minutes (matching MAUI)
- Shift date must be within internship period

### Removed Restrictions

The following restrictions were removed to match MAUI behavior:
- Maximum 24-hour limit for Old SMK
- Maximum 16-hour limit for New SMK
- Minimum 4-hour requirement for New SMK
- Minutes limited to 0-59

### Time Calculation

Total hours for statistics and summaries:
```csharp
double totalHours = hours + (minutes / 60.0);
```

### Display Format

Time is displayed in Polish format:
```
"X godz. Y min."
```

With normalization applied for display (but not storage).

## Helper Classes

### TimeNormalizationHelper

Located in `/src/SledzSpecke.Application/Helpers/TimeNormalizationHelper.cs`

Provides methods for:
- `NormalizeTime(hours, minutes)` - Converts excess minutes to hours
- `CalculateTotalHours(hours, minutes)` - Returns decimal hours
- `FormatTime(hours, minutes, normalize)` - Formats for display

### MedicalShiftDto

Enhanced with computed properties:
- `FormattedTime` - Normalized time display
- `TotalHours` - Decimal hours for calculations

## Migration Notes

When migrating from the previous strict validation:
1. Existing data with minutes > 59 will now be valid
2. Shifts longer than previous limits are now allowed
3. UI should handle display normalization, not enforce input restrictions

## Testing Considerations

Test cases should include:
- Zero duration validation (should fail)
- Minutes > 59 (should pass)
- Very long shifts (e.g., 36 hours)
- Negative values (should fail)
- Time normalization in summaries
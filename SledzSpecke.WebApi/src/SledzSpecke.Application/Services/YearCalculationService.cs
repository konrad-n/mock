using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Services;

/// <summary>
/// Service for calculating year information based on specialization structure.
/// Implements MAUI-compatible year calculation logic.
/// </summary>
public class YearCalculationService : IYearCalculationService
{
    /// <summary>
    /// Gets the available years for a specialization based on its structure.
    /// For Old SMK: Returns years 1-6 (or less based on duration)
    /// For New SMK: Returns empty array (module-based, no years)
    /// </summary>
    public int[] GetAvailableYears(Specialization specialization)
    {
        if (specialization.SmkVersion == SmkVersion.New)
        {
            // New SMK doesn't use years
            return Array.Empty<int>();
        }

        // Calculate total years based on specialization duration
        int totalYears = specialization.DurationYears;
        
        // If there are additional months, add one more year
        var totalDuration = specialization.PlannedEndDate - specialization.StartDate;
        if (totalDuration.Days % 365 > 30) // More than a month of extra days
        {
            totalYears++;
        }

        // Limit to standard medical education years (1-6)
        totalYears = Math.Max(1, Math.Min(6, totalYears));

        // Return array of available years
        return Enumerable.Range(1, totalYears).ToArray();
    }

    /// <summary>
    /// Gets the year range for a specific module in Old SMK.
    /// Basic module: Years 1-2
    /// Specialistic module: Years 3-6 (or 1-6 if no basic module)
    /// </summary>
    public (int startYear, int endYear) GetModuleYearRange(Module module, Specialization specialization)
    {
        if (specialization.SmkVersion == SmkVersion.New)
        {
            // New SMK doesn't use years
            return (0, 0);
        }

        var hasBasicModule = specialization.Modules.Any(m => m.Type == ModuleType.Basic);
        
        if (module.Type == ModuleType.Basic)
        {
            return (1, 2);
        }
        else if (module.Type == ModuleType.Specialistic)
        {
            if (hasBasicModule)
            {
                return (3, 6);
            }
            else
            {
                // If no basic module, specialistic covers all years
                return (1, 6);
            }
        }

        return (1, 6); // Default range
    }

    /// <summary>
    /// Validates if a year is valid for a given module.
    /// Also allows year 0 for unassigned items (Old SMK only).
    /// </summary>
    public bool IsYearValidForModule(int year, Module module, Specialization specialization)
    {
        if (specialization.SmkVersion == SmkVersion.New)
        {
            // New SMK doesn't use years
            return year == 0;
        }

        // Year 0 is always valid for unassigned items
        if (year == 0)
        {
            return true;
        }

        var (startYear, endYear) = GetModuleYearRange(module, specialization);
        return year >= startYear && year <= endYear;
    }

    /// <summary>
    /// Gets the current year based on elapsed time from specialization start.
    /// Note: MAUI doesn't auto-calculate this - users manually select their year.
    /// This method is provided for reference/reporting purposes.
    /// </summary>
    public int CalculateCurrentYear(Specialization specialization, DateTime? referenceDate = null)
    {
        if (specialization.SmkVersion == SmkVersion.New)
        {
            // New SMK doesn't use years
            return 0;
        }

        var currentDate = referenceDate ?? DateTime.UtcNow;
        var elapsedTime = currentDate - specialization.StartDate;
        
        // Calculate year based on elapsed time (365 days per year)
        int currentYear = (int)(elapsedTime.TotalDays / 365) + 1;
        
        // Limit to available years
        var availableYears = GetAvailableYears(specialization);
        if (availableYears.Length > 0)
        {
            currentYear = Math.Min(currentYear, availableYears.Max());
        }

        return Math.Max(1, currentYear);
    }

    /// <summary>
    /// Gets the module that should be active for a given year.
    /// </summary>
    public Module? GetModuleForYear(int year, Specialization specialization)
    {
        if (specialization.SmkVersion == SmkVersion.New)
        {
            // New SMK uses current module, not year-based
            return specialization.Modules.FirstOrDefault(m => m.Id == specialization.CurrentModuleId);
        }

        // Find the module that contains this year
        foreach (var module in specialization.Modules)
        {
            var (startYear, endYear) = GetModuleYearRange(module, specialization);
            if (year >= startYear && year <= endYear)
            {
                return module;
            }
        }

        return null;
    }

    /// <summary>
    /// Determines if an entity (procedure, shift) should be counted in year-based statistics.
    /// </summary>
    public bool ShouldIncludeInYearStatistics(int entityYear, int targetYear, SmkVersion smkVersion)
    {
        if (smkVersion == SmkVersion.New)
        {
            // New SMK doesn't use year-based statistics
            return false;
        }

        // Include if it matches the target year or if it's unassigned (year 0)
        return entityYear == targetYear || (entityYear == 0 && targetYear > 0);
    }
}
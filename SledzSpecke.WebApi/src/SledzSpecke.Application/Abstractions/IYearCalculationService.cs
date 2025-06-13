using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Abstractions;

/// <summary>
/// Service for calculating year information based on specialization structure.
/// </summary>
public interface IYearCalculationService
{
    /// <summary>
    /// Gets the available years for a specialization.
    /// </summary>
    int[] GetAvailableYears(Specialization specialization);

    /// <summary>
    /// Gets the year range for a specific module.
    /// </summary>
    (int startYear, int endYear) GetModuleYearRange(Module module, Specialization specialization);

    /// <summary>
    /// Validates if a year is valid for a given module.
    /// </summary>
    bool IsYearValidForModule(int year, Module module, Specialization specialization);

    /// <summary>
    /// Calculates the current year based on elapsed time.
    /// </summary>
    int CalculateCurrentYear(Specialization specialization, DateTime? referenceDate = null);

    /// <summary>
    /// Gets the module that should be active for a given year.
    /// </summary>
    Module? GetModuleForYear(int year, Specialization specialization);

    /// <summary>
    /// Determines if an entity should be counted in year-based statistics.
    /// </summary>
    bool ShouldIncludeInYearStatistics(int entityYear, int targetYear, SmkVersion smkVersion);
}
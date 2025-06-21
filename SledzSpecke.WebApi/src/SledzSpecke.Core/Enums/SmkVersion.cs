namespace SledzSpecke.Core.Enums;

/// <summary>
/// Represents the SMK (System Monitorowania Kształcenia) version.
/// Determines which set of rules and data structures to use.
/// </summary>
public enum SmkVersion
{
    /// <summary>
    /// Old SMK system (pre-2023)
    /// </summary>
    Old = 0,

    /// <summary>
    /// New SMK system (2023 and later)
    /// </summary>
    New = 1
}
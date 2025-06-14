namespace SledzSpecke.Core.Options;

public class SmkOptions
{
    public const string SectionName = "Smk";

    public int MaxShiftHours { get; set; } = 24;
    public int MinShiftMinutes { get; set; } = 15;
    public int MaxProceduresPerDay { get; set; } = 20;
    public int MaxYearForOldSmk { get; set; } = 6;
    public int MinYearForSpecialist { get; set; } = 3;
    public bool AllowFutureDates { get; set; } = true;
    public bool RequireApprovalForSync { get; set; } = false;
    public int DaysBeforeAutoSync { get; set; } = 7;
}

public class ValidationOptions
{
    public const string SectionName = "Validation";

    public bool StrictProcedureCodeValidation { get; set; } = true;
    public bool RequirePatientData { get; set; } = false;
    public bool ValidateAgainstTemplate { get; set; } = true;
    public int MaxLocationLength { get; set; } = 100;
    public int MaxCodeLength { get; set; } = 20;
}

public class FeatureFlags
{
    public const string SectionName = "FeatureFlags";

    public bool EnableDomainEvents { get; set; } = true;
    public bool EnableDetailedLogging { get; set; } = false;
    public bool EnableAutoSync { get; set; } = false;
    public bool EnableProcedureTemplateValidation { get; set; } = true;
    public bool EnableYearCalculationService { get; set; } = true;
}
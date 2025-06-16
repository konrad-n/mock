using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Policies.MedicalShift;

public class OldSmkMedicalShiftPolicy : ISmkPolicy<Entities.MedicalShift>
{
    private const int MinimumShiftDurationMinutes = 120; // 2 hours
    private const int MaximumDailyHours = 24;
    private const int MaximumWeeklyHours = 48;
    private const int MinimumMonthlyHours = 160;

    public SmkVersion ApplicableVersion => SmkVersion.Old;

    public Result Validate(Entities.MedicalShift shift, SpecializationContext context)
    {
        // Validate minimum duration
        if (shift.Duration.TotalMinutes < MinimumShiftDurationMinutes)
        {
            return Result.Failure($"Dyżur musi trwać minimum {MinimumShiftDurationMinutes / 60} godziny");
        }

        // Validate maximum daily duration
        if ((shift.Duration.TotalMinutes / 60.0) > MaximumDailyHours)
        {
            return Result.Failure($"Dyżur nie może przekraczać {MaximumDailyHours} godzin");
        }

        // Additional Old SMK specific validations can be added here
        // For example: Year-based restrictions, department-specific rules, etc.

        return Result.Success();
    }

    public static class ErrorCodes
    {
        public const string MinimumDurationNotMet = "SMK_001";
        public const string ExceedsDailyLimit = "SMK_002";
        public const string ExceedsWeeklyLimit = "SMK_003";
        public const string BelowMonthlyMinimum = "SMK_004";
    }
}
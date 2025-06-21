using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.Policies.MedicalShift;

public class NewSmkMedicalShiftPolicy : ISmkPolicy<Entities.MedicalShift>
{
    private const int MinimumShiftDurationMinutes = 60; // 1 hour
    private const int MaximumDailyHours = 24;
    private const int MaximumWeeklyHours = 48;

    public SmkVersion ApplicableVersion => SmkVersion.New;

    public Result Validate(Entities.MedicalShift shift, SpecializationContext context)
    {
        // Calculate total minutes from Hours and Minutes
        var totalMinutes = shift.Hours * 60 + shift.Minutes;
        
        // Validate minimum duration
        if (totalMinutes < MinimumShiftDurationMinutes)
        {
            return Result.Failure($"Dyżur musi trwać minimum {MinimumShiftDurationMinutes / 60} godzinę");
        }

        // Validate maximum daily duration
        if ((totalMinutes / 60.0) > MaximumDailyHours)
        {
            return Result.Failure($"Dyżur nie może przekraczać {MaximumDailyHours} godzin");
        }

        // New SMK requires supervisor approval
        if (!shift.IsApproved)
        {
            return Result.Failure("Dyżur wymaga zatwierdzenia przez opiekuna");
        }

        // Validate module context
        if (context.CurrentModuleId == null)
        {
            return Result.Failure("Brak aktywnego modułu dla nowego systemu SMK");
        }

        return Result.Success();
    }

    public static class ErrorCodes
    {
        public const string MinimumDurationNotMet = "SMK_001";
        public const string ExceedsDailyLimit = "SMK_002";
        public const string ExceedsWeeklyLimit = "SMK_003";
        public const string SupervisorApprovalRequired = "SMK_005";
        public const string ModuleNotActive = "SMK_004";
    }
}
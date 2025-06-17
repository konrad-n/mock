using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Policies;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.DomainServices;

public class MedicalShiftValidationService : IMedicalShiftValidationService
{
    private readonly IMedicalShiftRepository _shiftRepository;
    private readonly ISmkPolicyFactory _policyFactory;
    private readonly ISpecializationRepository _specializationRepository;
    
    private const int WeeklyHoursLimit = 48;
    private const int OldSmkMonthlyMinimum = 160;

    public MedicalShiftValidationService(
        IMedicalShiftRepository shiftRepository,
        ISmkPolicyFactory policyFactory,
        ISpecializationRepository specializationRepository)
    {
        _shiftRepository = shiftRepository;
        _policyFactory = policyFactory;
        _specializationRepository = specializationRepository;
    }

    public async Task<Result> ValidateShiftAsync(
        MedicalShift shift,
        UserId userId,
        Specialization specialization,
        ModuleId? moduleId,
        IEnumerable<MedicalShift> existingShifts)
    {
        // Create context for policy validation
        var context = new SpecializationContext(
            specialization.Id,
            userId,
            specialization.SmkVersion,
            moduleId,
            shift.Date);

        // Apply version-specific policy
        var policy = _policyFactory.GetPolicy<MedicalShift>(specialization.SmkVersion);
        var policyResult = policy.Validate(shift, context);
        
        if (!policyResult.IsSuccess)
        {
            return policyResult;
        }

        // Check for overlapping shifts
        var overlapResult = await CheckOverlappingShiftsAsync(shift, existingShifts);
        if (!overlapResult.IsSuccess || overlapResult.Value)
        {
            return Result.Failure("Dyżur nakłada się z istniejącym dyżurem");
        }

        // Check weekly limit
        var week = new Week(shift.Date);
        var weeklyTotal = await CalculateWeeklyTotalAsync(userId, week);
        
        if (weeklyTotal.IsSuccess)
        {
            var totalMinutesWithNewShift = weeklyTotal.Value.TotalMinutes + shift.Duration.TotalMinutes;
            
            if (totalMinutesWithNewShift > WeeklyHoursLimit * 60)
            {
                return Result.Failure($"Przekroczono tygodniowy limit {WeeklyHoursLimit} godzin");
            }
        }

        return Result.Success();
    }

    public async Task<Result<Duration>> CalculateMonthlyTotalAsync(
        UserId userId,
        YearMonth yearMonth)
    {
        var shifts = await _shiftRepository.GetByUserIdAndDateRangeAsync(
            userId,
            yearMonth.StartDate,
            yearMonth.EndDate);

        var totalMinutes = shifts.Sum(s => s.Duration.TotalMinutes);
        if (totalMinutes == 0)
            return Result<Duration>.Success(new Duration(0, 1)); // Minimum duration
        return Result<Duration>.Success(Duration.FromMinutes(totalMinutes));
    }

    public async Task<Result<Duration>> CalculateWeeklyTotalAsync(
        UserId userId,
        Week week)
    {
        var shifts = await _shiftRepository.GetByUserIdAndDateRangeAsync(
            userId,
            week.StartDate,
            week.EndDate);

        var totalMinutes = shifts.Sum(s => s.Duration.TotalMinutes);
        if (totalMinutes == 0)
            return Result<Duration>.Success(new Duration(0, 1)); // Minimum duration
        return Result<Duration>.Success(Duration.FromMinutes(totalMinutes));
    }

    public Result<bool> CheckOverlappingShifts(
        MedicalShift newShift,
        IEnumerable<MedicalShift> existingShifts)
    {
        var newShiftEnd = newShift.Date.AddMinutes(newShift.Duration.TotalMinutes);

        foreach (var existingShift in existingShifts)
        {
            if (existingShift.Id == newShift.Id)
                continue;

            var existingShiftEnd = existingShift.Date.AddMinutes(existingShift.Duration.TotalMinutes);

            // Check for overlap
            if (newShift.Date < existingShiftEnd && newShiftEnd > existingShift.Date)
            {
                return Result<bool>.Success(true); // Overlap found
            }
        }

        return Result<bool>.Success(false); // No overlap
    }

    public async Task<Result<MedicalShiftValidationSummary>> GetValidationSummaryAsync(
        UserId userId,
        DateTime date)
    {
        var summary = new MedicalShiftValidationSummary
        {
            WeeklyHoursLimit = WeeklyHoursLimit
        };

        // Get user's specialization to determine SMK version
        var specializations = await _specializationRepository.GetByUserIdAsync(userId);
        var activeSpecialization = specializations.FirstOrDefault();
        
        if (activeSpecialization == null)
        {
            return Result<MedicalShiftValidationSummary>.Failure("Brak aktywnej specjalizacji");
        }

        // Set monthly minimum based on SMK version
        summary.MonthlyHoursMinimum = activeSpecialization.SmkVersion == SmkVersion.Old 
            ? OldSmkMonthlyMinimum 
            : GetNewSmkMonthlyMinimum(activeSpecialization);

        // Calculate weekly total
        var week = new Week(date);
        var weeklyResult = await CalculateWeeklyTotalAsync(userId, week);
        if (weeklyResult.IsSuccess)
        {
            summary.WeeklyTotal = weeklyResult.Value;
            summary.ExceedsWeeklyLimit = (summary.WeeklyTotal.TotalMinutes / 60.0) > WeeklyHoursLimit;
            
            if (summary.ExceedsWeeklyLimit)
            {
                summary.ValidationErrors.Add($"Przekroczono tygodniowy limit {WeeklyHoursLimit} godzin");
            }
        }

        // Calculate monthly total
        var yearMonth = new YearMonth(date.Year, date.Month);
        var monthlyResult = await CalculateMonthlyTotalAsync(userId, yearMonth);
        if (monthlyResult.IsSuccess)
        {
            summary.MonthlyTotal = monthlyResult.Value;
            summary.BelowMonthlyMinimum = (summary.MonthlyTotal.TotalMinutes / 60.0) < summary.MonthlyHoursMinimum;
            
            if (summary.BelowMonthlyMinimum && date >= yearMonth.EndDate)
            {
                summary.ValidationWarnings.Add($"Poniżej miesięcznego minimum {summary.MonthlyHoursMinimum} godzin");
            }
        }

        return Result<MedicalShiftValidationSummary>.Success(summary);
    }

    private int GetNewSmkMonthlyMinimum(Specialization specialization)
    {
        // In New SMK, monthly minimums vary by module
        // This would be configured per module in the template
        // For now, returning a default value
        return 140;
    }
}
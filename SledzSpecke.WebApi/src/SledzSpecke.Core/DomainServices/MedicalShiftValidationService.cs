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
            new SpecializationId(specialization.SpecializationId),
            userId,
            specialization.SmkVersion == Enums.SmkVersion.Old ? ValueObjects.SmkVersion.Old : ValueObjects.SmkVersion.New,
            moduleId,
            shift.Date);

        // Apply version-specific policy
        var smkVersion = specialization.SmkVersion == Enums.SmkVersion.Old ? ValueObjects.SmkVersion.Old : ValueObjects.SmkVersion.New;
        var policy = _policyFactory.GetPolicy<MedicalShift>(smkVersion);
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
            var totalMinutesWithNewShift = weeklyTotal.Value.TotalMinutes + (shift.Hours * 60 + shift.Minutes);
            
            if (totalMinutesWithNewShift > WeeklyHoursLimit * 60)
            {
                return Result.Failure($"Przekroczono tygodniowy limit {WeeklyHoursLimit} godzin");
            }
        }

        return Result.Success();
    }

    public async Task<Result<ShiftDuration>> CalculateMonthlyTotalAsync(
        UserId userId,
        YearMonth yearMonth)
    {
        var shifts = await _shiftRepository.GetByUserIdAndDateRangeAsync(
            userId,
            yearMonth.StartDate,
            yearMonth.EndDate);

        var totalMinutes = shifts.Sum(s => s.Hours * 60 + s.Minutes);
        if (totalMinutes == 0)
            return Result<ShiftDuration>.Success(new ShiftDuration(1, 0)); // Minimum 60 minutes
        return Result<ShiftDuration>.Success(ShiftDuration.FromMinutes(totalMinutes));
    }

    public async Task<Result<ShiftDuration>> CalculateWeeklyTotalAsync(
        UserId userId,
        Week week)
    {
        var shifts = await _shiftRepository.GetByUserIdAndDateRangeAsync(
            userId,
            week.StartDate,
            week.EndDate);

        var totalMinutes = shifts.Sum(s => s.Hours * 60 + s.Minutes);
        if (totalMinutes == 0)
            return Result<ShiftDuration>.Success(new ShiftDuration(1, 0)); // Minimum 60 minutes
        return Result<ShiftDuration>.Success(ShiftDuration.FromMinutes(totalMinutes));
    }

    public async Task<Result<bool>> CheckOverlappingShiftsAsync(
        MedicalShift newShift,
        IEnumerable<MedicalShift> existingShifts)
    {
        var newShiftEnd = newShift.Date.AddMinutes(newShift.Hours * 60 + newShift.Minutes);

        foreach (var existingShift in existingShifts)
        {
            if (existingShift.ShiftId == newShift.ShiftId)
                continue;

            var existingShiftEnd = existingShift.Date.AddMinutes(existingShift.Hours * 60 + existingShift.Minutes);

            // Check for overlap
            if (newShift.Date < existingShiftEnd && newShiftEnd > existingShift.Date)
            {
                return await Task.FromResult(Result<bool>.Success(true)); // Overlap found
            }
        }

        return await Task.FromResult(Result<bool>.Success(false)); // No overlap
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
        summary.MonthlyHoursMinimum = activeSpecialization.SmkVersion == Enums.SmkVersion.Old
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
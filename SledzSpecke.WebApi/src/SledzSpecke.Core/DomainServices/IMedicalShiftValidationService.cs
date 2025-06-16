using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.DomainServices;

public interface IMedicalShiftValidationService
{
    Task<Result> ValidateShiftAsync(
        MedicalShift shift,
        UserId userId,
        Specialization specialization,
        ModuleId? moduleId,
        IEnumerable<MedicalShift> existingShifts);
    
    Task<Result<Duration>> CalculateMonthlyTotalAsync(
        UserId userId,
        YearMonth yearMonth);
    
    Task<Result<Duration>> CalculateWeeklyTotalAsync(
        UserId userId,
        Week week);
    
    Task<Result<bool>> CheckOverlappingShiftsAsync(
        MedicalShift newShift,
        IEnumerable<MedicalShift> existingShifts);
    
    Task<Result<MedicalShiftValidationSummary>> GetValidationSummaryAsync(
        UserId userId,
        DateTime date);
}

public class YearMonth
{
    public int Year { get; }
    public int Month { get; }
    
    public YearMonth(int year, int month)
    {
        if (year < 2000 || year > 2100)
            throw new ArgumentException("Invalid year");
        if (month < 1 || month > 12)
            throw new ArgumentException("Invalid month");
            
        Year = year;
        Month = month;
    }
    
    public DateTime StartDate => new DateTime(Year, Month, 1);
    public DateTime EndDate => StartDate.AddMonths(1).AddDays(-1);
}

public class Week
{
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }
    
    public Week(DateTime date)
    {
        // Week starts on Monday
        var dayOfWeek = (int)date.DayOfWeek;
        var daysToMonday = dayOfWeek == 0 ? -6 : 1 - dayOfWeek;
        
        StartDate = date.AddDays(daysToMonday).Date;
        EndDate = StartDate.AddDays(6).Date.AddHours(23).AddMinutes(59).AddSeconds(59);
    }
}

public class MedicalShiftValidationSummary
{
    public Duration WeeklyTotal { get; set; }
    public Duration MonthlyTotal { get; set; }
    public int WeeklyHoursLimit { get; set; }
    public int MonthlyHoursMinimum { get; set; }
    public bool ExceedsWeeklyLimit { get; set; }
    public bool BelowMonthlyMinimum { get; set; }
    public List<string> ValidationErrors { get; set; } = new();
    public List<string> ValidationWarnings { get; set; } = new();
}
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Core.DomainServices;

/// <summary>
/// Service for calculating various duration-related metrics for medical specializations.
/// Handles shift hours, internship durations, and module time requirements.
/// </summary>
public interface IDurationCalculationService
{
    Task<Result<DurationSummary>> CalculateInternshipDurationAsync(
        int internshipId,
        CancellationToken cancellationToken = default);
        
    Task<Result<DurationSummary>> CalculateModuleDurationAsync(
        int moduleId,
        CancellationToken cancellationToken = default);
        
    Task<Result<WeeklyHoursValidation>> ValidateWeeklyHoursAsync(
        int internshipId,
        DateTime weekStartDate,
        CancellationToken cancellationToken = default);
        
    Task<Result<MonthlyHoursValidation>> ValidateMonthlyHoursAsync(
        int moduleId,
        int year,
        int month,
        CancellationToken cancellationToken = default);
}

public class DurationCalculationService : IDurationCalculationService
{
    private const int MaxWeeklyHours = 48;
    private const int MinMonthlyHours = 160;
    
    private readonly IMedicalShiftRepository _medicalShiftRepository;
    private readonly IInternshipRepository _internshipRepository;
    private readonly IAdditionalSelfEducationDaysRepository _selfEducationRepository;
    private readonly IAbsenceRepository _absenceRepository;
    private readonly ISpecializationRepository _specializationRepository;

    public DurationCalculationService(
        IMedicalShiftRepository medicalShiftRepository,
        IInternshipRepository internshipRepository,
        IAdditionalSelfEducationDaysRepository selfEducationRepository,
        IAbsenceRepository absenceRepository,
        ISpecializationRepository specializationRepository)
    {
        _medicalShiftRepository = medicalShiftRepository;
        _internshipRepository = internshipRepository;
        _selfEducationRepository = selfEducationRepository;
        _absenceRepository = absenceRepository;
        _specializationRepository = specializationRepository;
    }

    public async Task<Result<DurationSummary>> CalculateInternshipDurationAsync(
        int internshipId,
        CancellationToken cancellationToken = default)
    {
        var internship = await _internshipRepository.GetByIdAsync(new InternshipId(internshipId));
        if (internship == null)
        {
            return Result<DurationSummary>.Failure(
                "Internship not found",
                "INTERNSHIP_NOT_FOUND");
        }

        var summary = new DurationSummary
        {
            EntityId = internshipId,
            EntityType = "Internship",
            StartDate = internship.StartDate,
            EndDate = internship.EndDate,
            PlannedDays = (internship.EndDate - internship.StartDate).Days + 1
        };

        // Get all shifts for this internship
        var shifts = await _medicalShiftRepository.GetByInternshipIdAsync(
            new InternshipId(internshipId));
            
        // Calculate total shift hours
        foreach (var shift in shifts.Where(s => s.IsApproved))
        {
            summary.TotalShiftMinutes += shift.Hours * 60 + shift.Minutes;
            summary.ShiftCount++;
        }

        // Get additional self-education days
        var selfEducationDays = await _selfEducationRepository.GetByInternshipIdAsync(
            new InternshipId(internshipId));
            
        foreach (var edu in selfEducationDays.Where(e => e.IsApproved))
        {
            summary.SelfEducationDays += edu.NumberOfDays;
        }

        // Get absences that overlap with internship period
        // We need to get the user ID from the specialization
        var specialization = await _specializationRepository.GetByIdAsync(internship.SpecializationId);
        if (specialization == null)
        {
            // If we can't find the specialization, skip absence calculation
            summary.AbsenceDays = 0;
            summary.SickLeaveDays = 0;
            summary.VacationDays = 0;
        }
        else
        {
            var absences = await _absenceRepository.GetByDateRangeAsync(
                specialization.UserId, 
                internship.StartDate, 
                internship.EndDate);
            
            foreach (var absence in absences.Where(a => a.IsApproved))
            {
                // Calculate the actual overlap with internship period
                var overlapStart = absence.StartDate > internship.StartDate ? absence.StartDate : internship.StartDate;
                var overlapEnd = absence.EndDate < internship.EndDate ? absence.EndDate : internship.EndDate;
                
                if (overlapStart <= overlapEnd)
                {
                    var absenceDays = (overlapEnd - overlapStart).Days + 1;
                    summary.AbsenceDays += absenceDays;
                    
                    if (absence.Type == ValueObjects.AbsenceType.Sick)
                        summary.SickLeaveDays += absenceDays;
                    else if (absence.Type == ValueObjects.AbsenceType.Vacation)
                        summary.VacationDays += absenceDays;
                }
            }
        }

        // Calculate effective days
        summary.EffectiveDays = summary.PlannedDays - summary.AbsenceDays + summary.SelfEducationDays;
        
        // Calculate average hours per day
        if (summary.EffectiveDays > 0)
        {
            summary.AverageHoursPerDay = Math.Round(
                (decimal)summary.TotalShiftMinutes / 60 / summary.EffectiveDays, 2);
        }

        return Result<DurationSummary>.Success(summary);
    }

    public async Task<Result<DurationSummary>> CalculateModuleDurationAsync(
        int moduleId,
        CancellationToken cancellationToken = default)
    {
        var internships = await _internshipRepository.GetByModuleIdAsync(new ModuleId(moduleId));
        
        if (!internships.Any())
        {
            return Result<DurationSummary>.Success(new DurationSummary
            {
                EntityId = moduleId,
                EntityType = "Module"
            });
        }

        var summary = new DurationSummary
        {
            EntityId = moduleId,
            EntityType = "Module",
            StartDate = internships.Min(i => i.StartDate),
            EndDate = internships.Max(i => i.EndDate)
        };

        // Aggregate data from all internships
        foreach (var internship in internships)
        {
            var internshipSummary = await CalculateInternshipDurationAsync(
                internship.InternshipId);
                
            if (internshipSummary.IsSuccess)
            {
                var data = internshipSummary.Value;
                summary.TotalShiftMinutes += data.TotalShiftMinutes;
                summary.ShiftCount += data.ShiftCount;
                summary.SelfEducationDays += data.SelfEducationDays;
                summary.AbsenceDays += data.AbsenceDays;
                summary.SickLeaveDays += data.SickLeaveDays;
                summary.VacationDays += data.VacationDays;
                summary.PlannedDays += data.PlannedDays;
            }
        }

        summary.EffectiveDays = summary.PlannedDays - summary.AbsenceDays + summary.SelfEducationDays;
        
        if (summary.EffectiveDays > 0)
        {
            summary.AverageHoursPerDay = Math.Round(
                (decimal)summary.TotalShiftMinutes / 60 / summary.EffectiveDays, 2);
        }

        return Result<DurationSummary>.Success(summary);
    }

    public async Task<Result<WeeklyHoursValidation>> ValidateWeeklyHoursAsync(
        int internshipId,
        DateTime weekStartDate,
        CancellationToken cancellationToken = default)
    {
        // Ensure week starts on Monday
        var dayOfWeek = (int)weekStartDate.DayOfWeek;
        if (dayOfWeek != 1) // Monday
        {
            weekStartDate = weekStartDate.AddDays(1 - (dayOfWeek == 0 ? 7 : dayOfWeek));
        }
        
        var weekEndDate = weekStartDate.AddDays(6);
        
        var validation = new WeeklyHoursValidation
        {
            InternshipId = internshipId,
            WeekStartDate = weekStartDate,
            WeekEndDate = weekEndDate,
            MaxAllowedHours = MaxWeeklyHours
        };

        // Get shifts for this week
        var allShifts = await _medicalShiftRepository.GetByInternshipIdAsync(
            new InternshipId(internshipId));
            
        var weekShifts = allShifts
            .Where(s => s.Date >= weekStartDate && s.Date <= weekEndDate)
            .Where(s => s.IsApproved)
            .ToList();

        foreach (var shift in weekShifts)
        {
            validation.TotalMinutes += shift.Hours * 60 + shift.Minutes;
            validation.ShiftDates.Add(shift.Date);
        }

        validation.TotalHours = Math.Round(validation.TotalMinutes / 60.0, 2);
        validation.IsValid = validation.TotalHours <= MaxWeeklyHours;
        
        if (!validation.IsValid)
        {
            validation.ExcessHours = validation.TotalHours - MaxWeeklyHours;
            validation.ValidationMessage = 
                $"Weekly hours ({validation.TotalHours}h) exceed maximum allowed ({MaxWeeklyHours}h) by {validation.ExcessHours:F2}h";
        }

        return Result<WeeklyHoursValidation>.Success(validation);
    }

    public async Task<Result<MonthlyHoursValidation>> ValidateMonthlyHoursAsync(
        int moduleId,
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        var validation = new MonthlyHoursValidation
        {
            ModuleId = moduleId,
            Year = year,
            Month = month,
            MinRequiredHours = MinMonthlyHours
        };

        var monthStart = new DateTime(year, month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        // Get all internships for this module
        var internships = await _internshipRepository.GetByModuleIdAsync(new ModuleId(moduleId));
        
        foreach (var internship in internships)
        {
            // Get shifts for this month
            var allShifts = await _medicalShiftRepository.GetByInternshipIdAsync(
                internship.InternshipId);
                
            var monthShifts = allShifts
                .Where(s => s.Date >= monthStart && s.Date <= monthEnd)
                .Where(s => s.IsApproved)
                .ToList();

            foreach (var shift in monthShifts)
            {
                validation.TotalMinutes += shift.Hours * 60 + shift.Minutes;
                validation.ShiftCount++;
            }
        }

        validation.TotalHours = Math.Round(validation.TotalMinutes / 60.0, 2);
        validation.IsValid = validation.TotalHours >= MinMonthlyHours;
        
        if (!validation.IsValid)
        {
            validation.MissingHours = MinMonthlyHours - validation.TotalHours;
            validation.ValidationMessage = 
                $"Monthly hours ({validation.TotalHours}h) are below minimum required ({MinMonthlyHours}h). Missing: {validation.MissingHours:F2}h";
        }
        else
        {
            validation.ValidationMessage = 
                $"Monthly requirement met: {validation.TotalHours}h / {MinMonthlyHours}h required";
        }

        return Result<MonthlyHoursValidation>.Success(validation);
    }
}

/// <summary>
/// Summary of duration calculations for an entity (internship or module).
/// </summary>
public class DurationSummary
{
    public int EntityId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int PlannedDays { get; set; }
    public int EffectiveDays { get; set; }
    public int TotalShiftMinutes { get; set; }
    public int ShiftCount { get; set; }
    public int SelfEducationDays { get; set; }
    public int AbsenceDays { get; set; }
    public int SickLeaveDays { get; set; }
    public int VacationDays { get; set; }
    public decimal AverageHoursPerDay { get; set; }
    
    public decimal TotalHours => Math.Round(TotalShiftMinutes / 60.0m, 2);
}

/// <summary>
/// Validation result for weekly hour limits.
/// </summary>
public class WeeklyHoursValidation
{
    public int InternshipId { get; set; }
    public DateTime WeekStartDate { get; set; }
    public DateTime WeekEndDate { get; set; }
    public int TotalMinutes { get; set; }
    public double TotalHours { get; set; }
    public int MaxAllowedHours { get; set; }
    public bool IsValid { get; set; }
    public double ExcessHours { get; set; }
    public List<DateTime> ShiftDates { get; set; } = new();
    public string ValidationMessage { get; set; } = string.Empty;
}

/// <summary>
/// Validation result for monthly hour requirements.
/// </summary>
public class MonthlyHoursValidation
{
    public int ModuleId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public int TotalMinutes { get; set; }
    public double TotalHours { get; set; }
    public int MinRequiredHours { get; set; }
    public bool IsValid { get; set; }
    public double MissingHours { get; set; }
    public int ShiftCount { get; set; }
    public string ValidationMessage { get; set; } = string.Empty;
}
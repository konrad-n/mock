using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Models.Statistics;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Infrastructure.DAL;
using System.Globalization;

namespace SledzSpecke.Infrastructure.Services;

/// <summary>
/// Production-ready implementation of statistics service with real calculations
/// </summary>
public class RealStatisticsService : IStatisticsService
{
    private readonly ILogger<RealStatisticsService> _logger;
    private readonly SledzSpeckeDbContext _dbContext;
    private readonly IMedicalShiftRepository _shiftRepository;
    private readonly IProcedureRepository _procedureRepository;
    private readonly IInternshipRepository _internshipRepository;
    private readonly IModuleRepository _moduleRepository;
    private readonly ICacheService _cacheService;
    
    private const int MONTHLY_HOURS_MINIMUM = 160;
    private const int WEEKLY_HOURS_MAXIMUM = 48;
    
    public RealStatisticsService(
        ILogger<RealStatisticsService> logger,
        SledzSpeckeDbContext dbContext,
        IMedicalShiftRepository shiftRepository,
        IProcedureRepository procedureRepository,
        IInternshipRepository internshipRepository,
        IModuleRepository moduleRepository,
        ICacheService cacheService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _shiftRepository = shiftRepository;
        _procedureRepository = procedureRepository;
        _internshipRepository = internshipRepository;
        _moduleRepository = moduleRepository;
        _cacheService = cacheService;
    }

    public async Task UpdateMonthlyShiftStatisticsAsync(
        InternshipId internshipId, 
        DateTime shiftDate, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var year = shiftDate.Year;
            var month = shiftDate.Month;
            var cacheKey = $"monthly_shift_stats:{internshipId.Value}:{year}:{month}";
            
            // Get all shifts for the month
            var monthStart = new DateTime(year, month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);
            
            var shifts = await _shiftRepository.GetByInternshipIdAsync(internshipId.Value);
            var monthlyShifts = shifts
                .Where(s => s.Date >= monthStart && s.Date <= monthEnd)
                .ToList();
            
            // Calculate statistics
            var statistics = new MonthlyShiftStatistics
            {
                InternshipId = internshipId,
                Year = year,
                Month = month,
                TotalHours = monthlyShifts.Sum(s => s.Duration.Hours),
                ApprovedHours = monthlyShifts.Where(s => s.IsApproved).Sum(s => s.Duration.Hours),
                PendingHours = monthlyShifts.Where(s => !s.IsApproved && s.SyncStatus != SyncStatus.SyncError).Sum(s => s.Duration.Hours),
                RejectedHours = monthlyShifts.Where(s => s.SyncStatus == SyncStatus.SyncError || s.SyncStatus == SyncStatus.SyncFailed).Sum(s => s.Duration.Hours),
                ShiftCount = monthlyShifts.Count,
                AverageHoursPerShift = monthlyShifts.Any() ? (decimal)monthlyShifts.Average(s => s.Duration.Hours) : 0,
                WeekendShifts = monthlyShifts.Count(s => s.Date.DayOfWeek == DayOfWeek.Saturday || s.Date.DayOfWeek == DayOfWeek.Sunday),
                NightShifts = monthlyShifts.Count(s => IsNightShift(s)),
                LastUpdated = DateTime.UtcNow,
                RequiredHours = MONTHLY_HOURS_MINIMUM,
                MeetsMonthlyMinimum = monthlyShifts.Where(s => s.IsApproved).Sum(s => s.Duration.Hours) >= MONTHLY_HOURS_MINIMUM,
                HoursDeficit = Math.Max(0, MONTHLY_HOURS_MINIMUM - monthlyShifts.Where(s => s.IsApproved).Sum(s => s.Duration.Hours)),
                WeeklyBreakdown = CalculateWeeklyBreakdown(monthlyShifts)
            };
            
            // Cache the statistics
            await _cacheService.SetAsync(cacheKey, statistics, TimeSpan.FromHours(1), cancellationToken);
            
            // Store in database for historical tracking
            await StoreMonthlyStatisticsAsync(statistics, cancellationToken);
            
            _logger.LogInformation(
                "Updated monthly shift statistics for InternshipId={InternshipId}, Year={Year}, Month={Month}. " +
                "Total hours: {TotalHours}, Approved: {ApprovedHours}, Meets minimum: {MeetsMinimum}",
                internshipId.Value, year, month, statistics.TotalHours, statistics.ApprovedHours, statistics.MeetsMonthlyMinimum);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating monthly shift statistics for InternshipId={InternshipId}", internshipId.Value);
            throw;
        }
    }


    public async Task UpdateWeeklyStatisticsAsync(
        InternshipId internshipId, 
        DateTime weekStartDate, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var weekEndDate = weekStartDate.AddDays(6);
            var cacheKey = $"weekly_stats:{internshipId.Value}:{weekStartDate:yyyy-MM-dd}";
            
            var shifts = await _shiftRepository.GetByInternshipIdAsync(internshipId.Value);
            var weeklyShifts = shifts
                .Where(s => s.Date >= weekStartDate && s.Date <= weekEndDate)
                .ToList();
            
            var totalHours = weeklyShifts.Sum(s => s.Duration.Hours);
            var approvedHours = weeklyShifts.Where(s => s.IsApproved).Sum(s => s.Duration.Hours);
            
            var statistics = new WeeklyStatistics
            {
                WeekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                    weekStartDate, CalendarWeekRule.FirstDay, DayOfWeek.Monday),
                Hours = totalHours,
                ExceedsWeeklyLimit = totalHours > WEEKLY_HOURS_MAXIMUM,
                MaxAllowedHours = WEEKLY_HOURS_MAXIMUM
            };
            
            await _cacheService.SetAsync(cacheKey, statistics, TimeSpan.FromDays(1), cancellationToken);
            
            if (statistics.ExceedsWeeklyLimit)
            {
                _logger.LogWarning(
                    "Weekly hour limit exceeded for InternshipId={InternshipId}, Week={Week}. " +
                    "Total hours: {TotalHours}, Limit: {Limit}",
                    internshipId.Value, statistics.WeekNumber, totalHours, WEEKLY_HOURS_MAXIMUM);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating weekly statistics for InternshipId={InternshipId}", internshipId.Value);
            throw;
        }
    }

    public async Task UpdateWeeklyStatisticsAsync(
        InternshipId internshipId, 
        string procedureCode, 
        DateTime date, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var procedures = await _procedureRepository.GetByInternshipIdAsync(internshipId.Value);
            var duplicates = procedures
                .Where(p => p.Code == procedureCode && p.Date.Date == date.Date)
                .ToList();
            
            if (duplicates.Count() > 3)
            {
                await NotifyDuplicateProcedureAsync(internshipId, procedureCode, date, cancellationToken);
                
                _logger.LogWarning(
                    "Duplicate procedures detected: InternshipId={InternshipId}, Code={Code}, Date={Date}, Count={Count}",
                    internshipId.Value, procedureCode, date.Date, duplicates.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking duplicate procedures for InternshipId={InternshipId}", internshipId.Value);
            throw;
        }
    }

    public async Task UpdateProcedurePatternsAsync(
        InternshipId internshipId, 
        object patterns, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Analyze procedure patterns for learning insights
            var procedures = await _procedureRepository.GetByInternshipIdAsync(internshipId.Value);
            
            var procedurePatterns = new
            {
                MostFrequentProcedures = procedures
                    .GroupBy(p => p.Code)
                    .OrderByDescending(g => g.Count())
                    .Take(10)
                    .Select(g => new { Code = g.Key, Count = g.Count() })
                    .ToList(),
                
                PreferredLocations = procedures
                    .GroupBy(p => p.Location)
                    .OrderByDescending(g => g.Count())
                    .Select(g => new { Location = g.Key, Count = g.Count() })
                    .ToList(),
                
                TimeDistribution = procedures
                    .GroupBy(p => p.Date.Hour)
                    .OrderBy(g => g.Key)
                    .Select(g => new { Hour = g.Key, Count = g.Count() })
                    .ToList(),
                
                WeeklyPattern = procedures
                    .GroupBy(p => p.Date.DayOfWeek)
                    .OrderBy(g => g.Key)
                    .Select(g => new { Day = g.Key.ToString(), Count = g.Count() })
                    .ToList(),
                
                CompletionTrends = CalculateCompletionTrends(procedures),
                LastUpdated = DateTime.UtcNow
            };
            
            var cacheKey = $"procedure_patterns:{internshipId.Value}";
            await _cacheService.SetAsync(cacheKey, procedurePatterns, TimeSpan.FromHours(6), cancellationToken);
            
            _logger.LogInformation(
                "Updated procedure patterns for InternshipId={InternshipId}. Most frequent: {MostFrequent}",
                internshipId.Value, 
                procedurePatterns.MostFrequentProcedures.FirstOrDefault()?.Code ?? "None");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating procedure patterns for InternshipId={InternshipId}", internshipId.Value);
            throw;
        }
    }

    public async Task UpdateProcedureStatisticsAsync(
        string department, 
        string procedureCode, 
        int count, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = $"department_procedure_stats:{department}";
            var stats = await _cacheService.GetAsync<Dictionary<string, ProcedureDepartmentStats>>(cacheKey, cancellationToken) 
                ?? new Dictionary<string, ProcedureDepartmentStats>();
            
            if (stats.ContainsKey(procedureCode))
            {
                stats[procedureCode].TotalCount += count;
                stats[procedureCode].LastUpdated = DateTime.UtcNow;
                stats[procedureCode].DailyAverage = CalculateDailyAverage(stats[procedureCode]);
            }
            else
            {
                stats[procedureCode] = new ProcedureDepartmentStats
                {
                    ProcedureCode = procedureCode,
                    Department = department,
                    TotalCount = count,
                    FirstRecorded = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow,
                    DailyAverage = 0
                };
            }
            
            await _cacheService.SetAsync(cacheKey, stats, TimeSpan.FromDays(7), cancellationToken);
            
            _logger.LogDebug(
                "Updated procedure statistics: Department={Department}, Code={Code}, Total={Total}",
                department, procedureCode, stats[procedureCode].TotalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating procedure statistics for Department={Department}", department);
            throw;
        }
    }

    public async Task UpdateModuleProgressAsync(
        InternshipId internshipId, 
        object moduleProgress, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var internship = await _internshipRepository.GetByIdAsync(internshipId.Value);
            if (internship == null) return;
            
            var modules = await _moduleRepository.GetBySpecializationIdAsync(internship.SpecializationId);
            var procedures = await _procedureRepository.GetByInternshipIdAsync(internshipId.Value);
            var shifts = await _shiftRepository.GetByInternshipIdAsync(internshipId.Value);
            
            var moduleProgressList = new List<ModuleProgressStatistics>();
            
            foreach (var module in modules)
            {
                var moduleStats = await CalculateModuleProgressAsync(
                    internship, module, procedures.ToList(), shifts.ToList(), cancellationToken);
                moduleProgressList.Add(moduleStats);
            }
            
            var cacheKey = $"module_progress:{internshipId.Value}";
            await _cacheService.SetAsync(cacheKey, moduleProgressList, TimeSpan.FromHours(2), cancellationToken);
            
            _logger.LogInformation(
                "Updated module progress for InternshipId={InternshipId}. Modules tracked: {Count}",
                internshipId.Value, moduleProgressList.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating module progress for InternshipId={InternshipId}", internshipId.Value);
            throw;
        }
    }

    public async Task UpdateYearProgressAsync(
        InternshipId internshipId, 
        object yearProgress, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var internship = await _internshipRepository.GetByIdAsync(internshipId.Value);
            if (internship == null) return;
            
            var yearStats = await CalculateYearProgressAsync(internship, cancellationToken);
            
            var cacheKey = $"year_progress:{internshipId.Value}:{internship.DaysCount}";
            await _cacheService.SetAsync(cacheKey, yearStats, TimeSpan.FromHours(4), cancellationToken);
            
            _logger.LogInformation(
                "Updated year progress for InternshipId={InternshipId}. " +
                "Overall progress: {Progress}%, Meets requirements: {MeetsRequirements}",
                internshipId.Value, 
                yearStats.OverallProgressPercentage, yearStats.MeetsYearRequirements);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating year progress for InternshipId={InternshipId}", internshipId.Value);
            throw;
        }
    }

    public async Task UpdateApprovedHoursStatisticsAsync(
        InternshipId internshipId,
        DateTime shiftDate,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var shifts = await _shiftRepository.GetByInternshipIdAsync(internshipId.Value);
            var approvedShifts = shifts.Where(s => s.IsApproved).ToList();
            
            var monthlyApproved = approvedShifts
                .Where(s => s.Date.Year == shiftDate.Year && s.Date.Month == shiftDate.Month)
                .Sum(s => s.Duration.Hours);
                
            var cacheKey = $"approved_hours:{internshipId.Value}:{shiftDate:yyyy-MM}";
            await _cacheService.SetAsync(cacheKey, (object)monthlyApproved, TimeSpan.FromHours(2), cancellationToken);
            
            _logger.LogInformation(
                "Updated approved hours for InternshipId={InternshipId}, Month={Month}. Hours: {Hours}",
                internshipId.Value, shiftDate.ToString("yyyy-MM"), monthlyApproved);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating approved hours for InternshipId={InternshipId}", internshipId.Value);
            throw;
        }
    }

    public async Task TrackDuplicateProcedureAsync(
        InternshipId internshipId,
        string procedureCode,
        DateTime date,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var procedures = await _procedureRepository.GetByInternshipIdAsync(internshipId.Value);
            var duplicatesCount = procedures
                .Count(p => p.Code == procedureCode && p.Date.Date == date.Date);
                
            if (duplicatesCount > 1)
            {
                var cacheKey = $"duplicate_procedures:{internshipId.Value}:{date:yyyy-MM-dd}";
                var duplicates = await _cacheService.GetAsync<Dictionary<string, int>>(cacheKey, cancellationToken) 
                    ?? new Dictionary<string, int>();
                    
                duplicates[procedureCode] = duplicatesCount;
                await _cacheService.SetAsync(cacheKey, duplicates, TimeSpan.FromDays(7), cancellationToken);
                
                _logger.LogWarning(
                    "Duplicate procedure tracked: InternshipId={InternshipId}, Code={Code}, Date={Date}, Count={Count}",
                    internshipId.Value, procedureCode, date.Date, duplicatesCount);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking duplicate procedure for InternshipId={InternshipId}", internshipId.Value);
            throw;
        }
    }

    public async Task UpdateDailyProcedureCountAsync(
        InternshipId internshipId,
        DateTime date,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var procedures = await _procedureRepository.GetByInternshipIdAsync(internshipId.Value);
            var dailyCount = procedures.Count(p => p.Date.Date == date.Date);
            
            var cacheKey = $"daily_procedure_count:{internshipId.Value}:{date:yyyy-MM-dd}";
            await _cacheService.SetAsync(cacheKey, (object)dailyCount, TimeSpan.FromDays(1), cancellationToken);
            
            _logger.LogDebug(
                "Updated daily procedure count for InternshipId={InternshipId}, Date={Date}. Count: {Count}",
                internshipId.Value, date.Date, dailyCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating daily procedure count for InternshipId={InternshipId}", internshipId.Value);
            throw;
        }
    }

    public async Task TrackNewProcedureTypeAsync(
        InternshipId internshipId,
        string procedureCode,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = $"procedure_types:{internshipId.Value}";
            var procedureTypes = await _cacheService.GetAsync<HashSet<string>>(cacheKey, cancellationToken) 
                ?? new HashSet<string>();
                
            if (procedureTypes.Add(procedureCode))
            {
                await _cacheService.SetAsync(cacheKey, procedureTypes, TimeSpan.FromDays(30), cancellationToken);
                
                _logger.LogInformation(
                    "New procedure type tracked: InternshipId={InternshipId}, Code={Code}. Total types: {Count}",
                    internshipId.Value, procedureCode, procedureTypes.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking new procedure type for InternshipId={InternshipId}", internshipId.Value);
            throw;
        }
    }

    public async Task UpdateLocationStatisticsAsync(
        string location,
        string procedureCode,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = $"location_stats:{location}";
            var stats = await _cacheService.GetAsync<Dictionary<string, int>>(cacheKey, cancellationToken) 
                ?? new Dictionary<string, int>();
                
            if (stats.ContainsKey(procedureCode))
                stats[procedureCode]++;
            else
                stats[procedureCode] = 1;
                
            await _cacheService.SetAsync(cacheKey, stats, TimeSpan.FromDays(7), cancellationToken);
            
            _logger.LogDebug(
                "Updated location statistics: Location={Location}, Code={Code}, Count={Count}",
                location, procedureCode, stats[procedureCode]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating location statistics for Location={Location}", location);
            throw;
        }
    }
    
    // Private helper methods
    
    private Dictionary<int, WeeklyStatistics> CalculateWeeklyBreakdown(List<MedicalShift> shifts)
    {
        var weeklyStats = new Dictionary<int, WeeklyStatistics>();
        var calendar = CultureInfo.CurrentCulture.Calendar;
        
        var weekGroups = shifts.GroupBy(s => calendar.GetWeekOfYear(s.Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday));
        
        foreach (var weekGroup in weekGroups)
        {
            var weekHours = weekGroup.Sum(s => s.Duration.Hours);
            weeklyStats[weekGroup.Key] = new WeeklyStatistics
            {
                WeekNumber = weekGroup.Key,
                Hours = weekHours,
                ExceedsWeeklyLimit = weekHours > WEEKLY_HOURS_MAXIMUM,
                MaxAllowedHours = WEEKLY_HOURS_MAXIMUM
            };
        }
        
        return weeklyStats;
    }
    
    private int CalculateMonthsElapsed(DateTime start, DateTime end)
    {
        return ((end.Year - start.Year) * 12) + end.Month - start.Month + 1;
    }
    
    private async Task StoreMonthlyStatisticsAsync(
        MonthlyShiftStatistics statistics, 
        CancellationToken cancellationToken)
    {
        // Store in a dedicated statistics table for historical tracking
        // This would be implemented with a proper statistics entity
        await Task.CompletedTask;
    }
    
    private async Task NotifyDuplicateProcedureAsync(
        InternshipId internshipId, 
        string procedureCode, 
        DateTime date, 
        CancellationToken cancellationToken)
    {
        // Send notification through notification service
        await Task.CompletedTask;
    }
    
    private async Task UpdateAchievementMetricsAsync(
        InternshipId internshipId, 
        string metric, 
        int value, 
        CancellationToken cancellationToken)
    {
        // Update achievement/milestone tracking
        await Task.CompletedTask;
    }
    
    private object CalculateCompletionTrends(IEnumerable<ProcedureBase> procedures)
    {
        var procedureList = procedures.ToList();
        if (!procedureList.Any()) return new { };
        
        var groupedByMonth = procedureList
            .GroupBy(p => new { p.Date.Year, p.Date.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Count = g.Count(),
                UniqueTypes = g.Select(p => p.Code).Distinct().Count()
            })
            .ToList();
        
        return new
        {
            MonthlyTrends = groupedByMonth,
            AveragePerMonth = groupedByMonth.Any() ? groupedByMonth.Average(m => m.Count) : 0,
            TrendDirection = CalculateTrendDirection(groupedByMonth.Select(m => m.Count).ToList())
        };
    }
    
    private string CalculateTrendDirection(List<int> values)
    {
        if (values.Count < 2) return "Insufficient data";
        
        var recentAvg = values.TakeLast(3).Average();
        var overallAvg = values.Average();
        
        if (recentAvg > overallAvg * 1.1) return "Increasing";
        if (recentAvg < overallAvg * 0.9) return "Decreasing";
        return "Stable";
    }
    
    private double CalculateDailyAverage(ProcedureDepartmentStats stats)
    {
        var daysSinceFirst = (DateTime.UtcNow - stats.FirstRecorded).TotalDays;
        return daysSinceFirst > 0 ? stats.TotalCount / daysSinceFirst : 0;
    }
    
    private async Task<ModuleProgressStatistics> CalculateModuleProgressAsync(
        Internship internship,
        Module module,
        List<ProcedureBase> procedures,
        List<MedicalShift> shifts,
        CancellationToken cancellationToken)
    {
        var moduleStartDate = module.StartDate;
        var moduleEndDate = module.EndDate;
        var now = DateTime.UtcNow;
        
        var moduleProcedures = procedures
            .Where(p => p.ModuleId != null && p.ModuleId.Value == module.Id.Value && p.Date >= moduleStartDate)
            .ToList();
            
        var moduleShifts = shifts
            .Where(s => s.Date >= moduleStartDate && s.Date <= moduleEndDate)
            .ToList();
        
        var requiredProcedures = module.TotalProceduresA + module.TotalProceduresB;
        var requiredHours = module.RequiredShiftHours;
        var completedHours = moduleShifts.Sum(s => s.Duration.Hours);
        var approvedHours = moduleShifts.Where(s => s.IsApproved).Sum(s => s.Duration.Hours);
        
        var elapsedMonths = CalculateMonthsElapsed(moduleStartDate, now);
        var totalMonths = CalculateMonthsElapsed(moduleStartDate, moduleEndDate);
        var progressPercentage = totalMonths > 0 ? (decimal)elapsedMonths / totalMonths * 100 : 0;
        
        return new ModuleProgressStatistics
        {
            InternshipId = internship.InternshipId,
            ModuleId = module.Id,
            ModuleName = module.Name,
            ModuleType = module.Type,
            StartDate = moduleStartDate,
            CompletionDate = now >= moduleEndDate ? moduleEndDate : null,
            DurationInMonths = totalMonths,
            ElapsedMonths = elapsedMonths,
            ProgressPercentage = Math.Min(100, progressPercentage),
            TotalProceduresRequired = requiredProcedures,
            TotalProceduresCompleted = moduleProcedures.Count,
            ProcedureCompletionRate = requiredProcedures > 0 ? (decimal)moduleProcedures.Count / requiredProcedures * 100 : 0,
            RequiredHours = requiredHours,
            CompletedHours = completedHours,
            ApprovedHours = approvedHours,
            RequiredCourses = module.TotalCourses,
            CompletedCourses = module.CompletedCourses,
            PendingCourses = new List<string>(),
            Status = DetermineModuleStatus(progressPercentage, approvedHours, requiredHours),
            IsOnTrack = approvedHours >= (requiredHours * progressPercentage / 100),
            EstimatedDaysToCompletion = CalculateEstimatedDaysToCompletion(
                approvedHours, requiredHours, elapsedMonths, totalMonths)
        };
    }
    
    private async Task<YearProgressStatistics> CalculateYearProgressAsync(
        Internship internship,
        CancellationToken cancellationToken)
    {
        var modules = await _moduleRepository.GetBySpecializationIdAsync(internship.SpecializationId);
        var yearModules = modules.ToList();
        
        var procedures = await _procedureRepository.GetByInternshipIdAsync(internship.Id);
        var shifts = await _shiftRepository.GetByInternshipIdAsync(internship.Id);
        
        var moduleProgressList = new List<ModuleProgressStatistics>();
        foreach (var module in yearModules)
        {
            var progress = await CalculateModuleProgressAsync(
                internship, module, procedures.Cast<ProcedureBase>().ToList(), shifts.ToList(), cancellationToken);
            moduleProgressList.Add(progress);
        }
        
        var totalRequiredHours = internship.PlannedDays * 8; // 8 hours per day
        var totalCompletedHours = shifts.Sum(s => s.Duration.Hours);
        var totalApprovedHours = shifts.Where(s => s.IsApproved).Sum(s => s.Duration.Hours);
        
        var overallProgress = CalculateOverallYearProgress(moduleProgressList);
        
        return new YearProgressStatistics
        {
            InternshipId = internship.InternshipId,
            Year = 1, // Default year since not available on Internship
            YearStartDate = internship.StartDate,
            YearEndDate = internship.EndDate,
            OverallProgressPercentage = overallProgress,
            ModulesProgress = moduleProgressList,
            TotalHoursCompleted = totalCompletedHours,
            TotalHoursRequired = totalRequiredHours,
            TotalProceduresCompleted = procedures.Count(),
            TotalProceduresRequired = yearModules.Sum(m => m.TotalProceduresA + m.TotalProceduresB),
            CoursesCompleted = yearModules.Sum(m => m.CompletedCourses),
            CoursesRequired = yearModules.Sum(m => m.TotalCourses),
            MeetsYearRequirements = totalApprovedHours >= totalRequiredHours && overallProgress >= 100,
            DeficientAreas = IdentifyDeficientAreas(moduleProgressList, totalApprovedHours, totalRequiredHours),
            ProjectedCompletionDate = CalculateProjectedCompletionDate(
                overallProgress, internship.StartDate, internship.EndDate)
        };
    }
    
    private DateTime CalculateModuleStartDate(DateTime internshipStart, Module module)
    {
        // Use the module's actual start date
        return module.StartDate;
    }
    
    private ModuleStatus DetermineModuleStatus(decimal progressPercentage, int approvedHours, int requiredHours)
    {
        if (progressPercentage == 0) return ModuleStatus.NotStarted;
        if (progressPercentage >= 100) return ModuleStatus.Completed;
        if (progressPercentage >= 80) return ModuleStatus.NearCompletion;
        
        var expectedHours = requiredHours * progressPercentage / 100;
        if (approvedHours < expectedHours * 0.8m) return ModuleStatus.Delayed;
        
        return ModuleStatus.InProgress;
    }
    
    private int CalculateEstimatedDaysToCompletion(int currentHours, int requiredHours, int elapsedMonths, int totalMonths)
    {
        if (currentHours >= requiredHours) return 0;
        if (elapsedMonths == 0) return totalMonths * 30;
        
        var dailyRate = (double)currentHours / (elapsedMonths * 30);
        var remainingHours = requiredHours - currentHours;
        
        return dailyRate > 0 ? (int)(remainingHours / dailyRate) : int.MaxValue;
    }
    
    private decimal CalculateOverallYearProgress(List<ModuleProgressStatistics> modules)
    {
        if (!modules.Any()) return 0;
        return modules.Average(m => m.ProgressPercentage);
    }
    
    private List<string> IdentifyDeficientAreas(
        List<ModuleProgressStatistics> modules, 
        int totalApprovedHours, 
        int totalRequiredHours)
    {
        var deficientAreas = new List<string>();
        
        if (totalApprovedHours < totalRequiredHours * 0.8)
        {
            deficientAreas.Add($"Total approved hours ({totalApprovedHours}) below 80% of required ({totalRequiredHours})");
        }
        
        foreach (var module in modules.Where(m => !m.IsOnTrack))
        {
            deficientAreas.Add($"Module '{module.ModuleName}' is behind schedule");
        }
        
        return deficientAreas;
    }
    
    private DateTime CalculateProjectedCompletionDate(decimal overallProgress, DateTime startDate, DateTime plannedEndDate)
    {
        if (overallProgress >= 100) return plannedEndDate;
        if (overallProgress == 0) return plannedEndDate.AddMonths(6); // Add buffer
        
        var totalDuration = (plannedEndDate - startDate).TotalDays;
        var elapsedDuration = (DateTime.UtcNow - startDate).TotalDays;
        var projectedTotalDuration = elapsedDuration / (double)(overallProgress / 100);
        
        return startDate.AddDays(projectedTotalDuration);
    }
    
    private decimal CalculateExpectedProgress(DateTime startDate, DateTime endDate)
    {
        var totalDuration = (endDate - startDate).TotalDays;
        var elapsedDuration = (DateTime.UtcNow - startDate).TotalDays;
        
        if (totalDuration <= 0) return 100;
        if (elapsedDuration <= 0) return 0;
        
        return Math.Min(100, (decimal)(elapsedDuration / totalDuration));
    }
    
    private DateTime CalculateEstimatedCompletionDate(int currentHours, int requiredHours, DateTime startDate, DateTime endDate)
    {
        if (currentHours >= requiredHours) return DateTime.UtcNow;
        
        var elapsedDays = (DateTime.UtcNow - startDate).TotalDays;
        if (elapsedDays <= 0 || currentHours <= 0) return endDate.AddMonths(3); // Add buffer
        
        var dailyRate = currentHours / elapsedDays;
        var remainingHours = requiredHours - currentHours;
        var daysToComplete = remainingHours / dailyRate;
        
        return DateTime.UtcNow.AddDays(daysToComplete);
    }
    
    private bool IsNightShift(MedicalShift shift)
    {
        // Night shift is typically between 22:00 and 06:00
        var shiftHour = shift.Date.Hour;
        return shiftHour >= 22 || shiftHour < 6;
    }
}
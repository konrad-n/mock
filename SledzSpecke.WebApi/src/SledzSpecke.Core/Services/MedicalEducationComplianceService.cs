using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Enums;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Interfaces;
using SledzSpecke.Core.Services.Interfaces;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Shared.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SledzSpecke.Core.Services;

public sealed class MedicalEducationComplianceService : IMedicalEducationComplianceService
{
    private const int MaxWeeklyHours = 48;
    private const int MinMonthlyHours = 160;
    private const int MaxDailyHours = 24;
    private const int MaxConsecutiveHours = 24;
    private const decimal MinQualityScore = 3.0m;
    
    private readonly IUserRepository _userRepository;
    private readonly IMedicalShiftRepository _shiftRepository;
    private readonly IInternshipRepository _internshipRepository;
    private readonly IProcedureRepository _procedureRepository;
    private readonly IModuleRepository _moduleRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public MedicalEducationComplianceService(
        IUserRepository userRepository,
        IMedicalShiftRepository shiftRepository,
        IInternshipRepository internshipRepository,
        IProcedureRepository procedureRepository,
        IModuleRepository moduleRepository,
        ICourseRepository courseRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _shiftRepository = shiftRepository ?? throw new ArgumentNullException(nameof(shiftRepository));
        _internshipRepository = internshipRepository ?? throw new ArgumentNullException(nameof(internshipRepository));
        _procedureRepository = procedureRepository ?? throw new ArgumentNullException(nameof(procedureRepository));
        _moduleRepository = moduleRepository ?? throw new ArgumentNullException(nameof(moduleRepository));
        _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
    }

    public async Task<Result<bool>> ValidateShiftLimitsAsync(UserId userId, DateTime shiftDate, TimeSpan duration)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            return Result<bool>.Failure("User not found", ErrorCodes.NOT_FOUND);

        // Check daily limit
        if (duration.TotalHours > MaxDailyHours)
            return Result<bool>.Failure($"Shift duration exceeds maximum daily limit of {MaxDailyHours} hours", ErrorCodes.VALIDATION_ERROR);

        // Check consecutive hours limit
        if (duration.TotalHours > MaxConsecutiveHours)
            return Result<bool>.Failure($"Shift duration exceeds maximum consecutive hours limit of {MaxConsecutiveHours}", ErrorCodes.VALIDATION_ERROR);

        // Get week boundaries (Monday to Sunday)
        var weekStart = shiftDate.AddDays(-(int)shiftDate.DayOfWeek + (int)DayOfWeek.Monday);
        if (weekStart > shiftDate) weekStart = weekStart.AddDays(-7);
        var weekEnd = weekStart.AddDays(7);

        // Get shifts for the week
        var weeklyShifts = await _shiftRepository.GetByUserAndDateRangeAsync(userId, weekStart, weekEnd);
        var totalWeeklyHours = weeklyShifts.Sum(s => s.Duration.Value.TotalHours) + duration.TotalHours;

        if (totalWeeklyHours > MaxWeeklyHours)
            return Result<bool>.Failure($"Adding this shift would exceed weekly limit of {MaxWeeklyHours} hours. Current: {totalWeeklyHours - duration.TotalHours}h", ErrorCodes.BUSINESS_RULE_VIOLATION);

        // Check for overlapping shifts
        var dayShifts = await _shiftRepository.GetByUserAndDateRangeAsync(userId, shiftDate.Date, shiftDate.Date.AddDays(1));
        foreach (var existingShift in dayShifts)
        {
            var existingStart = existingShift.Date;
            var existingEnd = existingStart.Add(existingShift.Duration.Value);
            var newEnd = shiftDate.Add(duration);

            if ((shiftDate >= existingStart && shiftDate < existingEnd) ||
                (newEnd > existingStart && newEnd <= existingEnd) ||
                (shiftDate <= existingStart && newEnd >= existingEnd))
            {
                return Result<bool>.Failure($"Shift overlaps with existing shift from {existingStart:HH:mm} to {existingEnd:HH:mm}", ErrorCodes.CONFLICT);
            }
        }

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> ValidateInternshipSequenceAsync(UserId userId, InternshipId internshipId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            return Result<bool>.Failure("User not found", ErrorCodes.NOT_FOUND);

        var internship = await _internshipRepository.GetByIdAsync(internshipId);
        if (internship is null)
            return Result<bool>.Failure("Internship not found", ErrorCodes.NOT_FOUND);

        var userInternships = await _internshipRepository.GetByUserIdAsync(userId);
        
        // Check module sequence - basic module internships must be completed first
        if (internship.Module.Type == ModuleType.Specialistic)
        {
            var basicModuleInternships = userInternships
                .Where(i => i.Module.Type == ModuleType.Basic && 
                           i.Status != InternshipStatus.Completed &&
                           i.Id != internshipId)
                .ToList();

            if (basicModuleInternships.Any())
            {
                return Result<bool>.Failure(
                    "Cannot start specialistic module internship while basic module internships are incomplete", 
                    ErrorCodes.BUSINESS_RULE_VIOLATION);
            }
        }

        // Check for prerequisites based on internship template
        if (internship.InternshipTemplate?.PrerequisiteTemplateIds?.Any() == true)
        {
            foreach (var prereqId in internship.InternshipTemplate.PrerequisiteTemplateIds)
            {
                var hasCompletedPrereq = userInternships.Any(i => 
                    i.InternshipTemplate?.Id == prereqId && 
                    i.Status == InternshipStatus.Completed);

                if (!hasCompletedPrereq)
                {
                    return Result<bool>.Failure(
                        $"Prerequisite internship not completed: {prereqId}", 
                        ErrorCodes.BUSINESS_RULE_VIOLATION);
                }
            }
        }

        // Check year progression
        if (internship.InternshipTemplate?.RequiredYear > user.Specialization?.Year)
        {
            return Result<bool>.Failure(
                $"Internship requires year {internship.InternshipTemplate.RequiredYear}, current year is {user.Specialization?.Year}", 
                ErrorCodes.BUSINESS_RULE_VIOLATION);
        }

        return Result<bool>.Success(true);
    }

    public async Task<Result<ComplianceReport>> GenerateComplianceReportAsync(UserId userId, DateTime? asOfDate = null)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            return Result<ComplianceReport>.Failure("User not found", ErrorCodes.NOT_FOUND);

        var reportDate = asOfDate ?? _dateTimeProvider.Now;
        var report = new ComplianceReport
        {
            UserId = userId,
            GeneratedAt = reportDate,
            Issues = new List<ComplianceIssue>(),
            Recommendations = new List<string>()
        };

        // Check monthly hours
        var monthStart = new DateTime(reportDate.Year, reportDate.Month, 1);
        var monthEnd = monthStart.AddMonths(1);
        var monthlyShifts = await _shiftRepository.GetByUserAndDateRangeAsync(userId, monthStart, monthEnd);
        var monthlyHours = monthlyShifts.Sum(s => s.Duration.Value.TotalHours);

        if (monthlyHours < MinMonthlyHours && reportDate >= monthEnd.AddDays(-7))
        {
            report.Issues.Add(new ComplianceIssue
            {
                Type = ComplianceIssueType.InsufficientHours,
                Severity = IssueSeverity.High,
                Description = $"Monthly hours ({monthlyHours}h) below minimum requirement ({MinMonthlyHours}h)",
                AffectedPeriod = $"{monthStart:yyyy-MM}"
            });
            report.Recommendations.Add($"Schedule additional {MinMonthlyHours - monthlyHours} hours before month end");
        }

        // Check internship progress
        var activeInternships = await _internshipRepository.GetActiveByUserIdAsync(userId);
        foreach (var internship in activeInternships)
        {
            if (internship.EndDate < reportDate && internship.Status != InternshipStatus.Completed)
            {
                report.Issues.Add(new ComplianceIssue
                {
                    Type = ComplianceIssueType.OverdueInternship,
                    Severity = IssueSeverity.High,
                    Description = $"Internship '{internship.Department}' is overdue",
                    AffectedPeriod = $"Since {internship.EndDate:yyyy-MM-dd}"
                });
            }
        }

        // Check module progression
        var modules = await _moduleRepository.GetByUserIdAsync(userId);
        var basicModule = modules.FirstOrDefault(m => m.Type == ModuleType.Basic);
        var specialisticModule = modules.FirstOrDefault(m => m.Type == ModuleType.Specialistic);

        if (basicModule != null && specialisticModule != null)
        {
            if (specialisticModule.IsActive && basicModule.CompletionPercentage < 100)
            {
                report.Issues.Add(new ComplianceIssue
                {
                    Type = ComplianceIssueType.InvalidModuleProgression,
                    Severity = IssueSeverity.Critical,
                    Description = "Specialistic module active while basic module incomplete",
                    AffectedPeriod = "Current"
                });
                report.Recommendations.Add("Complete basic module before progressing to specialistic module");
            }
        }

        // Calculate overall compliance score
        report.OverallComplianceScore = CalculateComplianceScore(report.Issues);
        report.IsCompliant = report.OverallComplianceScore >= 80;

        return Result<ComplianceReport>.Success(report);
    }

    public async Task<Result<QualityMetrics>> CalculateQualityMetricsAsync(UserId userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            return Result<QualityMetrics>.Failure("User not found", ErrorCodes.NOT_FOUND);

        var metrics = new QualityMetrics
        {
            UserId = userId,
            CalculatedAt = _dateTimeProvider.Now
        };

        // Shift regularity score (0-100)
        var shifts = await _shiftRepository.GetByUserIdAsync(userId);
        metrics.ShiftRegularityScore = CalculateShiftRegularityScore(shifts);

        // Procedure diversity score (0-100)
        var procedures = await _procedureRepository.GetByUserIdAsync(userId);
        metrics.ProcedureDiversityScore = CalculateProcedureDiversityScore(procedures);

        // Learning consistency score (0-100)
        var courses = await _courseRepository.GetByUserIdAsync(userId);
        metrics.LearningConsistencyScore = CalculateLearningConsistencyScore(courses);

        // Publication quality score (0-100)
        metrics.PublicationQualityScore = user.Publications.Any() 
            ? (decimal)user.Publications.Average(p => p.CalculateImpactScore()) * 10
            : 0;

        // Overall quality score (weighted average)
        metrics.OverallQualityScore = (
            metrics.ShiftRegularityScore * 0.3m +
            metrics.ProcedureDiversityScore * 0.3m +
            metrics.LearningConsistencyScore * 0.2m +
            metrics.PublicationQualityScore * 0.2m
        );

        // Generate recommendations
        if (metrics.ShiftRegularityScore < 70)
            metrics.Recommendations.Add("Improve shift scheduling consistency");
        if (metrics.ProcedureDiversityScore < 70)
            metrics.Recommendations.Add("Perform more diverse procedure types");
        if (metrics.LearningConsistencyScore < 70)
            metrics.Recommendations.Add("Maintain regular course participation");
        if (metrics.PublicationQualityScore < 50)
            metrics.Recommendations.Add("Consider publishing in higher-impact journals");

        return Result<QualityMetrics>.Success(metrics);
    }

    public async Task<Result<ExaminationReadiness>> CheckExaminationReadinessAsync(UserId userId, ExaminationType examType)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            return Result<ExaminationReadiness>.Failure("User not found", ErrorCodes.NOT_FOUND);

        var readiness = new ExaminationReadiness
        {
            UserId = userId,
            ExaminationType = examType,
            CheckedAt = _dateTimeProvider.Now,
            MissingRequirements = new List<string>(),
            Recommendations = new List<string>()
        };

        // Check basic requirements based on exam type
        switch (examType)
        {
            case ExaminationType.ModuleCompletion:
                await CheckModuleCompletionReadiness(user, readiness);
                break;
            case ExaminationType.SpecializationFinal:
                await CheckSpecializationFinalReadiness(user, readiness);
                break;
            case ExaminationType.PES:
                await CheckPESReadiness(user, readiness);
                break;
        }

        readiness.IsReady = !readiness.MissingRequirements.Any();
        readiness.ReadinessPercentage = CalculateReadinessPercentage(readiness);

        return Result<ExaminationReadiness>.Success(readiness);
    }

    private async Task CheckModuleCompletionReadiness(User user, ExaminationReadiness readiness)
    {
        var activeModule = user.Modules.FirstOrDefault(m => m.IsActive);
        if (activeModule == null)
        {
            readiness.MissingRequirements.Add("No active module found");
            return;
        }

        // Check internships
        var moduleInternships = await _internshipRepository.GetByModuleIdAsync(activeModule.Id);
        var incompleteInternships = moduleInternships.Where(i => i.Status != InternshipStatus.Completed).ToList();
        if (incompleteInternships.Any())
        {
            readiness.MissingRequirements.Add($"{incompleteInternships.Count} incomplete internships");
            foreach (var internship in incompleteInternships.Take(3))
            {
                readiness.Recommendations.Add($"Complete internship at {internship.Department}");
            }
        }

        // Check procedures
        var moduleProcedures = await _procedureRepository.GetByModuleIdAsync(activeModule.Id);
        var proceduresByType = moduleProcedures.GroupBy(p => p.Type);
        
        if (activeModule.Type == ModuleType.Basic)
        {
            // Basic module requires minimum procedures per type
            CheckProcedureRequirements(proceduresByType, readiness, minTypeA: 50, minTypeB: 30, minTypeC: 20);
        }
        else
        {
            // Specialistic module has higher requirements
            CheckProcedureRequirements(proceduresByType, readiness, minTypeA: 100, minTypeB: 60, minTypeC: 40);
        }

        // Check courses
        var moduleCourses = await _courseRepository.GetByModuleIdAsync(activeModule.Id);
        var incompleteCourses = moduleCourses.Where(c => !c.IsCompleted).ToList();
        if (incompleteCourses.Any())
        {
            readiness.MissingRequirements.Add($"{incompleteCourses.Count} incomplete courses");
        }

        // Check minimum hours
        var moduleShifts = await _shiftRepository.GetByModuleIdAsync(activeModule.Id);
        var totalHours = moduleShifts.Sum(s => s.Duration.Value.TotalHours);
        var requiredHours = activeModule.Type == ModuleType.Basic ? 1000 : 2000;
        
        if (totalHours < requiredHours)
        {
            readiness.MissingRequirements.Add($"Insufficient hours: {totalHours}/{requiredHours}");
            readiness.Recommendations.Add($"Complete {requiredHours - totalHours} more hours");
        }
    }

    private async Task CheckSpecializationFinalReadiness(User user, ExaminationReadiness readiness)
    {
        // All modules must be completed
        var incompleteModules = user.Modules.Where(m => m.CompletionPercentage < 100).ToList();
        if (incompleteModules.Any())
        {
            readiness.MissingRequirements.Add($"{incompleteModules.Count} incomplete modules");
            return;
        }

        // Check total specialization duration
        if (user.Specialization?.StartDate != null)
        {
            var duration = _dateTimeProvider.Now - user.Specialization.StartDate;
            var minDuration = TimeSpan.FromDays(user.Specialization.RequiredYears * 365);
            
            if (duration < minDuration)
            {
                var remainingDays = (minDuration - duration).Days;
                readiness.MissingRequirements.Add($"Insufficient specialization duration (need {remainingDays} more days)");
            }
        }

        // Check quality metrics
        var qualityResult = await CalculateQualityMetricsAsync(user.Id);
        if (qualityResult.IsSuccess && qualityResult.Value.OverallQualityScore < 70)
        {
            readiness.MissingRequirements.Add($"Quality score below threshold: {qualityResult.Value.OverallQualityScore:F1}/70");
            readiness.Recommendations.AddRange(qualityResult.Value.Recommendations);
        }

        // Check for any outstanding compliance issues
        var complianceResult = await GenerateComplianceReportAsync(user.Id);
        if (complianceResult.IsSuccess && complianceResult.Value.Issues.Any(i => i.Severity >= IssueSeverity.High))
        {
            readiness.MissingRequirements.Add("Outstanding compliance issues must be resolved");
            readiness.Recommendations.AddRange(complianceResult.Value.Recommendations);
        }
    }

    private async Task CheckPESReadiness(User user, ExaminationReadiness readiness)
    {
        // PES (State Specialization Exam) has specific requirements
        await CheckSpecializationFinalReadiness(user, readiness);

        // Additional PES-specific checks
        if (user.Specialization?.SMKRegistrationNumber == null)
        {
            readiness.MissingRequirements.Add("SMK registration number required");
        }

        // Check for minimum publication requirement
        if (!user.Publications.Any())
        {
            readiness.MissingRequirements.Add("At least one publication required for PES");
            readiness.Recommendations.Add("Submit a case study or research article for publication");
        }

        // Check self-education hours
        var selfEducationHours = user.SelfEducations.Sum(s => s.Hours);
        if (selfEducationHours < 100)
        {
            readiness.MissingRequirements.Add($"Insufficient self-education hours: {selfEducationHours}/100");
        }
    }

    private void CheckProcedureRequirements(
        IEnumerable<IGrouping<ProcedureType, ProcedureBase>> proceduresByType, 
        ExaminationReadiness readiness,
        int minTypeA, int minTypeB, int minTypeC)
    {
        var typeACoun = proceduresByType.FirstOrDefault(g => g.Key == ProcedureType.TypeA)?.Count() ?? 0;
        var typeBCount = proceduresByType.FirstOrDefault(g => g.Key == ProcedureType.TypeB)?.Count() ?? 0;
        var typeCCount = proceduresByType.FirstOrDefault(g => g.Key == ProcedureType.TypeC)?.Count() ?? 0;

        if (typeACoun < minTypeA)
            readiness.MissingRequirements.Add($"Type A procedures: {typeACoun}/{minTypeA}");
        if (typeBCount < minTypeB)
            readiness.MissingRequirements.Add($"Type B procedures: {typeBCount}/{minTypeB}");
        if (typeCCount < minTypeC)
            readiness.MissingRequirements.Add($"Type C procedures: {typeCCount}/{minTypeC}");
    }

    private decimal CalculateShiftRegularityScore(IEnumerable<MedicalShift> shifts)
    {
        if (!shifts.Any()) return 0;

        var shiftsByWeek = shifts
            .GroupBy(s => new { s.Date.Year, Week = s.Date.DayOfYear / 7 })
            .Select(g => g.Sum(s => s.Duration.Value.TotalHours))
            .ToList();

        if (shiftsByWeek.Count < 2) return 100;

        // Calculate standard deviation of weekly hours
        var avg = shiftsByWeek.Average();
        var variance = shiftsByWeek.Average(h => Math.Pow((double)(h - avg), 2));
        var stdDev = Math.Sqrt(variance);

        // Convert to score (lower deviation = higher score)
        var score = Math.Max(0, 100 - (decimal)(stdDev * 2));
        return Math.Min(100, score);
    }

    private decimal CalculateProcedureDiversityScore(IEnumerable<ProcedureBase> procedures)
    {
        if (!procedures.Any()) return 0;

        var uniqueCodes = procedures.Select(p => p.ProcedureCode).Distinct().Count();
        var totalProcedures = procedures.Count();

        // Diversity ratio (unique/total) weighted by total count
        var diversityRatio = (decimal)uniqueCodes / Math.Max(1, totalProcedures);
        var volumeBonus = Math.Min(20, totalProcedures / 10); // Up to 20 bonus points for volume

        return Math.Min(100, diversityRatio * 80 + volumeBonus);
    }

    private decimal CalculateLearningConsistencyScore(IEnumerable<Course> courses)
    {
        if (!courses.Any()) return 0;

        var completedCourses = courses.Where(c => c.IsCompleted).ToList();
        if (!completedCourses.Any()) return 0;

        // Check completion time consistency
        var completionTimes = completedCourses
            .Where(c => c.CompletedAt.HasValue)
            .Select(c => (c.CompletedAt!.Value - c.StartDate).Days)
            .ToList();

        if (!completionTimes.Any()) return 50;

        var avgCompletionTime = completionTimes.Average();
        var onTimeCompletions = completionTimes.Count(t => t <= avgCompletionTime * 1.2);

        return (decimal)onTimeCompletions / completionTimes.Count * 100;
    }

    private decimal CalculateComplianceScore(List<ComplianceIssue> issues)
    {
        if (!issues.Any()) return 100;

        var deductions = issues.Sum(i => i.Severity switch
        {
            IssueSeverity.Critical => 30,
            IssueSeverity.High => 20,
            IssueSeverity.Medium => 10,
            IssueSeverity.Low => 5,
            _ => 0
        });

        return Math.Max(0, 100 - deductions);
    }

    private decimal CalculateReadinessPercentage(ExaminationReadiness readiness)
    {
        if (!readiness.MissingRequirements.Any()) return 100;

        // Estimate based on missing requirements
        var totalRequirements = readiness.ExaminationType switch
        {
            ExaminationType.ModuleCompletion => 10,
            ExaminationType.SpecializationFinal => 15,
            ExaminationType.PES => 20,
            _ => 10
        };

        var completedRequirements = totalRequirements - readiness.MissingRequirements.Count;
        return Math.Max(0, (decimal)completedRequirements / totalRequirements * 100);
    }
}

public class ComplianceReport
{
    public UserId UserId { get; set; }
    public DateTime GeneratedAt { get; set; }
    public List<ComplianceIssue> Issues { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
    public decimal OverallComplianceScore { get; set; }
    public bool IsCompliant { get; set; }
}

public class ComplianceIssue
{
    public ComplianceIssueType Type { get; set; }
    public IssueSeverity Severity { get; set; }
    public string Description { get; set; }
    public string AffectedPeriod { get; set; }
}

public enum ComplianceIssueType
{
    InsufficientHours,
    ExcessiveHours,
    OverdueInternship,
    InvalidModuleProgression,
    MissingRequiredProcedures,
    IncompleteCourses
}

public enum IssueSeverity
{
    Low,
    Medium,
    High,
    Critical
}

public class QualityMetrics
{
    public UserId UserId { get; set; }
    public DateTime CalculatedAt { get; set; }
    public decimal ShiftRegularityScore { get; set; }
    public decimal ProcedureDiversityScore { get; set; }
    public decimal LearningConsistencyScore { get; set; }
    public decimal PublicationQualityScore { get; set; }
    public decimal OverallQualityScore { get; set; }
    public List<string> Recommendations { get; set; } = new();
}

public class ExaminationReadiness
{
    public UserId UserId { get; set; }
    public ExaminationType ExaminationType { get; set; }
    public DateTime CheckedAt { get; set; }
    public bool IsReady { get; set; }
    public decimal ReadinessPercentage { get; set; }
    public List<string> MissingRequirements { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}

public enum ExaminationType
{
    ModuleCompletion,
    SpecializationFinal,
    PES
}
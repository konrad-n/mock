using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Core.Entities;

namespace SledzSpecke.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public Task SendShiftConflictNotificationAsync(InternshipId internshipId, DateTime conflictDate, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Shift conflict notification: InternshipId={InternshipId}, Date={Date}", internshipId.Value, conflictDate);
        return Task.CompletedTask;
    }

    public Task SendMonthlyHoursReminderAsync(InternshipId internshipId, int hoursNeeded, int daysRemaining, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Monthly hours reminder: InternshipId={InternshipId}, HoursNeeded={Hours}, DaysRemaining={Days}", 
            internshipId.Value, hoursNeeded, daysRemaining);
        return Task.CompletedTask;
    }

    public Task SendPendingApprovalNotificationAsync(string supervisorEmail, MedicalShiftId shiftId, DateTime shiftDate, int hours, int minutes, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Pending approval notification: Supervisor={Supervisor}, ShiftId={ShiftId}, Date={Date}", 
            supervisorEmail, shiftId.Value, shiftDate);
        return Task.CompletedTask;
    }

    public Task SendShiftApprovalConfirmationAsync(UserId userId, MedicalShift shift, string approvedBy, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Shift approval confirmation: UserId={UserId}, ShiftId={ShiftId}, ApprovedBy={ApprovedBy}", 
            userId.Value, shift.ShiftId, approvedBy);
        return Task.CompletedTask;
    }

    public Task SendMonthlyRequirementMetNotificationAsync(UserId userId, DateTime month, int totalHours, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Monthly requirement met: UserId={UserId}, Month={Month}, Hours={Hours}", 
            userId.Value, month.ToString("yyyy-MM"), totalHours);
        return Task.CompletedTask;
    }

    public Task NotifyDepartmentCoordinatorAsync(string department, UserId userId, object monthlyProgress, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Department coordinator notification: Department={Department}, UserId={UserId}", 
            department, userId.Value);
        return Task.CompletedTask;
    }

    public Task SendMonthlyReportGeneratedNotificationAsync(UserId userId, DateTime month, string reportPath, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Monthly report generated: UserId={UserId}, Month={Month}, Path={Path}", 
            userId.Value, month.ToString("yyyy-MM"), reportPath);
        return Task.CompletedTask;
    }

    public Task SendInvalidProcedureWarningAsync(InternshipId internshipId, string procedureCode, string reason, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Invalid procedure: InternshipId={InternshipId}, Code={Code}, Reason={Reason}", 
            internshipId.Value, procedureCode, reason);
        return Task.CompletedTask;
    }

    public Task SendFirstProcedureAchievementAsync(InternshipId internshipId, string procedureCode, string procedureName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("First procedure achievement: InternshipId={InternshipId}, Code={Code}", 
            internshipId.Value, procedureCode);
        return Task.CompletedTask;
    }

    public Task SendDailyLimitExceededWarningAsync(InternshipId internshipId, string procedureCode, int dailyLimit, int actualCount, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Daily limit exceeded: InternshipId={InternshipId}, Code={Code}, Limit={Limit}, Count={Count}", 
            internshipId.Value, procedureCode, dailyLimit, actualCount);
        return Task.CompletedTask;
    }

    public Task SendMilestoneAchievementNotificationAsync(UserId userId, Milestone milestone, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Milestone achieved: UserId={UserId}, Type={Type}, Name={Name}", 
            userId.Value, milestone.Type, milestone.Name);
        return Task.CompletedTask;
    }

    public Task SendModuleCompletionNotificationAsync(UserId userId, string moduleName, DateTime completionDate, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Module completed: UserId={UserId}, Module={Module}, Date={Date}", 
            userId.Value, moduleName, completionDate);
        return Task.CompletedTask;
    }

    public Task RequestCertificateGenerationAsync(UserId userId, ModuleId moduleId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Certificate generation requested: UserId={UserId}, ModuleId={ModuleId}", 
            userId.Value, moduleId.Value);
        return Task.CompletedTask;
    }

    public Task SendSpecializationCompletionNotificationAsync(UserId userId, int specializationId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Specialization completed: UserId={UserId}, SpecializationId={SpecializationId}", 
            userId.Value, specializationId);
        return Task.CompletedTask;
    }

    public Task SendUnlockedProceduresNotificationAsync(UserId userId, List<string> unlockedProcedures, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Procedures unlocked: UserId={UserId}, Count={Count}, Procedures={Procedures}", 
            userId.Value, unlockedProcedures.Count, string.Join(", ", unlockedProcedures));
        return Task.CompletedTask;
    }

    public Task SendYearCompletionNotificationAsync(UserId userId, int year, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Year completed: UserId={UserId}, Year={Year}", userId.Value, year);
        return Task.CompletedTask;
    }
}
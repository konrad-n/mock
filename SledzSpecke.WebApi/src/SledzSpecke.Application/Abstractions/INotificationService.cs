using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Core.Entities;

namespace SledzSpecke.Application.Abstractions;

public interface INotificationService
{
    // Medical Shift Notifications
    Task SendShiftConflictNotificationAsync(
        InternshipId internshipId,
        DateTime conflictDate,
        CancellationToken cancellationToken = default);

    Task SendMonthlyHoursReminderAsync(
        InternshipId internshipId,
        int hoursNeeded,
        int daysRemaining,
        CancellationToken cancellationToken = default);

    Task SendPendingApprovalNotificationAsync(
        string supervisorEmail,
        MedicalShiftId shiftId,
        DateTime shiftDate,
        int hours,
        int minutes,
        CancellationToken cancellationToken = default);

    Task SendShiftApprovalConfirmationAsync(
        UserId userId,
        MedicalShift shift,
        string approvedBy,
        CancellationToken cancellationToken = default);

    Task SendMonthlyRequirementMetNotificationAsync(
        UserId userId,
        DateTime month,
        int totalHours,
        CancellationToken cancellationToken = default);

    Task NotifyDepartmentCoordinatorAsync(
        string department,
        UserId userId,
        object monthlyProgress,
        CancellationToken cancellationToken = default);

    Task SendMonthlyReportGeneratedNotificationAsync(
        UserId userId,
        DateTime month,
        string reportPath,
        CancellationToken cancellationToken = default);

    // Procedure Notifications
    Task SendInvalidProcedureWarningAsync(
        InternshipId internshipId,
        string procedureCode,
        string reason,
        CancellationToken cancellationToken = default);

    Task SendFirstProcedureAchievementAsync(
        InternshipId internshipId,
        string procedureCode,
        string procedureName,
        CancellationToken cancellationToken = default);

    Task SendDailyLimitExceededWarningAsync(
        InternshipId internshipId,
        string procedureCode,
        int dailyLimit,
        int actualCount,
        CancellationToken cancellationToken = default);

    Task SendMilestoneAchievementNotificationAsync(
        UserId userId,
        Milestone milestone,
        CancellationToken cancellationToken = default);

    Task SendModuleCompletionNotificationAsync(
        UserId userId,
        string moduleName,
        DateTime completionDate,
        CancellationToken cancellationToken = default);

    Task RequestCertificateGenerationAsync(
        UserId userId,
        ModuleId moduleId,
        CancellationToken cancellationToken = default);

    Task SendSpecializationCompletionNotificationAsync(
        UserId userId,
        int specializationId,
        CancellationToken cancellationToken = default);

    Task SendUnlockedProceduresNotificationAsync(
        UserId userId,
        List<string> unlockedProcedures,
        CancellationToken cancellationToken = default);

    Task SendYearCompletionNotificationAsync(
        UserId userId,
        int year,
        CancellationToken cancellationToken = default);
}

public class Milestone
{
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime AchievedAt { get; set; }
    public object Metadata { get; set; } = new { };
}
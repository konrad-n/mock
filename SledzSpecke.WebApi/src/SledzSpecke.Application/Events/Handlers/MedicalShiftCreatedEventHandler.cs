using MediatR;
using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Events;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Events.Handlers;

public sealed class MedicalShiftCreatedEventHandler : INotificationHandler<MedicalShiftCreatedEvent>
{
    private readonly ILogger<MedicalShiftCreatedEventHandler> _logger;
    private readonly IMedicalShiftRepository _medicalShiftRepository;
    private readonly IInternshipRepository _internshipRepository;
    private readonly INotificationService _notificationService;
    private readonly IStatisticsService _statisticsService;

    public MedicalShiftCreatedEventHandler(
        ILogger<MedicalShiftCreatedEventHandler> logger,
        IMedicalShiftRepository medicalShiftRepository,
        IInternshipRepository internshipRepository,
        INotificationService notificationService,
        IStatisticsService statisticsService)
    {
        _logger = logger;
        _medicalShiftRepository = medicalShiftRepository;
        _internshipRepository = internshipRepository;
        _notificationService = notificationService;
        _statisticsService = statisticsService;
    }

    public async Task Handle(MedicalShiftCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing MedicalShiftCreated event: ShiftId={ShiftId}, InternshipId={InternshipId}, Date={Date}, Duration={Hours}h {Minutes}m",
            notification.ShiftId.Value,
            notification.InternshipId.Value,
            notification.Date,
            notification.Hours,
            notification.Minutes);

        try
        {
            // 1. Check for shift conflicts
            var existingShifts = await _medicalShiftRepository.GetByInternshipIdAsync(notification.InternshipId.Value);
            var conflictingShift = existingShifts.FirstOrDefault(s => 
                s.Date == notification.Date && 
                s.Id != notification.ShiftId.Value &&
                s.IsApproved);

            if (conflictingShift != null)
            {
                await _notificationService.SendShiftConflictNotificationAsync(
                    notification.InternshipId,
                    notification.Date,
                    cancellationToken);
            }

            // 2. Update statistics
            await _statisticsService.UpdateMonthlyShiftStatisticsAsync(
                notification.InternshipId,
                notification.Date,
                cancellationToken);

            // 3. Check monthly progress and send reminders if needed
            var monthStart = new DateTime(notification.Date.Year, notification.Date.Month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);
            var monthlyShifts = existingShifts.Where(s => 
                s.Date >= monthStart && 
                s.Date <= monthEnd && 
                s.IsApproved).ToList();

            var totalHours = monthlyShifts.Sum(s => s.Hours);
            var daysRemaining = (monthEnd - notification.Date).Days;
            
            // If less than 7 days remaining and insufficient hours, send reminder
            if (daysRemaining < 7 && totalHours < 160) // Assuming 160 hours/month requirement
            {
                var hoursNeeded = 160 - totalHours;
                await _notificationService.SendMonthlyHoursReminderAsync(
                    notification.InternshipId,
                    hoursNeeded,
                    daysRemaining,
                    cancellationToken);
            }

            // 4. Notify supervisor about pending approval
            var internship = await _internshipRepository.GetByIdAsync(notification.InternshipId.Value);
            if (internship != null && !string.IsNullOrEmpty(internship.SupervisorName))
            {
                // In real implementation, supervisor email would be retrieved from a separate service
                // For now, we'll use the supervisor name as a placeholder
                await _notificationService.SendPendingApprovalNotificationAsync(
                    internship.SupervisorName, // Would be email in production
                    notification.ShiftId,
                    notification.Date,
                    notification.Hours,
                    notification.Minutes,
                    cancellationToken);
            }

            _logger.LogInformation(
                "Successfully processed MedicalShiftCreated event for ShiftId={ShiftId}",
                notification.ShiftId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error processing MedicalShiftCreated event for ShiftId={ShiftId}",
                notification.ShiftId.Value);
            // Don't throw - event handlers should be resilient
        }
    }
}
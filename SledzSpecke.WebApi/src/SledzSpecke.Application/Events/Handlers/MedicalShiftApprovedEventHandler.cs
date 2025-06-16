using MediatR;
using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Events;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Core.Entities;
using SledzSpecke.Application.Extensions;

namespace SledzSpecke.Application.Events.Handlers;

public sealed class MedicalShiftApprovedEventHandler : INotificationHandler<MedicalShiftApprovedEvent>
{
    private readonly ILogger<MedicalShiftApprovedEventHandler> _logger;
    private readonly IMedicalShiftRepository _medicalShiftRepository;
    private readonly IInternshipRepository _internshipRepository;
    private readonly INotificationService _notificationService;
    private readonly IStatisticsService _statisticsService;
    private readonly IProjectionService _projectionService;
    private readonly IPdfGenerationService _pdfService;

    public MedicalShiftApprovedEventHandler(
        ILogger<MedicalShiftApprovedEventHandler> logger,
        IMedicalShiftRepository medicalShiftRepository,
        IInternshipRepository internshipRepository,
        INotificationService notificationService,
        IStatisticsService statisticsService,
        IProjectionService projectionService,
        IPdfGenerationService pdfService)
    {
        _logger = logger;
        _medicalShiftRepository = medicalShiftRepository;
        _internshipRepository = internshipRepository;
        _notificationService = notificationService;
        _statisticsService = statisticsService;
        _projectionService = projectionService;
        _pdfService = pdfService;
    }

    public async Task Handle(MedicalShiftApprovedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing MedicalShiftApproved event: ShiftId={ShiftId}, ApprovedOn={ApprovedOn}, ApprovedBy={ApprovedBy}",
            notification.ShiftId.Value,
            notification.ApprovedOn,
            notification.ApprovedBy);

        try
        {
            // 1. Get shift details
            var shift = await _medicalShiftRepository.GetByIdAsync(notification.ShiftId.Value);
            if (shift == null)
            {
                _logger.LogWarning("Medical shift not found: ShiftId={ShiftId}", notification.ShiftId.Value);
                return;
            }

            // 2. Send confirmation to intern
            var internship = await _internshipRepository.GetByIdAsync(shift.InternshipId);
            if (internship != null)
            {
                // In a real system, we would get the user ID from the internship context
                // For now, we'll use a placeholder approach
                var userId = new UserId(1); // Would be retrieved from context/service
                await _notificationService.SendShiftApprovalConfirmationAsync(
                    userId,
                    shift,
                    notification.ApprovedBy,
                    cancellationToken);
            }

            // 3. Update statistics cache
            await _statisticsService.UpdateApprovedHoursStatisticsAsync(
                new InternshipId(shift.InternshipId),
                shift.Date,
                cancellationToken);

            // 4. Check if monthly requirement is met
            var monthlyProgress = await CalculateMonthlyProgress(shift, cancellationToken);
            
            if (monthlyProgress.IsRequirementMet && !monthlyProgress.WasPreviouslyMet)
            {
                var userId = new UserId(1); // Would be retrieved from context/service
                await _notificationService.SendMonthlyRequirementMetNotificationAsync(
                    userId,
                    monthlyProgress.Month,
                    monthlyProgress.TotalHours,
                    cancellationToken);
            }

            // 5. Update read models
            await _projectionService.UpdateStudentProgressProjectionAsync(
                new InternshipId(shift.InternshipId),
                cancellationToken);

            await _projectionService.UpdateSupervisorWorkloadProjectionAsync(
                notification.ApprovedBy,
                cancellationToken);

            // 6. Generate monthly report if month is complete
            if (IsMonthComplete(shift.Date) && monthlyProgress.IsRequirementMet)
            {
                await GenerateMonthlyReport(internship, shift.Date, cancellationToken);
            }

            // 7. Update coordinator if this is a significant milestone
            if (monthlyProgress.TotalHours >= 160 && monthlyProgress.TotalHours - shift.Hours < 160)
            {
                var userId = new UserId(1); // Would be retrieved from context/service
                await _notificationService.NotifyDepartmentCoordinatorAsync(
                    internship.DepartmentName,
                    userId,
                    monthlyProgress,
                    cancellationToken);
            }

            _logger.LogInformation(
                "Successfully processed MedicalShiftApproved event for ShiftId={ShiftId}",
                notification.ShiftId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error processing MedicalShiftApproved event for ShiftId={ShiftId}",
                notification.ShiftId.Value);
        }
    }

    private async Task<MonthlyProgress> CalculateMonthlyProgress(MedicalShift approvedShift, CancellationToken cancellationToken)
    {
        var monthStart = new DateTime(approvedShift.Date.Year, approvedShift.Date.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);
        
        var monthlyShifts = await _medicalShiftRepository.GetByInternshipIdAndDateRangeAsync(
            approvedShift.InternshipId,
            monthStart,
            monthEnd);

        var approvedShifts = monthlyShifts.Where(s => s.IsApproved).ToList();
        var totalHours = approvedShifts.Sum(s => s.Hours);
        var totalMinutes = approvedShifts.Sum(s => s.Minutes);
        
        // Convert excess minutes to hours
        totalHours += totalMinutes / 60;
        totalMinutes = totalMinutes % 60;

        return new MonthlyProgress
        {
            Month = monthStart,
            TotalHours = totalHours,
            TotalMinutes = totalMinutes,
            IsRequirementMet = totalHours >= 160, // Assuming 160 hours/month
            WasPreviouslyMet = totalHours - approvedShift.Hours >= 160,
            ShiftCount = approvedShifts.Count()
        };
    }

    private bool IsMonthComplete(DateTime date)
    {
        var today = DateTime.Today;
        return date.Month < today.Month || date.Year < today.Year;
    }

    private async Task GenerateMonthlyReport(Internship internship, DateTime shiftDate, CancellationToken cancellationToken)
    {
        try
        {
            var monthStart = new DateTime(shiftDate.Year, shiftDate.Month, 1);
            var reportPath = await _pdfService.GenerateMonthlyReportAsync(
                new InternshipId(internship.Id),
                monthStart,
                cancellationToken);

            var userId = new UserId(1); // Would be retrieved from context/service
            await _notificationService.SendMonthlyReportGeneratedNotificationAsync(
                userId,
                monthStart,
                reportPath,
                cancellationToken);

            _logger.LogInformation(
                "Generated monthly report for InternshipId={InternshipId}, Month={Month}",
                internship.Id,
                monthStart.ToString("yyyy-MM"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to generate monthly report for InternshipId={InternshipId}",
                internship.Id);
        }
    }

    private record MonthlyProgress
    {
        public DateTime Month { get; init; }
        public int TotalHours { get; init; }
        public int TotalMinutes { get; init; }
        public bool IsRequirementMet { get; init; }
        public bool WasPreviouslyMet { get; init; }
        public int ShiftCount { get; init; }
    }
}
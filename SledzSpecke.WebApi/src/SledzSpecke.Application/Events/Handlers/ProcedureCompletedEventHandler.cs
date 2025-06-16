using MediatR;
using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Events;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.ValueObjects;
using SledzSpecke.Core.Entities;

namespace SledzSpecke.Application.Events.Handlers;

public sealed class ProcedureCompletedEventHandler : INotificationHandler<Core.Events.ProcedureCompletedEvent>
{
    private readonly ILogger<ProcedureCompletedEventHandler> _logger;
    private readonly IProcedureRepository _procedureRepository;
    private readonly IInternshipRepository _internshipRepository;
    private readonly IStatisticsService _statisticsService;
    private readonly IProjectionService _projectionService;

    public ProcedureCompletedEventHandler(
        ILogger<ProcedureCompletedEventHandler> logger,
        IProcedureRepository procedureRepository,
        IInternshipRepository internshipRepository,
        IStatisticsService statisticsService,
        IProjectionService projectionService)
    {
        _logger = logger;
        _procedureRepository = procedureRepository;
        _internshipRepository = internshipRepository;
        _statisticsService = statisticsService;
        _projectionService = projectionService;
    }

    public async Task Handle(Core.Events.ProcedureCompletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing ProcedureCompleted event: ProcedureId={ProcedureId}, Code={Code}, Count={Count}",
            notification.ProcedureId.Value,
            notification.ProcedureCode,
            notification.Count);

        try
        {
            // 1. Get procedure and related data
            var procedure = await _procedureRepository.GetByIdAsync(notification.ProcedureId.Value);
            if (procedure == null)
            {
                _logger.LogWarning("Procedure not found: ProcedureId={ProcedureId}", notification.ProcedureId.Value);
                return;
            }

            var internship = await _internshipRepository.GetByIdAsync(procedure.InternshipId);
            if (internship == null)
            {
                _logger.LogWarning("Internship not found: InternshipId={InternshipId}", procedure.InternshipId);
                return;
            }

            // 2. Update statistics for the procedure
            await _statisticsService.UpdateProcedureStatisticsAsync(
                internship.DepartmentName,
                notification.ProcedureCode,
                notification.Count,
                cancellationToken);

            // 3. Update projections
            await _projectionService.UpdateStudentProgressProjectionAsync(
                new InternshipId(internship.Id),
                cancellationToken);

            await _projectionService.UpdateDepartmentAnalyticsProjectionAsync(
                internship.DepartmentName,
                cancellationToken);

            // 4. Track procedure count milestones
            var allProcedures = await _procedureRepository.GetByInternshipIdAsync(internship.Id);
            var totalProceduresOfType = allProcedures.Count(p => p.Code == notification.ProcedureCode);
            
            if (totalProceduresOfType == 10 || totalProceduresOfType == 50 || totalProceduresOfType == 100)
            {
                _logger.LogInformation(
                    "Procedure milestone reached: InternshipId={InternshipId}, Code={Code}, Total={Total}",
                    internship.Id,
                    notification.ProcedureCode,
                    totalProceduresOfType);
            }

            _logger.LogInformation(
                "Successfully processed ProcedureCompleted event for ProcedureId={ProcedureId}",
                notification.ProcedureId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error processing ProcedureCompleted event for ProcedureId={ProcedureId}",
                notification.ProcedureId.Value);
        }
    }
}
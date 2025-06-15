using MediatR;
using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Events;

namespace SledzSpecke.Application.Events.Handlers;

public sealed class MedicalShiftCreatedEventHandler : INotificationHandler<MedicalShiftCreatedEvent>
{
    private readonly ILogger<MedicalShiftCreatedEventHandler> _logger;

    public MedicalShiftCreatedEventHandler(ILogger<MedicalShiftCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(MedicalShiftCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Medical shift created: ShiftId={ShiftId}, InternshipId={InternshipId}, Date={Date}, Duration={Hours}h {Minutes}m, Location={Location}, Year={Year}",
            notification.ShiftId.Value,
            notification.InternshipId.Value,
            notification.Date,
            notification.Hours,
            notification.Minutes,
            notification.Location,
            notification.Year);

        // Here you could:
        // - Send notifications
        // - Update statistics
        // - Trigger other workflows
        // - Update read models/projections
        
        return Task.CompletedTask;
    }
}
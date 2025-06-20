using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Application.Commands.Handlers;

internal sealed class UpdateProcedureRealizationHandler : ICommandHandler<UpdateProcedureRealizationCommand>
{
    private readonly IProcedureRealizationRepository _procedureRealizationRepository;
    private readonly ILogger<UpdateProcedureRealizationHandler> _logger;

    public UpdateProcedureRealizationHandler(
        IProcedureRealizationRepository procedureRealizationRepository,
        ILogger<UpdateProcedureRealizationHandler> logger)
    {
        _procedureRealizationRepository = procedureRealizationRepository;
        _logger = logger;
    }

    public async Task HandleAsync(UpdateProcedureRealizationCommand command)
    {
        // Get existing realization
        var realization = await _procedureRealizationRepository.GetByIdAsync(command.Id);
        if (realization is null)
        {
            _logger.LogWarning("Procedure realization {Id} not found", command.Id);
            throw new InvalidOperationException($"Realizacja procedury o ID {command.Id} nie została znaleziona");
        }

        // Update realization
        var updateResult = realization.Update(command.Date, command.Location, command.Role);
        if (!updateResult.IsSuccess)
        {
            _logger.LogWarning("Failed to update procedure realization: {Error}", updateResult.Error);
            throw new InvalidOperationException($"Nie można zaktualizować realizacji procedury: {updateResult.Error}");
        }

        // Save changes
        await _procedureRealizationRepository.UpdateAsync(realization);

        _logger.LogInformation(
            "Updated procedure realization {Id} with new date {Date}, location {Location}, role {Role}",
            command.Id, command.Date, command.Location, command.Role);
    }
}
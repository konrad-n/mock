using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Application.Commands.Handlers;

internal sealed class DeleteProcedureRealizationHandler : ICommandHandler<DeleteProcedureRealizationCommand>
{
    private readonly IProcedureRealizationRepository _procedureRealizationRepository;
    private readonly ILogger<DeleteProcedureRealizationHandler> _logger;

    public DeleteProcedureRealizationHandler(
        IProcedureRealizationRepository procedureRealizationRepository,
        ILogger<DeleteProcedureRealizationHandler> logger)
    {
        _procedureRealizationRepository = procedureRealizationRepository;
        _logger = logger;
    }

    public async Task HandleAsync(DeleteProcedureRealizationCommand command)
    {
        // Get existing realization
        var realization = await _procedureRealizationRepository.GetByIdAsync(command.Id);
        if (realization is null)
        {
            _logger.LogWarning("Procedure realization {Id} not found", command.Id);
            throw new InvalidOperationException($"Realizacja procedury o ID {command.Id} nie została znaleziona");
        }

        // Delete realization
        await _procedureRealizationRepository.DeleteAsync(realization);

        _logger.LogInformation("Deleted procedure realization {Id}", command.Id);
    }
}
using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

internal sealed class AddProcedureRealizationHandler : ICommandHandler<AddProcedureRealizationCommand>
{
    private readonly IProcedureRealizationRepository _procedureRealizationRepository;
    private readonly IProcedureRequirementRepository _procedureRequirementRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AddProcedureRealizationHandler> _logger;

    public AddProcedureRealizationHandler(
        IProcedureRealizationRepository procedureRealizationRepository,
        IProcedureRequirementRepository procedureRequirementRepository,
        IUserRepository userRepository,
        ILogger<AddProcedureRealizationHandler> logger)
    {
        _procedureRealizationRepository = procedureRealizationRepository;
        _procedureRequirementRepository = procedureRequirementRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task HandleAsync(AddProcedureRealizationCommand command)
    {
        // Validate requirement exists
        var requirement = await _procedureRequirementRepository.GetByIdAsync(command.RequirementId);
        if (requirement is null)
        {
            _logger.LogWarning("Procedure requirement {RequirementId} not found", command.RequirementId);
            throw new InvalidOperationException($"Wymaganie procedury o ID {command.RequirementId} nie zostało znalezione");
        }

        // Validate user exists
        var user = await _userRepository.GetByIdAsync(command.UserId);
        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found", command.UserId);
            throw new InvalidOperationException($"Użytkownik o ID {command.UserId} nie został znaleziony");
        }

        // Create procedure realization
        var createResult = ProcedureRealization.Create(
            command.RequirementId,
            command.UserId,
            command.Date,
            command.Location,
            command.Role,
            command.Year);

        if (!createResult.IsSuccess)
        {
            _logger.LogWarning("Failed to create procedure realization: {Error}", createResult.Error);
            throw new InvalidOperationException($"Nie można utworzyć realizacji procedury: {createResult.Error}");
        }

        var realization = createResult.Value!;

        // Add to repository
        var realizationId = await _procedureRealizationRepository.AddAsync(realization);

        _logger.LogInformation(
            "Added procedure realization {RealizationId} for requirement {RequirementId} by user {UserId}",
            realizationId, command.RequirementId, command.UserId);
    }
}
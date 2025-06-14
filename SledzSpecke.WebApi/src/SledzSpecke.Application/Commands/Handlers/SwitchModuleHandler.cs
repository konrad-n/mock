using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class SwitchModuleHandler : ICommandHandler<SwitchModule>
{
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IModuleRepository _moduleRepository;
    private readonly IUserContextService _userContextService;
    private readonly IUserRepository _userRepository;

    public SwitchModuleHandler(
        ISpecializationRepository specializationRepository,
        IModuleRepository moduleRepository,
        IUserContextService userContextService,
        IUserRepository userRepository)
    {
        _specializationRepository = specializationRepository;
        _moduleRepository = moduleRepository;
        _userContextService = userContextService;
        _userRepository = userRepository;
    }

    public async Task HandleAsync(SwitchModule command)
    {
        // Get current user
        var userId = _userContextService.GetUserId();
        var user = await _userRepository.GetByIdAsync(new UserId(userId));
        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        // Verify user owns this specialization
        if (user.SpecializationId.Value != command.SpecializationId)
        {
            throw new UnauthorizedAccessException("You can only switch modules in your own specialization.");
        }

        // Get specialization
        var specialization = await _specializationRepository.GetByIdAsync(new SpecializationId(command.SpecializationId));
        if (specialization is null)
        {
            throw new NotFoundException($"Specialization with ID {command.SpecializationId} not found.");
        }

        // Get and verify module
        var module = await _moduleRepository.GetByIdAsync(new ModuleId(command.ModuleId));
        if (module is null)
        {
            throw new NotFoundException($"Module with ID {command.ModuleId} not found.");
        }

        // Verify module belongs to specialization
        if (module.SpecializationId != specialization.Id)
        {
            throw new InvalidOperationException("Module does not belong to the specified specialization.");
        }

        // Update current module
        specialization.SetCurrentModule(new ModuleId(command.ModuleId));
        await _specializationRepository.UpdateAsync(specialization);
    }
}
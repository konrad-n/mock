using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class UpdateUserPreferencesHandler : ICommandHandler<UpdateUserPreferences>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public UpdateUserPreferencesHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IUserContextService userContextService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task HandleAsync(UpdateUserPreferences command)
    {
        var currentUserId = _userContextService.GetUserId();
        var user = await _userRepository.GetByIdAsync(new UserId((int)currentUserId));
        
        if (user is null)
        {
            throw new NotFoundException($"User with ID {currentUserId} not found.");
        }

        user.UpdateNotificationPreferences(
            command.NotificationsEnabled,
            command.EmailNotificationsEnabled);

        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }
}
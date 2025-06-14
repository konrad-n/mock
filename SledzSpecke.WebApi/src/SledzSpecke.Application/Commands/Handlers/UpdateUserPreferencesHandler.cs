using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

internal sealed class UpdateUserPreferencesHandler : ICommandHandler<UpdateUserPreferences>
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

        // Validate language and theme
        var allowedLanguages = new[] { "en", "pl", "de" };
        var allowedThemes = new[] { "light", "dark", "auto" };

        if (!allowedLanguages.Contains(command.Language))
        {
            throw new ValidationException($"Invalid language. Allowed values: {string.Join(", ", allowedLanguages)}");
        }

        if (!allowedThemes.Contains(command.Theme))
        {
            throw new ValidationException($"Invalid theme. Allowed values: {string.Join(", ", allowedThemes)}");
        }

        user.UpdatePreferences(
            command.Language,
            command.Theme,
            command.NotificationsEnabled,
            command.EmailNotificationsEnabled);

        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }
}
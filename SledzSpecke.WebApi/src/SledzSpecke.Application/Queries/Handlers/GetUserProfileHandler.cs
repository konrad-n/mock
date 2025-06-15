using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

public sealed class GetUserProfileHandler : IQueryHandler<GetUserProfile, UserProfileDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IUserContextService _userContextService;

    public GetUserProfileHandler(
        IUserRepository userRepository,
        ISpecializationRepository specializationRepository,
        IUserContextService userContextService)
    {
        _userRepository = userRepository;
        _specializationRepository = specializationRepository;
        _userContextService = userContextService;
    }

    public async Task<UserProfileDto> HandleAsync(GetUserProfile query)
    {
        var currentUserId = _userContextService.GetUserId();
        var user = await _userRepository.GetByIdAsync(new UserId((int)currentUserId));
        
        if (user is null)
        {
            throw new NotFoundException($"User with ID {currentUserId} not found.");
        }

        var specialization = await _specializationRepository.GetByIdAsync(user.SpecializationId);
        
        return new UserProfileDto
        {
            Id = user.Id.Value,
            Email = user.Email.Value,
            Username = user.Username.Value,
            FullName = user.FullName.Value,
            SmkVersion = user.SmkVersion.Value,
            SpecializationId = user.SpecializationId.Value,
            SpecializationName = specialization?.Name ?? "Unknown",
            PhoneNumber = user.PhoneNumber?.Value,
            DateOfBirth = user.DateOfBirth,
            Bio = user.Bio?.Value,
            ProfilePicturePath = user.ProfilePicturePath?.Value,
            Preferences = new UserPreferencesDto
            {
                Language = user.PreferredLanguage?.Value ?? "en",
                Theme = user.PreferredTheme?.Value ?? "light",
                NotificationsEnabled = user.NotificationsEnabled,
                EmailNotificationsEnabled = user.EmailNotificationsEnabled
            },
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        };
    }
}
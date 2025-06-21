using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Queries.Handlers;

public sealed class GetUserProfileHandler : IQueryHandler<GetUserProfile, UserProfileDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserContextService _userContextService;

    public GetUserProfileHandler(
        IUserRepository userRepository,
        IUserContextService userContextService)
    {
        _userRepository = userRepository;
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
        
        var nameParts = user.Name.Split(' ');
        var firstName = nameParts.FirstOrDefault() ?? "";
        var lastName = nameParts.Skip(1).FirstOrDefault() ?? "";
        
        return new UserProfileDto
        {
            Id = user.UserId,
            Email = user.Email,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = user.PhoneNumber,
            DateOfBirth = user.DateOfBirth,
            CorrespondenceAddress = new AddressDto
            {
                Street = user.CorrespondenceAddress,
                HouseNumber = "",
                ApartmentNumber = "",
                PostalCode = "",
                City = "",
                Province = ""
            },
            Preferences = new UserPreferencesDto
            {
                NotificationsEnabled = user.NotificationsEnabled,
                EmailNotificationsEnabled = user.EmailNotificationsEnabled
            },
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        };
    }
}
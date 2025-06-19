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
        
        return new UserProfileDto
        {
            Id = user.Id.Value,
            Email = user.Email.Value,
            FirstName = user.FirstName.Value,
            LastName = user.LastName.Value,
            PhoneNumber = user.PhoneNumber.Value,
            DateOfBirth = user.DateOfBirth,
            CorrespondenceAddress = new AddressDto
            {
                Street = user.CorrespondenceAddress.Street,
                HouseNumber = user.CorrespondenceAddress.HouseNumber,
                ApartmentNumber = user.CorrespondenceAddress.ApartmentNumber,
                PostalCode = user.CorrespondenceAddress.PostalCode,
                City = user.CorrespondenceAddress.City,
                Province = user.CorrespondenceAddress.Province
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
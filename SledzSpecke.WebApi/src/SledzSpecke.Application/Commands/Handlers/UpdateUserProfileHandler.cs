using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class UpdateUserProfileHandler : IResultCommandHandler<UpdateUserProfile>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public UpdateUserProfileHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IUserContextService userContextService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _userContextService = userContextService;
    }

    public async Task<Result> HandleAsync(UpdateUserProfile command, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = _userContextService.GetUserId();
            var user = await _userRepository.GetByIdAsync((int)currentUserId);
            
            if (user is null)
            {
                return Result.Failure($"User with ID {currentUserId} not found.");
            }

            // Validate input
            if (string.IsNullOrEmpty(command.Email))
                return Result.Failure("Email is required");
            if (string.IsNullOrEmpty(command.FirstName) || string.IsNullOrEmpty(command.LastName))
                return Result.Failure("First name and last name are required");
            if (string.IsNullOrEmpty(command.PhoneNumber))
                return Result.Failure("Phone number is required");
            
            // Build full name and address
            var fullName = $"{command.FirstName} {command.LastName}";
            var address = $"{command.CorrespondenceAddress.Street} {command.CorrespondenceAddress.HouseNumber}" +
                         (string.IsNullOrEmpty(command.CorrespondenceAddress.ApartmentNumber) ? "" : $"/{command.CorrespondenceAddress.ApartmentNumber}") +
                         $", {command.CorrespondenceAddress.PostalCode} {command.CorrespondenceAddress.City}, {command.CorrespondenceAddress.Province}";
            
            user.UpdateProfile(command.Email, fullName, command.PhoneNumber, address);

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update profile: {ex.Message}");
        }
    }
}
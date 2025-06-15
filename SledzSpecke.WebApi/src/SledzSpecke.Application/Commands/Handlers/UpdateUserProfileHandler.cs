using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

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

    public async Task<Result> HandleAsync(UpdateUserProfile command)
    {
        try
        {
            var currentUserId = _userContextService.GetUserId();
            var user = await _userRepository.GetByIdAsync(new UserId((int)currentUserId));
            
            if (user is null)
            {
                return Result.Failure($"User with ID {currentUserId} not found.");
            }

            // Validate and update basic profile
            Email email;
            FullName fullName;
            
            try
            {
                email = new Email(command.Email);
                fullName = new FullName(command.FullName);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Invalid profile data: {ex.Message}");
            }
            
            user.UpdateProfile(email, fullName);

            // Update additional details
            var phoneNumber = command.PhoneNumber != null ? new PhoneNumber(command.PhoneNumber) : null;
            var bio = command.Bio != null ? new UserBio(command.Bio) : null;
            user.UpdateProfileDetails(phoneNumber, command.DateOfBirth, bio);

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
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

            // Validate and create value objects
            Email email;
            FirstName firstName;
            LastName lastName;
            PhoneNumber phoneNumber;
            Address address;
            
            try
            {
                email = new Email(command.Email);
                firstName = new FirstName(command.FirstName);
                lastName = new LastName(command.LastName);
                phoneNumber = new PhoneNumber(command.PhoneNumber);
                address = new Address(
                    command.CorrespondenceAddress.Street,
                    command.CorrespondenceAddress.HouseNumber,
                    command.CorrespondenceAddress.ApartmentNumber,
                    command.CorrespondenceAddress.PostalCode,
                    command.CorrespondenceAddress.City,
                    command.CorrespondenceAddress.Province
                );
            }
            catch (Exception ex)
            {
                return Result.Failure($"Invalid profile data: {ex.Message}");
            }
            
            user.UpdateProfile(email, firstName, lastName, phoneNumber, address);

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
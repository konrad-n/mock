using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Constants;
using SledzSpecke.Application.Security;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.Specifications;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class SignUpHandler : IResultCommandHandler<SignUp>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordManager _passwordManager;
    private readonly IClock _clock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SignUpHandler> _logger;

    public SignUpHandler(
        IUserRepository userRepository, 
        IPasswordManager passwordManager, 
        IClock clock,
        IUnitOfWork unitOfWork,
        ILogger<SignUpHandler> logger)
    {
        _userRepository = userRepository;
        _passwordManager = passwordManager;
        _clock = clock;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(SignUp command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting sign up for email: {Email}, PESEL: {Pesel}", 
                command.Email, command.Pesel);
                
            var email = new Email(command.Email);
            var password = new Password(command.Password);
            var firstName = new FirstName(command.FirstName);
            var lastName = new LastName(command.LastName);
            var pesel = new Pesel(command.Pesel);
            var pwzNumber = new PwzNumber(command.PwzNumber);
            var phoneNumber = new PhoneNumber(command.PhoneNumber);
            var address = new Address(
                command.CorrespondenceAddress.Street,
                command.CorrespondenceAddress.HouseNumber,
                command.CorrespondenceAddress.ApartmentNumber,
                command.CorrespondenceAddress.PostalCode,
                command.CorrespondenceAddress.City,
                command.CorrespondenceAddress.Province
            );
            
            _logger.LogInformation("Value objects created successfully");

            if (await _userRepository.ExistsByEmailAsync(email))
            {
                _logger.LogWarning("Registration failed: Email {Email} already in use", command.Email);
                return Result.Failure($"Email '{command.Email}' is already in use.", ErrorCodes.EMAIL_ALREADY_IN_USE);
            }

            // Check if PESEL already exists
            var existingUserByPesel = await _userRepository.GetSingleBySpecificationAsync(new UserByPeselSpecification(pesel));
            if (existingUserByPesel != null)
            {
                _logger.LogWarning("Registration failed: PESEL already registered");
                return Result.Failure("User with this PESEL already exists.", ErrorCodes.PESEL_ALREADY_IN_USE);
            }

            // Check if PWZ already exists
            var existingUserByPwz = await _userRepository.GetSingleBySpecificationAsync(new UserByPwzNumberSpecification(pwzNumber));
            if (existingUserByPwz != null)
            {
                _logger.LogWarning("Registration failed: PWZ number already registered");
                return Result.Failure("User with this PWZ number already exists.", ErrorCodes.PWZ_ALREADY_IN_USE);
            }

            var securedPassword = _passwordManager.Secure(password);
            var hashedPassword = new HashedPassword(securedPassword);
            
            _logger.LogInformation("Creating user entity...");
            var user = User.Create(
                email, 
                hashedPassword, 
                firstName,
                null, // SecondName - optional
                lastName,
                pesel,
                pwzNumber,
                phoneNumber,
                command.DateOfBirth,
                address);
            
            _logger.LogInformation("User entity created with ID: {UserId}", user.Id?.Value ?? 0);

            await _userRepository.AddAsync(user);
            // SaveChangesAsync is already called in the repository's AddAsync method
            
            _logger.LogInformation("User saved successfully");
            
            return Result.Success();
        }
        catch (Exception ex) when (ex is CustomException)
        {
            _logger.LogError(ex, "Domain exception during sign up: {Message}", ex.Message);
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during sign up");
            return Result.Failure("An error occurred during sign up.");
        }
    }
}
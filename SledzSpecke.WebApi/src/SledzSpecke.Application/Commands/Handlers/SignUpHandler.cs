using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Constants;
using SledzSpecke.Application.Security;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.Specifications;
using SledzSpecke.Core.Enums;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class SignUpHandler : IResultCommandHandler<SignUp>
{
    private readonly IUserRepository _userRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IPasswordManager _passwordManager;
    private readonly IClock _clock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SignUpHandler> _logger;

    public SignUpHandler(
        IUserRepository userRepository,
        ISpecializationRepository specializationRepository,
        IPasswordManager passwordManager, 
        IClock clock,
        IUnitOfWork unitOfWork,
        ILogger<SignUpHandler> logger)
    {
        _userRepository = userRepository;
        _specializationRepository = specializationRepository;
        _passwordManager = passwordManager;
        _clock = clock;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> HandleAsync(SignUp command, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting sign up for email: {Email}", 
                command.Email);
                
            // Validate inputs
            if (string.IsNullOrEmpty(command.Email) || !command.Email.Contains("@"))
                return Result.Failure("Invalid email format.");
            if (string.IsNullOrEmpty(command.Password) || command.Password.Length < 8)
                return Result.Failure("Password must be at least 8 characters long.");
            if (string.IsNullOrEmpty(command.FirstName) || string.IsNullOrEmpty(command.LastName))
                return Result.Failure("First name and last name are required.");
            if (string.IsNullOrEmpty(command.PhoneNumber))
                return Result.Failure("Phone number is required.");
            
            _logger.LogInformation("Input validation passed");

            if (await _userRepository.ExistsByEmailAsync(command.Email))
            {
                _logger.LogWarning("Registration failed: Email {Email} already in use", command.Email);
                return Result.Failure($"Email '{command.Email}' is already in use.", ErrorCodes.EMAIL_ALREADY_IN_USE);
            }

            var securedPassword = _passwordManager.Secure(command.Password);
            
            // Build full name and address
            var fullName = $"{command.FirstName} {command.LastName}";
            var address = $"{command.CorrespondenceAddress.Street} {command.CorrespondenceAddress.HouseNumber}" +
                         (string.IsNullOrEmpty(command.CorrespondenceAddress.ApartmentNumber) ? "" : $"/{command.CorrespondenceAddress.ApartmentNumber}") +
                         $", {command.CorrespondenceAddress.PostalCode} {command.CorrespondenceAddress.City}, {command.CorrespondenceAddress.Province}";
            
            _logger.LogInformation("Creating user entity...");
            var user = User.Create(
                command.Email, 
                securedPassword, 
                fullName,
                command.PhoneNumber,
                command.DateOfBirth,
                address);
            
            _logger.LogInformation("User entity created with ID: {UserId}", user.UserId);

            await _userRepository.AddAsync(user);
            // SaveChangesAsync is already called in the repository's AddAsync method
            
            _logger.LogInformation("User saved successfully with ID: {UserId}", user.UserId);
            
            // Create user's specialization from template
            try
            {
                var smkVersion = (SmkVersion)Enum.Parse(typeof(SmkVersion), command.SmkVersion, true);
                var startDate = _clock.Current();
                
                // For now, use hardcoded values until we properly load from database
                // TODO: Load from SpecializationTemplates table
                var specializationName = command.SpecializationTemplateId == 1 ? "Kardiologia" : "Psychiatria";
                var programCode = command.SpecializationTemplateId == 1 ? "cardiology" : "psychiatry";
                var durationYears = 5; // Most specializations are 5 years
                
                // Calculate planned end date
                var plannedEndDate = startDate.AddYears(durationYears);
                
                var specialization = Specialization.Create(
                    user.UserId,
                    specializationName,
                    programCode,
                    smkVersion,
                    "standard", // program variant
                    startDate,
                    plannedEndDate,
                    1, // planned PES year
                    "{}", // program structure (empty JSON)
                    durationYears
                );
                
                await _specializationRepository.AddAsync(specialization);
                await _unitOfWork.SaveChangesAsync();
                
                _logger.LogInformation("Specialization created successfully for user {UserId}", user.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create specialization for user {UserId}", user.UserId);
                return Result.Failure("User created but failed to create specialization. Please contact support.");
            }
            
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
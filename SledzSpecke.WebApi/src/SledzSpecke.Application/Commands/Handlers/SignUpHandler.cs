using Microsoft.Extensions.Logging;
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Security;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Repositories;
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

    public async Task<Result> HandleAsync(SignUp command)
    {
        try
        {
            _logger.LogInformation("Starting sign up for email: {Email}, username: {Username}, specializationId: {SpecializationId}", 
                command.Email, command.Username, command.SpecializationId);
                
            var email = new Email(command.Email);
            var username = new Username(command.Username);
            var password = new Password(command.Password);
            var fullName = new FullName(command.FullName);
            var specializationId = new SpecializationId(command.SpecializationId);
            
            _logger.LogInformation("Value objects created successfully");

            if (await _userRepository.ExistsByEmailAsync(email))
            {
                return Result.Failure($"Email '{command.Email}' is already in use.");
            }

            if (await _userRepository.ExistsByUsernameAsync(username))
            {
                return Result.Failure($"Username '{command.Username}' is already in use.");
            }

            var securedPassword = _passwordManager.Secure(password);
            var smkVersion = new SmkVersion(command.SmkVersion);
            
            _logger.LogInformation("Creating user entity...");
            var user = new User(
                email, 
                username, 
                securedPassword, 
                fullName,
                smkVersion, 
                specializationId, 
                _clock.Current());
            
            _logger.LogInformation("User entity created with ID: {UserId}", user.Id?.Value ?? 0);

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
            
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
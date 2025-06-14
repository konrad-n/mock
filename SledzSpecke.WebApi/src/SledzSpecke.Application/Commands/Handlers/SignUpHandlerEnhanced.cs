using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Application.Security;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

/// <summary>
/// Enhanced version of SignUpHandler using Result pattern and Value Objects
/// </summary>
internal sealed class SignUpHandlerEnhanced : ICommandHandler<SignUp>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordManager _passwordManager;
    private readonly IClock _clock;
    private readonly IUnitOfWork _unitOfWork;

    public SignUpHandlerEnhanced(
        IUserRepository userRepository,
        IPasswordManager passwordManager,
        IClock clock,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordManager = passwordManager;
        _clock = clock;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(SignUp command)
    {
        // Step 1: Validate and create Value Objects
        var validationResult = ValidateCommand(command);
        if (validationResult.IsFailure)
        {
            throw new ValidationException(validationResult.Error);
        }

        var (email, username, password, fullName, smkVersion, specializationId) = validationResult.Value;

        // Step 2: Check for duplicate email
        var emailCheckResult = await CheckEmailAvailabilityAsync(email);
        if (emailCheckResult.IsFailure)
        {
            throw new EmailAlreadyInUseException(email);
        }

        // Step 3: Check for duplicate username
        var usernameCheckResult = await CheckUsernameAvailabilityAsync(username);
        if (usernameCheckResult.IsFailure)
        {
            throw new UsernameAlreadyInUseException(username);
        }

        // Step 4: Create user
        var createUserResult = CreateUser(email, username, password, fullName,
            smkVersion, specializationId);
        if (createUserResult.IsFailure)
        {
            throw new DomainException(createUserResult.Error);
        }

        var user = createUserResult.Value;

        // Step 5: Persist user
        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    private Result<(Email email, Username username, Password password, FullName fullName, SmkVersion smkVersion, SpecializationId specializationId)>
        ValidateCommand(SignUp command)
    {
        try
        {
            var email = new Email(command.Email);
            var username = new Username(command.Username);
            var password = new Password(command.Password);
            var fullName = new FullName(command.FullName);
            var smkVersion = new SmkVersion(command.SmkVersion);
            var specializationId = new SpecializationId(command.SpecializationId);

            return Result.Success((email, username, password, fullName, smkVersion, specializationId));
        }
        catch (Exception ex)
        {
            return Result.Failure<(Email, Username, Password, FullName, SmkVersion, SpecializationId)>(
                $"Validation failed: {ex.Message}");
        }
    }

    private async Task<Result> CheckEmailAvailabilityAsync(Email email)
    {
        if (await _userRepository.ExistsByEmailAsync(email))
        {
            return Result.Failure($"Email '{email}' is already in use.");
        }

        return Result.Success();
    }

    private async Task<Result> CheckUsernameAvailabilityAsync(Username username)
    {
        if (await _userRepository.ExistsByUsernameAsync(username))
        {
            return Result.Failure($"Username '{username}' is already taken.");
        }

        return Result.Success();
    }

    private Result<User> CreateUser(Email email, Username username, Password password,
        FullName fullName, SmkVersion smkVersion, SpecializationId specializationId)
    {
        try
        {
            var securedPassword = _passwordManager.Secure(password);
            var user = new User(email, username, securedPassword, fullName,
                smkVersion, specializationId, _clock.Current());

            return Result.Success(user);
        }
        catch (Exception ex)
        {
            return Result.Failure<User>($"Failed to create user: {ex.Message}");
        }
    }
}
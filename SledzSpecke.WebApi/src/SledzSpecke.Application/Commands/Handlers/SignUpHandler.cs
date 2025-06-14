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

    public SignUpHandler(
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

    public async Task<Result> HandleAsync(SignUp command)
    {
        try
        {
            var email = new Email(command.Email);
            var username = new Username(command.Username);
            var password = new Password(command.Password);
            var fullName = new FullName(command.FullName);
            var specializationId = new SpecializationId(command.SpecializationId);

            if (await _userRepository.ExistsByEmailAsync(email))
            {
                return Result.Failure($"Email '{command.Email}' is already in use.");
            }

            if (await _userRepository.ExistsByUsernameAsync(username))
            {
                return Result.Failure($"Username '{command.Username}' is already in use.");
            }

            var securedPassword = _passwordManager.Secure(password);
            var user = new User(
                email, 
                username, 
                securedPassword, 
                fullName,
                command.SmkVersion, 
                specializationId, 
                _clock.Current());

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
            
            return Result.Success();
        }
        catch (Exception ex) when (ex is CustomException)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception)
        {
            return Result.Failure("An error occurred during sign up.");
        }
    }
}
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Application.Security;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

internal sealed class SignUpHandler : ICommandHandler<SignUp>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordManager _passwordManager;
    private readonly IClock _clock;

    public SignUpHandler(IUserRepository userRepository, IPasswordManager passwordManager, IClock clock)
    {
        _userRepository = userRepository;
        _passwordManager = passwordManager;
        _clock = clock;
    }

    public async Task HandleAsync(SignUp command)
    {
        var email = new Email(command.Email);
        var username = new Username(command.Username);
        var password = new Password(command.Password);
        var fullName = new FullName(command.FullName);
        var specializationId = new SpecializationId(command.SpecializationId);

        if (await _userRepository.ExistsByEmailAsync(email))
        {
            throw new EmailAlreadyInUseException(email);
        }

        if (await _userRepository.ExistsByUsernameAsync(username))
        {
            throw new UsernameAlreadyInUseException(username);
        }

        var securedPassword = _passwordManager.Secure(password);
        var user = new User(email, username, securedPassword, fullName, 
            command.SmkVersion, specializationId, _clock.Current());

        await _userRepository.AddAsync(user);
    }
}
using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Exceptions;
using SledzSpecke.Application.Security;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

internal sealed class SignInHandler : ICommandHandler<SignIn, JwtDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordManager _passwordManager;
    private readonly IAuthenticator _authenticator;

    public SignInHandler(IUserRepository userRepository, IPasswordManager passwordManager, IAuthenticator authenticator)
    {
        _userRepository = userRepository;
        _passwordManager = passwordManager;
        _authenticator = authenticator;
    }

    public async Task<JwtDto> HandleAsync(SignIn command)
    {
        var username = new Username(command.Username);
        var user = await _userRepository.GetByUsernameAsync(username);

        if (user is null)
        {
            throw new InvalidCredentialsException();
        }

        if (!_passwordManager.Verify(command.Password, user.Password))
        {
            throw new InvalidCredentialsException();
        }

        var claims = new Dictionary<string, IEnumerable<string>>
        {
            ["permissions"] = new[] { "users" }
        };

        var jwt = _authenticator.CreateToken(user.Id, "user", claims);
        return jwt;
    }
}
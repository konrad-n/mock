using SledzSpecke.Application.Abstractions;
using SledzSpecke.Application.DTO;
using SledzSpecke.Application.Security;
using SledzSpecke.Core.Abstractions;
using SledzSpecke.Core.Entities;
using SledzSpecke.Core.Exceptions;
using SledzSpecke.Core.Repositories;
using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Commands.Handlers;

public sealed class SignInHandler : IResultCommandHandler<SignIn, JwtDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordManager _passwordManager;
    private readonly IAuthenticator _authenticator;

    public SignInHandler(
        IUserRepository userRepository, 
        IPasswordManager passwordManager, 
        IAuthenticator authenticator)
    {
        _userRepository = userRepository;
        _passwordManager = passwordManager;
        _authenticator = authenticator;
    }

    public async Task<Result<JwtDto>> HandleAsync(SignIn command)
    {
        try
        {
            // Check if the input looks like an email
            User? user = null;
            var loginIdentifier = command.Username?.Trim();
            
            if (string.IsNullOrWhiteSpace(loginIdentifier))
            {
                return Result.Failure<JwtDto>("Username or email is required.");
            }

            // Simple email pattern check
            if (loginIdentifier.Contains('@'))
            {
                // Try to parse as email
                try
                {
                    var email = new Email(loginIdentifier);
                    user = await _userRepository.GetByEmailAsync(email);
                }
                catch (InvalidEmailException)
                {
                    // If it's not a valid email, user will remain null
                }
            }
            else
            {
                // TODO: User-Specialization relationship needs to be redesigned
                // Username lookup is temporarily disabled
                // // Try to parse as username
                // try
                // {
                //     var username = new Username(loginIdentifier);
                //     user = await _userRepository.GetByUsernameAsync(username);
                // }
                // catch (InvalidUsernameException)
                // {
                //     // If it's not a valid username, user will remain null
                // }
            }

            if (user is null)
            {
                return Result.Failure<JwtDto>("Invalid username or password.");
            }

            if (!_passwordManager.Verify(command.Password, user.Password.Value))
            {
                return Result.Failure<JwtDto>("Invalid username or password.");
            }

            // Update last login time
            user.RecordLogin();
            await _userRepository.UpdateAsync(user);

            var claims = new Dictionary<string, IEnumerable<string>>
            {
                ["permissions"] = new[] { "users" }
            };

            var jwt = _authenticator.CreateToken(user.Id, "user", claims);
            return Result.Success(jwt);
        }
        catch (Exception ex) when (ex is CustomException)
        {
            return Result.Failure<JwtDto>(ex.Message);
        }
        catch (Exception)
        {
            return Result.Failure<JwtDto>("An error occurred during sign in.");
        }
    }
}
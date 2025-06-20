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

    public async Task<Result<JwtDto>> HandleAsync(SignIn command, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(command.Email))
            {
                return Result.Failure<JwtDto>("Email is required.");
            }

            // Try to parse as email
            Email email;
            try
            {
                email = new Email(command.Email.Trim());
            }
            catch (InvalidEmailException)
            {
                return Result.Failure<JwtDto>("Invalid email format.");
            }

            var user = await _userRepository.GetByEmailAsync(email);

            if (user is null)
            {
                return Result.Failure<JwtDto>("Invalid email or password.");
            }

            if (!_passwordManager.Verify(command.Password, user.Password.Value))
            {
                return Result.Failure<JwtDto>("Invalid email or password.");
            }

            // Update last login time
            user.RecordLogin();
            await _userRepository.UpdateAsync(user);

            // Determine role based on email (temporary solution)
            var role = user.Email.Value == "admin@sledzspecke.pl" ? "Admin" : "user";
            
            var claims = new Dictionary<string, IEnumerable<string>>
            {
                ["permissions"] = new[] { "users" }
            };

            var jwt = _authenticator.CreateToken(user.Id, role, claims);
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
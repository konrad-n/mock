using SledzSpecke.Core.Exceptions;

namespace SledzSpecke.Application.Exceptions;

public sealed class UserNotFoundException : CustomException
{
    public UserNotFoundException(int userId) : base($"User with ID {userId} was not found.")
    {
    }

    public UserNotFoundException(string username) : base($"User with username '{username}' was not found.")
    {
    }
}
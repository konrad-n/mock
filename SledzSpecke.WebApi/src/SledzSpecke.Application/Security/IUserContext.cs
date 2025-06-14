using SledzSpecke.Core.ValueObjects;

namespace SledzSpecke.Application.Security;

public interface IUserContext
{
    UserId? UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
}
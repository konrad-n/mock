using SledzSpecke.Application.DTO;

namespace SledzSpecke.Application.Security;

public interface IAuthenticator
{
    JwtDto CreateToken(int userId, string role, IDictionary<string, IEnumerable<string>>? claims = null);
}
using SledzSpecke.App.Models;

namespace SledzSpecke.App.Services.Authentication
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string username, string password);

        Task<bool> RegisterAsync(User user, string password, Models.Specialization specialization);

        Task LogoutAsync();

        Task<User> GetCurrentUserAsync();

        Task<bool> IsAuthenticatedAsync();
    }
}

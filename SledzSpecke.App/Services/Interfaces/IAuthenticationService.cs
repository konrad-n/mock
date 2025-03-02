using SledzSpecke.Core.Models;

namespace SledzSpecke.App.Services.Interfaces
{
    public interface IAuthenticationService
    {
        bool IsAuthenticated { get; }

        User CurrentUser { get; }

        Task<bool> RegisterAsync(string username, string email, string password, int specializationTypeId);

        Task<bool> LoginAsync(string email, string password);

        void Logout();

        Task<bool> ChangePasswordAsync(string currentPassword, string newPassword);

        Task<bool> SeedTestUserAsync();
    }
}
using SledzSpecke.Core.Models.Domain;
using System.Threading.Tasks;

namespace SledzSpecke.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<int> GetCurrentUserIdAsync();
        Task<User> GetCurrentUserAsync();
        Task<bool> UpdateUserAsync(User user);
        Task<bool> ExportUserDataAsync(string filePath = null);
        Task<bool> ImportUserDataAsync(string filePath);
        Task<bool> LogoutAsync();
        Task<bool> UpdateSpecializationAsync(Specialization specialization);
    }
}

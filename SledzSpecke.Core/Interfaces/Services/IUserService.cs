using SledzSpecke.Core.Models.Domain;
using System.Threading.Tasks;

namespace SledzSpecke.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<int> GetCurrentUserIdAsync();
        Task<User> GetCurrentUserAsync();
        Task<bool> ExportUserDataAsync();
        Task<bool> LogoutAsync();
        Task<bool> UpdateSpecializationAsync(Specialization specialization);
    }
}

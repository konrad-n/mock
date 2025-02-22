using System.Threading.Tasks;

namespace SledzSpecke.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<int> GetCurrentUserIdAsync();
        // Inne metody...
    }
}

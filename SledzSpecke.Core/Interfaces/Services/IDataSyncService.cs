using System.Threading.Tasks;

namespace SledzSpecke.Core.Interfaces.Services
{
    public interface IDataSyncService
    {
        Task<bool> SyncAllDataAsync();
        Task<bool> CreateBackupAsync();
        Task<bool> RestoreFromBackupAsync();
        Task<bool> ClearAllDataAsync();
    }
}

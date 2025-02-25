using System.Threading.Tasks;

namespace SledzSpecke.Core.Interfaces.Services
{
    public interface ISettingsService
    {
        T GetSetting<T>(string key, T defaultValue);
        void SetSetting<T>(string key, T value);
        Task<bool> ClearAllSettingsAsync();
    }
}
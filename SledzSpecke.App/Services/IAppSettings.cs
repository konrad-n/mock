using System.Threading.Tasks;

namespace SledzSpecke.App.Services
{
    public interface IAppSettings
    {
        Task LoadAsync();
        Task SaveAsync();
        T GetSetting<T>(string key, T defaultValue = default);
        void SetSetting<T>(string key, T value);
        void RemoveSetting(string key);
    }
}
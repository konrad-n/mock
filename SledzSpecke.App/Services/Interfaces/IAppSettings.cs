namespace SledzSpecke.App.Services.Interfaces
{
    public interface IAppSettings
    {
        Task LoadAsync();

        Task SaveAsync();

        T GetSetting<T>(string key, T defaultValue = default!);

        void SetSetting<T>(string key, T value);

        void RemoveSetting(string key);
    }
}
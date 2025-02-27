using CommunityToolkit.Mvvm.ComponentModel;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.Core.Interfaces.Services;

namespace SledzSpecke.App.ViewModels.Profile;

public partial class SettingsViewModel : BaseViewModel
{
    private readonly ISettingsService _settingsService;

    public SettingsViewModel(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        Title = "Ustawienia";
    }

    [ObservableProperty]
    private string versionInfo;

    public async Task LoadSettingsAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var version = AppInfo.VersionString;
            var build = AppInfo.BuildString;
            VersionInfo = $"Wersja {version} (Build {build})";

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Błąd",
                $"Nie udało się załadować ustawień: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
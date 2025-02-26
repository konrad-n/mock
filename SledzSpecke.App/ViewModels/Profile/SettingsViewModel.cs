using CommunityToolkit.Mvvm.ComponentModel;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.Core.Interfaces.Services;

namespace SledzSpecke.App.ViewModels.Profile;

public partial class SettingsViewModel : BaseViewModel
{
    private readonly ISettingsService _settingsService;

    public SettingsViewModel(
        ISettingsService settingsService)
    {
        _settingsService = settingsService;
        Title = "Ustawienia";
    }

    [ObservableProperty]
    private bool notifyCourses;

    [ObservableProperty]
    private bool notifyDuties;

    [ObservableProperty]
    private bool notifyProcedures;

    [ObservableProperty]
    private bool backgroundSync;

    [ObservableProperty]
    private bool wifiOnlySync;

    [ObservableProperty]
    private string versionInfo;

    partial void OnNotifyCoursesChanged(bool value)
    {
        _settingsService.SetSetting("NotifyCourses", value);
    }

    partial void OnNotifyDutiesChanged(bool value)
    {
        _settingsService.SetSetting("NotifyDuties", value);
    }

    partial void OnNotifyProceduresChanged(bool value)
    {
        _settingsService.SetSetting("NotifyProcedures", value);
    }

    partial void OnBackgroundSyncChanged(bool value)
    {
        _settingsService.SetSetting("BackgroundSync", value);
    }

    partial void OnWifiOnlySyncChanged(bool value)
    {
        _settingsService.SetSetting("WifiOnlySync", value);
    }

    public async Task LoadSettingsAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            NotifyCourses = _settingsService.GetSetting("NotifyCourses", true);
            NotifyDuties = _settingsService.GetSetting("NotifyDuties", true);
            NotifyProcedures = _settingsService.GetSetting("NotifyProcedures", true);

            BackgroundSync = _settingsService.GetSetting("BackgroundSync", true);
            WifiOnlySync = _settingsService.GetSetting("WifiOnlySync", true);

            var version = AppInfo.VersionString;
            var build = AppInfo.BuildString;
            VersionInfo = $"Wersja {version} (Build {build})";

            await Task.CompletedTask;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
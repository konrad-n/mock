using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Profile;

public partial class SettingsViewModel : BaseViewModel
{
    private readonly ISettingsService _settingsService;
    private readonly IDataSyncService _syncService;

    public SettingsViewModel(
        ISettingsService settingsService,
        IDataSyncService syncService)
    {
        _settingsService = settingsService;
        _syncService = syncService;
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

            // Załaduj ustawienia powiadomień
            NotifyCourses = _settingsService.GetSetting("NotifyCourses", true);
            NotifyDuties = _settingsService.GetSetting("NotifyDuties", true);
            NotifyProcedures = _settingsService.GetSetting("NotifyProcedures", true);

            // Załaduj ustawienia synchronizacji
            BackgroundSync = _settingsService.GetSetting("BackgroundSync", true);
            WifiOnlySync = _settingsService.GetSetting("WifiOnlySync", true);

            // Załaduj informacje o wersji
            var version = AppInfo.VersionString;
            var build = AppInfo.BuildString;
            VersionInfo = $"Wersja {version} (Build {build})";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SyncNowAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            await _syncService.SyncAllDataAsync();
            await Shell.Current.DisplayAlert(
                "Sukces",
                "Synchronizacja zakończona pomyślnie",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Błąd",
                "Nie udało się zsynchronizować danych",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task BackupAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            await _syncService.CreateBackupAsync();
            await Shell.Current.DisplayAlert(
                "Sukces",
                "Kopia zapasowa została utworzona",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Błąd",
                "Nie udało się utworzyć kopii zapasowej",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RestoreAsync()
    {
        var shouldRestore = await Shell.Current.DisplayAlert(
            "Potwierdzenie",
            "Przywrócenie kopii zapasowej nadpisze obecne dane. Czy chcesz kontynuować?",
            "Przywróć",
            "Anuluj");

        if (!shouldRestore) return;

        try
        {
            IsBusy = true;
            await _syncService.RestoreFromBackupAsync();
            await Shell.Current.DisplayAlert(
                "Sukces",
                "Dane zostały przywrócone z kopii zapasowej",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Błąd",
                "Nie udało się przywrócić danych",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ClearDataAsync()
    {
        var shouldClear = await Shell.Current.DisplayAlert(
            "Potwierdzenie",
            "Czy na pewno chcesz wyczyścić wszystkie dane? Tej operacji nie można cofnąć.",
            "Wyczyść",
            "Anuluj");

        if (!shouldClear) return;

        try
        {
            IsBusy = true;
            await _syncService.ClearAllDataAsync();
            await Shell.Current.GoToAsync("//login");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Błąd",
                "Nie udało się wyczyścić danych",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
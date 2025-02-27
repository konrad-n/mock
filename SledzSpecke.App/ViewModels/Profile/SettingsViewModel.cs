using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.Core.Interfaces.Services;

namespace SledzSpecke.App.ViewModels.Profile;

public partial class SettingsViewModel : BaseViewModel
{
    private readonly ISettingsService _settingsService;
    private readonly ISpecializationService _specializationService;
    private readonly IUserService _userService;

    public SettingsViewModel(
        ISettingsService settingsService,
        ISpecializationService specializationService,
        IUserService userService)
    {
        _settingsService = settingsService;
        _specializationService = specializationService;
        _userService = userService;
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

    [RelayCommand]
    private async Task UpdateSpecializationAsync()
    {
        var specOptions = new List<string> { "Hematologia", "Medycyna morska i tropikalna" };
        var selected = await Shell.Current.DisplayActionSheet(
            "Wybierz program specjalizacji",
            "Anuluj",
            null,
            specOptions.ToArray());

        if (selected != null && selected != "Anuluj")
        {
            var user = await _userService.GetCurrentUserAsync();
            if (user?.CurrentSpecializationId != null)
            {
                var specialization = await _specializationService.GetSpecializationAsync(user.CurrentSpecializationId.Value);
                if (specialization != null)
                {
                    var result = await _userService.UpdateSpecializationAsync(specialization);
                    if (result)
                    {
                        await Shell.Current.DisplayAlert("Sukces", "Specjalizacja przypisana do użytkownika", "OK");
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Błąd", "Nie udało się zaktualizować programu specjalizacji", "OK");
                    }
                }
            }
        }
    }

    [RelayCommand]
    private async Task ExportSpecializationAsync()
    {
        await Shell.Current.DisplayAlert("Eksport",
            "Funkcja eksportu danych specjalizacji zostanie zaimplementowana wkrótce.", "OK");
    }

    [RelayCommand]
    private async Task ImportSpecializationAsync()
    {
        await Shell.Current.DisplayAlert("Import",
            "Funkcja importu danych specjalizacji zostanie zaimplementowana wkrótce.", "OK");
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
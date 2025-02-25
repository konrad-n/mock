using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.Core.Interfaces.Services;

namespace SledzSpecke.App.ViewModels.Profile;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly IUserService _userService;
    private readonly ISpecializationService _specializationService;

    public ProfileViewModel(
        IUserService userService,
        ISpecializationService specializationService)
    {
        _userService = userService;
        _specializationService = specializationService;
        Title = "Profil";
    }

    [ObservableProperty]
    private string userName;

    [ObservableProperty]
    private string pwz;

    [ObservableProperty]
    private string specialization;

    [ObservableProperty]
    private double progress;

    [ObservableProperty]
    private string progressText;

    [ObservableProperty]
    private string timeLeft;

    public async Task LoadDataAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var user = await _userService.GetCurrentUserAsync();
            if (user != null)
            {
                UserName = user.Name;
                PWZ = $"PWZ: {user.PWZ}";

                var spec = await _specializationService.GetSpecializationAsync((int)user.CurrentSpecializationId);
                if (spec != null)
                {
                    Specialization = spec.Name;

                    // Oblicz postęp
                    var stats = await _specializationService.GetProgressStatisticsAsync((int)user.CurrentSpecializationId);
                    Progress = stats.TotalProgress;
                    ProgressText = $"{stats.TotalProgress:P0} ukończone";

                    // Oblicz pozostały czas
                    var daysLeft = (user.ExpectedEndDate - DateTime.Today).Days;
                    timeLeft = daysLeft switch
                    {
                        < 0 => "Specjalizacja zakończona",
                        0 => "Ostatni dzień specjalizacji",
                        1 => "Pozostał 1 dzień",
                        _ => $"Pozostało {daysLeft} dni"
                    };
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Błąd",
                "Nie udało się załadować danych profilu",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task EditProfileAsync()
    {
        await Shell.Current.GoToAsync("profile/edit");
    }

    [RelayCommand]
    private async Task OpenSettingsAsync()
    {
        await Shell.Current.GoToAsync("settings");
    }

    [RelayCommand]
    private async Task ExportDataAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            await _userService.ExportUserDataAsync();
            await Shell.Current.DisplayAlert(
                "Sukces",
                "Dane zostały wyeksportowane",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Błąd",
                "Nie udało się wyeksportować danych",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        var shouldLogout = await Shell.Current.DisplayAlert(
            "Potwierdzenie",
            "Czy na pewno chcesz się wylogować?",
            "Wyloguj",
            "Anuluj");

        if (shouldLogout)
        {
            await _userService.LogoutAsync();
            await Shell.Current.GoToAsync("//login");
        }
    }
}
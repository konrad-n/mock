using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.Core.Interfaces.Services;
using System.Threading.Tasks;

namespace SledzSpecke.App.ViewModels.Profile;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly IUserService _userService;
    private readonly ISpecializationService _specializationService;
    private readonly IFileSystemService _fileSystemService;

    public ProfileViewModel(
        IUserService userService,
        ISpecializationService specializationService,
        IFileSystemService fileSystemService)
    {
        _userService = userService;
        _specializationService = specializationService;
        _fileSystemService = fileSystemService;
        Title = "Profil";
    }

    [ObservableProperty]
    private string userName;

    [ObservableProperty]
    private string specialization;

    [ObservableProperty]
    private double progress;

    [ObservableProperty]
    private string progressText;

    [ObservableProperty]
    private string timeLeft;

    public override async Task LoadDataAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var user = await _userService.GetCurrentUserAsync();
            if (user != null)
            {
                UserName = user.Name;

                var spec = user.CurrentSpecializationId.HasValue
                    ? await _specializationService.GetSpecializationAsync((int)user.CurrentSpecializationId)
                    : null;

                if (spec != null)
                {
                    Specialization = spec.Name;

                    // Calculate progress
                    var stats = await _specializationService.GetProgressStatisticsAsync((int)user.CurrentSpecializationId);
                    Progress = stats.OverallProgress;
                    ProgressText = $"{stats.OverallProgress:P0} ukończone";

                    // Calculate remaining time
                    var daysLeft = (user.ExpectedEndDate - DateTime.Today).Days;
                    TimeLeft = daysLeft switch
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

            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string fileName = Path.Combine(documentsPath, $"SledzSpecke_Data_{DateTime.Now:yyyyMMdd}.json");

            bool result = await _userService.ExportUserDataAsync(fileName);

            if (result)
            {
                await Shell.Current.DisplayAlert(
                    "Sukces",
                    $"Dane zostały wyeksportowane do pliku:\n{fileName}",
                    "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert(
                    "Błąd",
                    "Nie udało się wyeksportować danych",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Błąd",
                $"Nie udało się wyeksportować danych: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ImportDataAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            // W prawdziwej aplikacji tutaj byłby picker plików lub inne rozwiązanie
            // Dla uproszczenia użyjemy domyślnej lokalizacji
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string[] files = Directory.GetFiles(documentsPath, "SledzSpecke_Data_*.json");

            if (files.Length == 0)
            {
                await Shell.Current.DisplayAlert(
                    "Informacja",
                    "Nie znaleziono plików z danymi do importu",
                    "OK");
                return;
            }

            // Użyj najnowszego pliku
            string newestFile = files.OrderByDescending(f => new FileInfo(f).CreationTime).First();

            bool result = await _userService.ImportUserDataAsync(newestFile);

            if (result)
            {
                await Shell.Current.DisplayAlert(
                    "Sukces",
                    $"Dane zostały zaimportowane z pliku:\n{newestFile}",
                    "OK");

                // Odśwież dane
                await LoadDataAsync();
            }
            else
            {
                await Shell.Current.DisplayAlert(
                    "Błąd",
                    "Nie udało się zaimportować danych",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Błąd",
                $"Nie udało się zaimportować danych: {ex.Message}",
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
            await Shell.Current.GoToAsync("//dashboard");
        }
    }
}
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
using System.Collections.ObjectModel;

namespace SledzSpecke.App.ViewModels.Profile;

public partial class ProfileEditViewModel : BaseViewModel
{
    private readonly IUserService _userService;
    private readonly ISpecializationService _specializationService;

    public ProfileEditViewModel(
        IUserService userService,
        ISpecializationService specializationService)
    {
        _userService = userService;
        _specializationService = specializationService;
        Title = "Edytuj profil";
        StartDate = DateTime.Now.AddYears(-1);
        AvailableSpecializations = new ObservableCollection<string>();
    }

    [ObservableProperty]
    private string userName;

    [ObservableProperty]
    private ObservableCollection<string> availableSpecializations;

    [ObservableProperty]
    private string selectedSpecialization;

    [ObservableProperty]
    private DateTime startDate;

    [ObservableProperty]
    private string endDateText;

    private Specialization _currentSpecialization;

    public override async Task LoadDataAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            // Pobierz dostępne specjalizacje
            var specializations = await _specializationService.GetAllSpecializationsAsync();
            AvailableSpecializations.Clear();
            foreach (var spec in specializations)
            {
                AvailableSpecializations.Add(spec.Name);
            }

            // Pobierz dane użytkownika
            var user = await _userService.GetCurrentUserAsync();
            if (user != null)
            {
                UserName = user.Name;

                if (user.SpecializationStartDate != default)
                    StartDate = user.SpecializationStartDate;

                if (user.CurrentSpecializationId.HasValue)
                {
                    _currentSpecialization = await _specializationService.GetSpecializationAsync(user.CurrentSpecializationId.Value);
                    if (_currentSpecialization != null)
                    {
                        SelectedSpecialization = _currentSpecialization.Name;
                        UpdateEndDate();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading profile data: {ex}");
            await Shell.Current.DisplayAlert("Błąd", "Nie udało się załadować danych profilu", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnStartDateChanged(DateTime value)
    {
        UpdateEndDate();
    }

    partial void OnSelectedSpecializationChanged(string value)
    {
        _ = UpdateSpecializationAsync();
    }

    private async Task UpdateSpecializationAsync()
    {
        if (string.IsNullOrEmpty(SelectedSpecialization))
            return;

        try
        {
            var specializations = await _specializationService.GetAllSpecializationsAsync();
            var selected = specializations.FirstOrDefault(s => s.Name == SelectedSpecialization);

            if (selected != null)
            {
                _currentSpecialization = selected;
                UpdateEndDate();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error updating specialization: {ex}");
        }
    }

    private void UpdateEndDate()
    {
        if (_currentSpecialization == null)
            return;

        // Oblicz datę zakończenia na podstawie daty rozpoczęcia i czasu trwania specjalizacji
        var endDate = StartDate.AddDays(_currentSpecialization.DurationInWeeks * 7);
        EndDateText = endDate.ToString("d");
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(UserName))
        {
            await Shell.Current.DisplayAlert("Błąd", "Imię i nazwisko jest wymagane", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(SelectedSpecialization))
        {
            await Shell.Current.DisplayAlert("Błąd", "Wybierz specjalizację", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            var user = await _userService.GetCurrentUserAsync();
            if (user != null)
            {
                user.Name = UserName;
                user.SpecializationStartDate = StartDate;

                // Aktualizuj specjalizację
                if (_currentSpecialization != null)
                {
                    user.CurrentSpecializationId = _currentSpecialization.Id;
                    user.ExpectedEndDate = StartDate.AddDays(_currentSpecialization.DurationInWeeks * 7);
                }

                await _userService.UpdateUserAsync(user);
                await Shell.Current.DisplayAlert("Sukces", "Dane profilu zostały zaktualizowane", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Błąd", $"Nie udało się zapisać zmian: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}

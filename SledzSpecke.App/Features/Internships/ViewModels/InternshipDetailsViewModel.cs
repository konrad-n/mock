using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Features.Internships.ViewModels
{
    public partial class InternshipDetailsViewModel : ViewModelBase
    {
        private Internship _internship;
        private ModuleType _currentModule;
        private Func<Internship, Task> _onSaveCallback;

        [ObservableProperty]
        private string _pageTitle;

        [ObservableProperty]
        private string _durationWeeks;

        [ObservableProperty]
        private string _workingDays;

        [ObservableProperty]
        private DateTime _startDate = DateTime.Now;

        [ObservableProperty]
        private DateTime _endDate = DateTime.Now.AddMonths(1);

        [ObservableProperty]
        private bool _isStartDateVisible;

        [ObservableProperty]
        private bool _isEndDateVisible;

        [ObservableProperty]
        private bool _isCompletionVisible;

        [ObservableProperty]
        private int _modulePickerSelectedIndex;

        [ObservableProperty]
        private int _statusPickerSelectedIndex;

        public Internship Internship => _internship;

        public InternshipDetailsViewModel(ILogger<InternshipDetailsViewModel> logger) : base(logger)
        {
            Title = "Szczegóły stażu";
        }

        public void Initialize(Internship internship, ModuleType currentModule, Func<Internship, Task> onSaveCallback)
        {
            _currentModule = currentModule;
            _onSaveCallback = onSaveCallback;

            if (internship == null)
            {
                // Nowy staż
                _internship = new Internship
                {
                    Id = new Random().Next(1000, 9999), // Tymczasowe ID
                    Module = currentModule,
                    IsRequired = true
                };
                PageTitle = "Dodaj staż";
                ModulePickerSelectedIndex = currentModule == ModuleType.Basic ? 0 : 1;
                StatusPickerSelectedIndex = 0; // Domyślnie "Oczekujący"
            }
            else
            {
                // Edycja istniejącego stażu
                _internship = internship;
                PageTitle = "Szczegóły stażu";
                DurationWeeks = internship.DurationWeeks.ToString();
                WorkingDays = internship.WorkingDays.ToString();

                // Ustawienie pickerów
                ModulePickerSelectedIndex = internship.Module == ModuleType.Basic ? 0 : 1;

                // Ustawienie statusu
                if (internship.IsCompleted)
                {
                    StatusPickerSelectedIndex = 3; // Ukończony
                    IsStartDateVisible = true;
                    IsEndDateVisible = true;
                    IsCompletionVisible = true;
                    if (internship.StartDate.HasValue)
                        StartDate = internship.StartDate.Value;
                    if (internship.EndDate.HasValue)
                        EndDate = internship.EndDate.Value;
                }
                else if (internship.StartDate.HasValue && !internship.EndDate.HasValue)
                {
                    StatusPickerSelectedIndex = 2; // W trakcie
                    IsStartDateVisible = true;
                    IsEndDateVisible = false;
                    IsCompletionVisible = false;
                    StartDate = internship.StartDate.Value;
                }
                else if (internship.StartDate.HasValue)
                {
                    StatusPickerSelectedIndex = 1; // Zaplanowany
                    IsStartDateVisible = true;
                    IsEndDateVisible = false;
                    IsCompletionVisible = false;
                    StartDate = internship.StartDate.Value;
                }
                else
                {
                    StatusPickerSelectedIndex = 0; // Oczekujący
                    IsStartDateVisible = false;
                    IsEndDateVisible = false;
                    IsCompletionVisible = false;
                }
            }
        }

        [RelayCommand]
        public void UpdateModuleType(int selectedIndex)
        {
            if (_internship != null)
            {
                _internship.Module = selectedIndex == 0 ? ModuleType.Basic : ModuleType.Specialistic;
            }
        }

        [RelayCommand]
        public void UpdateStatus(int selectedIndex)
        {
            if (_internship == null) return;

            switch (selectedIndex)
            {
                case 0: // Oczekujący
                    IsStartDateVisible = false;
                    IsEndDateVisible = false;
                    IsCompletionVisible = false;
                    _internship.StartDate = null;
                    _internship.EndDate = null;
                    _internship.IsCompleted = false;
                    break;
                case 1: // Zaplanowany
                    IsStartDateVisible = true;
                    IsEndDateVisible = false;
                    IsCompletionVisible = false;
                    _internship.StartDate = StartDate;
                    _internship.EndDate = null;
                    _internship.IsCompleted = false;
                    break;
                case 2: // W trakcie
                    IsStartDateVisible = true;
                    IsEndDateVisible = false;
                    IsCompletionVisible = false;
                    _internship.StartDate = StartDate;
                    _internship.EndDate = null;
                    _internship.IsCompleted = false;
                    break;
                case 3: // Ukończony
                    IsStartDateVisible = true;
                    IsEndDateVisible = true;
                    IsCompletionVisible = true;
                    _internship.StartDate = StartDate;
                    _internship.EndDate = EndDate;
                    _internship.IsCompleted = true;
                    break;
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.Navigation.PopAsync();
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            // Walidacja
            if (string.IsNullOrWhiteSpace(_internship.Name))
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nazwa stażu jest wymagana.", "OK");
                return;
            }

            if (!int.TryParse(DurationWeeks, out int durationWeeks) || durationWeeks <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Wprowadź poprawny czas trwania stażu (tygodnie).", "OK");
                return;
            }

            if (!int.TryParse(WorkingDays, out int workingDays) || workingDays <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Wprowadź poprawną liczbę dni roboczych.", "OK");
                return;
            }

            _internship.DurationWeeks = durationWeeks;
            _internship.WorkingDays = workingDays;

            // Dodatkowe ustawienia w zależności od statusu
            if (IsStartDateVisible && _internship.StartDate == null)
            {
                _internship.StartDate = StartDate;
            }

            if (IsEndDateVisible && _internship.EndDate == null)
            {
                _internship.EndDate = EndDate;
            }

            if (_onSaveCallback != null)
            {
                await _onSaveCallback(_internship);
            }

            await Shell.Current.Navigation.PopAsync();
        }
    }
}
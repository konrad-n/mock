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

        public Internship Internship => this._internship;

        public InternshipDetailsViewModel(ILogger<InternshipDetailsViewModel> logger) : base(logger)
        {
            this.Title = "Szczegóły stażu";
        }

        public void Initialize(Internship internship, ModuleType currentModule, Func<Internship, Task> onSaveCallback)
        {
            this._currentModule = currentModule;
            this._onSaveCallback = onSaveCallback;

            if (internship == null)
            {
                // Nowy staż
                this._internship = new Internship
                {
                    Id = new Random().Next(1000, 9999), // Tymczasowe ID
                    Module = currentModule,
                    IsRequired = true
                };
                this.PageTitle = "Dodaj staż";
                this.ModulePickerSelectedIndex = currentModule == ModuleType.Basic ? 0 : 1;
                this.StatusPickerSelectedIndex = 0; // Domyślnie "Oczekujący"
            }
            else
            {
                // Edycja istniejącego stażu
                this._internship = internship;
                this.PageTitle = "Szczegóły stażu";
                this.DurationWeeks = internship.DurationWeeks.ToString();
                this.WorkingDays = internship.WorkingDays.ToString();

                // Ustawienie pickerów
                this.ModulePickerSelectedIndex = internship.Module == ModuleType.Basic ? 0 : 1;

                // Ustawienie statusu
                if (internship.IsCompleted)
                {
                    this.StatusPickerSelectedIndex = 3; // Ukończony
                    this.IsStartDateVisible = true;
                    this.IsEndDateVisible = true;
                    this.IsCompletionVisible = true;
                    if (internship.StartDate.HasValue)
                        this.StartDate = internship.StartDate.Value;
                    if (internship.EndDate.HasValue)
                        this.EndDate = internship.EndDate.Value;
                }
                else if (internship.StartDate.HasValue && !internship.EndDate.HasValue)
                {
                    this.StatusPickerSelectedIndex = 2; // W trakcie
                    this.IsStartDateVisible = true;
                    this.IsEndDateVisible = false;
                    this.IsCompletionVisible = false;
                    this.StartDate = internship.StartDate.Value;
                }
                else if (internship.StartDate.HasValue)
                {
                    this.StatusPickerSelectedIndex = 1; // Zaplanowany
                    this.IsStartDateVisible = true;
                    this.IsEndDateVisible = false;
                    this.IsCompletionVisible = false;
                    this.StartDate = internship.StartDate.Value;
                }
                else
                {
                    this.StatusPickerSelectedIndex = 0; // Oczekujący
                    this.IsStartDateVisible = false;
                    this.IsEndDateVisible = false;
                    this.IsCompletionVisible = false;
                }
            }
        }

        [RelayCommand]
        public void UpdateModuleType(int selectedIndex)
        {
            if (this._internship != null)
            {
                this._internship.Module = selectedIndex == 0 ? ModuleType.Basic : ModuleType.Specialistic;
            }
        }

        [RelayCommand]
        public void UpdateStatus(int selectedIndex)
        {
            if (this._internship == null) return;

            switch (selectedIndex)
            {
                case 0: // Oczekujący
                    this.IsStartDateVisible = false;
                    this.IsEndDateVisible = false;
                    this.IsCompletionVisible = false;
                    this._internship.StartDate = null;
                    this._internship.EndDate = null;
                    this._internship.IsCompleted = false;
                    break;
                case 1: // Zaplanowany
                    this.IsStartDateVisible = true;
                    this.IsEndDateVisible = false;
                    this.IsCompletionVisible = false;
                    this._internship.StartDate = this.StartDate;
                    this._internship.EndDate = null;
                    this._internship.IsCompleted = false;
                    break;
                case 2: // W trakcie
                    this.IsStartDateVisible = true;
                    this.IsEndDateVisible = false;
                    this.IsCompletionVisible = false;
                    this._internship.StartDate = this.StartDate;
                    this._internship.EndDate = null;
                    this._internship.IsCompleted = false;
                    break;
                case 3: // Ukończony
                    this.IsStartDateVisible = true;
                    this.IsEndDateVisible = true;
                    this.IsCompletionVisible = true;
                    this._internship.StartDate = this.StartDate;
                    this._internship.EndDate = this.EndDate;
                    this._internship.IsCompleted = true;
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
            if (string.IsNullOrWhiteSpace(this._internship.Name))
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nazwa stażu jest wymagana.", "OK");
                return;
            }

            if (!int.TryParse(this.DurationWeeks, out int durationWeeks) || durationWeeks <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Wprowadź poprawny czas trwania stażu (tygodnie).", "OK");
                return;
            }

            if (!int.TryParse(this.WorkingDays, out int workingDays) || workingDays <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Wprowadź poprawną liczbę dni roboczych.", "OK");
                return;
            }

            this._internship.DurationWeeks = durationWeeks;
            this._internship.WorkingDays = workingDays;

            // Dodatkowe ustawienia w zależności od statusu
            if (this.IsStartDateVisible && this._internship.StartDate == null)
            {
                this._internship.StartDate = this.StartDate;
            }

            if (this.IsEndDateVisible && this._internship.EndDate == null)
            {
                this._internship.EndDate = this.EndDate;
            }

            if (this._onSaveCallback != null)
            {
                await this._onSaveCallback(this._internship);
            }

            await Shell.Current.Navigation.PopAsync();
        }
    }
}
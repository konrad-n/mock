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
        private Internship internship;
        private Func<Internship, Task> onSaveCallback;

        [ObservableProperty]
        private string pageTitle;

        [ObservableProperty]
        private string durationWeeks;

        [ObservableProperty]
        private string workingDays;

        [ObservableProperty]
        private DateTime startDate = DateTime.Now;

        [ObservableProperty]
        private DateTime endDate = DateTime.Now.AddMonths(1);

        [ObservableProperty]
        private bool isStartDateVisible;

        [ObservableProperty]
        private bool isEndDateVisible;

        [ObservableProperty]
        private bool isCompletionVisible;

        [ObservableProperty]
        private int modulePickerSelectedIndex;

        [ObservableProperty]
        private int statusPickerSelectedIndex;

        public InternshipDetailsViewModel(ILogger<InternshipDetailsViewModel> logger) : base(logger)
        {
            this.Title = "Szczegóły stażu";
        }

        public Internship Internship => this.internship;

        public void Initialize(Internship internship, ModuleType currentModule, Func<Internship, Task> onSaveCallback)
        {
            this.onSaveCallback = onSaveCallback;

            if (internship == null)
            {
                this.internship = new Internship
                {
                    Id = new Random().Next(1000, 9999),
                    Module = currentModule,
                    IsRequired = true,
                };
                this.PageTitle = "Dodaj staż";
                this.ModulePickerSelectedIndex = currentModule == ModuleType.Basic ? 0 : 1;
                this.StatusPickerSelectedIndex = 0;
            }
            else
            {
                this.internship = internship;
                this.PageTitle = "Szczegóły stażu";
                this.DurationWeeks = internship.DurationWeeks.ToString();
                this.WorkingDays = internship.WorkingDays.ToString();
                this.ModulePickerSelectedIndex = internship.Module == ModuleType.Basic ? 0 : 1;

                if (internship.IsCompleted)
                {
                    this.StatusPickerSelectedIndex = 3;
                    this.IsStartDateVisible = true;
                    this.IsEndDateVisible = true;
                    this.IsCompletionVisible = true;
                    if (internship.StartDate.HasValue)
                    {
                        this.StartDate = internship.StartDate.Value;
                    }

                    if (internship.EndDate.HasValue)
                    {
                        this.EndDate = internship.EndDate.Value;
                    }
                }
                else if (internship.StartDate.HasValue && !internship.EndDate.HasValue)
                {
                    this.StatusPickerSelectedIndex = 2;
                    this.IsStartDateVisible = true;
                    this.IsEndDateVisible = false;
                    this.IsCompletionVisible = false;
                    this.StartDate = internship.StartDate.Value;
                }
                else if (internship.StartDate.HasValue)
                {
                    this.StatusPickerSelectedIndex = 1;
                    this.IsStartDateVisible = true;
                    this.IsEndDateVisible = false;
                    this.IsCompletionVisible = false;
                    this.StartDate = internship.StartDate.Value;
                }
                else
                {
                    this.StatusPickerSelectedIndex = 0;
                    this.IsStartDateVisible = false;
                    this.IsEndDateVisible = false;
                    this.IsCompletionVisible = false;
                }
            }
        }

        [RelayCommand]
        public void UpdateModuleType(int selectedIndex)
        {
            if (this.internship != null)
            {
                this.internship.Module = selectedIndex == 0 ? ModuleType.Basic : ModuleType.Specialistic;
            }
        }

        [RelayCommand]
        public void UpdateStatus(int selectedIndex)
        {
            if (this.internship == null)
            {
                return;
            }

            switch (selectedIndex)
            {
                case 0: // Oczekujący
                    this.IsStartDateVisible = false;
                    this.IsEndDateVisible = false;
                    this.IsCompletionVisible = false;
                    this.internship.StartDate = null;
                    this.internship.EndDate = null;
                    this.internship.IsCompleted = false;
                    break;
                case 1: // Zaplanowany
                case 2: // W trakcie
                    this.IsStartDateVisible = true;
                    this.IsEndDateVisible = false;
                    this.IsCompletionVisible = false;
                    this.internship.StartDate = this.StartDate;
                    this.internship.EndDate = null;
                    this.internship.IsCompleted = false;
                    break;
                case 3: // Ukończony
                    this.IsStartDateVisible = true;
                    this.IsEndDateVisible = true;
                    this.IsCompletionVisible = true;
                    this.internship.StartDate = this.StartDate;
                    this.internship.EndDate = this.EndDate;
                    this.internship.IsCompleted = true;
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
            if (string.IsNullOrWhiteSpace(this.internship.Name))
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

            this.internship.DurationWeeks = durationWeeks;
            this.internship.WorkingDays = workingDays;

            if (this.IsStartDateVisible && this.internship.StartDate == null)
            {
                this.internship.StartDate = this.StartDate;
            }

            if (this.IsEndDateVisible && this.internship.EndDate == null)
            {
                this.internship.EndDate = this.EndDate;
            }

            if (this.onSaveCallback != null)
            {
                await this.onSaveCallback(this.internship);
            }

            await Shell.Current.Navigation.PopAsync();
        }
    }
}
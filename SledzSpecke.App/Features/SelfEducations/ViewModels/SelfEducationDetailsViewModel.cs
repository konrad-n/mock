using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SledzSpecke.App.Common.ViewModels;
using SledzSpecke.Core.Models;
using SledzSpecke.Core.Models.Enums;

namespace SledzSpecke.App.Features.SelfEducations.ViewModels
{
    public partial class SelfEducationDetailsViewModel : ViewModelBase
    {
        private SelfEducation _selfEducation;
        private Action<SelfEducation> _onSaveCallback;

        [ObservableProperty]
        private string _pageTitle;

        [ObservableProperty]
        private string _selfEducationTitle;

        [ObservableProperty]
        private DateTime _startDate = DateTime.Now;

        [ObservableProperty]
        private DateTime _endDate = DateTime.Now.AddDays(1);

        [ObservableProperty]
        private string _durationDays = "1";

        [ObservableProperty]
        private string _location;

        [ObservableProperty]
        private string _organizer;

        [ObservableProperty]
        private bool _isRequired;

        [ObservableProperty]
        private string _notes;

        [ObservableProperty]
        private int _typePickerSelectedIndex;

        public SelfEducation SelfEducation => this._selfEducation;

        public SelfEducationDetailsViewModel(ILogger<SelfEducationDetailsViewModel> logger) : base(logger)
        {
            this.Title = "Szczegóły wydarzenia";
        }

        public void Initialize(SelfEducation selfEducation, Action<SelfEducation> onSaveCallback)
        {
            this._onSaveCallback = onSaveCallback;

            if (selfEducation == null)
            {
                // Nowe wydarzenie
                this._selfEducation = new SelfEducation
                {
                    Id = new Random().Next(1000, 9999), // Tymczasowe ID
                    StartDate = this.StartDate,
                    EndDate = this.EndDate,
                    DurationDays = 1,
                    Type = SelfEducationType.Conference
                };
                this.PageTitle = "Dodaj wydarzenie";
                this.TypePickerSelectedIndex = 0; // Konferencja
            }
            else
            {
                // Edycja istniejącego wydarzenia
                this._selfEducation = selfEducation;
                this.PageTitle = "Edytuj wydarzenie";

                this.SelfEducationTitle = selfEducation.Title;
                this.StartDate = selfEducation.StartDate;
                this.EndDate = selfEducation.EndDate;
                this.DurationDays = selfEducation.DurationDays.ToString();
                this.Location = selfEducation.Location;
                this.Organizer = selfEducation.Organizer;
                this.IsRequired = selfEducation.IsRequired;
                this.Notes = selfEducation.Notes;

                this.TypePickerSelectedIndex = (int)selfEducation.Type;
            }

            this.UpdateDurationDays();
        }

        [RelayCommand]
        public void UpdateType(int selectedIndex)
        {
            if (this._selfEducation != null)
            {
                this._selfEducation.Type = (SelfEducationType)selectedIndex;
            }
        }

        [RelayCommand]
        public void UpdateDate()
        {
            this.UpdateDurationDays();
        }

        private void UpdateDurationDays()
        {
            if (this.EndDate >= this.StartDate)
            {
                TimeSpan duration = this.EndDate - this.StartDate;
                this.DurationDays = (duration.Days + 1).ToString();
            }
            else
            {
                this.DurationDays = "1";
            }
        }

        [RelayCommand]
        private async Task AddAttachmentAsync()
        {
            try
            {
                var fileResult = await FilePicker.PickAsync();
                if (fileResult != null)
                {
                    this._selfEducation.CertificateFilePath = fileResult.FullPath;
                    await Application.Current.MainPage.DisplayAlert("Sukces", "Plik został dodany pomyślnie.", "OK");
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error adding attachment");
                await Application.Current.MainPage.DisplayAlert("Błąd", $"Wystąpił problem z wyborem pliku: {ex.Message}", "OK");
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
            if (string.IsNullOrWhiteSpace(this.SelfEducationTitle))
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Tytuł wydarzenia jest wymagany.", "OK");
                return;
            }

            if (this.EndDate < this.StartDate)
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Data zakończenia musi być późniejsza lub równa dacie rozpoczęcia.", "OK");
                return;
            }

            if (!int.TryParse(this.DurationDays, out int durationDays) || durationDays <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Wprowadź poprawną liczbę dni.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(this.Location))
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Miejsce wydarzenia jest wymagane.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(this.Organizer))
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nazwa organizatora jest wymagana.", "OK");
                return;
            }

            // Aktualizacja wydarzenia
            this._selfEducation.Title = this.SelfEducationTitle;
            this._selfEducation.StartDate = this.StartDate;
            this._selfEducation.EndDate = this.EndDate;
            this._selfEducation.DurationDays = durationDays;
            this._selfEducation.Location = this.Location;
            this._selfEducation.Organizer = this.Organizer;
            this._selfEducation.IsRequired = this.IsRequired;
            this._selfEducation.Notes = this.Notes;

            this._onSaveCallback?.Invoke(this._selfEducation);
            await Shell.Current.Navigation.PopAsync();
        }
    }
}
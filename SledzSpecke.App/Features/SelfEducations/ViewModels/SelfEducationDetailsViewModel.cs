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

        public SelfEducation SelfEducation => _selfEducation;

        public SelfEducationDetailsViewModel(ILogger<SelfEducationDetailsViewModel> logger) : base(logger)
        {
            Title = "Szczegóły wydarzenia";
        }

        public void Initialize(SelfEducation selfEducation, Action<SelfEducation> onSaveCallback)
        {
            _onSaveCallback = onSaveCallback;

            if (selfEducation == null)
            {
                // Nowe wydarzenie
                _selfEducation = new SelfEducation
                {
                    Id = new Random().Next(1000, 9999), // Tymczasowe ID
                    StartDate = StartDate,
                    EndDate = EndDate,
                    DurationDays = 1,
                    Type = SelfEducationType.Conference
                };
                PageTitle = "Dodaj wydarzenie";
                TypePickerSelectedIndex = 0; // Konferencja
            }
            else
            {
                // Edycja istniejącego wydarzenia
                _selfEducation = selfEducation;
                PageTitle = "Edytuj wydarzenie";

                SelfEducationTitle = selfEducation.Title;
                StartDate = selfEducation.StartDate;
                EndDate = selfEducation.EndDate;
                DurationDays = selfEducation.DurationDays.ToString();
                Location = selfEducation.Location;
                Organizer = selfEducation.Organizer;
                IsRequired = selfEducation.IsRequired;
                Notes = selfEducation.Notes;

                TypePickerSelectedIndex = (int)selfEducation.Type;
            }
        }

        [RelayCommand]
        public void UpdateType(int selectedIndex)
        {
            if (_selfEducation != null)
            {
                _selfEducation.Type = (SelfEducationType)selectedIndex;
            }
        }

        [RelayCommand]
        public void UpdateDate()
        {
            UpdateDurationDays();
        }

        private void UpdateDurationDays()
        {
            if (EndDate >= StartDate)
            {
                TimeSpan duration = EndDate - StartDate;
                DurationDays = (duration.Days + 1).ToString();
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
                    _selfEducation.CertificateFilePath = fileResult.FullPath;
                    await Application.Current.MainPage.DisplayAlert("Sukces", "Plik został dodany pomyślnie.", "OK");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding attachment");
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
            if (string.IsNullOrWhiteSpace(SelfEducationTitle))
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Tytuł wydarzenia jest wymagany.", "OK");
                return;
            }

            if (EndDate < StartDate)
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Data zakończenia musi być późniejsza lub równa dacie rozpoczęcia.", "OK");
                return;
            }

            if (!int.TryParse(DurationDays, out int durationDays) || durationDays <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Wprowadź poprawną liczbę dni.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Location))
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Miejsce wydarzenia jest wymagane.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Organizer))
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Nazwa organizatora jest wymagana.", "OK");
                return;
            }

            // Aktualizacja wydarzenia
            _selfEducation.Title = SelfEducationTitle;
            _selfEducation.StartDate = StartDate;
            _selfEducation.EndDate = EndDate;
            _selfEducation.DurationDays = durationDays;
            _selfEducation.Location = Location;
            _selfEducation.Organizer = Organizer;
            _selfEducation.IsRequired = IsRequired;
            _selfEducation.Notes = Notes;

            _onSaveCallback?.Invoke(_selfEducation);
            await Shell.Current.Navigation.PopAsync();
        }
    }
}
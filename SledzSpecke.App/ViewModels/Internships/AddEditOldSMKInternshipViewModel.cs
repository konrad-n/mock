using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Internships;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Internships
{
    [QueryProperty(nameof(RealizedInternshipOldSMKId), nameof(RealizedInternshipOldSMKId))]
    [QueryProperty(nameof(InternshipName), nameof(InternshipName))]
    public class AddEditOldSMKInternshipViewModel : BaseViewModel
    {
        private readonly ISpecializationService specializationService;
        private readonly IInternshipService internshipService;
        private readonly IDialogService dialogService;

        private bool isEdit;
        private RealizedInternshipOldSMK realizedInternship;
        private string internshipName;
        private int specialistYear;

        public AddEditOldSMKInternshipViewModel(
            ISpecializationService specializationService,
            IInternshipService internshipService,
            IDialogService dialogService)
        {
            this.specializationService = specializationService;
            this.internshipService = internshipService;
            this.dialogService = dialogService;

            this.realizedInternship = new RealizedInternshipOldSMK
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(30),
                DaysCount = 30,
                Year = 1,
                SyncStatus = SyncStatus.NotSynced
            };

            this.SaveCommand = new AsyncRelayCommand(this.SaveAsync);
            this.CancelCommand = new AsyncRelayCommand(this.CancelAsync);
            this.RecalculateDaysCommand = new RelayCommand(this.RecalculateDays);

            this.Years = new List<int> { 1, 2, 3, 4, 5, 6 };
            this.specialistYear = 1;
        }

        public string RealizedInternshipOldSMKId
        {
            set
            {
                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int id) && id > 0)
                {
                    this.LoadRealizedInternshipAsync(id).ConfigureAwait(false);
                }
                else
                {
                    this.IsEdit = false;
                    this.Title = "Dodaj realizację stażu";
                }
            }
        }

        public string InternshipName
        {
            get => this.internshipName;
            set
            {
                this.internshipName = value;
                if (!string.IsNullOrEmpty(value) && !this.IsEdit)
                {
                    this.realizedInternship.InternshipName = value;
                }
                this.OnPropertyChanged();
            }
        }

        public bool IsEdit
        {
            get => this.isEdit;
            set => this.SetProperty(ref this.isEdit, value);
        }

        public RealizedInternshipOldSMK RealizedInternship
        {
            get => this.realizedInternship;
            set => this.SetProperty(ref this.realizedInternship, value);
        }

        public List<int> Years { get; }

        public int SpecialistYear
        {
            get => this.specialistYear;
            set
            {
                if (this.SetProperty(ref this.specialistYear, value))
                {
                    this.RealizedInternship.Year = value;
                }
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand RecalculateDaysCommand { get; }

        public async Task InitializeAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            // Pobieramy specjalizację, by sprawdzić, w którym roku jesteśmy
            var specialization = await this.specializationService.GetCurrentSpecializationAsync();
            if (specialization != null)
            {
                int yearsSinceStart = (DateTime.Today.Year - specialization.StartDate.Year);
                if (DateTime.Today.Month < specialization.StartDate.Month ||
                    (DateTime.Today.Month == specialization.StartDate.Month && DateTime.Today.Day < specialization.StartDate.Day))
                {
                    yearsSinceStart--;
                }

                this.SpecialistYear = Math.Max(1, Math.Min(yearsSinceStart + 1, 6));
            }

            this.IsBusy = false;
        }

        private async Task LoadRealizedInternshipAsync(int realizedInternshipId)
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            var loadedRealization = await this.internshipService.GetRealizedInternshipOldSMKAsync(realizedInternshipId);
            if (loadedRealization != null)
            {
                this.IsEdit = true;
                this.Title = "Edytuj realizację stażu";
                this.RealizedInternship = loadedRealization;
                this.InternshipName = loadedRealization.InternshipName;
                this.SpecialistYear = loadedRealization.Year;
            }
            else
            {
                this.IsEdit = false;
                this.Title = "Dodaj realizację stażu";
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie znaleziono realizacji stażu o podanym identyfikatorze.",
                    "OK");
            }

            this.IsBusy = false;
        }

        private void RecalculateDays()
        {
            TimeSpan duration = this.RealizedInternship.EndDate - this.RealizedInternship.StartDate;
            this.RealizedInternship.DaysCount = duration.Days + 1;
            this.OnPropertyChanged(nameof(this.RealizedInternship));
        }

        private async Task SaveAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            if (string.IsNullOrWhiteSpace(this.RealizedInternship.InstitutionName))
            {
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nazwa placówki realizującej szkolenie jest wymagana.",
                    "OK");
                this.IsBusy = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(this.RealizedInternship.DepartmentName))
            {
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nazwa komórki organizacyjnej jest wymagana.",
                    "OK");
                this.IsBusy = false;
                return;
            }

            if (this.RealizedInternship.DaysCount <= 0)
            {
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Liczba dni musi być większa od zera.",
                    "OK");
                this.IsBusy = false;
                return;
            }

            if (this.RealizedInternship.EndDate < this.RealizedInternship.StartDate)
            {
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Data zakończenia nie może być wcześniejsza niż data rozpoczęcia.",
                    "OK");
                this.IsBusy = false;
                return;
            }

            // Upewniamy się, że nazwa stażu jest ustawiona
            if (string.IsNullOrEmpty(this.RealizedInternship.InternshipName))
            {
                this.RealizedInternship.InternshipName = this.InternshipName;
            }

            // Obliczanie liczby dni między datami
            this.RecalculateDays();

            bool success = await this.internshipService.SaveRealizedInternshipOldSMKAsync(this.RealizedInternship);

            if (success)
            {
                await this.dialogService.DisplayAlertAsync(
                    "Sukces",
                    this.IsEdit ? "Realizacja stażu została zaktualizowana." : "Realizacja stażu została dodana.",
                    "OK");
                await Shell.Current.GoToAsync("/OldSMKInternships");
            }
            else
            {
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie udało się zapisać realizacji stażu. Sprawdź poprawność danych.",
                    "OK");
            }

            this.IsBusy = false;
        }

        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("/OldSMKInternships");
        }
    }
}
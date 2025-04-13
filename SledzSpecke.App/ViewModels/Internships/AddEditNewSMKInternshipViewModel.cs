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
    [QueryProperty(nameof(RealizedInternshipNewSMKId), nameof(RealizedInternshipNewSMKId))]
    [QueryProperty(nameof(InternshipRequirementId), nameof(InternshipRequirementId))]
    [QueryProperty(nameof(ModuleId), nameof(ModuleId))]
    public class AddEditNewSMKInternshipViewModel : BaseViewModel
    {
        private readonly ISpecializationService specializationService;
        private readonly IInternshipService internshipService;
        private readonly IDialogService dialogService;

        private bool isEdit;
        private RealizedInternshipNewSMK realizedInternship;
        private int internshipRequirementId;
        private int moduleId;
        private Internship requirement;

        public AddEditNewSMKInternshipViewModel(
            ISpecializationService specializationService,
            IInternshipService internshipService,
            IDialogService dialogService)
        {
            this.specializationService = specializationService;
            this.internshipService = internshipService;
            this.dialogService = dialogService;

            this.realizedInternship = new RealizedInternshipNewSMK
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(30),
                DaysCount = 30,
                SyncStatus = SyncStatus.NotSynced
            };

            this.SaveCommand = new AsyncRelayCommand(this.SaveAsync);
            this.CancelCommand = new AsyncRelayCommand(this.CancelAsync);
            this.RecalculateDaysCommand = new RelayCommand(this.RecalculateDays);
        }

        public string RealizedInternshipNewSMKId
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

        public string InternshipRequirementId
        {
            set
            {
                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int id) && id > 0)
                {
                    this.internshipRequirementId = id;
                    this.LoadInternshipRequirementAsync(this.internshipRequirementId).ConfigureAwait(false);
                }
            }
        }

        public string ModuleId
        {
            set
            {
                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int id) && id > 0)
                {
                    this.moduleId = id;
                    this.realizedInternship.ModuleId = id;
                }
            }
        }

        public bool IsEdit
        {
            get => this.isEdit;
            set => this.SetProperty(ref this.isEdit, value);
        }

        public RealizedInternshipNewSMK RealizedInternship
        {
            get => this.realizedInternship;
            set => this.SetProperty(ref this.realizedInternship, value);
        }

        public string InternshipName => this.requirement?.InternshipName ?? string.Empty;

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

            // Dodatkowy kod inicjalizujący, jeśli potrzebny

            this.IsBusy = false;
        }

        private async Task LoadRealizedInternshipAsync(int realizedInternshipId)
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            var loadedRealization = await this.internshipService.GetRealizedInternshipNewSMKAsync(realizedInternshipId);
            if (loadedRealization != null)
            {
                this.IsEdit = true;
                this.Title = "Edytuj realizację stażu";
                this.RealizedInternship = loadedRealization;
                this.internshipRequirementId = loadedRealization.InternshipRequirementId;
                this.LoadInternshipRequirementAsync(this.internshipRequirementId).ConfigureAwait(false);
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

        private async Task LoadInternshipRequirementAsync(int requirementId)
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            this.requirement = await this.specializationService.GetInternshipAsync(requirementId);

            if (this.requirement != null)
            {
                this.RealizedInternship.InternshipRequirementId = requirementId;
                this.RealizedInternship.InternshipName = this.requirement.InternshipName;

                if (!this.IsEdit)
                {
                    this.RealizedInternship.DaysCount = this.requirement.DaysCount;
                    this.RealizedInternship.EndDate = this.RealizedInternship.StartDate.AddDays(this.requirement.DaysCount - 1);
                }

                this.OnPropertyChanged(nameof(this.InternshipName));
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

            // Obliczanie liczby dni między datami
            this.RecalculateDays();

            bool success = await this.internshipService.SaveRealizedInternshipNewSMKAsync(this.RealizedInternship);

            if (success)
            {
                await this.dialogService.DisplayAlertAsync(
                    "Sukces",
                    this.IsEdit ? "Realizacja stażu została zaktualizowana." : "Realizacja stażu została dodana.",
                    "OK");
                await Shell.Current.GoToAsync("/NewSMKInternships");
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
            await Shell.Current.GoToAsync("/NewSMKInternships");
        }
    }
}
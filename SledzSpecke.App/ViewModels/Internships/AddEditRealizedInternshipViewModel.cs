using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Internships
{
    [QueryProperty(nameof(RealizedInternshipId), nameof(RealizedInternshipId))]
    [QueryProperty(nameof(InternshipName), nameof(InternshipName))]
    [QueryProperty(nameof(DaysCount), nameof(DaysCount))]
    [QueryProperty(nameof(ModuleId), nameof(ModuleId))]
    [QueryProperty(nameof(Year), nameof(Year))]
    public class AddEditRealizedInternshipViewModel : BaseViewModel
    {
        private readonly ISpecializationService specializationService;
        private readonly IDialogService dialogService;
        private readonly IAuthService authService;

        private bool isEdit;
        private bool isNewSMK;
        private RealizedInternshipNewSMK newSMKInternship;
        private RealizedInternshipOldSMK oldSMKInternship;
        private int internshipRequirementId;
        private string internshipName;
        private int daysCount;
        private int moduleId;
        private int year;

        public AddEditRealizedInternshipViewModel(
            ISpecializationService specializationService,
            IDialogService dialogService,
            IAuthService authService)
        {
            this.specializationService = specializationService;
            this.dialogService = dialogService;
            this.authService = authService;

            this.newSMKInternship = new RealizedInternshipNewSMK
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(30),
                DaysCount = 30,
                SyncStatus = SyncStatus.NotSynced
            };

            this.oldSMKInternship = new RealizedInternshipOldSMK
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(30),
                DaysCount = 30,
                Year = 1,
                SyncStatus = SyncStatus.NotSynced
            };

            this.SaveCommand = new AsyncRelayCommand(this.SaveAsync);
            this.CancelCommand = new AsyncRelayCommand(this.CancelAsync);

            // Sprawdzanie wersji SMK
            this.CheckSMKVersionAsync().ConfigureAwait(false);
        }

        public string RealizedInternshipId
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
            set
            {
                this.internshipName = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (this.IsNewSMK)
                    {
                        this.NewSMKInternship.InternshipName = value;
                    }
                    else
                    {
                        this.OldSMKInternship.InternshipName = value;
                    }
                }
            }
        }

        public string DaysCount
        {
            set
            {
                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int days))
                {
                    this.daysCount = days;
                    if (this.IsNewSMK)
                    {
                        this.NewSMKInternship.DaysCount = days;
                        this.NewSMKInternship.EndDate = this.NewSMKInternship.StartDate.AddDays(days - 1);
                    }
                    else
                    {
                        this.OldSMKInternship.DaysCount = days;
                        this.OldSMKInternship.EndDate = this.OldSMKInternship.StartDate.AddDays(days - 1);
                    }
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
                    this.newSMKInternship.ModuleId = id;
                }
            }
        }

        public string Year
        {
            set
            {
                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int yearVal) && yearVal > 0)
                {
                    this.year = yearVal;
                    this.oldSMKInternship.Year = yearVal;
                }
            }
        }

        public bool IsEdit
        {
            get => this.isEdit;
            set => this.SetProperty(ref this.isEdit, value);
        }

        public bool IsNewSMK
        {
            get => this.isNewSMK;
            set => this.SetProperty(ref this.isNewSMK, value);
        }

        public RealizedInternshipNewSMK NewSMKInternship
        {
            get => this.newSMKInternship;
            set => this.SetProperty(ref this.newSMKInternship, value);
        }

        public RealizedInternshipOldSMK OldSMKInternship
        {
            get => this.oldSMKInternship;
            set => this.SetProperty(ref this.oldSMKInternship, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private async Task CheckSMKVersionAsync()
        {
            var user = await this.authService.GetCurrentUserAsync();
            this.IsNewSMK = user?.SmkVersion == SmkVersion.New;
        }

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

            if (this.IsNewSMK)
            {
                var loadedInternship = await this.specializationService.GetRealizedInternshipNewSMKAsync(realizedInternshipId);
                if (loadedInternship != null)
                {
                    this.IsEdit = true;
                    this.Title = "Edytuj realizację stażu";
                    this.NewSMKInternship = loadedInternship;
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
            }
            else
            {
                var loadedInternship = await this.specializationService.GetRealizedInternshipOldSMKAsync(realizedInternshipId);
                if (loadedInternship != null)
                {
                    this.IsEdit = true;
                    this.Title = "Edytuj realizację stażu";
                    this.OldSMKInternship = loadedInternship;
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
            }

            this.IsBusy = false;
        }

        private async Task SaveAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            bool validationPassed;

            if (this.IsNewSMK)
            {
                validationPassed = await this.ValidateNewSMKInternshipAsync();
            }
            else
            {
                validationPassed = await this.ValidateOldSMKInternshipAsync();
            }

            if (!validationPassed)
            {
                this.IsBusy = false;
                return;
            }

            bool success;

            if (this.IsNewSMK)
            {
                // Obliczanie liczby dni między datami
                TimeSpan duration = this.NewSMKInternship.EndDate - this.NewSMKInternship.StartDate;
                this.NewSMKInternship.DaysCount = duration.Days + 1;

                if (this.IsEdit)
                {
                    success = await this.specializationService.UpdateRealizedInternshipNewSMKAsync(this.NewSMKInternship);
                }
                else
                {
                    this.NewSMKInternship.ModuleId = this.moduleId;
                    success = await this.specializationService.AddRealizedInternshipNewSMKAsync(this.NewSMKInternship);
                }
            }
            else
            {
                // Obliczanie liczby dni między datami
                TimeSpan duration = this.OldSMKInternship.EndDate - this.OldSMKInternship.StartDate;
                this.OldSMKInternship.DaysCount = duration.Days + 1;

                if (this.IsEdit)
                {
                    success = await this.specializationService.UpdateRealizedInternshipOldSMKAsync(this.OldSMKInternship);
                }
                else
                {
                    this.OldSMKInternship.Year = this.year;
                    success = await this.specializationService.AddRealizedInternshipOldSMKAsync(this.OldSMKInternship);
                }
            }

            if (success)
            {
                await this.dialogService.DisplayAlertAsync(
                    "Sukces",
                    this.IsEdit ? "Realizacja stażu została zaktualizowana." : "Realizacja stażu została dodana.",
                    "OK");

                if (this.IsNewSMK)
                {
                    await Shell.Current.GoToAsync("/NewSMKInternships");
                }
                else
                {
                    await Shell.Current.GoToAsync("/OldSMKInternships");
                }
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

        private async Task<bool> ValidateNewSMKInternshipAsync()
        {
            if (string.IsNullOrWhiteSpace(this.NewSMKInternship.InstitutionName))
            {
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nazwa placówki realizującej szkolenie jest wymagana.",
                    "OK");
                return false;
            }

            if (string.IsNullOrWhiteSpace(this.NewSMKInternship.DepartmentName))
            {
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nazwa komórki organizacyjnej jest wymagana.",
                    "OK");
                return false;
            }

            if (this.NewSMKInternship.DaysCount <= 0)
            {
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Liczba dni musi być większa od zera.",
                    "OK");
                return false;
            }

            if (this.NewSMKInternship.EndDate < this.NewSMKInternship.StartDate)
            {
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Data zakończenia nie może być wcześniejsza niż data rozpoczęcia.",
                    "OK");
                return false;
            }

            return true;
        }

        private async Task<bool> ValidateOldSMKInternshipAsync()
        {
            if (string.IsNullOrWhiteSpace(this.OldSMKInternship.InstitutionName))
            {
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nazwa placówki realizującej szkolenie jest wymagana.",
                    "OK");
                return false;
            }

            if (string.IsNullOrWhiteSpace(this.OldSMKInternship.DepartmentName))
            {
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nazwa komórki organizacyjnej jest wymagana.",
                    "OK");
                return false;
            }

            if (this.OldSMKInternship.DaysCount <= 0)
            {
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Liczba dni musi być większa od zera.",
                    "OK");
                return false;
            }

            if (this.OldSMKInternship.EndDate < this.OldSMKInternship.StartDate)
            {
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Data zakończenia nie może być wcześniejsza niż data rozpoczęcia.",
                    "OK");
                return false;
            }

            return true;
        }

        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
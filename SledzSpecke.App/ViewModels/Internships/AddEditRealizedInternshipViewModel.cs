using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
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
            IAuthService authService,
            IExceptionHandlerService exceptionHandler) : base(exceptionHandler)
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

            // Check SMK version
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
            await SafeExecuteAsync(async () =>
            {
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    throw new ResourceNotFoundException(
                        "User not found",
                        "Nie znaleziono zalogowanego użytkownika.");
                }
                this.IsNewSMK = user.SmkVersion == SmkVersion.New;
            }, "Wystąpił problem z identyfikacją wersji SMK.");
        }

        public async Task InitializeAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            await SafeExecuteAsync(async () =>
            {
                // Additional initialization code if needed
            }, "Nie udało się zainicjalizować ekranu.");

            this.IsBusy = false;
        }

        private async Task LoadRealizedInternshipAsync(int realizedInternshipId)
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            await SafeExecuteAsync(async () =>
            {
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
                        throw new ResourceNotFoundException(
                            $"Realized internship with ID {realizedInternshipId} not found",
                            "Nie znaleziono realizacji stażu o podanym identyfikatorze.");
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
                        throw new ResourceNotFoundException(
                            $"Realized internship with ID {realizedInternshipId} not found",
                            "Nie znaleziono realizacji stażu o podanym identyfikatorze.");
                    }
                }
            }, "Nie udało się załadować realizacji stażu.");

            this.IsBusy = false;
        }

        private async Task SaveAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                await SafeExecuteAsync(async () =>
                {
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
                        throw new InvalidInputException(
                            "Validation failed",
                            "Walidacja danych nie powiodła się.");
                    }

                    bool success;

                    if (this.IsNewSMK)
                    {
                        // Calculate days
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
                        // Calculate days
                        TimeSpan duration = this.OldSMKInternship.EndDate - this.OldSMKInternship.StartDate;
                        this.OldSMKInternship.DaysCount = duration.Days + 1;

                        // Make sure year is set correctly
                        if (this.OldSMKInternship.Year == 0)
                        {
                            var currentModule = await this.specializationService.GetCurrentModuleAsync();
                            if (currentModule != null)
                            {
                                this.OldSMKInternship.Year = currentModule.Type == ModuleType.Basic ? 1 : 3;
                            }
                            else
                            {
                                this.OldSMKInternship.Year = 1;
                            }
                        }

                        if (this.IsEdit)
                        {
                            success = await this.specializationService.UpdateRealizedInternshipOldSMKAsync(this.OldSMKInternship);
                        }
                        else
                        {
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
                        throw new DomainLogicException(
                            "Saving internship failed",
                            "Nie udało się zapisać realizacji stażu. Sprawdź poprawność danych.");
                    }
                }, "Wystąpił problem podczas zapisywania realizacji stażu.");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task<bool> ValidateNewSMKInternshipAsync()
        {
            if (string.IsNullOrWhiteSpace(this.NewSMKInternship.InstitutionName))
            {
                throw new InvalidInputException(
                    "Institution name is required",
                    "Nazwa placówki realizującej szkolenie jest wymagana.");
            }

            if (string.IsNullOrWhiteSpace(this.NewSMKInternship.DepartmentName))
            {
                throw new InvalidInputException(
                    "Department name is required",
                    "Nazwa komórki organizacyjnej jest wymagana.");
            }

            if (this.NewSMKInternship.DaysCount <= 0)
            {
                throw new InvalidInputException(
                    "Days count must be greater than zero",
                    "Liczba dni musi być większa od zera.");
            }

            if (this.NewSMKInternship.EndDate < this.NewSMKInternship.StartDate)
            {
                throw new InvalidInputException(
                    "End date cannot be earlier than start date",
                    "Data zakończenia nie może być wcześniejsza niż data rozpoczęcia.");
            }

            return true;
        }

        private async Task<bool> ValidateOldSMKInternshipAsync()
        {
            if (string.IsNullOrWhiteSpace(this.OldSMKInternship.InstitutionName))
            {
                throw new InvalidInputException(
                    "Institution name is required",
                    "Nazwa placówki realizującej szkolenie jest wymagana.");
            }

            if (string.IsNullOrWhiteSpace(this.OldSMKInternship.DepartmentName))
            {
                throw new InvalidInputException(
                    "Department name is required",
                    "Nazwa komórki organizacyjnej jest wymagana.");
            }

            if (this.OldSMKInternship.DaysCount <= 0)
            {
                throw new InvalidInputException(
                    "Days count must be greater than zero",
                    "Liczba dni musi być większa od zera.");
            }

            if (this.OldSMKInternship.EndDate < this.OldSMKInternship.StartDate)
            {
                throw new InvalidInputException(
                    "End date cannot be earlier than start date",
                    "Data zakończenia nie może być wcześniejsza niż data rozpoczęcia.");
            }

            return true;
        }

        private async Task CancelAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                await Shell.Current.GoToAsync("..");
            }, "Nie udało się wykonać operacji anulowania.");
        }
    }
}

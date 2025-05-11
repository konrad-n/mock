using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.MedicalShifts;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.MedicalShifts
{
    [QueryProperty(nameof(ShiftId), nameof(ShiftId))]
    [QueryProperty(nameof(YearParam), nameof(YearParam))]
    public class AddEditOldSMKMedicalShiftViewModel : BaseViewModel
    {
        private readonly IMedicalShiftsService medicalShiftsService;
        private readonly IAuthService authService;
        private readonly IDialogService dialogService;
        private readonly IDatabaseService databaseService;

        private bool isEdit;
        private RealizedMedicalShiftOldSMK shift;
        private int year;
        private string shiftId;
        private bool lastLocationLoaded = false;

        public AddEditOldSMKMedicalShiftViewModel(
            IMedicalShiftsService medicalShiftsService,
            IAuthService authService,
            IDialogService dialogService,
            IDatabaseService databaseService,
            IExceptionHandlerService exceptionHandler) : base(exceptionHandler)
        {
            this.medicalShiftsService = medicalShiftsService;
            this.authService = authService;
            this.dialogService = dialogService;
            this.databaseService = databaseService;
            this.shift = new RealizedMedicalShiftOldSMK
            {
                StartDate = DateTime.Today,
                Hours = 24,
                Minutes = 0,
                Year = 1,
            };
            this.SaveCommand = new AsyncRelayCommand(this.SaveAsync);
            this.CancelCommand = new AsyncRelayCommand(this.CancelAsync);
        }

        private string yearParam;
        public string ShiftId
        {
            set
            {
                this.shiftId = value;
                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int id) && id > 0)
                {
                    this.LoadShiftAsync(id).ConfigureAwait(false);
                }
                else
                {
                    this.IsEdit = false;
                    this.Title = "Dodaj dyżur";
                }
            }
        }

        public string YearParam
        {
            set
            {
                this.yearParam = value;

                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int yearValue))
                {
                    this.year = yearValue;
                    this.shift.Year = yearValue;

                    this.OnPropertyChanged(nameof(Shift));
                    this.OnPropertyChanged(nameof(Shift.Year));
                }
            }
        }

        public bool IsEdit
        {
            get => this.isEdit;
            set => this.SetProperty(ref this.isEdit, value);
        }

        public RealizedMedicalShiftOldSMK Shift
        {
            get => this.shift;
            set
            {
                if (this.SetProperty(ref this.shift, value))
                {
                    if (!this.IsEdit && !this.lastLocationLoaded)
                    {
                        this.LoadLastLocationAsync().ConfigureAwait(false);
                    }
                }
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private async Task LoadLastLocationAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                this.lastLocationLoaded = true;
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    throw new ResourceNotFoundException(
                        "User not found",
                        "Nie znaleziono aktywnego użytkownika.");
                }

                var query = "SELECT * FROM RealizedMedicalShiftOldSMK WHERE SpecializationId = ? ORDER BY ShiftId DESC LIMIT 1";
                var lastShifts = await this.databaseService.QueryAsync<RealizedMedicalShiftOldSMK>(query, user.SpecializationId);

                if (lastShifts.Count > 0)
                {
                    this.shift.SpecializationId = user.SpecializationId;

                    if (string.IsNullOrEmpty(this.shift.Location))
                    {
                        this.shift.Location = lastShifts[0].Location;
                        this.OnPropertyChanged(nameof(Shift));
                    }
                }
            }, "Nie udało się załadować ostatniej lokalizacji dyżuru.");
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
                bool isEditMode = !string.IsNullOrEmpty(this.shiftId) &&
                                  int.TryParse(this.shiftId, out int shiftIdValue) &&
                                  shiftIdValue > 0;
                this.IsEdit = isEditMode;
                this.Title = this.IsEdit ? "Edytuj dyżur" : "Dodaj dyżur";

                if (this.IsEdit)
                {
                    await this.LoadShiftAsync(int.Parse(this.shiftId));
                }
                else
                {
                    if (this.year > 0)
                    {
                        this.shift.Year = this.year;
                        this.OnPropertyChanged(nameof(Shift));
                        this.OnPropertyChanged(nameof(Shift.Year));
                    }

                    var user = await this.authService.GetCurrentUserAsync();
                    if (user != null)
                    {
                        this.shift.SpecializationId = user.SpecializationId;
                    }
                    else
                    {
                        throw new ResourceNotFoundException(
                            "User not found",
                            "Nie znaleziono aktywnego użytkownika.");
                    }

                    if (!this.lastLocationLoaded)
                    {
                        await this.LoadLastLocationAsync();
                    }
                }
            }, "Wystąpił problem podczas inicjalizacji formularza dyżuru.");

            this.IsBusy = false;
        }

        private async Task LoadShiftAsync(int shiftId)
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            await SafeExecuteAsync(async () =>
            {
                var loadedShift = await this.medicalShiftsService.GetOldSMKShiftAsync(shiftId);
                if (loadedShift != null)
                {
                    this.IsEdit = true;
                    this.Title = "Edytuj dyżur";
                    this.Shift = loadedShift;
                }
                else
                {
                    this.IsEdit = false;
                    this.Title = "Dodaj dyżur";
                    throw new ResourceNotFoundException(
                        $"Medical shift with ID {shiftId} not found",
                        "Nie znaleziono dyżuru o podanym identyfikatorze.");
                }
            }, "Wystąpił problem podczas ładowania danych dyżuru.");

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
                    if (this.shift.Hours <= 0 && this.shift.Minutes <= 0)
                    {
                        throw new InvalidInputException(
                            "Invalid shift duration",
                            "Czas dyżuru musi być większy od zera.");
                    }

                    if (string.IsNullOrWhiteSpace(this.shift.Location))
                    {
                        throw new InvalidInputException(
                            "Location is required",
                            "Nazwa komórki organizacyjnej jest wymagana.");
                    }

                    if (this.shift.Year <= 0)
                    {
                        this.shift.Year = this.year;
                    }

                    var user = await this.authService.GetCurrentUserAsync();
                    if (user != null)
                    {
                        this.shift.SpecializationId = user.SpecializationId;
                    }
                    else
                    {
                        throw new ResourceNotFoundException(
                            "User not found",
                            "Nie znaleziono aktywnego użytkownika.");
                    }

                    bool success = await this.medicalShiftsService.SaveOldSMKShiftAsync(this.shift);

                    if (success)
                    {
                        await this.dialogService.DisplayAlertAsync(
                            "Sukces",
                            this.IsEdit ? "Dyżur został zaktualizowany." : "Dyżur został dodany.",
                            "OK");
                        await Shell.Current.GoToAsync("..");
                    }
                    else
                    {
                        throw new DomainLogicException(
                            "Failed to save medical shift",
                            "Nie udało się zapisać dyżuru.");
                    }
                }, "Wystąpił problem podczas zapisywania dyżuru.");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task CancelAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                await Shell.Current.GoToAsync("..");
            }, "Wystąpił problem podczas anulowania edycji.");
        }
    }
}

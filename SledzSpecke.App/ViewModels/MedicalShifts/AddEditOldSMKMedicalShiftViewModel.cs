using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.MedicalShifts;
using SledzSpecke.App.Services.Specialization;
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

        private bool isEdit;
        private RealizedMedicalShiftOldSMK shift;
        private int year;
        private string shiftId;

        public AddEditOldSMKMedicalShiftViewModel(
            IMedicalShiftsService medicalShiftsService,
            IAuthService authService,
            IDialogService dialogService)
        {
            this.medicalShiftsService = medicalShiftsService;
            this.authService = authService;
            this.dialogService = dialogService;

            // Inicjalizacja pustego dyżuru
            this.shift = new RealizedMedicalShiftOldSMK
            {
                StartDate = DateTime.Today,
                Hours = 24,
                Minutes = 0,
                Year = 1 // Domyślnie rok 1
            };

            // Inicjalizacja komend
            this.SaveCommand = new AsyncRelayCommand(this.SaveAsync);
            this.CancelCommand = new AsyncRelayCommand(this.CancelAsync);

            System.Diagnostics.Debug.WriteLine("Konstruktor AddEditOldSMKMedicalShiftViewModel wywołany");
        }

        private string yearParam;

        // Właściwości QueryProperties
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
                System.Diagnostics.Debug.WriteLine($"YearParam ustawiony na: {value}");

                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int yearValue))
                {
                    this.year = yearValue;
                    this.shift.Year = yearValue;

                    this.OnPropertyChanged(nameof(Shift));
                    this.OnPropertyChanged(nameof(Shift.Year));

                    System.Diagnostics.Debug.WriteLine($"Rok został ustawiony na: {yearValue}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Nie udało się sparsować roku z: {value}");
                }
            }
        }

        // Pozostałe właściwości
        public bool IsEdit
        {
            get => this.isEdit;
            set => this.SetProperty(ref this.isEdit, value);
        }

        public RealizedMedicalShiftOldSMK Shift
        {
            get => this.shift;
            set => this.SetProperty(ref this.shift, value);
        }

        // Komendy
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        // Metody
        private async Task LoadShiftAsync(int shiftId)
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Pobierz dyżur
                var loadedShift = await this.medicalShiftsService.GetOldSMKShiftAsync(shiftId);
                if (loadedShift != null)
                {
                    this.Shift = loadedShift;
                    this.IsEdit = true;
                    this.Title = "Edytuj dyżur";
                }
                else
                {
                    this.IsEdit = false;
                    this.Title = "Dodaj dyżur";
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie znaleziono dyżuru o podanym identyfikatorze.",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania dyżuru: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem podczas ładowania dyżuru.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
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
                // Walidacja danych
                if (this.shift.Hours <= 0 && this.shift.Minutes <= 0)
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Czas dyżuru musi być większy od zera.",
                        "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(this.shift.Location))
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nazwa komórki organizacyjnej jest wymagana.",
                        "OK");
                    return;
                }

                // Upewnij się, że rok jest ustawiony (dla nowego dyżuru)
                if (this.shift.Year <= 0)
                {
                    this.shift.Year = this.year;
                }

                // Zapisz dyżur
                bool success = await this.medicalShiftsService.SaveOldSMKShiftAsync(this.shift);

                if (success)
                {
                    // Zamknij stronę
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie udało się zapisać dyżuru.",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas zapisywania dyżuru: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem podczas zapisywania dyżuru.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task CancelAsync()
        {
            // Powrót do poprzedniej strony
            await Shell.Current.GoToAsync("..");
        }
    }
}
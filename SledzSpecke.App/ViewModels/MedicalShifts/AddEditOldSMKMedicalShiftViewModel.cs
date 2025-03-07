using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
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
            IDatabaseService databaseService)
        {
            this.medicalShiftsService = medicalShiftsService;
            this.authService = authService;
            this.dialogService = dialogService;
            this.databaseService = databaseService;

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
            set
            {
                if (this.SetProperty(ref this.shift, value))
                {
                    // Jeśli to nowy dyżur i jeszcze nie pobieraliśmy lokacji, zróbmy to teraz
                    if (!this.IsEdit && !this.lastLocationLoaded)
                    {
                        this.LoadLastLocationAsync().ConfigureAwait(false);
                    }
                }
            }
        }

        // Komendy
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        // Metoda ładująca ostatnią lokację
        private async Task LoadLastLocationAsync()
        {
            try
            {
                // Oznacz, że już próbowaliśmy załadować lokację
                this.lastLocationLoaded = true;

                // Pobierz użytkownika
                var user = await this.authService.GetCurrentUserAsync();
                if (user == null)
                {
                    System.Diagnostics.Debug.WriteLine("LoadLastLocationAsync: Nie znaleziono użytkownika");
                    return;
                }

                // Pobierz ostatni dyżur (stary SMK)
                var query = "SELECT * FROM RealizedMedicalShiftOldSMK WHERE SpecializationId = ? ORDER BY ShiftId DESC LIMIT 1";
                var lastShifts = await this.databaseService.QueryAsync<RealizedMedicalShiftOldSMK>(query, user.SpecializationId);

                if (lastShifts.Count > 0)
                {
                    // Ustawienie ID specjalizacji
                    this.shift.SpecializationId = user.SpecializationId;

                    // Ustawienie lokacji z ostatniego dyżuru tylko gdy pole lokacji jest puste
                    if (string.IsNullOrEmpty(this.shift.Location))
                    {
                        this.shift.Location = lastShifts[0].Location;

                        // Powiadom o zmianie właściwości
                        this.OnPropertyChanged(nameof(Shift));

                        System.Diagnostics.Debug.WriteLine($"LoadLastLocationAsync: Uzupełniono lokację z ostatniego dyżuru: {this.shift.Location}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("LoadLastLocationAsync: Brak wcześniejszych dyżurów");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadLastLocationAsync: Błąd podczas pobierania ostatniego dyżuru: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        // Inicjalizacja ViewModelu
        public async Task InitializeAsync()
        {
            System.Diagnostics.Debug.WriteLine("InitializeAsync: Rozpoczęto inicjalizację");

            if (this.IsBusy)
            {
                System.Diagnostics.Debug.WriteLine("InitializeAsync: IsBusy=true, przerwanie inicjalizacji");
                return;
            }

            this.IsBusy = true;

            try
            {
                // Ustawienie tytułu strony
                this.Title = this.IsEdit ? "Edytuj dyżur" : "Dodaj dyżur";
                System.Diagnostics.Debug.WriteLine($"InitializeAsync: Ustawiono tytuł strony: {this.Title}");

                // Jeśli to nowy dyżur i ID specjalizacji nie zostało jeszcze ustawione
                if (!this.IsEdit && this.shift.SpecializationId <= 0)
                {
                    var user = await this.authService.GetCurrentUserAsync();
                    if (user != null)
                    {
                        this.shift.SpecializationId = user.SpecializationId;
                        System.Diagnostics.Debug.WriteLine($"InitializeAsync: Ustawiono ID specjalizacji: {this.shift.SpecializationId}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"InitializeAsync: Błąd: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    $"Wystąpił problem podczas inicjalizacji: {ex.Message}",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
                System.Diagnostics.Debug.WriteLine("InitializeAsync: Zakończono inicjalizację");
            }
        }

        // Metody
        private async Task LoadShiftAsync(int shiftId)
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;
            System.Diagnostics.Debug.WriteLine($"LoadShiftAsync: Ładowanie dyżuru o ID={shiftId}");

            try
            {
                // Pobierz dyżur
                var loadedShift = await this.medicalShiftsService.GetOldSMKShiftAsync(shiftId);
                if (loadedShift != null)
                {
                    this.IsEdit = true;
                    this.Title = "Edytuj dyżur";
                    this.Shift = loadedShift;
                    System.Diagnostics.Debug.WriteLine($"LoadShiftAsync: Załadowano dyżur: {loadedShift.Location}");
                }
                else
                {
                    this.IsEdit = false;
                    this.Title = "Dodaj dyżur";
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie znaleziono dyżuru o podanym identyfikatorze.",
                        "OK");
                    System.Diagnostics.Debug.WriteLine("LoadShiftAsync: Nie znaleziono dyżuru!");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadShiftAsync: Błąd: {ex.Message}");
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

                // Pobierz specjalizację
                var user = await this.authService.GetCurrentUserAsync();
                if (user != null)
                {
                    this.shift.SpecializationId = user.SpecializationId;
                }

                // Zapisz dyżur
                bool success = await this.medicalShiftsService.SaveOldSMKShiftAsync(this.shift);

                if (success)
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Sukces",
                        this.IsEdit ? "Dyżur został zaktualizowany." : "Dyżur został dodany.",
                        "OK");

                    // Wróć do poprzedniej strony
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
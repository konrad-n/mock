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
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;

namespace SledzSpecke.App.ViewModels.MedicalShifts
{
    public class OldSMKMedicalShiftsListViewModel : BaseViewModel
    {
        private readonly IMedicalShiftsService medicalShiftsService;
        private readonly IAuthService authService;
        private readonly IDialogService dialogService;
        private readonly ISpecializationService specializationService;

        private ObservableCollection<int> availableYears;
        private int selectedYear;
        private ObservableCollection<RealizedMedicalShiftOldSMK> shifts;
        private bool isRefreshing;
        private MedicalShiftsSummary summary;
        private string moduleTitle;

        public OldSMKMedicalShiftsListViewModel(
            IMedicalShiftsService medicalShiftsService,
            IAuthService authService,
            IDialogService dialogService,
            ISpecializationService specializationService)
        {
            this.medicalShiftsService = medicalShiftsService;
            this.authService = authService;
            this.dialogService = dialogService;
            this.specializationService = specializationService;

            this.Title = "Dyżury medyczne (Stary SMK)";
            this.AvailableYears = new ObservableCollection<int>();
            this.Shifts = new ObservableCollection<RealizedMedicalShiftOldSMK>();
            this.Summary = new MedicalShiftsSummary();

            // Inicjalizacja komend
            this.RefreshCommand = new AsyncRelayCommand(this.LoadDataAsync);
            this.SelectYearCommand = new AsyncRelayCommand<int>(this.SelectYearAsync);
            this.AddShiftCommand = new AsyncRelayCommand(this.AddShiftAsync);
            this.EditShiftCommand = new AsyncRelayCommand<RealizedMedicalShiftOldSMK>(this.EditShiftAsync);
            this.DeleteShiftCommand = new AsyncRelayCommand<RealizedMedicalShiftOldSMK>(this.DeleteShiftAsync);

            // Załaduj dostępne lata i dane
            this.LoadYearsAsync().ConfigureAwait(false);
        }

        public ObservableCollection<int> AvailableYears
        {
            get => this.availableYears;
            set => this.SetProperty(ref this.availableYears, value);
        }

        public int SelectedYear
        {
            get => this.selectedYear;
            set
            {
                if (this.SetProperty(ref this.selectedYear, value))
                {
                    System.Diagnostics.Debug.WriteLine($"SelectedYear zmieniony na: {value}, wywołuję LoadDataAsync");
                    // Bezpośrednio wywołaj LoadDataAsync przy zmianie roku
                    this.LoadDataAsyncForSelectedYear(value).ConfigureAwait(false);
                }
            }
        }

        private async Task LoadDataAsyncForSelectedYear(int year)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"LoadDataAsyncForSelectedYear: Rozpoczynam ładowanie danych dla roku {year}");
                await this.LoadDataAsync();
                System.Diagnostics.Debug.WriteLine($"LoadDataAsyncForSelectedYear: Zakończono ładowanie danych dla roku {year}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadDataAsyncForSelectedYear: Błąd: {ex.Message}");
            }
        }


        public ObservableCollection<RealizedMedicalShiftOldSMK> Shifts
        {
            get => this.shifts;
            set => this.SetProperty(ref this.shifts, value);
        }

        public bool IsRefreshing
        {
            get => this.isRefreshing;
            set => this.SetProperty(ref this.isRefreshing, value);
        }

        public MedicalShiftsSummary Summary
        {
            get => this.summary;
            set => this.SetProperty(ref this.summary, value);
        }

        public string ModuleTitle
        {
            get => this.moduleTitle;
            set => this.SetProperty(ref this.moduleTitle, value);
        }

        public ICommand RefreshCommand { get; }
        public ICommand SelectYearCommand { get; }
        public ICommand AddShiftCommand { get; }
        public ICommand EditShiftCommand { get; }
        public ICommand DeleteShiftCommand { get; }

        private async Task LoadYearsAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Pobierz tytuł modułu
                var module = await this.specializationService.GetCurrentModuleAsync();
                if (module != null)
                {
                    this.ModuleTitle = module.Name;
                }

                // Pobierz dostępne lata
                var years = await this.medicalShiftsService.GetAvailableYearsAsync();
                System.Diagnostics.Debug.WriteLine($"LoadYearsAsync: Pobrano {years.Count} lat");

                this.AvailableYears.Clear();
                foreach (var year in years)
                {
                    this.AvailableYears.Add(year);
                }

                // Upewnij się, że lista przycisków lat zostanie zaktualizowana
                this.OnPropertyChanged(nameof(AvailableYears));

                // KLUCZOWA ZMIANA: Zamiast polegać na LoadDataAsync, 
                // bezpośrednio pobieramy dyżury dla roku 1
                if (this.AvailableYears.Contains(1))
                {
                    System.Diagnostics.Debug.WriteLine("LoadYearsAsync: Bezpośrednio pobieram dyżury dla roku 1");

                    // 1. Ustawiamy rok
                    this.selectedYear = 1;
                    this.OnPropertyChanged(nameof(SelectedYear));

                    // 2. BEZPOŚREDNIO pobieramy dyżury z serwisu
                    try
                    {
                        var shifts = await this.medicalShiftsService.GetOldSMKShiftsAsync(1);
                        System.Diagnostics.Debug.WriteLine($"LoadYearsAsync: Bezpośrednio pobrano {shifts.Count} dyżurów dla roku 1");

                        // 3. Aktualizujemy UI
                        MainThread.BeginInvokeOnMainThread(() => {
                            this.Shifts.Clear();
                            foreach (var shift in shifts)
                            {
                                this.Shifts.Add(shift);
                            }

                            // Usuwamy wywołanie CreateYearButtons, bo ta metoda jest w widoku
                            // this.CreateYearButtons();
                        });

                        // 4. Pobieramy podsumowanie
                        this.Summary = await this.medicalShiftsService.GetShiftsSummaryAsync(year: 1);
                        this.OnPropertyChanged(nameof(Summary));
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"LoadYearsAsync: Błąd podczas bezpośredniego pobierania dyżurów: {ex.Message}");
                    }
                }
                else if (this.AvailableYears.Count > 0)
                {
                    // Podobna logika dla pierwszego roku z listy
                    int firstYear = this.AvailableYears[0];
                    this.selectedYear = firstYear;
                    this.OnPropertyChanged(nameof(SelectedYear));

                    try
                    {
                        var shifts = await this.medicalShiftsService.GetOldSMKShiftsAsync(firstYear);
                        System.Diagnostics.Debug.WriteLine($"LoadYearsAsync: Bezpośrednio pobrano {shifts.Count} dyżurów dla roku {firstYear}");

                        MainThread.BeginInvokeOnMainThread(() => {
                            this.Shifts.Clear();
                            foreach (var shift in shifts)
                            {
                                this.Shifts.Add(shift);
                            }

                            // Usuwamy również stąd wywołanie CreateYearButtons
                            // this.CreateYearButtons();
                        });

                        this.Summary = await this.medicalShiftsService.GetShiftsSummaryAsync(year: firstYear);
                        this.OnPropertyChanged(nameof(Summary));
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"LoadYearsAsync: Błąd podczas bezpośredniego pobierania dyżurów: {ex.Message}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania lat: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem podczas ładowania lat specjalizacji.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        public async Task LoadDataAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;
            this.IsRefreshing = true;

            try
            {
                // Pobierz dyżury dla wybranego roku
                var shifts = await this.medicalShiftsService.GetOldSMKShiftsAsync(this.SelectedYear);

                this.Shifts.Clear();
                foreach (var shift in shifts)
                {
                    this.Shifts.Add(shift);
                }

                // Pobierz podsumowanie dyżurów
                this.Summary = await this.medicalShiftsService.GetShiftsSummaryAsync(year: this.SelectedYear);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania dyżurów: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem podczas ładowania dyżurów.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
                this.IsRefreshing = false;
            }
        }

        private async Task SelectYearAsync(int year)
        {
            if (this.IsBusy)
            {
                return;
            }

            System.Diagnostics.Debug.WriteLine($"Wybrano rok: {year}");
            this.SelectedYear = year;
            await this.LoadDataAsync();
        }

        private async Task AddShiftAsync()
        {
            if (this.SelectedYear <= 0 && this.AvailableYears.Count > 0)
            {
                this.SelectedYear = this.AvailableYears[0];
            }

            System.Diagnostics.Debug.WriteLine($"Dodawanie dyżuru dla roku: {this.SelectedYear}");

            // Poprawiona nawigacja z parametrami
            var navigationParameter = new Dictionary<string, object>
            {
                { "ShiftId", "-1" },  // -1 oznacza nowy dyżur
                { "YearParam", this.SelectedYear.ToString() }
            };

            await Shell.Current.GoToAsync("AddEditOldSMKMedicalShift", navigationParameter);

            System.Diagnostics.Debug.WriteLine("Nawigacja do strony dodawania została wywołana");
        }

        private async Task EditShiftAsync(RealizedMedicalShiftOldSMK shift)
        {
            if (shift == null)
            {
                return;
            }

            try
            {
                // Dodane szczegółowe logowanie dla diagnostyki
                System.Diagnostics.Debug.WriteLine($"EditShiftAsync: Edycja dyżuru: ID={shift.ShiftId}, Rok={shift.Year}");

                // Poprawiona nawigacja z parametrami
                var navigationParameter = new Dictionary<string, object>
        {
            { "ShiftId", shift.ShiftId.ToString() },
            { "YearParam", shift.Year.ToString() }
        };

                // Bardziej szczegółowa ścieżka nawigacji
                await Shell.Current.GoToAsync("//medicalshifts/AddEditOldSMKMedicalShift", navigationParameter);

                System.Diagnostics.Debug.WriteLine("EditShiftAsync: Nawigacja do strony edycji została wywołana");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EditShiftAsync: Błąd podczas nawigacji: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"EditShiftAsync: Stack trace: {ex.StackTrace}");

                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    $"Wystąpił problem podczas przejścia do edycji dyżuru: {ex.Message}",
                    "OK");
            }
        }

        private async Task DeleteShiftAsync(RealizedMedicalShiftOldSMK shift)
        {
            if (shift == null)
            {
                return;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine($"DeleteShiftAsync: Usuwanie dyżuru: ID={shift.ShiftId}, Rok={shift.Year}");

                bool confirm = await this.dialogService.DisplayAlertAsync(
                    "Potwierdzenie",
                    "Czy na pewno chcesz usunąć ten dyżur?",
                    "Tak",
                    "Nie");

                if (confirm)
                {
                    System.Diagnostics.Debug.WriteLine("DeleteShiftAsync: Użytkownik potwierdził usunięcie");

                    bool success = await this.medicalShiftsService.DeleteOldSMKShiftAsync(shift.ShiftId);
                    if (success)
                    {
                        System.Diagnostics.Debug.WriteLine("DeleteShiftAsync: Pomyślnie usunięto dyżur");
                        this.Shifts.Remove(shift);

                        // Odśwież podsumowanie
                        this.Summary = await this.medicalShiftsService.GetShiftsSummaryAsync(year: this.SelectedYear);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("DeleteShiftAsync: Nie udało się usunąć dyżuru (usługa zwróciła false)");
                        await this.dialogService.DisplayAlertAsync(
                            "Błąd",
                            "Nie udało się usunąć dyżuru.",
                            "OK");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("DeleteShiftAsync: Użytkownik anulował usunięcie");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DeleteShiftAsync: Błąd podczas usuwania dyżuru: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"DeleteShiftAsync: Stack trace: {ex.StackTrace}");

                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    $"Wystąpił problem podczas usuwania dyżuru: {ex.Message}",
                    "OK");
            }
        }
    }
}
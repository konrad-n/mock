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
            set => this.SetProperty(ref this.selectedYear, value);
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

                this.AvailableYears.Clear();
                foreach (var year in years)
                {
                    this.AvailableYears.Add(year);
                }

                // Jeśli są dostępne lata, wybierz pierwszy
                if (this.AvailableYears.Count > 0)
                {
                    this.SelectedYear = this.AvailableYears[0];
                    await this.LoadDataAsync();
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

            // Przejdź do ekranu dodawania dyżuru
            var parameters = new Dictionary<string, object>
            {
                { "year", this.SelectedYear.ToString() }
            };

            System.Diagnostics.Debug.WriteLine($"Dodawanie dyżuru dla roku: {this.SelectedYear}");
            await Shell.Current.GoToAsync("AddEditOldSMKMedicalShift", parameters);
        }

        private async Task EditShiftAsync(RealizedMedicalShiftOldSMK shift)
        {
            if (shift == null)
            {
                return;
            }

            // Przejdź do ekranu edycji dyżuru
            var parameters = new Dictionary<string, object>
            {
                { "shiftId", shift.ShiftId }
            };

            await Shell.Current.GoToAsync("AddEditOldSMKMedicalShift", parameters);
        }

        private async Task DeleteShiftAsync(RealizedMedicalShiftOldSMK shift)
        {
            if (shift == null)
            {
                return;
            }

            bool confirm = await this.dialogService.DisplayAlertAsync(
                "Potwierdzenie",
                "Czy na pewno chcesz usunąć ten dyżur?",
                "Tak",
                "Nie");

            if (confirm)
            {
                try
                {
                    bool success = await this.medicalShiftsService.DeleteOldSMKShiftAsync(shift.ShiftId);
                    if (success)
                    {
                        this.Shifts.Remove(shift);

                        // Odśwież podsumowanie
                        this.Summary = await this.medicalShiftsService.GetShiftsSummaryAsync(year: this.SelectedYear);
                    }
                    else
                    {
                        await this.dialogService.DisplayAlertAsync(
                            "Błąd",
                            "Nie udało się usunąć dyżuru.",
                            "OK");
                    }
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Błąd podczas usuwania dyżuru: {ex.Message}");
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Wystąpił problem podczas usuwania dyżuru.",
                        "OK");
                }
            }
        }
    }
}
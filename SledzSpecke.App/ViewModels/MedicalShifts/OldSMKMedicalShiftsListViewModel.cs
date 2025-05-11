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
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.MedicalShifts;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;

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
            ISpecializationService specializationService,
            IExceptionHandlerService exceptionHandler) : base(exceptionHandler)
        {
            this.medicalShiftsService = medicalShiftsService;
            this.authService = authService;
            this.dialogService = dialogService;
            this.specializationService = specializationService;

            this.Title = "Dyżury medyczne (Stary SMK)";
            this.AvailableYears = new ObservableCollection<int>();
            this.Shifts = new ObservableCollection<RealizedMedicalShiftOldSMK>();
            this.Summary = new MedicalShiftsSummary();

            this.RefreshCommand = new AsyncRelayCommand(this.LoadDataAsync);
            this.SelectYearCommand = new AsyncRelayCommand<int>(this.SelectYearAsync);
            this.AddShiftCommand = new AsyncRelayCommand(this.AddShiftAsync);
            this.EditShiftCommand = new AsyncRelayCommand<RealizedMedicalShiftOldSMK>(this.EditShiftAsync);
            this.DeleteShiftCommand = new AsyncRelayCommand<RealizedMedicalShiftOldSMK>(this.DeleteShiftAsync);

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
                    this.LoadDataAsyncForSelectedYear(value).ConfigureAwait(false);
                }
            }
        }

        private async Task LoadDataAsyncForSelectedYear(int year)
        {
            await this.LoadDataAsync();
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

            await SafeExecuteAsync(async () =>
            {
                var module = await this.specializationService.GetCurrentModuleAsync();
                if (module != null)
                {
                    this.ModuleTitle = module.Name;
                }
                else
                {
                    throw new ResourceNotFoundException(
                        "Current module not found",
                        "Nie znaleziono aktualnego modułu.");
                }

                var years = await this.medicalShiftsService.GetAvailableYearsAsync();
                this.AvailableYears.Clear();

                foreach (var year in years)
                {
                    this.AvailableYears.Add(year);
                }

                this.OnPropertyChanged(nameof(AvailableYears));

                if (this.AvailableYears.Contains(1))
                {
                    this.selectedYear = 1;
                    this.OnPropertyChanged(nameof(SelectedYear));

                    var shifts = await this.medicalShiftsService.GetOldSMKShiftsAsync(1);

                    await MainThread.InvokeOnMainThreadAsync(() => {
                        this.Shifts.Clear();
                        foreach (var shift in shifts)
                        {
                            this.Shifts.Add(shift);
                        }
                    });

                    this.Summary = await this.medicalShiftsService.GetShiftsSummaryAsync(year: 1);
                    this.OnPropertyChanged(nameof(Summary));
                }
                else if (this.AvailableYears.Count > 0)
                {
                    int firstYear = this.AvailableYears[0];
                    this.selectedYear = firstYear;
                    this.OnPropertyChanged(nameof(SelectedYear));

                    var shifts = await this.medicalShiftsService.GetOldSMKShiftsAsync(firstYear);

                    await MainThread.InvokeOnMainThreadAsync(() => {
                        this.Shifts.Clear();
                        foreach (var shift in shifts)
                        {
                            this.Shifts.Add(shift);
                        }
                    });

                    this.Summary = await this.medicalShiftsService.GetShiftsSummaryAsync(year: firstYear);
                    this.OnPropertyChanged(nameof(Summary));
                }
                else
                {
                    // No years available, probably first time
                    this.AvailableYears.Add(1); // Add default year 1
                    this.selectedYear = 1;
                    this.OnPropertyChanged(nameof(SelectedYear));
                    this.Shifts.Clear();
                    this.Summary = new MedicalShiftsSummary();
                    this.OnPropertyChanged(nameof(Summary));
                }
            }, "Wystąpił problem podczas ładowania dostępnych lat specjalizacji.");

            this.IsBusy = false;
        }

        public async Task LoadDataAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;
            this.IsRefreshing = true;

            await SafeExecuteAsync(async () =>
            {
                var shifts = await this.medicalShiftsService.GetOldSMKShiftsAsync(this.SelectedYear);

                this.Shifts.Clear();
                foreach (var shift in shifts)
                {
                    this.Shifts.Add(shift);
                }

                this.Summary = await this.medicalShiftsService.GetShiftsSummaryAsync(year: this.SelectedYear);
            }, $"Wystąpił problem podczas ładowania dyżurów dla roku {this.SelectedYear}.");

            this.IsBusy = false;
            this.IsRefreshing = false;
        }

        private async Task SelectYearAsync(int year)
        {
            if (this.IsBusy)
            {
                return;
            }

            await SafeExecuteAsync(async () =>
            {
                this.SelectedYear = year;
                await this.LoadDataAsync();
            }, $"Wystąpił problem podczas wybierania roku {year}.");
        }

        private async Task AddShiftAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                if (this.SelectedYear <= 0 && this.AvailableYears.Count > 0)
                {
                    this.SelectedYear = this.AvailableYears[0];
                }

                var navigationParameter = new Dictionary<string, object>
                {
                    { "ShiftId", "-1" },
                    { "YearParam", this.SelectedYear.ToString() }
                };

                await Shell.Current.GoToAsync("AddEditOldSMKMedicalShift", navigationParameter);
            }, "Wystąpił problem podczas nawigacji do formularza dodawania dyżuru.");
        }

        private async Task EditShiftAsync(RealizedMedicalShiftOldSMK shift)
        {
            if (shift == null)
            {
                return;
            }

            await SafeExecuteAsync(async () =>
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "ShiftId", shift.ShiftId.ToString() },
                    { "YearParam", shift.Year.ToString() }
                };

                await Shell.Current.GoToAsync("//medicalshifts/AddEditOldSMKMedicalShift", navigationParameter);
            }, "Wystąpił problem podczas nawigacji do formularza edycji dyżuru.");
        }

        private async Task DeleteShiftAsync(RealizedMedicalShiftOldSMK shift)
        {
            if (shift == null)
            {
                return;
            }

            await SafeExecuteAsync(async () =>
            {
                bool confirm = await this.dialogService.DisplayAlertAsync(
                    "Potwierdzenie",
                    "Czy na pewno chcesz usunąć ten dyżur?",
                    "Tak",
                    "Nie");

                if (confirm)
                {
                    bool success = await this.medicalShiftsService.DeleteOldSMKShiftAsync(shift.ShiftId);
                    if (success)
                    {
                        this.Shifts.Remove(shift);
                        this.Summary = await this.medicalShiftsService.GetShiftsSummaryAsync(year: this.SelectedYear);
                    }
                    else
                    {
                        throw new DomainLogicException(
                            "Failed to delete medical shift",
                            "Nie udało się usunąć dyżuru.");
                    }
                }
            }, "Wystąpił problem podczas usuwania dyżuru.");
        }
    }
}

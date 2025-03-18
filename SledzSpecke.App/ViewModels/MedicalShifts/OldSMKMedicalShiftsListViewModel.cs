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

            var module = await this.specializationService.GetCurrentModuleAsync();
            if (module != null)
            {
                this.ModuleTitle = module.Name;
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
                MainThread.BeginInvokeOnMainThread(() => {
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

                MainThread.BeginInvokeOnMainThread(() => {
                    this.Shifts.Clear();
                    foreach (var shift in shifts)
                    {
                        this.Shifts.Add(shift);
                    }
                });

                this.Summary = await this.medicalShiftsService.GetShiftsSummaryAsync(year: firstYear);
                this.OnPropertyChanged(nameof(Summary));
            }

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

            var shifts = await this.medicalShiftsService.GetOldSMKShiftsAsync(this.SelectedYear);

            this.Shifts.Clear();
            foreach (var shift in shifts)
            {
                this.Shifts.Add(shift);
            }

            this.Summary = await this.medicalShiftsService.GetShiftsSummaryAsync(year: this.SelectedYear);

            this.IsBusy = false;
            this.IsRefreshing = false;
        }

        private async Task SelectYearAsync(int year)
        {
            if (this.IsBusy)
            {
                return;
            }

            this.SelectedYear = year;
            await this.LoadDataAsync();
        }

        private async Task AddShiftAsync()
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
        }

        private async Task EditShiftAsync(RealizedMedicalShiftOldSMK shift)
        {
            if (shift == null)
            {
                return;
            }

            var navigationParameter = new Dictionary<string, object>
            {
                { "ShiftId", shift.ShiftId.ToString() },
                { "YearParam", shift.Year.ToString() }
            };

            await Shell.Current.GoToAsync("//medicalshifts/AddEditOldSMKMedicalShift", navigationParameter);
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
                bool success = await this.medicalShiftsService.DeleteOldSMKShiftAsync(shift.ShiftId);
                if (success)
                {
                    this.Shifts.Remove(shift);
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
        }
    }
}
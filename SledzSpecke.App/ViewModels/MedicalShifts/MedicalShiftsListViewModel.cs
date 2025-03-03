using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.MedicalShifts
{
    public partial class MedicalShiftsListViewModel : BaseViewModel
    {
        private readonly ISpecializationService specializationService;
        private readonly IDatabaseService databaseService;

        // Filter state
        private bool allShiftsSelected = true;
        private bool currentInternshipSelected = false;
        private string searchText = string.Empty;
        private MedicalShift selectedShift;

        // Data
        private ObservableCollection<MedicalShift> shifts;
        private int currentInternshipId;

        public MedicalShiftsListViewModel(
            ISpecializationService specializationService,
            IDatabaseService databaseService)
        {
            this.specializationService = specializationService;
            this.databaseService = databaseService;

            // Initialize commands
            this.RefreshCommand = new AsyncRelayCommand(this.LoadShiftsAsync);
            this.FilterShiftsCommand = new AsyncRelayCommand<string>(this.OnFilterShiftsAsync);
            this.FilterCommand = new AsyncRelayCommand(this.ApplyFiltersAsync);
            this.ShiftSelectedCommand = new AsyncRelayCommand(this.OnShiftSelectedAsync);
            this.AddShiftCommand = new AsyncRelayCommand(this.OnAddShiftAsync);

            // Initialize properties
            this.Title = "Dyżury medyczne";
            this.Shifts = new ObservableCollection<MedicalShift>();

            // Load data
            this.LoadShiftsAsync();
        }

        // Properties
        public ObservableCollection<MedicalShift> Shifts
        {
            get => this.shifts;
            set => this.SetProperty(ref this.shifts, value);
        }

        public MedicalShift SelectedShift
        {
            get => this.selectedShift;
            set => this.SetProperty(ref this.selectedShift, value);
        }

        public bool AllShiftsSelected
        {
            get => this.allShiftsSelected;
            set => this.SetProperty(ref this.allShiftsSelected, value);
        }

        public bool CurrentInternshipSelected
        {
            get => this.currentInternshipSelected;
            set => this.SetProperty(ref this.currentInternshipSelected, value);
        }

        public string SearchText
        {
            get => this.searchText;
            set => this.SetProperty(ref this.searchText, value);
        }

        // Commands
        public ICommand RefreshCommand { get; }

        public ICommand FilterShiftsCommand { get; }

        public ICommand FilterCommand { get; }

        public ICommand ShiftSelectedCommand { get; }

        public ICommand AddShiftCommand { get; }

        // Methods
        private async Task LoadShiftsAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Get current internship if filtering is active
                if (this.CurrentInternshipSelected)
                {
                    var currentInternship = await this.specializationService.GetCurrentInternshipAsync();
                    this.currentInternshipId = currentInternship?.InternshipId ?? 0;
                }
                else
                {
                    this.currentInternshipId = 0;
                }

                // Clear existing shifts
                this.Shifts.Clear();

                // Load shifts from database
                var shifts = this.currentInternshipId > 0
                    ? await this.databaseService.GetMedicalShiftsAsync(this.currentInternshipId)
                    : await this.databaseService.GetMedicalShiftsAsync();

                // Apply search filter if needed
                if (!string.IsNullOrEmpty(this.SearchText))
                {
                    var searchLower = this.SearchText.ToLowerInvariant();
                    shifts = shifts.Where(s =>
                        s.Location.ToLowerInvariant().Contains(searchLower) ||
                        s.Date.ToString("d").Contains(searchLower)
                    ).ToList();
                }

                // Add shifts to collection
                foreach (var shift in shifts.OrderByDescending(s => s.Date))
                {
                    this.Shifts.Add(shift);
                }
            }
            catch (Exception ex)
            {
                // Handle error
                System.Diagnostics.Debug.WriteLine($"Error loading shifts: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert(
                    "Błąd",
                    "Nie udało się załadować dyżurów. Spróbuj ponownie.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task OnFilterShiftsAsync(string filter)
        {
            if (filter == "All")
            {
                this.AllShiftsSelected = true;
                this.CurrentInternshipSelected = false;
            }
            else if (filter == "Current")
            {
                this.AllShiftsSelected = false;
                this.CurrentInternshipSelected = true;
            }

            await this.LoadShiftsAsync();
        }

        private async Task ApplyFiltersAsync()
        {
            await this.LoadShiftsAsync();
        }

        private async Task OnShiftSelectedAsync()
        {
            if (this.SelectedShift == null)
            {
                return;
            }

            // Navigate to shift details page
            await Shell.Current.GoToAsync($"MedicalShiftDetails?shiftId={this.SelectedShift.ShiftId}");

            // Reset selection
            this.SelectedShift = null;
        }

        private async Task OnAddShiftAsync()
        {
            // Navigate to add shift page
            await Shell.Current.GoToAsync("AddEditMedicalShift");
        }
    }
}
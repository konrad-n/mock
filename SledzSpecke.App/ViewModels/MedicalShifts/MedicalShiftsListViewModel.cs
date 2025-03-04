using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.MedicalShifts
{
    public partial class MedicalShiftsListViewModel : BaseViewModel
    {
        private readonly ISpecializationService specializationService;
        private readonly IDatabaseService databaseService;
        private readonly IDialogService dialogService;

        // Filter state
        private bool allShiftsSelected = true;
        private bool currentInternshipSelected = false;
        private string searchText = string.Empty;
        private MedicalShift selectedShift;

        // Data
        private ObservableCollection<MedicalShift> shifts;
        private int currentInternshipId;
        private int? currentModuleId;

        public MedicalShiftsListViewModel(
            ISpecializationService specializationService,
            IDatabaseService databaseService,
            IDialogService dialogService)
        {
            this.specializationService = specializationService;
            this.databaseService = databaseService;
            this.dialogService = dialogService;

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
            this.LoadShiftsAsync().ConfigureAwait(false);
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
                // Pobierz bieżącą specjalizację i moduł (jeśli specjalizacja ma moduły)
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie znaleziono aktywnej specjalizacji.",
                        "OK");
                    return;
                }

                // Ustaw bieżący moduł, jeśli specjalizacja ma moduły
                if (specialization.HasModules && specialization.CurrentModuleId.HasValue)
                {
                    this.currentModuleId = specialization.CurrentModuleId.Value;
                }
                else
                {
                    this.currentModuleId = null;
                }

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

                // Pobierz wszystkie staże dla bieżącego modułu (jeśli jest wybrany)
                var internships = new List<Internship>();
                if (this.currentModuleId.HasValue)
                {
                    internships = await this.databaseService.GetInternshipsAsync(moduleId: this.currentModuleId.Value);
                }
                else
                {
                    var specializationId = specialization.SpecializationId;
                    internships = await this.databaseService.GetInternshipsAsync(specializationId: specializationId);
                }

                // Pobierz dyżury dla staży
                var allShifts = new List<MedicalShift>();

                if (this.currentInternshipId > 0)
                {
                    // Filtrowanie tylko dla bieżącego stażu
                    var shifts = await this.databaseService.GetMedicalShiftsAsync(this.currentInternshipId);
                    allShifts.AddRange(shifts);
                }
                else
                {
                    // Pobierz dyżury dla wszystkich staży w module lub całej specjalizacji
                    foreach (var internship in internships)
                    {
                        var shifts = await this.databaseService.GetMedicalShiftsAsync(internship.InternshipId);

                        // Dodaj informacje o stażu do każdego dyżuru (do wyświetlenia w UI)
                        foreach (var shift in shifts)
                        {
                            // Używamy AdditionalFields do tymczasowego przechowywania danych o stażu
                            var additionalFields = new Dictionary<string, object>();
                            if (!string.IsNullOrEmpty(shift.AdditionalFields))
                            {
                                try
                                {
                                    additionalFields = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(shift.AdditionalFields);
                                }
                                catch
                                {
                                    additionalFields = new Dictionary<string, object>();
                                }
                            }

                            additionalFields["InternshipName"] = internship.InternshipName;
                            shift.AdditionalFields = System.Text.Json.JsonSerializer.Serialize(additionalFields);
                        }

                        allShifts.AddRange(shifts);
                    }
                }

                // Apply search filter if needed
                if (!string.IsNullOrEmpty(this.SearchText))
                {
                    var searchLower = this.SearchText.ToLowerInvariant();
                    allShifts = allShifts.Where(s =>
                        s.Location.ToLowerInvariant().Contains(searchLower) ||
                        s.Date.ToString("d").Contains(searchLower)
                    ).ToList();
                }

                // Add shifts to collection
                foreach (var shift in allShifts.OrderByDescending(s => s.Date))
                {
                    // Pobierz nazwę stażu z AdditionalFields (jeśli dostępne)
                    string internshipName = string.Empty;
                    if (!string.IsNullOrEmpty(shift.AdditionalFields))
                    {
                        try
                        {
                            var additionalFields = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(shift.AdditionalFields);
                            if (additionalFields.TryGetValue("InternshipName", out object name))
                            {
                                internshipName = name?.ToString() ?? string.Empty;
                            }
                        }
                        catch
                        {
                            // Ignorowanie błędów deserializacji
                        }
                    }

                    // Ustaw właściwość InternshipName w VM dla każdego dyżuru
                    var shiftVM = new MedicalShiftViewModel
                    {
                        ShiftId = shift.ShiftId,
                        Date = shift.Date,
                        Hours = shift.Hours,
                        Minutes = shift.Minutes,
                        Location = shift.Location,
                        Year = shift.Year,
                        InternshipName = internshipName,
                    };

                    this.Shifts.Add(shift);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania dyżurów: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
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
            // Get current module ID to pass to the add page
            var specialization = await this.specializationService.GetCurrentSpecializationAsync();
            int? moduleId = null;

            if (specialization != null && specialization.HasModules && specialization.CurrentModuleId.HasValue)
            {
                moduleId = specialization.CurrentModuleId.Value;
            }

            // Navigate to add shift page, passing current module ID if applicable
            if (moduleId.HasValue)
            {
                await Shell.Current.GoToAsync($"AddEditMedicalShift?moduleId={moduleId.Value}");
            }
            else
            {
                await Shell.Current.GoToAsync("AddEditMedicalShift");
            }
        }
    }
}
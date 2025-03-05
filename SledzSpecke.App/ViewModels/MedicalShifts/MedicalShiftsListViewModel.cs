using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
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

        // Nowe pola dla funkcjonalności zgodnej ze starym SMK
        private MedicalShiftsSummary shiftsSummary = new();
        private bool hasUnsavedChanges;
        private ObservableCollection<MedicalShift> shiftsToDelete = new();

        // Istniejące pola
        private bool allShiftsSelected = true;
        private bool currentInternshipSelected = false;
        private string searchText = string.Empty;
        private MedicalShift selectedShift = null!;
        private ObservableCollection<MedicalShift> shifts = new();
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

            // Inicjalizacja komend
            this.RefreshCommand = new AsyncRelayCommand(this.LoadShiftsAsync);
            this.FilterShiftsCommand = new AsyncRelayCommand<string>(this.OnFilterShiftsAsync);
            this.FilterCommand = new AsyncRelayCommand(this.ApplyFiltersAsync);
            this.ShiftSelectedCommand = new AsyncRelayCommand(this.OnShiftSelectedAsync);
            this.AddShiftCommand = new AsyncRelayCommand(this.OnAddShiftAsync);

            // Nowe komendy zgodne ze starym SMK
            this.DeleteShiftCommand = new AsyncRelayCommand<MedicalShift>(this.OnDeleteShiftAsync);
            this.SaveChangesCommand = new AsyncRelayCommand(this.OnSaveChangesAsync);
            this.CancelChangesCommand = new AsyncRelayCommand(this.OnCancelChangesAsync);

            // Inicjalizacja właściwości
            this.Title = "Dyżury medyczne";
            this.Shifts = new ObservableCollection<MedicalShift>();
            this.ShiftsSummary = new MedicalShiftsSummary();
            this.ShiftsToDelete = new ObservableCollection<MedicalShift>();
            this.HasUnsavedChanges = false;

            // Wczytanie danych
            this.LoadShiftsAsync().ConfigureAwait(false);
        }

        // Nowe właściwości zgodne ze starym SMK
        public MedicalShiftsSummary ShiftsSummary
        {
            get => this.shiftsSummary;
            set => this.SetProperty(ref this.shiftsSummary, value);
        }

        public bool HasUnsavedChanges
        {
            get => this.hasUnsavedChanges;
            set => this.SetProperty(ref this.hasUnsavedChanges, value);
        }

        public ObservableCollection<MedicalShift> ShiftsToDelete
        {
            get => this.shiftsToDelete;
            set => this.SetProperty(ref this.shiftsToDelete, value);
        }

        // Istniejące właściwości
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

        // Istniejące komendy
        public ICommand RefreshCommand { get; }
        public ICommand FilterShiftsCommand { get; }
        public ICommand FilterCommand { get; }
        public ICommand ShiftSelectedCommand { get; }
        public ICommand AddShiftCommand { get; }

        // Nowe komendy zgodne ze starym SMK
        public ICommand DeleteShiftCommand { get; }
        public ICommand SaveChangesCommand { get; }
        public ICommand CancelChangesCommand { get; }

        // Metody
        private async Task LoadShiftsAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Pobranie danych specjalizacji i modułu
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie znaleziono aktywnej specjalizacji.",
                        "OK");
                    return;
                }

                // Ustawienie modułu
                if (specialization.HasModules && specialization.CurrentModuleId.HasValue)
                {
                    this.currentModuleId = specialization.CurrentModuleId.Value;
                }
                else
                {
                    this.currentModuleId = null;
                }

                // Ustawienie bieżącego stażu dla filtrowania
                if (this.CurrentInternshipSelected)
                {
                    var currentInternship = await this.specializationService.GetCurrentInternshipAsync();
                    this.currentInternshipId = currentInternship?.InternshipId ?? 0;
                }
                else
                {
                    this.currentInternshipId = 0;
                }

                // Wyczyszczenie istniejących dyżurów
                this.Shifts.Clear();

                // Pobranie staży
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

                // Pobranie dyżurów
                var allShifts = new List<MedicalShift>();

                if (this.currentInternshipId > 0)
                {
                    // Filtrowanie dla bieżącego stażu
                    var shifts = await this.databaseService.GetMedicalShiftsAsync(this.currentInternshipId);
                    allShifts.AddRange(shifts);
                }
                else
                {
                    // Pobranie dyżurów dla wszystkich staży
                    foreach (var internship in internships)
                    {
                        var shifts = await this.databaseService.GetMedicalShiftsAsync(internship.InternshipId);

                        // Dodanie informacji o stażu do każdego dyżuru
                        foreach (var shift in shifts)
                        {
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

                // Filtrowanie wyszukiwania
                if (!string.IsNullOrEmpty(this.SearchText))
                {
                    var searchLower = this.SearchText.ToLowerInvariant();
                    allShifts = allShifts.Where(s =>
                        s.Location.ToLowerInvariant().Contains(searchLower) ||
                        s.Date.ToString("d").Contains(searchLower)
                    ).ToList();
                }

                // Dodanie dyżurów do kolekcji
                foreach (var shift in allShifts.OrderByDescending(s => s.Date))
                {
                    this.Shifts.Add(shift);
                }

                // Obliczenie podsumowania
                this.ShiftsSummary = MedicalShiftsSummary.CalculateFromShifts(allShifts);
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

        // Nowe metody zgodne ze starym SMK
        private async Task OnDeleteShiftAsync(MedicalShift shift)
        {
            if (shift == null || shift.IsApproved)
            {
                // Nie można usunąć zatwierdzonego dyżuru
                await this.dialogService.DisplayAlertAsync(
                    "Informacja",
                    "Nie można usunąć zatwierdzonego dyżuru.",
                    "OK");
                return;
            }

            // Dodanie dyżuru do listy do usunięcia
            if (!this.ShiftsToDelete.Contains(shift))
            {
                this.ShiftsToDelete.Add(shift);
                this.HasUnsavedChanges = true;
            }

            // Usunięcie z listy wyświetlanej
            this.Shifts.Remove(shift);

            // Przeliczenie podsumowania
            this.ShiftsSummary = MedicalShiftsSummary.CalculateFromShifts(this.Shifts.ToList());
        }

        private async Task OnSaveChangesAsync()
        {
            if (!this.HasUnsavedChanges)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Prośba o potwierdzenie
                bool confirmDelete = await this.dialogService.DisplayAlertAsync(
                    "Potwierdź zmiany",
                    $"Czy na pewno chcesz usunąć {this.ShiftsToDelete.Count} dyżur(y)?",
                    "Zapisz",
                    "Anuluj");

                if (!confirmDelete)
                {
                    return;
                }

                // Usunięcie dyżurów z bazy danych
                foreach (var shift in this.ShiftsToDelete)
                {
                    await this.databaseService.DeleteMedicalShiftAsync(shift);
                }

                // Wyczyszczenie listy do usunięcia
                this.ShiftsToDelete.Clear();
                this.HasUnsavedChanges = false;

                await this.dialogService.DisplayAlertAsync(
                    "Sukces",
                    "Zmiany zostały zapisane pomyślnie.",
                    "OK");

                // Odświeżenie listy
                await this.LoadShiftsAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas zapisywania zmian: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie udało się zapisać zmian. Spróbuj ponownie.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task OnCancelChangesAsync()
        {
            if (!this.HasUnsavedChanges)
            {
                return;
            }

            // Prośba o potwierdzenie anulowania
            bool confirmCancel = await this.dialogService.DisplayAlertAsync(
                "Anuluj zmiany",
                "Czy na pewno chcesz anulować wprowadzone zmiany?",
                "Tak",
                "Nie");

            if (confirmCancel)
            {
                // Wyczyszczenie listy do usunięcia
                this.ShiftsToDelete.Clear();
                this.HasUnsavedChanges = false;

                // Odświeżenie listy
                await this.LoadShiftsAsync();
            }
        }

        // Istniejące metody (bez zmian)
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
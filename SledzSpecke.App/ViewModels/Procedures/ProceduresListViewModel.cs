using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Procedures
{
    public class ProceduresListViewModel : BaseViewModel
    {
        private readonly ISpecializationService specializationService;
        private readonly IDatabaseService databaseService;
        private readonly IDialogService dialogService;

        // Stan filtrów
        private bool typeASelected = true;
        private bool typeBSelected = false;
        private string searchText = string.Empty;
        private ProcedureViewModel selectedProcedure;

        // Dane
        private ObservableCollection<ProcedureViewModel> procedures = new();
        private int? currentModuleId;
        private ObservableCollection<ProcedureGrouping> groupedProcedures = new();

        public ProceduresListViewModel(
            ISpecializationService specializationService,
            IDatabaseService databaseService,
            IDialogService dialogService)
        {
            this.specializationService = specializationService ?? throw new ArgumentNullException(nameof(specializationService));
            this.databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            this.dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            // Inicjalizacja komend
            this.RefreshCommand = new AsyncRelayCommand(this.LoadProceduresAsync);
            this.SelectTypeCommand = new AsyncRelayCommand<string>(this.OnSelectTypeAsync);
            this.FilterCommand = new AsyncRelayCommand(this.ApplyFiltersAsync);
            this.ProcedureSelectedCommand = new AsyncRelayCommand(this.OnProcedureSelectedAsync);
            this.AddProcedureCommand = new AsyncRelayCommand(this.OnAddProcedureAsync);

            // Inicjalizacja właściwości
            this.Title = "Procedury i zabiegi";

            // Wczytanie danych
            this.LoadProceduresAsync().ConfigureAwait(false);
        }

        // Właściwości
        public ObservableCollection<ProcedureViewModel> Procedures
        {
            get => this.procedures;
            set => this.SetProperty(ref this.procedures, value);
        }

        public ObservableCollection<ProcedureGrouping> GroupedProcedures
        {
            get => this.groupedProcedures;
            set => this.SetProperty(ref this.groupedProcedures, value);
        }

        public ProcedureViewModel SelectedProcedure
        {
            get => this.selectedProcedure;
            set => this.SetProperty(ref this.selectedProcedure, value);
        }

        public bool TypeASelected
        {
            get => this.typeASelected;
            set => this.SetProperty(ref this.typeASelected, value);
        }

        public bool TypeBSelected
        {
            get => this.typeBSelected;
            set => this.SetProperty(ref this.typeBSelected, value);
        }

        public string SearchText
        {
            get => this.searchText;
            set => this.SetProperty(ref this.searchText, value);
        }

        // Komendy
        public ICommand RefreshCommand { get; }
        public ICommand SelectTypeCommand { get; }
        public ICommand FilterCommand { get; }
        public ICommand ProcedureSelectedCommand { get; }
        public ICommand AddProcedureCommand { get; }

        // Metody
        private async Task LoadProceduresAsync()
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

                // Wyczyść istniejące procedury
                this.Procedures.Clear();
                this.GroupedProcedures.Clear();

                // Pobierz procedury z bazy danych
                var procedures = await this.databaseService.GetProceduresAsync(
                    internshipId: null,
                    searchText: this.SearchText);

                // Filtrowanie według typu operatora
                if (!this.TypeASelected || !this.TypeBSelected)
                {
                    procedures = procedures.Where(p =>
                        (this.TypeASelected && p.OperatorCode == "A") ||
                        (this.TypeBSelected && p.OperatorCode == "B")).ToList();
                }

                // Filtrowanie według wyszukiwanego tekstu
                if (!string.IsNullOrEmpty(this.SearchText))
                {
                    var searchLower = this.SearchText.ToLowerInvariant();
                    procedures = procedures.Where(p =>
                        p.Code.ToLowerInvariant().Contains(searchLower) ||
                        p.Location.ToLowerInvariant().Contains(searchLower) ||
                        p.PatientInitials.ToLowerInvariant().Contains(searchLower)).ToList();
                }

                // Konwersja na ViewModele i dodanie do kolekcji
                foreach (var procedure in procedures)
                {
                    this.Procedures.Add(ProcedureViewModel.FromModel(procedure));
                }

                // Grupowanie procedur
                var groupedByCode = procedures
                    .GroupBy(p => p.ProcedureGroup)
                    .Select(g => new ProcedureGrouping(g.Key, g.ToList()))
                    .OrderBy(g => g.GroupName)
                    .ToList();

                foreach (var group in groupedByCode)
                {
                    this.GroupedProcedures.Add(group);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania procedur: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie udało się załadować procedur. Spróbuj ponownie.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task OnSelectTypeAsync(string type)
        {
            if (type == "A")
            {
                this.TypeASelected = true;
                this.TypeBSelected = false;
            }
            else if (type == "B")
            {
                this.TypeASelected = false;
                this.TypeBSelected = true;
            }
            else
            {
                // Opcja "Wszystkie"
                this.TypeASelected = true;
                this.TypeBSelected = true;
            }

            await this.LoadProceduresAsync();
        }

        private async Task ApplyFiltersAsync()
        {
            await this.LoadProceduresAsync();
        }

        private async Task OnProcedureSelectedAsync()
        {
            if (this.SelectedProcedure == null)
            {
                return;
            }

            // Nawigacja do strony szczegółów procedury
            await Shell.Current.GoToAsync($"ProcedureDetails?procedureId={this.SelectedProcedure.ProcedureId}");

            // Resetuj zaznaczenie
            this.SelectedProcedure = null;
        }

        private async Task OnAddProcedureAsync()
        {
            await Shell.Current.GoToAsync("AddProcedure");
        }
    }
}
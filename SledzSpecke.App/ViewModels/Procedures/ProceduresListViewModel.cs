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
        private Procedure selectedProcedure;

        // Dane
        private ObservableCollection<Procedure> procedures;
        private int? currentModuleId;
        private ObservableCollection<ProcedureGrouping> groupedProcedures;

        public ProceduresListViewModel(
            ISpecializationService specializationService,
            IDatabaseService databaseService,
            IDialogService dialogService)
        {
            this.specializationService = specializationService;
            this.databaseService = databaseService;
            this.dialogService = dialogService;

            // Inicjalizacja komend
            this.RefreshCommand = new AsyncRelayCommand(this.LoadProceduresAsync);
            this.SelectTypeCommand = new AsyncRelayCommand<string>(this.OnSelectTypeAsync);
            this.FilterCommand = new AsyncRelayCommand(this.ApplyFiltersAsync);
            this.ProcedureSelectedCommand = new AsyncRelayCommand(this.OnProcedureSelectedAsync);
            this.AddProcedureCommand = new AsyncRelayCommand(this.OnAddProcedureAsync);

            // Inicjalizacja właściwości
            this.Title = "Procedury i zabiegi";
            this.Procedures = new ObservableCollection<Procedure>();
            this.GroupedProcedures = new ObservableCollection<ProcedureGrouping>();

            // Wczytanie danych
            this.LoadProceduresAsync().ConfigureAwait(false);
        }

        // Właściwości
        public ObservableCollection<Procedure> Procedures
        {
            get => this.procedures;
            set => this.SetProperty(ref this.procedures, value);
        }

        public ObservableCollection<ProcedureGrouping> GroupedProcedures
        {
            get => this.groupedProcedures;
            set => this.SetProperty(ref this.groupedProcedures, value);
        }

        public Procedure SelectedProcedure
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

                // Pobierz wszystkie staże dla bieżącego modułu (jeśli jest wybrany) lub całej specjalizacji
                var internships = new List<Internship>();
                if (this.currentModuleId.HasValue)
                {
                    internships = await this.databaseService.GetInternshipsAsync(moduleId: this.currentModuleId.Value);
                }
                else
                {
                    internships = await this.databaseService.GetInternshipsAsync(
                        specializationId: specialization.SpecializationId);
                }

                // Pobierz procedury dla każdego stażu
                var allProcedures = new List<Procedure>();

                foreach (var internship in internships)
                {
                    var procedures = await this.databaseService.GetProceduresAsync(
                        internshipId: internship.InternshipId,
                        searchText: this.SearchText);

                    // Filtruj według typu (A lub B)
                    if (this.TypeASelected && !this.TypeBSelected)
                    {
                        procedures = procedures.Where(p => p.OperatorCode == "A").ToList();
                    }
                    else if (!this.TypeASelected && this.TypeBSelected)
                    {
                        procedures = procedures.Where(p => p.OperatorCode == "B").ToList();
                    }

                    // Dodaj informacje o stażu do każdej procedury (do wyświetlenia w UI)
                    foreach (var procedure in procedures)
                    {
                        // Używamy AdditionalFields do tymczasowego przechowywania danych o stażu
                        var additionalFields = new Dictionary<string, object>();
                        if (!string.IsNullOrEmpty(procedure.AdditionalFields))
                        {
                            try
                            {
                                additionalFields = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(procedure.AdditionalFields);
                            }
                            catch
                            {
                                additionalFields = new Dictionary<string, object>();
                            }
                        }

                        additionalFields["InternshipName"] = internship.InternshipName;
                        additionalFields["InternshipLocation"] = internship.DepartmentName;
                        procedure.AdditionalFields = System.Text.Json.JsonSerializer.Serialize(additionalFields);
                    }

                    allProcedures.AddRange(procedures);
                }

                // Sortowanie procedur wg daty (od najnowszej)
                var sortedProcedures = allProcedures.OrderByDescending(p => p.Date).ToList();

                // Dodanie procedur do kolekcji
                foreach (var procedure in sortedProcedures)
                {
                    this.Procedures.Add(procedure);
                }

                // Grupowanie procedur według kodów/typów
                var groupedByCode = sortedProcedures
                    .GroupBy(p => p.Code)
                    .Select(g => new ProcedureGrouping(g.Key, g.ToList()))
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
            // Pobierz identyfikator bieżącego modułu, aby przekazać do strony dodawania
            var specialization = await this.specializationService.GetCurrentSpecializationAsync();
            int? moduleId = null;

            if (specialization != null && specialization.HasModules && specialization.CurrentModuleId.HasValue)
            {
                moduleId = specialization.CurrentModuleId.Value;
            }

            // Nawigacja do strony dodawania procedury, przekazując identyfikator modułu, jeśli dostępny
            if (moduleId.HasValue)
            {
                await Shell.Current.GoToAsync($"AddEditProcedure?moduleId={moduleId.Value}");
            }
            else
            {
                await Shell.Current.GoToAsync("AddEditProcedure");
            }
        }
    }
}
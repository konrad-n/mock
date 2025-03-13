using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Procedures;

namespace SledzSpecke.App.ViewModels.Procedures
{
    public class ProcedureGroupViewModel : ObservableObject
    {
        private readonly IProcedureService procedureService;
        private readonly IDialogService dialogService;

        private ProcedureRequirement requirement;
        private ProcedureSummary statistics;
        private ObservableCollection<RealizedProcedureOldSMK> procedures;
        private RealizedProcedureOldSMK selectedProcedure;
        private bool isExpanded;
        private bool isLoading;
        private bool hasLoadedData;

        public ProcedureGroupViewModel(
            ProcedureRequirement requirement,
            List<RealizedProcedureOldSMK> procedures,
            ProcedureSummary statistics,
            IProcedureService procedureService,
            IDialogService dialogService)
        {
            this.requirement = requirement;
            this.statistics = statistics;
            this.procedures = new ObservableCollection<RealizedProcedureOldSMK>(procedures);
            this.procedureService = procedureService;
            this.dialogService = dialogService;
            this.hasLoadedData = false;
            this.isLoading = false;

            this.ToggleExpandCommand = new AsyncRelayCommand(this.OnToggleExpandAsync);
            this.EditProcedureCommand = new AsyncRelayCommand<RealizedProcedureOldSMK>(this.OnEditProcedure);
            this.DeleteProcedureCommand = new AsyncRelayCommand<RealizedProcedureOldSMK>(this.OnDeleteProcedure);
            this.AddProcedureCommand = new AsyncRelayCommand(this.OnAddProcedure);
            this.SelectProcedureCommand = new RelayCommand<RealizedProcedureOldSMK>(this.OnSelectProcedure);

            // Nasłuchuj zmian w kolekcji procedur
            this.procedures.CollectionChanged += this.Procedures_CollectionChanged;
        }

        public ProcedureRequirement Requirement => this.requirement;

        public string Title => this.requirement?.Name ?? "Nieznana procedura";

        public string StatsInfo => $"{this.statistics.CompletedCountA}/{this.statistics.RequiredCountA} (A), " +
                                   $"{this.statistics.CompletedCountB}/{this.statistics.RequiredCountB} (B)";

        public string ApprovedInfo => $"{this.statistics.ApprovedCountA}/{this.statistics.CompletedCountA} (A), " +
                                      $"{this.statistics.ApprovedCountB}/{this.statistics.CompletedCountB} (B)";

        public ObservableCollection<RealizedProcedureOldSMK> Procedures
        {
            get => this.procedures;
            set => this.SetProperty(ref this.procedures, value);
        }

        public RealizedProcedureOldSMK SelectedProcedure
        {
            get => this.selectedProcedure;
            set
            {
                if (this.SetProperty(ref this.selectedProcedure, value))
                {
                    this.OnPropertyChanged(nameof(this.SelectedProcedure));
                }
            }
        }

        public bool IsExpanded
        {
            get => this.isExpanded;
            set => this.SetProperty(ref this.isExpanded, value);
        }

        public bool IsLoading
        {
            get => this.isLoading;
            set => this.SetProperty(ref this.isLoading, value);
        }

        public ICommand ToggleExpandCommand { get; }
        public ICommand EditProcedureCommand { get; }
        public ICommand DeleteProcedureCommand { get; }
        public ICommand AddProcedureCommand { get; }
        public ICommand SelectProcedureCommand { get; }

        private void Procedures_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Jeżeli dodano nowe procedury, a mamy już wybraną procedurę, sprawdźmy czy dalej istnieje
            if (e.Action == NotifyCollectionChangedAction.Add && this.procedures.Count > 0 && this.selectedProcedure == null)
            {
                this.SelectedProcedure = this.procedures[0];
            }
            // Jeżeli usunięto procedury, a wybrana procedura była jedną z nich, wybierz nową lub wyczyść
            else if (e.Action == NotifyCollectionChangedAction.Remove && this.selectedProcedure != null)
            {
                if (!this.procedures.Contains(this.selectedProcedure))
                {
                    this.SelectedProcedure = this.procedures.Count > 0 ? this.procedures[0] : null;
                }
            }
            // Jeżeli zresetowano kolekcję, resetuj też wybór
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.SelectedProcedure = this.procedures.Count > 0 ? this.procedures[0] : null;
            }
        }

        private void OnSelectProcedure(RealizedProcedureOldSMK procedure)
        {
            if (procedure != null)
            {
                this.SelectedProcedure = procedure;
                System.Diagnostics.Debug.WriteLine($"Wybrano procedurę: {procedure.ProcedureId}");
            }
        }

        private async Task OnToggleExpandAsync()
        {
            // Nie rozwijaj, jeśli właśnie zwijamy
            if (this.isExpanded)
            {
                this.IsExpanded = false;
                return;
            }

            // Leniwe ładowanie danych - ładuj tylko gdy rozwijamy i nie mamy jeszcze danych
            if (!this.hasLoadedData && !this.isLoading)
            {
                this.isLoading = true;
                this.IsExpanded = true;  // Rozwiń, żeby pokazać indykator ładowania

                try
                {
                    // Wykonaj ładowanie w tle
                    await Task.Run(async () =>
                    {
                        // Pobierz procedury powiązane z tym wymaganiem
                        // Używamy właściwej sygnatury metody bez parametru filterId
                        var relatedProcedures = await this.procedureService.GetOldSMKProceduresAsync();

                        // Filtrujemy po kodzie wymagania po stronie klienta
                        var filteredProcedures = relatedProcedures.Where(p =>
                            (p.Code == "A - operator" && this.requirement.RequiredCountA > 0) ||
                            (p.Code == "B - asysta" && this.requirement.RequiredCountB > 0)).ToList();

                        System.Diagnostics.Debug.WriteLine($"Znaleziono {filteredProcedures.Count} procedur dla wymagania {this.requirement.Name}");

                        // Aktualizuj UI na głównym wątku
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            // Zamiast używać nieistniejącej klasy, wykonujemy operacje bezpośrednio
                            this.Procedures.Clear();
                            foreach (var procedure in filteredProcedures)
                            {
                                this.Procedures.Add(procedure);
                            }

                            // Ustaw pierwszą procedurę jako wybraną, jeśli jest dostępna
                            if (this.Procedures.Count > 0 && this.SelectedProcedure == null)
                            {
                                this.SelectedProcedure = this.Procedures[0];
                            }

                            this.hasLoadedData = true;
                            this.isLoading = false;
                            this.OnPropertyChanged(nameof(this.IsLoading));
                        });
                    });
                }
                catch (Exception ex)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania procedur: {ex.Message}");
                        await this.dialogService.DisplayAlertAsync(
                            "Błąd",
                            "Wystąpił problem podczas ładowania procedur.",
                            "OK");

                        this.isLoading = false;
                        this.OnPropertyChanged(nameof(this.IsLoading));
                    });
                }
            }
            else
            {
                // Jeśli już mamy dane, po prostu rozwiń sekcję
                this.IsExpanded = !this.isExpanded;
            }
        }

        private async Task OnEditProcedure(RealizedProcedureOldSMK procedure)
        {
            if (procedure == null)
            {
                return;
            }

            try
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "ProcedureId", procedure.ProcedureId.ToString() }
                };

                await Shell.Current.GoToAsync("AddEditOldSMKProcedure", navigationParameter);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas nawigacji: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem podczas przejścia do formularza edycji procedury.",
                    "OK");
            }
        }

        private async Task OnDeleteProcedure(RealizedProcedureOldSMK procedure)
        {
            if (procedure == null)
            {
                return;
            }

            bool confirm = await this.dialogService.DisplayAlertAsync(
                "Potwierdzenie",
                "Czy na pewno chcesz usunąć tę procedurę?",
                "Tak",
                "Nie");

            if (confirm)
            {
                try
                {
                    bool result = await this.procedureService.DeleteOldSMKProcedureAsync(procedure.ProcedureId);

                    if (result)
                    {
                        // Sprawdź, czy usuwana procedura jest aktualnie wybrana
                        if (this.SelectedProcedure == procedure)
                        {
                            // Ustaw nową wybraną procedurę (jeśli dostępna)
                            int index = this.Procedures.IndexOf(procedure);
                            if (index >= 0 && this.Procedures.Count > 1)
                            {
                                // Wybierz następną lub poprzednią procedurę
                                int newIndex = Math.Min(index, this.Procedures.Count - 2);
                                this.SelectedProcedure = this.Procedures[newIndex];
                            }
                            else
                            {
                                this.SelectedProcedure = null;
                            }
                        }

                        // Aktualizuj lokalne dane
                        this.Procedures.Remove(procedure);

                        // Aktualizuj statystyki
                        if (procedure.Code == "A - operator")
                        {
                            this.statistics.CompletedCountA--;
                        }
                        else if (procedure.Code == "B - asysta")
                        {
                            this.statistics.CompletedCountB--;
                        }

                        this.OnPropertyChanged(nameof(this.StatsInfo));
                        this.OnPropertyChanged(nameof(this.ApprovedInfo));
                    }
                    else
                    {
                        await this.dialogService.DisplayAlertAsync(
                            "Błąd",
                            "Nie udało się usunąć procedury.",
                            "OK");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Błąd podczas usuwania procedury: {ex.Message}");
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Wystąpił problem podczas usuwania procedury.",
                        "OK");
                }
            }
        }

        private async Task OnAddProcedure()
        {
            try
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "RequirementId", this.requirement.Id.ToString() }
                };

                await Shell.Current.GoToAsync("AddEditOldSMKProcedure", navigationParameter);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas nawigacji: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem podczas przejścia do formularza dodawania procedury.",
                    "OK");
            }
        }
    }
}
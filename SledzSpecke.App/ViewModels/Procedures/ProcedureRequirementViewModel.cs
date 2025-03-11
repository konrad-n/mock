using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Procedures;

namespace SledzSpecke.App.ViewModels.Procedures
{
    public class ProcedureRequirementViewModel : ObservableObject
    {
        private readonly IProcedureService procedureService;
        private readonly IDialogService dialogService;

        private ProcedureRequirement requirement;
        private ProcedureSummary statistics;
        private ObservableCollection<RealizedProcedureNewSMK> realizations;
        private int index;
        private int? moduleId;
        private bool isExpanded;
        private bool isAddingRealization;
        private string internshipName;
        private bool isLoading;
        private bool hasLoadedData;

        // Pola dla nowej realizacji
        private int countA;
        private int countB;
        private DateTime startDate;
        private DateTime endDate;
        private RealizedProcedureNewSMK selectedRealization;

        public ProcedureRequirementViewModel(
            ProcedureRequirement requirement,
            ProcedureSummary statistics,
            List<RealizedProcedureNewSMK> realizations,
            int index,
            int? moduleId,
            IProcedureService procedureService,
            IDialogService dialogService,
            string internshipName = "")
        {
            this.requirement = requirement;
            this.statistics = statistics;
            this.realizations = new ObservableCollection<RealizedProcedureNewSMK>(realizations);
            this.index = index;
            this.moduleId = moduleId;
            this.procedureService = procedureService;
            this.dialogService = dialogService;
            this.internshipName = internshipName;
            this.isLoading = false;
            this.hasLoadedData = false;

            this.startDate = DateTime.Now;
            this.endDate = DateTime.Now;

            this.ToggleExpandCommand = new AsyncRelayCommand(this.OnToggleExpandAsync);
            this.ToggleAddRealizationCommand = new RelayCommand(this.OnToggleAddRealization);
            this.SaveRealizationCommand = new AsyncRelayCommand(this.OnSaveRealization, this.CanSaveRealization);
            this.CancelRealizationCommand = new RelayCommand(this.OnCancelRealization);
            this.EditRealizationCommand = new AsyncRelayCommand<RealizedProcedureNewSMK>(this.OnEditRealization);
            this.DeleteRealizationCommand = new AsyncRelayCommand<RealizedProcedureNewSMK>(this.OnDeleteRealization);
        }

        public ProcedureRequirement Requirement => this.requirement;

        public string Title => $"{this.index}. {this.requirement?.Name ?? "Nieznana procedura"}";

        public string InternshipName => this.internshipName;

        public ProcedureSummary Statistics => this.statistics;

        public ObservableCollection<RealizedProcedureNewSMK> Realizations
        {
            get => this.realizations;
            set => this.SetProperty(ref this.realizations, value);
        }

        public bool IsExpanded
        {
            get => this.isExpanded;
            set => this.SetProperty(ref this.isExpanded, value);
        }

        public bool IsAddingRealization
        {
            get => this.isAddingRealization;
            set => this.SetProperty(ref this.isAddingRealization, value);
        }

        public bool IsLoading
        {
            get => this.isLoading;
            set => this.SetProperty(ref this.isLoading, value);
        }

        public int CountA
        {
            get => this.countA;
            set
            {
                if (this.SetProperty(ref this.countA, value))
                {
                    ((AsyncRelayCommand)this.SaveRealizationCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public int CountB
        {
            get => this.countB;
            set
            {
                if (this.SetProperty(ref this.countB, value))
                {
                    ((AsyncRelayCommand)this.SaveRealizationCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public DateTime StartDate
        {
            get => this.startDate;
            set
            {
                if (this.SetProperty(ref this.startDate, value))
                {
                    ((AsyncRelayCommand)this.SaveRealizationCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public DateTime EndDate
        {
            get => this.endDate;
            set
            {
                if (this.SetProperty(ref this.endDate, value))
                {
                    ((AsyncRelayCommand)this.SaveRealizationCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public RealizedProcedureNewSMK SelectedRealization
        {
            get => this.selectedRealization;
            set => this.SetProperty(ref this.selectedRealization, value);
        }

        public string RealizationsCountInfo => $"Wszystkich pozycji: {this.Realizations.Count}";

        public ICommand ToggleExpandCommand { get; }
        public ICommand ToggleAddRealizationCommand { get; }
        public ICommand SaveRealizationCommand { get; }
        public ICommand CancelRealizationCommand { get; }
        public ICommand EditRealizationCommand { get; }
        public ICommand DeleteRealizationCommand { get; }

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
                this.IsLoading = true;
                this.IsExpanded = true;  // Rozwiń, żeby pokazać indykator ładowania

                try
                {
                    // Asynchroniczne ładowanie realizacji dla tego wymagania
                    await Task.Run(async () =>
                    {
                        try
                        {
                            // Pobierz realizacje dla tego wymagania
                            var realizations = await this.procedureService.GetNewSMKProceduresAsync(
                                this.moduleId, this.requirement.Id);

                            // Aktualizuj UI na głównym wątku
                            await MainThread.InvokeOnMainThreadAsync(() =>
                            {
                                this.Realizations.Clear();

                                // Dodawaj realizacje po jednej z małymi opóźnieniami
                                foreach (var realization in realizations)
                                {
                                    this.Realizations.Add(realization);
                                }

                                this.hasLoadedData = true;
                                this.IsLoading = false;
                                this.OnPropertyChanged(nameof(this.RealizationsCountInfo));
                            });
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania realizacji: {ex.Message}");

                            // Aktualizuj UI na głównym wątku
                            await MainThread.InvokeOnMainThreadAsync(() =>
                            {
                                this.IsLoading = false;
                            });
                        }
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania procedur: {ex.Message}");
                    this.IsLoading = false;
                }
            }
            else if (this.hasLoadedData)
            {
                // Jeśli już mamy dane, po prostu rozwiń sekcję
                this.IsExpanded = true;
            }
        }

        private void OnToggleAddRealization()
        {
            // Zresetuj wartości przy każdym otwarciu formularza
            this.CountA = 0;
            this.CountB = 0;
            this.StartDate = DateTime.Now;
            this.EndDate = DateTime.Now;
            this.SelectedRealization = null;

            this.IsAddingRealization = !this.IsAddingRealization;
        }

        private bool CanSaveRealization()
        {
            return (this.CountA > 0 || this.CountB > 0) &&
                   this.StartDate <= this.EndDate;
        }

        private async Task OnSaveRealization()
        {
            if (this.IsLoading)
            {
                return;
            }

            this.IsLoading = true;

            try
            {
                var procedure = this.SelectedRealization ?? new RealizedProcedureNewSMK();

                procedure.ProcedureRequirementId = this.requirement.Id;
                procedure.ModuleId = this.moduleId;
                procedure.CountA = this.CountA;
                procedure.CountB = this.CountB;
                procedure.StartDate = this.StartDate;
                procedure.EndDate = this.EndDate;
                procedure.ProcedureName = this.requirement.Name;
                procedure.InternshipName = this.internshipName;
                procedure.SyncStatus = SyncStatus.NotSynced;

                bool success = await this.procedureService.SaveNewSMKProcedureAsync(procedure);

                if (success)
                {
                    // Zaktualizuj statystyki
                    this.statistics.CompletedCountA += this.CountA;
                    this.statistics.CompletedCountB += this.CountB;

                    // Jeśli to edycja, znajdź i zastąp realizację w kolekcji
                    if (this.SelectedRealization != null)
                    {
                        int index = this.Realizations.IndexOf(this.SelectedRealization);
                        if (index >= 0)
                        {
                            this.Realizations[index] = procedure;
                        }
                    }
                    else
                    {
                        // Dodaj nową realizację do kolekcji
                        this.Realizations.Add(procedure);
                    }

                    // Zresetuj formularz i zamknij go
                    this.CountA = 0;
                    this.CountB = 0;
                    this.StartDate = DateTime.Now;
                    this.EndDate = DateTime.Now;
                    this.SelectedRealization = null;
                    this.IsAddingRealization = false;

                    // Odśwież informacje o liczbie realizacji
                    this.OnPropertyChanged(nameof(this.RealizationsCountInfo));
                }
                else
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie udało się zapisać realizacji procedury.",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas zapisywania realizacji: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem podczas zapisywania realizacji procedury.",
                    "OK");
            }
            finally
            {
                this.IsLoading = false;
            }
        }

        private void OnCancelRealization()
        {
            this.IsAddingRealization = false;
            this.SelectedRealization = null;
        }

        private async Task OnEditRealization(RealizedProcedureNewSMK realization)
        {
            if (realization == null || this.IsLoading)
            {
                return;
            }

            // Procedury zsynchronizowane nie mogą być edytowane
            if (realization.SyncStatus == SyncStatus.Synced)
            {
                await this.dialogService.DisplayAlertAsync(
                    "Informacja",
                    "Nie można edytować zsynchronizowanej realizacji procedury.",
                    "OK");
                return;
            }

            // Ustaw wartości formularza na podstawie wybranej realizacji
            this.CountA = realization.CountA;
            this.CountB = realization.CountB;
            this.StartDate = realization.StartDate;
            this.EndDate = realization.EndDate;
            this.SelectedRealization = realization;

            // Otwórz formularz
            this.IsAddingRealization = true;
        }

        private async Task OnDeleteRealization(RealizedProcedureNewSMK realization)
        {
            if (realization == null || this.IsLoading)
            {
                return;
            }

            // Procedury zsynchronizowane nie mogą być usunięte
            if (realization.SyncStatus == SyncStatus.Synced)
            {
                await this.dialogService.DisplayAlertAsync(
                    "Informacja",
                    "Nie można usunąć zsynchronizowanej realizacji procedury.",
                    "OK");
                return;
            }

            bool confirm = await this.dialogService.DisplayAlertAsync(
                "Potwierdzenie",
                "Czy na pewno chcesz usunąć tę realizację procedury?",
                "Tak",
                "Nie");

            if (!confirm)
            {
                return;
            }

            this.IsLoading = true;

            try
            {
                bool result = await this.procedureService.DeleteNewSMKProcedureAsync(realization.ProcedureId);

                if (result)
                {
                    // Aktualizuj statystyki
                    this.statistics.CompletedCountA -= realization.CountA;
                    this.statistics.CompletedCountB -= realization.CountB;

                    // Usuń z kolekcji
                    this.Realizations.Remove(realization);

                    // Odśwież informacje o liczbie realizacji
                    this.OnPropertyChanged(nameof(this.RealizationsCountInfo));
                }
                else
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie udało się usunąć realizacji procedury.",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas usuwania realizacji: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem podczas usuwania realizacji procedury.",
                    "OK");
            }
            finally
            {
                this.IsLoading = false;
            }
        }
    }
}
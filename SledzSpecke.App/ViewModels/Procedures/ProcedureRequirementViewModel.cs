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
            IDialogService dialogService)
        {
            this.requirement = requirement;
            this.statistics = statistics;
            this.realizations = new ObservableCollection<RealizedProcedureNewSMK>(realizations);
            this.index = index;
            this.moduleId = moduleId;
            this.procedureService = procedureService;
            this.dialogService = dialogService;

            this.startDate = DateTime.Now;
            this.endDate = DateTime.Now;

            this.ToggleExpandCommand = new RelayCommand(this.OnToggleExpand);
            this.ToggleAddRealizationCommand = new RelayCommand(this.OnToggleAddRealization);
            this.SaveRealizationCommand = new AsyncRelayCommand(this.OnSaveRealization, this.CanSaveRealization);
            this.CancelRealizationCommand = new RelayCommand(this.OnCancelRealization);
            this.EditRealizationCommand = new AsyncRelayCommand<RealizedProcedureNewSMK>(this.OnEditRealization);
            this.DeleteRealizationCommand = new AsyncRelayCommand<RealizedProcedureNewSMK>(this.OnDeleteRealization);
        }

        public ProcedureRequirement Requirement => this.requirement;

        public string Title => $"{this.index}. {this.requirement?.Name ?? "Nieznana procedura"}";

        public string InternshipName => this.requirement?.InternshipName ?? string.Empty;

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

        private void OnToggleExpand()
        {
            this.IsExpanded = !this.IsExpanded;
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
                procedure.InternshipName = this.requirement.InternshipName;
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
        }

        private void OnCancelRealization()
        {
            this.IsAddingRealization = false;
            this.SelectedRealization = null;
        }

        private async Task OnEditRealization(RealizedProcedureNewSMK realization)
        {
            if (realization == null)
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
            if (realization == null)
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

            if (confirm)
            {
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
            }
        }
    }
}
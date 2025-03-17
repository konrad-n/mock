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

            this.ToggleExpandCommand = new AsyncRelayCommand(this.OnToggleExpandAsync);
            this.ToggleAddRealizationCommand = new AsyncRelayCommand(this.OnToggleAddRealizationAsync);
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

        public ICommand ToggleExpandCommand { get; }
        public ICommand ToggleAddRealizationCommand { get; }
        public ICommand EditRealizationCommand { get; }
        public ICommand DeleteRealizationCommand { get; }

        private async Task OnToggleExpandAsync()
        {
            if (this.isExpanded)
            {
                this.IsExpanded = false;
                return;
            }

            if (!this.hasLoadedData && !this.isLoading)
            {
                this.isLoading = true;
                this.IsExpanded = true;

                try
                {
                    await Task.Run(async () =>
                    {
                        var realizations = await this.procedureService.GetNewSMKProceduresAsync(
                            this.moduleId, this.requirement.Id);

                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            this.Realizations.Clear();
                            foreach (var realization in realizations)
                            {
                                this.Realizations.Add(realization);
                            }

                            this.hasLoadedData = true;
                            this.isLoading = false;
                        });
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania realizacji: {ex.Message}");
                    this.isLoading = false;
                }
            }
            else
            {
                this.IsExpanded = true;
            }
        }

        private async Task OnToggleAddRealizationAsync()
        {
            try
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "RequirementId", this.requirement.Id.ToString() }
                };

                await Shell.Current.GoToAsync("AddEditNewSMKProcedure", navigationParameter);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas nawigacji: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem podczas otwierania formularza dodawania realizacji.",
                    "OK");
            }
        }

        private async Task OnEditRealization(RealizedProcedureNewSMK realization)
        {
            if (realization == null)
            {
                return;
            }

            if (realization.SyncStatus == SyncStatus.Synced)
            {
                await this.dialogService.DisplayAlertAsync(
                    "Informacja",
                    "Nie można edytować zsynchronizowanej realizacji.",
                    "OK");
                return;
            }

            try
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "ProcedureId", realization.ProcedureId.ToString() },
                    { "RequirementId", this.requirement.Id.ToString() }
                };

                await Shell.Current.GoToAsync("AddEditNewSMKProcedure", navigationParameter);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas nawigacji: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem podczas otwierania formularza edycji realizacji.",
                    "OK");
            }
        }

        private async Task OnDeleteRealization(RealizedProcedureNewSMK realization)
        {
            if (realization == null)
            {
                return;
            }

            if (realization.SyncStatus == SyncStatus.Synced)
            {
                await this.dialogService.DisplayAlertAsync(
                    "Informacja",
                    "Nie można usunąć zsynchronizowanej realizacji.",
                    "OK");
                return;
            }

            try
            {
                bool confirm = await this.dialogService.DisplayAlertAsync(
                    "Potwierdzenie",
                    "Czy na pewno chcesz usunąć tę realizację?",
                    "Tak",
                    "Nie");

                if (confirm)
                {
                    bool success = await this.procedureService.DeleteNewSMKProcedureAsync(realization.ProcedureId);

                    if (success)
                    {
                        this.statistics.CompletedCountA -= realization.CountA;
                        this.statistics.CompletedCountB -= realization.CountB;
                        this.Realizations.Remove(realization);
                    }
                    else
                    {
                        await this.dialogService.DisplayAlertAsync(
                            "Błąd",
                            "Nie udało się usunąć realizacji.",
                            "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas usuwania realizacji: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem podczas usuwania realizacji.",
                    "OK");
            }
        }
    }
}
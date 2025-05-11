using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Procedures;

namespace SledzSpecke.App.ViewModels.Procedures
{
    public class ProcedureRequirementViewModel : ObservableObject
    {
        private readonly IProcedureService procedureService;
        private readonly IDialogService dialogService;
        private readonly IExceptionHandlerService exceptionHandler;

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
            IExceptionHandlerService exceptionHandler = null,
            string internshipName = "")
        {
            this.requirement = requirement;
            this.statistics = statistics;
            this.realizations = new ObservableCollection<RealizedProcedureNewSMK>(realizations);
            this.index = index;
            this.moduleId = moduleId;
            this.procedureService = procedureService;
            this.dialogService = dialogService;
            this.exceptionHandler = exceptionHandler;
            this.internshipName = internshipName;
            this.isLoading = false;
            this.hasLoadedData = false;

            this.ToggleExpandCommand = new AsyncRelayCommand(this.OnToggleExpandAsync);
            this.ToggleAddRealizationCommand = new AsyncRelayCommand(this.OnToggleAddRealizationAsync);
            this.EditRealizationCommand = new AsyncRelayCommand<RealizedProcedureNewSMK>(this.OnEditRealization);
            this.DeleteRealizationCommand = new AsyncRelayCommand<RealizedProcedureNewSMK>(this.OnDeleteRealization);

            this.LoadRealizationsAsync().ConfigureAwait(false);
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

        public bool HasRealizations => this.Realizations != null && this.Realizations.Any();

        public ICommand ToggleExpandCommand { get; }
        public ICommand ToggleAddRealizationCommand { get; }
        public ICommand EditRealizationCommand { get; }
        public ICommand DeleteRealizationCommand { get; }

        private async Task LoadRealizationsAsync()
        {
            if (this.hasLoadedData || this.IsLoading)
            {
                return;
            }

            this.IsLoading = true;

            await SafeExecuteAsync(async () =>
            {
                var realizations = await this.procedureService.GetNewSMKProceduresAsync(
                    this.moduleId,
                    this.requirement.Id);

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    this.Realizations.Clear();
                    foreach (var realization in realizations)
                    {
                        this.Realizations.Add(realization);
                    }
                    this.hasLoadedData = true;
                });

                this.OnPropertyChanged(nameof(this.Realizations));
                this.OnPropertyChanged(nameof(this.HasRealizations));
                this.OnPropertyChanged(nameof(this.Statistics));
            }, "Wystąpił problem podczas ładowania realizacji procedury.");

            this.IsLoading = false;
        }

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

                await SafeExecuteAsync(async () =>
                {
                    var realizations = await this.procedureService.GetNewSMKProceduresAsync(
                        this.moduleId,
                        this.requirement.Id);

                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        this.Realizations.Clear();
                        foreach (var realization in realizations)
                        {
                            this.Realizations.Add(realization);
                        }

                        this.OnPropertyChanged(nameof(this.Realizations));
                        this.OnPropertyChanged(nameof(this.HasRealizations));
                        this.OnPropertyChanged(nameof(this.Statistics));

                        this.hasLoadedData = true;
                        this.isLoading = false;
                        this.OnPropertyChanged(nameof(this.IsLoading));
                    });
                }, "Wystąpił problem podczas ładowania realizacji procedury.");
            }
            else
            {
                this.IsExpanded = !this.isExpanded;
            }
        }

        private async Task OnToggleAddRealizationAsync()
        {
            await SafeExecuteAsync(async () =>
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "RequirementId", this.requirement.Id.ToString() }
                };

                await Shell.Current.GoToAsync("AddEditNewSMKProcedure", navigationParameter);
            }, "Wystąpił problem podczas nawigacji do formularza dodawania procedury.");
        }

        private async Task OnEditRealization(RealizedProcedureNewSMK realization)
        {
            if (realization == null)
            {
                return;
            }

            await SafeExecuteAsync(async () =>
            {
                if (realization.SyncStatus == SyncStatus.Synced)
                {
                    throw new BusinessRuleViolationException(
                        "Cannot edit synced realization",
                        "Nie można edytować zsynchronizowanej realizacji.");
                }

                var navigationParameter = new Dictionary<string, object>
                {
                    { "ProcedureId", realization.ProcedureId.ToString() },
                    { "RequirementId", this.requirement.Id.ToString() }
                };

                await Shell.Current.GoToAsync("AddEditNewSMKProcedure", navigationParameter);
            }, "Wystąpił problem podczas edycji realizacji procedury.");
        }

        private async Task OnDeleteRealization(RealizedProcedureNewSMK realization)
        {
            if (realization == null)
            {
                return;
            }

            await SafeExecuteAsync(async () =>
            {
                if (realization.SyncStatus == SyncStatus.Synced)
                {
                    throw new BusinessRuleViolationException(
                        "Cannot delete synced realization",
                        "Nie można usunąć zsynchronizowanej realizacji.");
                }

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
                        this.OnPropertyChanged(nameof(this.HasRealizations));
                    }
                    else
                    {
                        throw new DomainLogicException(
                            "Failed to delete realization",
                            "Nie udało się usunąć realizacji.");
                    }
                }
            }, "Wystąpił problem podczas usuwania realizacji procedury.");
        }

        // Helper method for error handling
        private async Task SafeExecuteAsync(Func<Task> operation, string errorMessage)
        {
            if (this.exceptionHandler != null)
            {
                await this.exceptionHandler.ExecuteAsync(operation, null, errorMessage);
            }
            else
            {
                try
                {
                    await operation();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                    await this.dialogService.DisplayAlertAsync("Błąd", errorMessage, "OK");
                }
            }
        }
    }
}

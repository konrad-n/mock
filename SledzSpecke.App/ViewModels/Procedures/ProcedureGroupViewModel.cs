using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Procedures;

namespace SledzSpecke.App.ViewModels.Procedures
{
    public class ProcedureGroupViewModel : ObservableObject
    {
        private readonly IProcedureService procedureService;
        private readonly IDialogService dialogService;
        private readonly IExceptionHandlerService exceptionHandler;

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
            IDialogService dialogService,
            IExceptionHandlerService exceptionHandler = null)
        {
            this.requirement = requirement;
            this.statistics = statistics;
            this.procedures = new ObservableCollection<RealizedProcedureOldSMK>(procedures);
            this.procedureService = procedureService;
            this.dialogService = dialogService;
            this.exceptionHandler = exceptionHandler;
            this.hasLoadedData = false;
            this.isLoading = false;

            this.ToggleExpandCommand = new AsyncRelayCommand(this.OnToggleExpandAsync);
            this.EditProcedureCommand = new AsyncRelayCommand<RealizedProcedureOldSMK>(this.OnEditProcedure);
            this.DeleteProcedureCommand = new AsyncRelayCommand<RealizedProcedureOldSMK>(this.OnDeleteProcedure);
            this.AddProcedureCommand = new AsyncRelayCommand(this.OnAddProcedure);
            this.SelectProcedureCommand = new RelayCommand<RealizedProcedureOldSMK>(this.OnSelectProcedure);
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
            if (e.Action == NotifyCollectionChangedAction.Add && this.procedures.Count > 0 && this.selectedProcedure == null)
            {
                this.SelectedProcedure = this.procedures[0];
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove && this.selectedProcedure != null)
            {
                if (!this.procedures.Contains(this.selectedProcedure))
                {
                    this.SelectedProcedure = this.procedures.Count > 0 ? this.procedures[0] : null;
                }
            }
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
            }
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
                    var relatedProcedures = await this.procedureService.GetOldSMKProceduresAsync(
                        requirementId: this.requirement.Id);

                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        this.Procedures.Clear();
                        foreach (var procedure in relatedProcedures)
                        {
                            this.Procedures.Add(procedure);
                        }

                        if (this.Procedures.Count > 0 && this.SelectedProcedure == null)
                        {
                            this.SelectedProcedure = this.Procedures[0];
                        }

                        this.hasLoadedData = true;
                        this.isLoading = false;
                        this.OnPropertyChanged(nameof(this.IsLoading));
                    });
                }, "Wystąpił problem podczas ładowania procedur.");
            }
            else
            {
                this.IsExpanded = !this.isExpanded;
            }
        }

        private async Task OnEditProcedure(RealizedProcedureOldSMK procedure)
        {
            if (procedure == null)
            {
                return;
            }

            await SafeExecuteAsync(async () =>
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "ProcedureId", procedure.ProcedureId.ToString() }
                };

                await Shell.Current.GoToAsync("AddEditOldSMKProcedure", navigationParameter);
            }, "Wystąpił problem podczas przejścia do edycji procedury.");
        }

        public async Task OnDeleteProcedure(RealizedProcedureOldSMK procedure)
        {
            if (procedure == null)
            {
                return;
            }

            await SafeExecuteAsync(async () =>
            {
                bool confirm = await this.dialogService.DisplayAlertAsync(
                    "Potwierdzenie",
                    "Czy na pewno chcesz usunąć tę procedurę?",
                    "Tak",
                    "Nie");

                if (confirm)
                {
                    bool result = await this.procedureService.DeleteOldSMKProcedureAsync(procedure.ProcedureId);

                    if (result)
                    {
                        if (this.SelectedProcedure == procedure)
                        {
                            int index = this.Procedures.IndexOf(procedure);
                            if (index >= 0 && this.Procedures.Count > 1)
                            {
                                int newIndex = Math.Min(index, this.Procedures.Count - 2);
                                this.SelectedProcedure = this.Procedures[newIndex];
                            }
                            else
                            {
                                this.SelectedProcedure = null;
                            }
                        }

                        this.Procedures.Remove(procedure);

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
                        throw new DomainLogicException(
                            "Failed to delete procedure",
                            "Nie udało się usunąć procedury.");
                    }
                }
            }, "Wystąpił problem podczas usuwania procedury.");
        }

        private async Task OnAddProcedure()
        {
            await SafeExecuteAsync(async () =>
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "RequirementId", this.requirement.Id.ToString() }
                };

                await Shell.Current.GoToAsync("AddEditOldSMKProcedure", navigationParameter);
            }, "Wystąpił problem podczas przejścia do dodawania procedury.");
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

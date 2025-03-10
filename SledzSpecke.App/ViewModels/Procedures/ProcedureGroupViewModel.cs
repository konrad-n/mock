using System.Collections.ObjectModel;
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
        private bool isExpanded;

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

            this.ToggleExpandCommand = new RelayCommand(this.OnToggleExpand);
            this.EditProcedureCommand = new AsyncRelayCommand<RealizedProcedureOldSMK>(this.OnEditProcedure);
            this.DeleteProcedureCommand = new AsyncRelayCommand<RealizedProcedureOldSMK>(this.OnDeleteProcedure);
            this.AddProcedureCommand = new AsyncRelayCommand(this.OnAddProcedure);
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

        public bool IsExpanded
        {
            get => this.isExpanded;
            set => this.SetProperty(ref this.isExpanded, value);
        }

        public ICommand ToggleExpandCommand { get; }
        public ICommand EditProcedureCommand { get; }
        public ICommand DeleteProcedureCommand { get; }
        public ICommand AddProcedureCommand { get; }

        private void OnToggleExpand()
        {
            this.IsExpanded = !this.IsExpanded;
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
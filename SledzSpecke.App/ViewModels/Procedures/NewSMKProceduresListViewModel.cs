using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Procedures;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Procedures
{
    public class NewSMKProceduresListViewModel : BaseViewModel
    {
        private readonly IProcedureService procedureService;
        private readonly IAuthService authService;
        private readonly IDialogService dialogService;
        private readonly ISpecializationService specializationService;

        private ObservableCollection<ProcedureRequirementViewModel> procedureRequirements;
        private ProcedureSummary summary;
        private bool isRefreshing;
        private string moduleTitle;
        private bool hasTwoModules;
        private bool basicModuleSelected;
        private bool specialisticModuleSelected;
        private int currentModuleId;

        public NewSMKProceduresListViewModel(
            IProcedureService procedureService,
            IAuthService authService,
            IDialogService dialogService,
            ISpecializationService specializationService)
        {
            this.procedureService = procedureService;
            this.authService = authService;
            this.dialogService = dialogService;
            this.specializationService = specializationService;

            this.Title = "Procedury (Nowy SMK)";
            this.ProcedureRequirements = new ObservableCollection<ProcedureRequirementViewModel>();
            this.Summary = new ProcedureSummary();

            // Inicjalizacja komend
            this.RefreshCommand = new AsyncRelayCommand(this.LoadDataAsync);
            this.SelectModuleCommand = new AsyncRelayCommand<string>(this.OnSelectModuleAsync);

            // Zarejestruj na zdarzenie zmiany modułu
            this.specializationService.CurrentModuleChanged += this.OnModuleChanged;

            // Załaduj dane
            this.LoadDataAsync().ConfigureAwait(false);
        }

        public ObservableCollection<ProcedureRequirementViewModel> ProcedureRequirements
        {
            get => this.procedureRequirements;
            set => this.SetProperty(ref this.procedureRequirements, value);
        }

        public ProcedureSummary Summary
        {
            get => this.summary;
            set => this.SetProperty(ref this.summary, value);
        }

        public bool IsRefreshing
        {
            get => this.isRefreshing;
            set => this.SetProperty(ref this.isRefreshing, value);
        }

        public string ModuleTitle
        {
            get => this.moduleTitle;
            set => this.SetProperty(ref this.moduleTitle, value);
        }

        public bool HasTwoModules
        {
            get => this.hasTwoModules;
            set => this.SetProperty(ref this.hasTwoModules, value);
        }

        public bool BasicModuleSelected
        {
            get => this.basicModuleSelected;
            set => this.SetProperty(ref this.basicModuleSelected, value);
        }

        public bool SpecialisticModuleSelected
        {
            get => this.specialisticModuleSelected;
            set => this.SetProperty(ref this.specialisticModuleSelected, value);
        }

        public int CurrentModuleId
        {
            get => this.currentModuleId;
            set
            {
                if (this.SetProperty(ref this.currentModuleId, value))
                {
                    this.LoadDataAsync().ConfigureAwait(false);
                }
            }
        }

        public ICommand RefreshCommand { get; }
        public ICommand SelectModuleCommand { get; }

        private async void OnModuleChanged(object sender, int moduleId)
        {
            this.CurrentModuleId = moduleId;
            await this.LoadDataAsync();
        }

        public async Task LoadDataAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;
            this.IsRefreshing = true;

            try
            {
                // Pobierz dane o specjalizacji i modułach
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return;
                }

                // Sprawdź, czy specjalizacja ma dwa moduły
                var modules = await this.specializationService.GetModulesAsync(specialization.SpecializationId);
                this.HasTwoModules = modules.Any(m => m.Type == ModuleType.Basic);

                // Pobierz bieżący moduł jeśli nie jest wybrany
                if (this.CurrentModuleId == 0 && specialization.CurrentModuleId.HasValue)
                {
                    this.CurrentModuleId = specialization.CurrentModuleId.Value;
                }

                var currentModule = modules.FirstOrDefault(m => m.ModuleId == this.CurrentModuleId)
                                     ?? modules.FirstOrDefault();

                if (currentModule != null)
                {
                    this.ModuleTitle = currentModule.Name;
                    this.BasicModuleSelected = currentModule.Type == ModuleType.Basic;
                    this.SpecialisticModuleSelected = currentModule.Type == ModuleType.Specialistic;

                    // Pobierz podsumowanie procedur dla bieżącego modułu
                    this.Summary = await this.procedureService.GetProcedureSummaryAsync(currentModule.ModuleId);

                    // Pobierz wymagania procedur dla bieżącego modułu
                    var requirements = await this.procedureService.GetAvailableProcedureRequirementsAsync(currentModule.ModuleId);

                    // Utwórz ViewModele dla każdego wymagania
                    this.ProcedureRequirements.Clear();

                    int index = 1;
                    foreach (var requirement in requirements)
                    {
                        // Pobierz realizacje dla tego wymagania
                        var realizations = await this.procedureService.GetNewSMKProceduresAsync(
                            currentModule.ModuleId, requirement.Id);

                        // Pobierz statystyki dla tego wymagania
                        var stats = await this.procedureService.GetProcedureSummaryAsync(
                            currentModule.ModuleId, requirement.Id);

                        // Utwórz ViewModel
                        var viewModel = new ProcedureRequirementViewModel(
                            requirement,
                            stats,
                            realizations,
                            index,
                            currentModule.ModuleId,
                            this.procedureService,
                            this.dialogService);

                        this.ProcedureRequirements.Add(viewModel);
                        index++;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania danych: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem podczas ładowania procedur.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
                this.IsRefreshing = false;
            }
        }

        private async Task OnSelectModuleAsync(string moduleType)
        {
            try
            {
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return;
                }

                var modules = await this.specializationService.GetModulesAsync(specialization.SpecializationId);

                if (moduleType == "Basic")
                {
                    var basicModule = modules.FirstOrDefault(m => m.Type == ModuleType.Basic);
                    if (basicModule != null)
                    {
                        await this.specializationService.SetCurrentModuleAsync(basicModule.ModuleId);
                        this.CurrentModuleId = basicModule.ModuleId;
                    }
                }
                else if (moduleType == "Specialistic")
                {
                    var specialisticModule = modules.FirstOrDefault(m => m.Type == ModuleType.Specialistic);
                    if (specialisticModule != null)
                    {
                        await this.specializationService.SetCurrentModuleAsync(specialisticModule.ModuleId);
                        this.CurrentModuleId = specialisticModule.ModuleId;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas zmiany modułu: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem podczas zmiany modułu.",
                    "OK");
            }
        }

        public void Dispose()
        {
            // Odłącz obserwator zdarzenia zmiany modułu
            this.specializationService.CurrentModuleChanged -= this.OnModuleChanged;
        }
    }
}
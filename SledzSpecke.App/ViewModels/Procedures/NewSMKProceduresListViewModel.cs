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
    public class NewSMKProceduresListViewModel : BaseViewModel, IDisposable
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
        private List<ProcedureRequirement> allRequirements;
        private bool isLoadingData;
        private int batchSize = 3;
        private bool isLoading;

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
            this.allRequirements = new List<ProcedureRequirement>();
            this.isLoadingData = false;

            this.RefreshCommand = new AsyncRelayCommand(this.LoadDataAsync);
            this.SelectModuleCommand = new AsyncRelayCommand<string>(this.OnSelectModuleAsync);
            this.LoadMoreCommand = new AsyncRelayCommand(this.LoadMoreItemsAsync);
            this.specializationService.CurrentModuleChanged += this.OnModuleChanged;

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

        public bool IsLoading
        {
            get => this.isLoading;
            set => this.SetProperty(ref this.isLoading, value);
        }

        public ICommand RefreshCommand { get; }
        public ICommand SelectModuleCommand { get; }
        public ICommand LoadMoreCommand { get; }

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

            var specialization = await this.specializationService.GetCurrentSpecializationAsync();
            if (specialization == null)
            {
                return;
            }

            var modules = await this.specializationService.GetModulesAsync(specialization.SpecializationId);
            this.HasTwoModules = modules.Any(m => m.Type == ModuleType.Basic);
            var currentModule = await this.specializationService.GetCurrentModuleAsync();

            if (currentModule == null)
            {
                return;
            }

            this.BasicModuleSelected = currentModule.Type == ModuleType.Basic;
            this.SpecialisticModuleSelected = currentModule.Type == ModuleType.Specialistic;
            this.ModuleTitle = currentModule.Name;

            var requirements = await this.procedureService.GetAvailableProcedureRequirementsAsync(currentModule.ModuleId);
            this.ProcedureRequirements.Clear();

            for (int i = 0; i < requirements.Count; i++)
            {
                var requirement = requirements[i];
                var stats = await this.procedureService.GetProcedureSummaryAsync(currentModule.ModuleId, requirement.Id);

                var viewModel = new ProcedureRequirementViewModel(
                    requirement,
                    stats,
                    new List<RealizedProcedureNewSMK>(),
                    i + 1,
                    currentModule.ModuleId,
                    this.procedureService,
                    this.dialogService);

                this.ProcedureRequirements.Add(viewModel);
            }

            this.IsBusy = false;
        }

        public async Task LoadMoreItemsAsync()
        {
            if (this.isLoadingData || this.allRequirements == null)
            {
                return;
            }

            this.isLoadingData = true;

            int currentCount = this.ProcedureRequirements.Count;

            if (currentCount >= this.allRequirements.Count)
            {
                return;
            }

            int itemsToLoad = Math.Min(this.batchSize, this.allRequirements.Count - currentCount);
            var requirementsToLoad = this.allRequirements.Skip(currentCount).Take(itemsToLoad).ToList();

            for (int i = 0; i < requirementsToLoad.Count; i++)
            {
                var requirement = requirementsToLoad[i];
                var stats = await this.procedureService.GetProcedureSummaryAsync(
                    this.CurrentModuleId, requirement.Id);

                var viewModel = new ProcedureRequirementViewModel(
                    requirement,
                    stats,
                    new List<RealizedProcedureNewSMK>(),
                    currentCount + i + 1,
                    this.CurrentModuleId,
                    this.procedureService,
                    this.dialogService);

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    this.ProcedureRequirements.Add(viewModel);
                });

                await Task.Delay(50);
            }
            this.isLoadingData = false;
        }

        private async Task OnSelectModuleAsync(string moduleType)
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

        public void Dispose()
        {
            this.specializationService.CurrentModuleChanged -= this.OnModuleChanged;
        }
    }
}
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.MedicalShifts;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.App.ViewModels.Internships;

namespace SledzSpecke.App.ViewModels.MedicalShifts
{
    public class NewSMKMedicalShiftsListViewModel : BaseViewModel
    {
        private readonly IMedicalShiftsService medicalShiftsService;
        private readonly IAuthService authService;
        private readonly IDialogService dialogService;
        private readonly ISpecializationService specializationService;

        private ObservableCollection<InternshipRequirementViewModel> internshipRequirements;
        private bool isRefreshing;
        private string moduleTitle;
        private bool basicModuleSelected;
        private bool specialisticModuleSelected;
        private bool hasTwoModules;

        public NewSMKMedicalShiftsListViewModel(
            IMedicalShiftsService medicalShiftsService,
            IAuthService authService,
            IDialogService dialogService,
            ISpecializationService specializationService)
        {
            this.medicalShiftsService = medicalShiftsService;
            this.authService = authService;
            this.dialogService = dialogService;
            this.specializationService = specializationService;

            this.Title = "Dyżury medyczne (Nowy SMK)";
            this.InternshipRequirements = new ObservableCollection<InternshipRequirementViewModel>();
            this.RefreshCommand = new AsyncRelayCommand(this.LoadDataAsync);
            this.SelectModuleCommand = new AsyncRelayCommand<string>(this.OnSelectModuleAsync);

            this.LoadDataAsync().ConfigureAwait(false);
        }

        public ObservableCollection<InternshipRequirementViewModel> InternshipRequirements
        {
            get => this.internshipRequirements;
            set => this.SetProperty(ref this.internshipRequirements, value);
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

        public bool HasTwoModules
        {
            get => this.hasTwoModules;
            set => this.SetProperty(ref this.hasTwoModules, value);
        }

        public ICommand RefreshCommand { get; }
        public ICommand SelectModuleCommand { get; }

        private async Task OnSelectModuleAsync(string moduleType)
        {
            var specialization = await this.specializationService.GetCurrentSpecializationAsync();
            if (specialization == null)
            {
                return;
            }

            var modules = await this.specializationService.GetModulesAsync(specialization.SpecializationId);

            int? newModuleId = null;

            if (moduleType == "Basic")
            {
                var basicModule = modules.FirstOrDefault(m => m.Type == ModuleType.Basic);
                if (basicModule != null)
                {
                    newModuleId = basicModule.ModuleId;
                    await this.specializationService.SetCurrentModuleAsync(basicModule.ModuleId);
                    this.BasicModuleSelected = true;
                    this.SpecialisticModuleSelected = false;
                }
            }
            else if (moduleType == "Specialistic")
            {
                var specialisticModule = modules.FirstOrDefault(m => m.Type == ModuleType.Specialistic);
                if (specialisticModule != null)
                {
                    newModuleId = specialisticModule.ModuleId;
                    await this.specializationService.SetCurrentModuleAsync(specialisticModule.ModuleId);
                    this.BasicModuleSelected = false;
                    this.SpecialisticModuleSelected = true;
                }
            }
            await this.LoadDataAsync();
        }

        private bool isLoading = false;

        public async Task LoadDataAsync()
        {
            if (this.IsBusy || isLoading)
            {
                return;
            }

            isLoading = true;
            this.IsBusy = true;
            this.IsRefreshing = true;

            var specialization = await this.specializationService.GetCurrentSpecializationAsync();
            if (specialization == null)
            {
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie znaleziono aktywnej specjalizacji.",
                    "OK");
                return;
            }

            var modules = await this.specializationService.GetModulesAsync(specialization.SpecializationId);
            this.HasTwoModules = modules.Any(m => m.Type == ModuleType.Basic);
            var currentModule = await this.specializationService.GetCurrentModuleAsync();

            if (currentModule != null)
            {
                this.ModuleTitle = currentModule.Name;
                this.BasicModuleSelected = currentModule.Type == ModuleType.Basic;
                this.SpecialisticModuleSelected = currentModule.Type == ModuleType.Specialistic;
            }

            var requirements = await this.medicalShiftsService.GetAvailableInternshipRequirementsAsync();
            var viewModels = new List<InternshipRequirementViewModel>();

            foreach (var requirement in requirements)
            {
                var summary = await this.medicalShiftsService.GetShiftsSummaryAsync(internshipRequirementId: requirement.Id);
                var shifts = await this.medicalShiftsService.GetNewSMKShiftsAsync(requirement.Id);
                var viewModel = new InternshipRequirementViewModel(
                    requirement,
                    summary,
                    shifts,
                    this.medicalShiftsService,
                    this.dialogService,
                    currentModule?.ModuleId);

                viewModels.Add(viewModel);
            }

            this.InternshipRequirements.Clear();
            foreach (var viewModel in viewModels)
            {
                this.InternshipRequirements.Add(viewModel);
            }

            this.IsBusy = false;
            this.IsRefreshing = false;
            isLoading = false;
        }
    }
}
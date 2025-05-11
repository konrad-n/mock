using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Internships
{
    public class NewSMKInternshipsListViewModel : BaseViewModel
    {
        private readonly ISpecializationService specializationService;
        private readonly IDialogService dialogService;
        private readonly IAuthService authService;

        private ObservableCollection<InternshipStageViewModel> internshipRequirements;
        private bool isRefreshing;
        private string moduleTitle;
        private bool basicModuleSelected;
        private bool specialisticModuleSelected;
        private bool hasTwoModules;
        private bool isLoading = false;

        public NewSMKInternshipsListViewModel(
            ISpecializationService specializationService,
            IDialogService dialogService,
            IAuthService authService,
            IExceptionHandlerService exceptionHandler) : base(exceptionHandler)
        {
            this.specializationService = specializationService;
            this.dialogService = dialogService;
            this.authService = authService;

            this.Title = "Staże (Nowy SMK)";
            this.InternshipRequirements = new ObservableCollection<InternshipStageViewModel>();
            this.RefreshCommand = new AsyncRelayCommand(this.LoadDataAsync);
            this.SelectModuleCommand = new AsyncRelayCommand<string>(this.OnSelectModuleAsync);

            this.LoadDataAsync().ConfigureAwait(false);
        }

        public ObservableCollection<InternshipStageViewModel> InternshipRequirements
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
            await SafeExecuteAsync(async () =>
            {
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    throw new ResourceNotFoundException(
                        "Specialization not found",
                        "Nie znaleziono aktywnej specjalizacji.");
                }

                var modules = await this.specializationService.GetModulesAsync(specialization.SpecializationId);

                if (moduleType == "Basic")
                {
                    var basicModule = modules.FirstOrDefault(m => m.Type == ModuleType.Basic);
                    if (basicModule != null)
                    {
                        await this.specializationService.SetCurrentModuleAsync(basicModule.ModuleId);
                        this.BasicModuleSelected = true;
                        this.SpecialisticModuleSelected = false;
                    }
                    else
                    {
                        throw new ResourceNotFoundException(
                            "Basic module not found",
                            "Nie znaleziono modułu podstawowego.");
                    }
                }
                else if (moduleType == "Specialistic")
                {
                    var specialisticModule = modules.FirstOrDefault(m => m.Type == ModuleType.Specialistic);
                    if (specialisticModule != null)
                    {
                        await this.specializationService.SetCurrentModuleAsync(specialisticModule.ModuleId);
                        this.BasicModuleSelected = false;
                        this.SpecialisticModuleSelected = true;
                    }
                    else
                    {
                        throw new ResourceNotFoundException(
                            "Specialistic module not found",
                            "Nie znaleziono modułu specjalistycznego.");
                    }
                }

                await this.LoadDataAsync();
            }, $"Nie udało się przełączyć na moduł {moduleType}.");
        }

        public async Task LoadDataAsync()
        {
            if (this.IsBusy || isLoading)
            {
                return;
            }

            isLoading = true;
            this.IsBusy = true;
            this.IsRefreshing = true;

            try
            {
                await SafeExecuteAsync(async () =>
                {
                    var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                    if (specialization == null)
                    {
                        throw new ResourceNotFoundException(
                            "Specialization not found",
                            "Nie znaleziono aktywnej specjalizacji.");
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
                    else
                    {
                        throw new ResourceNotFoundException(
                            "Current module not found",
                            "Nie znaleziono aktualnego modułu.");
                    }

                    var internships = await this.specializationService.GetInternshipsAsync(currentModule?.ModuleId);

                    var viewModels = new List<InternshipStageViewModel>();

                    foreach (var internship in internships)
                    {
                        // For new SMK
                        var realizedInternships = await this.specializationService.GetRealizedInternshipsNewSMKAsync(
                            moduleId: currentModule?.ModuleId,
                            internshipRequirementId: internship.InternshipId);

                        // Get the exception handler to pass to the InternshipStageViewModel
                        var exceptionHandler = IPlatformApplication.Current.Services.GetService<IExceptionHandlerService>();

                        var viewModel = new InternshipStageViewModel(
                            internship,
                            realizedInternships,
                            null, // Empty for new SMK
                            this.specializationService,
                            this.dialogService,
                            this.authService,
                            exceptionHandler,
                            currentModule?.ModuleId);

                        viewModels.Add(viewModel);
                    }

                    this.InternshipRequirements.Clear();
                    foreach (var viewModel in viewModels)
                    {
                        this.InternshipRequirements.Add(viewModel);
                    }
                }, "Wystąpił błąd podczas ładowania danych. Spróbuj ponownie.");
            }
            finally
            {
                this.IsBusy = false;
                this.IsRefreshing = false;
                isLoading = false;
            }
        }
    }
}

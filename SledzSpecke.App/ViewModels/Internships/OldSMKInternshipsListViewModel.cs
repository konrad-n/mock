using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Exceptions;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Exceptions;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Internships
{
    public class OldSMKInternshipsListViewModel : BaseViewModel
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

        public OldSMKInternshipsListViewModel(
            ISpecializationService specializationService,
            IDialogService dialogService,
            IAuthService authService,
            IExceptionHandlerService exceptionHandler) : base(exceptionHandler)
        {
            this.specializationService = specializationService;
            this.dialogService = dialogService;
            this.authService = authService;

            this.Title = "Staże (Stary SMK)";
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

                    int startYear = 1;
                    int endYear = 2;

                    if (currentModule != null && currentModule.Type == ModuleType.Specialistic)
                    {
                        startYear = 3;
                        endYear = 6;
                    }

                    var dbService = IPlatformApplication.Current.Services.GetRequiredService<SledzSpecke.App.Services.Database.IDatabaseService>();
                    var allDbRealizations = await dbService.QueryAsync<RealizedInternshipOldSMK>(
                        "SELECT * FROM RealizedInternshipOldSMK WHERE SpecializationId = ?", specialization.SpecializationId);
                    var allRealizedInternships = new List<RealizedInternshipOldSMK>();

                    // Get realizations from Year=0 first (not assigned to a specific year)
                    var yearZeroRealizations = await dbService.QueryAsync<RealizedInternshipOldSMK>(
                        "SELECT * FROM RealizedInternshipOldSMK WHERE SpecializationId = ? AND Year = 0",
                        specialization.SpecializationId);

                    foreach (var r in yearZeroRealizations)
                    {
                        allRealizedInternships.Add(r);
                    }

                    for (int year = startYear; year <= endYear; year++)
                    {
                        var yearRealizations = await this.specializationService.GetRealizedInternshipsOldSMKAsync(year);
                        allRealizedInternships.AddRange(yearRealizations.Where(r => r.Year == year));
                    }

                    var viewModels = new List<InternshipStageViewModel>();

                    // Get the exception handler to pass to the InternshipStageViewModel
                    var exceptionHandler = IPlatformApplication.Current.Services.GetService<IExceptionHandlerService>();

                    foreach (var internship in internships)
                    {
                        var realizationsForThisInternship = allRealizedInternships
                            .Where(r => {
                                string realizationName = r.InternshipName ?? "null";
                                string requirementName = internship.InternshipName ?? "null";

                                if (string.IsNullOrEmpty(r.InternshipName) || r.InternshipName == "Staż bez nazwy")
                                {
                                    if (internships.IndexOf(internship) == 0)
                                    {
                                        return true;
                                    }
                                    return false;
                                }

                                bool exactMatch = r.InternshipName != null &&
                                    r.InternshipName.Equals(internship.InternshipName, StringComparison.OrdinalIgnoreCase);
                                bool realizationContainsRequirement = r.InternshipName != null &&
                                    r.InternshipName.Contains(internship.InternshipName, StringComparison.OrdinalIgnoreCase);
                                bool requirementContainsRealization = internship.InternshipName != null &&
                                    internship.InternshipName.Contains(r.InternshipName, StringComparison.OrdinalIgnoreCase);

                                string cleanRealizationName = realizationName
                                    .Replace(" ", "").Replace("-", "").Replace("_", "").ToLowerInvariant();
                                string cleanRequirementName = requirementName
                                    .Replace(" ", "").Replace("-", "").Replace("_", "").ToLowerInvariant();
                                bool fuzzyMatch = cleanRealizationName.Contains(cleanRequirementName) ||
                                                cleanRequirementName.Contains(cleanRealizationName);

                                bool matches = exactMatch || realizationContainsRequirement || requirementContainsRealization || fuzzyMatch;

                                return matches;
                            })
                            .ToList();

                        var viewModel = new InternshipStageViewModel(
                            internship,
                            null, // Empty for old SMK
                            realizationsForThisInternship,
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

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
        private int batchSize = 3; // Ilość elementów ładowanych w jednej partii

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

            // Inicjalizacja komend
            this.RefreshCommand = new AsyncRelayCommand(this.LoadDataAsync);
            this.SelectModuleCommand = new AsyncRelayCommand<string>(this.OnSelectModuleAsync);
            this.LoadMoreCommand = new AsyncRelayCommand(this.LoadMoreItemsAsync);

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
            System.Diagnostics.Debug.WriteLine("Rozpoczynam ładowanie danych procedur");

            try
            {
                // Pobierz bieżący moduł
                var currentModule = await this.specializationService.GetCurrentModuleAsync();
                if (currentModule == null)
                {
                    System.Diagnostics.Debug.WriteLine("Nie znaleziono aktualnego modułu");
                    return;
                }

                // Pobierz wymagania procedur dla bieżącego modułu
                var requirements = await this.procedureService.GetAvailableProcedureRequirementsAsync(currentModule.ModuleId);
                System.Diagnostics.Debug.WriteLine($"Pobrano {requirements.Count} wymagań procedur");

                // Czyść i wypełnij listę procedur
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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania danych: {ex.Message}");
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem podczas ładowania procedur.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        // Metoda do ładowania kolejnej partii procedur
        public async Task LoadMoreItemsAsync()
        {
            if (this.isLoadingData || this.allRequirements == null)
            {
                return;
            }

            this.isLoadingData = true;

            try
            {
                // Oblicz, ile elementów już załadowano
                int currentCount = this.ProcedureRequirements.Count;

                // Sprawdź, czy są jeszcze elementy do załadowania
                if (currentCount >= this.allRequirements.Count)
                {
                    return;
                }

                // Oblicz, ile elementów załadować w tej partii
                int itemsToLoad = Math.Min(this.batchSize, this.allRequirements.Count - currentCount);

                // Pobierz elementy do załadowania
                var requirementsToLoad = this.allRequirements.Skip(currentCount).Take(itemsToLoad).ToList();

                // Utworzenie ViewModels dla każdego wymagania i dodanie do kolekcji
                for (int i = 0; i < requirementsToLoad.Count; i++)
                {
                    var requirement = requirementsToLoad[i];

                    // Pobierz statystyki dla tego wymagania
                    var stats = await this.procedureService.GetProcedureSummaryAsync(
                        this.CurrentModuleId, requirement.Id);

                    // Utwórz ViewModel bez ładowania realizacji (będą ładowane na żądanie)
                    var viewModel = new ProcedureRequirementViewModel(
                        requirement,
                        stats,
                        new List<RealizedProcedureNewSMK>(),
                        currentCount + i + 1,
                        this.CurrentModuleId,
                        this.procedureService,
                        this.dialogService);

                    // Dodaj ViewModel do kolekcji na wątku UI
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        this.ProcedureRequirements.Add(viewModel);
                    });

                    // Krótka pauza między dodawaniem elementów, aby dać UI czas na oddychanie
                    await Task.Delay(50);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania dodatkowych elementów: {ex.Message}");
            }
            finally
            {
                this.isLoadingData = false;
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
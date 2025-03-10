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
    public class OldSMKProceduresListViewModel : BaseViewModel
    {
        private readonly IProcedureService procedureService;
        private readonly IAuthService authService;
        private readonly IDialogService dialogService;
        private readonly ISpecializationService specializationService;

        private ObservableCollection<ProcedureGroupViewModel> procedureGroups;
        private ProcedureSummary summary;
        private bool isRefreshing;
        private string moduleTitle;
        private bool hasTwoModules;
        private bool basicModuleSelected;
        private bool specialisticModuleSelected;
        private int currentModuleId;
        private bool isInitialLoad = true;
        private bool isLoadingData = false;

        public OldSMKProceduresListViewModel(
            IProcedureService procedureService,
            IAuthService authService,
            IDialogService dialogService,
            ISpecializationService specializationService)
        {
            this.procedureService = procedureService;
            this.authService = authService;
            this.dialogService = dialogService;
            this.specializationService = specializationService;

            this.Title = "Procedury (Stary SMK)";
            this.ProcedureGroups = new ObservableCollection<ProcedureGroupViewModel>();
            this.Summary = new ProcedureSummary();

            // Inicjalizacja komend
            this.RefreshCommand = new AsyncRelayCommand(this.LoadDataAsync);
            this.SelectModuleCommand = new AsyncRelayCommand<string>(this.OnSelectModuleAsync);
            this.AddProcedureCommand = new AsyncRelayCommand(this.AddProcedureAsync);

            // Zarejestruj na zdarzenie zmiany modułu
            this.specializationService.CurrentModuleChanged += this.OnModuleChanged;
        }

        public ObservableCollection<ProcedureGroupViewModel> ProcedureGroups
        {
            get => this.procedureGroups;
            set => this.SetProperty(ref this.procedureGroups, value);
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
        public ICommand AddProcedureCommand { get; }

        private async void OnModuleChanged(object sender, int moduleId)
        {
            this.CurrentModuleId = moduleId;
            await this.LoadDataAsync();
        }

        public async Task LoadDataAsync()
        {
            if (this.IsBusy || this.isLoadingData)
            {
                return;
            }

            this.isLoadingData = true;
            this.IsBusy = true;
            this.IsRefreshing = true;

            try
            {
                // Pobierz dane o specjalizacji i modułach - te operacje są szybkie i mogą być na głównym wątku
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

                    // Najpierw pokaż szybkie informacje podsumowujące, żeby UI było responsywne
                    this.Summary = await this.procedureService.GetProcedureSummaryAsync(currentModule.ModuleId);

                    // Wykonaj pozostałe operacje w tle
                    await Task.Run(async () =>
                    {
                        // Pobierz wymagania procedur dla bieżącego modułu
                        var requirements = await this.procedureService.GetAvailableProcedureRequirementsAsync(currentModule.ModuleId);

                        // Utwórz nową kolekcję, żeby nie modyfikować tej, która jest wyświetlana
                        var newGroups = new List<ProcedureGroupViewModel>();

                        // Dodaj systematycznie grupy do kolekcji
                        foreach (var requirement in requirements)
                        {
                            // Pobierz statystyki dla tego wymagania
                            var stats = await this.procedureService.GetProcedureSummaryAsync(currentModule.ModuleId, requirement.Id);

                            // Tworzenie ViewModelu dla grupy procedur - ale bez ładowania procedur od razu
                            var groupViewModel = new ProcedureGroupViewModel(
                                requirement,
                                new List<RealizedProcedureOldSMK>(), // Pusta lista - procedury załadowane zostaną później
                                stats,
                                this.procedureService,
                                this.dialogService);

                            newGroups.Add(groupViewModel);
                        }

                        // Aktualizuj UI na głównym wątku
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            // Czyścimy całą listę naraz - jedna duża operacja zamiast wielu małych
                            this.ProcedureGroups.Clear();

                            // Dodajemy wszystkie grupy za jednym razem
                            foreach (var group in newGroups)
                            {
                                this.ProcedureGroups.Add(group);
                            }
                        });
                    });
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
                this.isLoadingData = false;
                this.IsBusy = false;
                this.IsRefreshing = false;
                this.isInitialLoad = false;
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

        private async Task AddProcedureAsync()
        {
            try
            {
                await Shell.Current.GoToAsync("AddEditOldSMKProcedure");
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

        public void Dispose()
        {
            // Odłącz obserwator zdarzenia zmiany modułu
            this.specializationService.CurrentModuleChanged -= this.OnModuleChanged;
        }
    }
}
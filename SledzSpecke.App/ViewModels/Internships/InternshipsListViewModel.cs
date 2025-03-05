using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Internships
{
    public class InternshipsListViewModel : BaseViewModel
    {
        private readonly ISpecializationService specializationService;
        private readonly IDatabaseService databaseService;
        private readonly IDialogService dialogService;

        // Filtry
        private string searchText = string.Empty;
        private bool showOnlyActive = true;
        private bool showBasicModule = false;
        private bool showSpecialisticModule = false;
        private Internship selectedInternship;

        // Dla starego SMK
        private bool showPartialOnly = false;
        private bool isOldSmkVersion = false;

        // Dane
        private ObservableCollection<InternshipViewModel> internships;
        private int? currentModuleId;
        private bool hasModules;
        private string moduleInfo = string.Empty;
        private Module currentModule;

        public InternshipsListViewModel(
            ISpecializationService specializationService,
            IDatabaseService databaseService,
            IDialogService dialogService)
        {
            this.specializationService = specializationService;
            this.databaseService = databaseService;
            this.dialogService = dialogService;

            // Inicjalizacja komend
            this.RefreshCommand = new AsyncRelayCommand(this.LoadInternshipsAsync);
            this.FilterCommand = new AsyncRelayCommand(this.ApplyFiltersAsync);
            this.InternshipSelectedCommand = new AsyncRelayCommand(this.OnInternshipSelectedAsync);
            this.AddInternshipCommand = new AsyncRelayCommand(this.OnAddInternshipAsync);
            this.ToggleActiveCommand = new RelayCommand(this.OnToggleActive);
            this.TogglePartialCommand = new RelayCommand(this.OnTogglePartial);
            this.SelectModuleCommand = new AsyncRelayCommand<string>(this.OnSelectModuleAsync);

            // Inicjalizacja właściwości
            this.Title = "Staże specjalizacyjne";
            this.Internships = new ObservableCollection<InternshipViewModel>();

            // Sprawdzenie wersji SMK
            this.CheckSmkVersionAsync().ConfigureAwait(false);

            // Wczytanie danych
            this.LoadInternshipsAsync().ConfigureAwait(false);
        }

        // Właściwości
        public ObservableCollection<InternshipViewModel> Internships
        {
            get => this.internships;
            set => this.SetProperty(ref this.internships, value);
        }

        public Internship SelectedInternship
        {
            get => this.selectedInternship;
            set => this.SetProperty(ref this.selectedInternship, value);
        }

        public string SearchText
        {
            get => this.searchText;
            set => this.SetProperty(ref this.searchText, value);
        }

        public bool ShowOnlyActive
        {
            get => this.showOnlyActive;
            set => this.SetProperty(ref this.showOnlyActive, value);
        }

        public bool ShowPartialOnly
        {
            get => this.showPartialOnly;
            set => this.SetProperty(ref this.showPartialOnly, value);
        }

        public bool IsOldSmkVersion
        {
            get => this.isOldSmkVersion;
            set => this.SetProperty(ref this.isOldSmkVersion, value);
        }

        public bool HasModules
        {
            get => this.hasModules;
            set => this.SetProperty(ref this.hasModules, value);
        }

        public string ModuleInfo
        {
            get => this.moduleInfo;
            set => this.SetProperty(ref this.moduleInfo, value);
        }

        public bool ShowBasicModule
        {
            get => this.showBasicModule;
            set => this.SetProperty(ref this.showBasicModule, value);
        }

        public bool ShowSpecialisticModule
        {
            get => this.showSpecialisticModule;
            set => this.SetProperty(ref this.showSpecialisticModule, value);
        }

        // Komendy
        public ICommand RefreshCommand { get; }
        public ICommand FilterCommand { get; }
        public ICommand InternshipSelectedCommand { get; }
        public ICommand AddInternshipCommand { get; }
        public ICommand ToggleActiveCommand { get; }
        public ICommand TogglePartialCommand { get; }
        public ICommand SelectModuleCommand { get; }

        // Metody
        private async Task CheckSmkVersionAsync()
        {
            try
            {
                var user = await this.databaseService.GetCurrentUserAsync();
                this.IsOldSmkVersion = user?.SmkVersion == SmkVersion.Old;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas sprawdzania wersji SMK: {ex.Message}");
            }
        }

        private async Task LoadInternshipsAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Pobierz bieżącą specjalizację
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie znaleziono aktywnej specjalizacji.",
                        "OK");
                    return;
                }

                // Aktualizacja flagi modułów
                this.HasModules = specialization.HasModules;

                // Ustaw bieżący moduł, jeśli specjalizacja ma moduły
                if (specialization.HasModules && specialization.CurrentModuleId.HasValue)
                {
                    this.currentModuleId = specialization.CurrentModuleId.Value;
                    var module = await this.databaseService.GetModuleAsync(this.currentModuleId.Value);
                    if (module != null)
                    {
                        this.currentModule = module;

                        // Ustaw stan przycisków wyboru modułu
                        this.ShowBasicModule = module.Type == ModuleType.Basic;
                        this.ShowSpecialisticModule = module.Type == ModuleType.Specialistic;

                        this.ModuleInfo = $"Staże dla modułu: {module.Name}";
                    }
                }
                else
                {
                    this.currentModuleId = null;
                    this.currentModule = null;
                    this.ShowBasicModule = false;
                    this.ShowSpecialisticModule = false;
                    this.ModuleInfo = string.Empty;
                }

                // Wyczyść istniejące staże
                this.Internships.Clear();

                // Pobierz staże dla bieżącego modułu lub całej specjalizacji
                var items = await this.databaseService.GetInternshipsAsync(
                    specializationId: specialization.SpecializationId,
                    moduleId: this.currentModuleId);

                // Filtrowanie według wyszukiwanego tekstu
                if (!string.IsNullOrEmpty(this.SearchText))
                {
                    var searchLower = this.SearchText.ToLowerInvariant();
                    items = items.Where(i =>
                        i.InternshipName.ToLowerInvariant().Contains(searchLower) ||
                        i.InstitutionName.ToLowerInvariant().Contains(searchLower) ||
                        (i.DepartmentName != null && i.DepartmentName.ToLowerInvariant().Contains(searchLower)))
                        .ToList();
                }

                // Filtrowanie według statusu (aktywne/wszystkie)
                if (this.ShowOnlyActive)
                {
                    items = items.Where(i => !i.IsCompleted).ToList();
                }

                // Filtrowanie po realizacji częściowej (tylko dla starego SMK)
                if (this.IsOldSmkVersion && this.ShowPartialOnly)
                {
                    items = items.Where(i => i.IsPartialRealization).ToList();
                }

                // Sortowanie staży - najpierw według roku, potem według daty rozpoczęcia (od najnowszych)
                var sortedItems = items
                    .OrderBy(i => i.Year)
                    .ThenByDescending(i => i.StartDate)
                    .ToList();

                // Dodaj staże do kolekcji
                foreach (var item in sortedItems)
                {
                    // Utwórz model widoku z uwzględnieniem wersji SMK
                    var viewModel = InternshipViewModel.FromModel(
                        item,
                        moduleName: string.Empty,
                        moduleType: ModuleType.Basic,
                        isOldSmk: this.IsOldSmkVersion);

                    // Pobierz nazwę modułu, jeśli dostępna
                    if (this.HasModules && item.ModuleId.HasValue)
                    {
                        var module = await this.databaseService.GetModuleAsync(item.ModuleId.Value);
                        if (module != null)
                        {
                            viewModel.ModuleName = module.Name;
                            viewModel.ModuleType = module.Type;
                        }
                    }

                    this.Internships.Add(viewModel);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania staży: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie udało się załadować staży. Spróbuj ponownie.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task ApplyFiltersAsync()
        {
            await this.LoadInternshipsAsync();
        }

        private void OnToggleActive()
        {
            this.ShowOnlyActive = !this.ShowOnlyActive;
            this.ApplyFiltersAsync().ConfigureAwait(false);
        }

        private void OnTogglePartial()
        {
            this.ShowPartialOnly = !this.ShowPartialOnly;
            this.ApplyFiltersAsync().ConfigureAwait(false);
        }

        private async Task OnSelectModuleAsync(string moduleType)
        {
            if (!this.HasModules)
            {
                return;
            }

            try
            {
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null || !specialization.HasModules)
                {
                    return;
                }

                var modules = await this.databaseService.GetModulesAsync(specialization.SpecializationId);

                if (moduleType == "Basic")
                {
                    var basicModule = modules.FirstOrDefault(m => m.Type == ModuleType.Basic);
                    if (basicModule != null)
                    {
                        this.currentModuleId = basicModule.ModuleId;
                        this.ShowBasicModule = true;
                        this.ShowSpecialisticModule = false;

                        // Ustaw bieżący moduł w specjalizacji
                        if (specialization.CurrentModuleId != this.currentModuleId)
                        {
                            specialization.CurrentModuleId = this.currentModuleId;
                            await this.databaseService.UpdateSpecializationAsync(specialization);
                        }
                    }
                }
                else if (moduleType == "Specialistic")
                {
                    var specialisticModule = modules.FirstOrDefault(m => m.Type == ModuleType.Specialistic);
                    if (specialisticModule != null)
                    {
                        this.currentModuleId = specialisticModule.ModuleId;
                        this.ShowBasicModule = false;
                        this.ShowSpecialisticModule = true;

                        // Ustaw bieżący moduł w specjalizacji
                        if (specialization.CurrentModuleId != this.currentModuleId)
                        {
                            specialization.CurrentModuleId = this.currentModuleId;
                            await this.databaseService.UpdateSpecializationAsync(specialization);
                        }
                    }
                }

                // Zapisz wybrany moduł w ustawieniach
                await Helpers.SettingsHelper.SetCurrentModuleIdAsync(this.currentModuleId.GetValueOrDefault());

                // Odśwież dane
                await this.LoadInternshipsAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas zmiany modułu: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem podczas przełączania modułu. Spróbuj ponownie.",
                    "OK");
            }
        }

        private async Task OnInternshipSelectedAsync()
        {
            if (this.SelectedInternship == null)
            {
                return;
            }

            // Nawigacja do strony szczegółów
            await Shell.Current.GoToAsync($"InternshipDetails?internshipId={this.SelectedInternship.InternshipId}");

            // Resetuj zaznaczenie
            this.SelectedInternship = null;
        }

        private async Task OnAddInternshipAsync()
        {
            // Pobierz identyfikator bieżącego modułu, aby przekazać go do strony dodawania
            var specialization = await this.specializationService.GetCurrentSpecializationAsync();
            int? moduleId = null;

            if (specialization != null && specialization.HasModules && specialization.CurrentModuleId.HasValue)
            {
                moduleId = specialization.CurrentModuleId.Value;
            }

            // Nawigacja do strony dodawania
            if (moduleId.HasValue)
            {
                await Shell.Current.GoToAsync($"AddEditInternship?moduleId={moduleId.Value}");
            }
            else
            {
                await Shell.Current.GoToAsync("AddEditInternship");
            }
        }
    }
}
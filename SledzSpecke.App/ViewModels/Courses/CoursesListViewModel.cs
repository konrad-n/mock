using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Courses
{
    public class CoursesListViewModel : BaseViewModel
    {
        private readonly ISpecializationService specializationService;
        private readonly IDatabaseService databaseService;
        private readonly IDialogService dialogService;

        // Filtry
        private string searchText = string.Empty;
        private Course selectedCourse;

        // Moduły
        private bool hasModules;
        private bool showBasicModule;
        private bool showSpecialisticModule;
        private string moduleInfo = string.Empty;
        private int? currentModuleId;
        private Module currentModule;

        // Dane
        private ObservableCollection<CourseViewModel> courses;

        public CoursesListViewModel(
            ISpecializationService specializationService,
            IDatabaseService databaseService,
            IDialogService dialogService)
        {
            this.specializationService = specializationService;
            this.databaseService = databaseService;
            this.dialogService = dialogService;

            // Inicjalizacja komend
            this.RefreshCommand = new AsyncRelayCommand(this.LoadCoursesAsync);
            this.FilterCommand = new AsyncRelayCommand(this.ApplyFiltersAsync);
            this.CourseSelectedCommand = new AsyncRelayCommand(this.OnCourseSelectedAsync);
            this.AddCourseCommand = new AsyncRelayCommand(this.OnAddCourseAsync);
            this.SelectModuleCommand = new AsyncRelayCommand<string>(this.OnSelectModuleAsync);

            // Inicjalizacja właściwości
            this.Title = "Kursy specjalizacyjne";
            this.Courses = new ObservableCollection<CourseViewModel>();

            // Wczytanie danych
            this.LoadCoursesAsync().ConfigureAwait(false);
        }

        // Właściwości
        public ObservableCollection<CourseViewModel> Courses
        {
            get => this.courses;
            set => this.SetProperty(ref this.courses, value);
        }

        public Course SelectedCourse
        {
            get => this.selectedCourse;
            set => this.SetProperty(ref this.selectedCourse, value);
        }

        public string SearchText
        {
            get => this.searchText;
            set => this.SetProperty(ref this.searchText, value);
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

        public ICommand CourseSelectedCommand { get; }

        public ICommand AddCourseCommand { get; }

        public ICommand SelectModuleCommand { get; }

        // Metody
        private async Task LoadCoursesAsync()
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

                        this.ModuleInfo = $"Kursy dla modułu: {module.Name}";
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

                // Wyczyść istniejące kursy
                this.Courses.Clear();

                // Pobierz kursy dla bieżącego modułu lub całej specjalizacji
                var items = await this.databaseService.GetCoursesAsync(
                    specializationId: specialization.SpecializationId,
                    moduleId: this.currentModuleId);

                // Filtrowanie według wyszukiwanego tekstu
                if (!string.IsNullOrEmpty(this.SearchText))
                {
                    var searchLower = this.SearchText.ToLowerInvariant();
                    items = items.Where(c =>
                        c.CourseName.ToLowerInvariant().Contains(searchLower) ||
                        c.CourseType.ToLowerInvariant().Contains(searchLower) ||
                        c.InstitutionName.ToLowerInvariant().Contains(searchLower))
                        .ToList();
                }

                // Sortowanie kursów - najpierw według roku, potem według daty ukończenia (od najnowszych)
                var sortedItems = items
                    .OrderBy(c => c.Year)
                    .ThenByDescending(c => c.CompletionDate)
                    .ToList();

                // Dodaj kursy do kolekcji
                foreach (var item in sortedItems)
                {
                    var viewModel = CourseViewModel.FromModel(item);

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

                    this.Courses.Add(viewModel);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania kursów: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie udało się załadować kursów. Spróbuj ponownie.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task ApplyFiltersAsync()
        {
            await this.LoadCoursesAsync();
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
                await this.LoadCoursesAsync();
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

        private async Task OnCourseSelectedAsync()
        {
            if (this.SelectedCourse == null)
            {
                return;
            }

            // Nawigacja do strony szczegółów
            await Shell.Current.GoToAsync($"CourseDetails?courseId={this.SelectedCourse.CourseId}");

            // Resetuj zaznaczenie
            this.SelectedCourse = null;
        }

        private async Task OnAddCourseAsync()
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
                await Shell.Current.GoToAsync($"AddEditCourse?moduleId={moduleId.Value}");
            }
            else
            {
                await Shell.Current.GoToAsync("AddEditCourse");
            }
        }
    }
}
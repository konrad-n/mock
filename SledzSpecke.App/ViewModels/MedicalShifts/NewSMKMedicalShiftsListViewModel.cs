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

        // Dodane właściwości do obsługi przełączania modułów
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

            // Inicjalizacja komend
            this.RefreshCommand = new AsyncRelayCommand(this.LoadDataAsync);
            this.SelectModuleCommand = new AsyncRelayCommand<string>(this.OnSelectModuleAsync);

            // Załaduj dane
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

        // Dodane właściwości dla przełączania modułów
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
            try
            {
                System.Diagnostics.Debug.WriteLine($"NewSMKMedicalShiftsListViewModel: Wybrano typ modułu: {moduleType}");

                // Pobierz specjalizację
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    return;
                }

                // Pobierz moduły
                var modules = await this.specializationService.GetModulesAsync(specialization.SpecializationId);

                int? newModuleId = null;

                if (moduleType == "Basic")
                {
                    var basicModule = modules.FirstOrDefault(m => m.Type == ModuleType.Basic);
                    if (basicModule != null)
                    {
                        newModuleId = basicModule.ModuleId;
                        System.Diagnostics.Debug.WriteLine($"NewSMKMedicalShiftsListViewModel: Znaleziono moduł podstawowy, ID={newModuleId}");
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
                        System.Diagnostics.Debug.WriteLine($"NewSMKMedicalShiftsListViewModel: Znaleziono moduł specjalistyczny, ID={newModuleId}");
                        await this.specializationService.SetCurrentModuleAsync(specialisticModule.ModuleId);
                        this.BasicModuleSelected = false;
                        this.SpecialisticModuleSelected = true;
                    }
                }

                // Odśwież dane
                System.Diagnostics.Debug.WriteLine($"NewSMKMedicalShiftsListViewModel: Odświeżanie danych po zmianie modułu na ID={newModuleId}");
                await this.LoadDataAsync();
                System.Diagnostics.Debug.WriteLine("NewSMKMedicalShiftsListViewModel: Dane odświeżone po zmianie modułu");
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas zmiany modułu: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem podczas zmiany modułu. Spróbuj ponownie.",
                    "OK");
            }
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

            try
            {
                // Pobierz specjalizację
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie znaleziono aktywnej specjalizacji.",
                        "OK");
                    return;
                }

                // Sprawdź, czy specjalizacja ma moduł podstawowy i specjalistyczny
                var modules = await this.specializationService.GetModulesAsync(specialization.SpecializationId);
                this.HasTwoModules = modules.Any(m => m.Type == ModuleType.Basic);

                // Pobierz bieżący moduł
                var currentModule = await this.specializationService.GetCurrentModuleAsync();
                if (currentModule != null)
                {
                    this.ModuleTitle = currentModule.Name;

                    // Ustaw właściwości wyboru modułu
                    this.BasicModuleSelected = currentModule.Type == ModuleType.Basic;
                    this.SpecialisticModuleSelected = currentModule.Type == ModuleType.Specialistic;
                }

                // Pobierz dostępne wymagania stażowe
                var requirements = await this.medicalShiftsService.GetAvailableInternshipRequirementsAsync();

                // Utworzenie ViewModeli dla każdego wymagania stażowego
                var viewModels = new List<InternshipRequirementViewModel>();

                foreach (var requirement in requirements)
                {
                    // Pobierz podsumowanie dyżurów dla tego wymagania
                    var summary = await this.medicalShiftsService.GetShiftsSummaryAsync(internshipRequirementId: requirement.Id);

                    // Pobierz wszystkie dyżury dla tego wymagania
                    var shifts = await this.medicalShiftsService.GetNewSMKShiftsAsync(requirement.Id);

                    // Utwórz ViewModel, przekazując ID bieżącego modułu
                    var viewModel = new InternshipRequirementViewModel(
                        requirement,
                        summary,
                        shifts,
                        this.medicalShiftsService,
                        this.dialogService,
                        currentModule?.ModuleId); // Przekazujemy ID modułu

                    viewModels.Add(viewModel);
                }

                // Zaktualizuj kolekcję
                this.InternshipRequirements.Clear();
                foreach (var viewModel in viewModels)
                {
                    this.InternshipRequirements.Add(viewModel);
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania wymagań stażowych: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem podczas ładowania danych dyżurów.",
                    "OK");
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
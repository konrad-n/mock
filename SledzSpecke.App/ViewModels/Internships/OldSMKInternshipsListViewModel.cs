using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
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

        public OldSMKInternshipsListViewModel(
            ISpecializationService specializationService,
            IDialogService dialogService,
            IAuthService authService)
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
                    this.BasicModuleSelected = true;
                    this.SpecialisticModuleSelected = false;
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

            try
            {
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

                // Pobierz wymagania stażowe dla aktualnego modułu
                var internships = await this.specializationService.GetInternshipsAsync(currentModule?.ModuleId);

                // Dodaj debugowanie
                System.Diagnostics.Debug.WriteLine($"Pobrano {internships.Count} wymagań stażowych");

                // Określenie zakresu lat dla starego SMK na podstawie typu modułu
                int startYear = 1;
                int endYear = 2;

                if (currentModule != null && currentModule.Type == ModuleType.Specialistic)
                {
                    startYear = 3;
                    endYear = 6; // Typowy zakres lat dla modułu specjalistycznego
                }

                // Pobierz wszystkie realizacje staży dla lat odpowiadających aktualnemu modułowi
                var allRealizedInternships = new List<RealizedInternshipOldSMK>();
                for (int year = startYear; year <= endYear; year++)
                {
                    var yearRealizations = await this.specializationService.GetRealizedInternshipsOldSMKAsync(year);
                    allRealizedInternships.AddRange(yearRealizations);
                }

                var viewModels = new List<InternshipStageViewModel>();
                foreach (var internship in internships)
                {
                    System.Diagnostics.Debug.WriteLine($"Wymaganie stażowe: {internship.InternshipName}");

                    // Wypisz wszystkie dostępne realizacje dla debugowania
                    foreach (var r in allRealizedInternships)
                    {
                        System.Diagnostics.Debug.WriteLine($"Dostępna realizacja: {r.InternshipName}, Dni: {r.DaysCount}, Rok: {r.Year}");
                    }

                    // Filtruj realizacje dla tego konkretnego stażu po nazwie
                    var realizationsForThisInternship = allRealizedInternships
                        .Where(r => {
                            // Jeśli nazwa jest pusta lub "Staż bez nazwy", próbujemy dopasować ją z innymi danymi
                            if (string.IsNullOrEmpty(r.InternshipName) || r.InternshipName == "Staż bez nazwy")
                            {
                                // Logujemy potencjalne dopasowanie
                                System.Diagnostics.Debug.WriteLine($"Próba dopasowania realizacji bez nazwy do: {internship.InternshipName}");

                                // Przypisujemy wszystkie realizacje bez nazwy do pierwszego stażu
                                // To rozwiązanie tymczasowe, aby pokazać dane
                                if (internships.IndexOf(internship) == 0)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Dopasowano realizację bez nazwy do pierwszego stażu: {internship.InternshipName}");
                                    return true;
                                }
                                return false;
                            }

                            // Standardowe porównanie nazw
                            return r.InternshipName != null &&
                                   (r.InternshipName.Equals(internship.InternshipName, StringComparison.OrdinalIgnoreCase) ||
                                    r.InternshipName.Contains(internship.InternshipName) ||
                                    internship.InternshipName.Contains(r.InternshipName));
                        })
                        .ToList();

                    System.Diagnostics.Debug.WriteLine($"Znaleziono {realizationsForThisInternship.Count} realizacji dla stażu {internship.InternshipName}");
                    foreach (var realization in realizationsForThisInternship)
                    {
                        System.Diagnostics.Debug.WriteLine($"  Realizacja: {realization.InternshipName}, Dni: {realization.DaysCount}, Rok: {realization.Year}");
                    }

                    var viewModel = new InternshipStageViewModel(
                        internship,
                        null, // Puste dla starego SMK
                        realizationsForThisInternship,
                        this.specializationService,
                        this.dialogService,
                        this.authService,
                        currentModule?.ModuleId);

                    viewModels.Add(viewModel);
                }

                this.InternshipRequirements.Clear();
                foreach (var viewModel in viewModels)
                {
                    this.InternshipRequirements.Add(viewModel);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania danych: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił błąd podczas ładowania danych. Spróbuj ponownie.",
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
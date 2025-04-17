using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Database;
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
            if (this.IsBusy || this.isLoading)
            {
                return;
            }

            this.isLoading = true;
            this.IsBusy = true;
            this.IsRefreshing = true;

            try
            {
                System.Diagnostics.Debug.WriteLine("===== ROZPOCZYNAM ŁADOWANIE DANYCH STAŻÓW OLD SMK =====");

                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie znaleziono aktywnej specjalizacji.",
                        "OK");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"Aktualna specjalizacja: {specialization.Name}, ID: {specialization.SpecializationId}");

                var modules = await this.specializationService.GetModulesAsync(specialization.SpecializationId);
                this.HasTwoModules = modules.Any(m => m.Type == ModuleType.Basic);
                var currentModule = await this.specializationService.GetCurrentModuleAsync();

                if (currentModule != null)
                {
                    this.ModuleTitle = currentModule.Name;
                    this.BasicModuleSelected = currentModule.Type == ModuleType.Basic;
                    this.SpecialisticModuleSelected = currentModule.Type == ModuleType.Specialistic;
                    System.Diagnostics.Debug.WriteLine($"Aktualny moduł: {currentModule.Name}, ID: {currentModule.ModuleId}, Typ: {currentModule.Type}");
                }

                // Pobierz wymagania stażowe dla aktualnego modułu
                var internships = await this.specializationService.GetInternshipsAsync(currentModule?.ModuleId);

                // Dodaj debugowanie
                System.Diagnostics.Debug.WriteLine($"Pobrano {internships.Count} wymagań stażowych");
                foreach (var i in internships)
                {
                    System.Diagnostics.Debug.WriteLine($"  Wymaganie: ID={i.InternshipId}, Nazwa={i.InternshipName}, Dni={i.DaysCount}");
                }

                // Określenie zakresu lat dla starego SMK na podstawie typu modułu
                int startYear = 1;
                int endYear = 2;

                if (currentModule != null && currentModule.Type == ModuleType.Specialistic)
                {
                    startYear = 3;
                    endYear = 6; // Typowy zakres lat dla modułu specjalistycznego
                }

                System.Diagnostics.Debug.WriteLine($"Zakres lat dla modułu: {startYear}-{endYear}");

                // NOWOŚĆ: Pobierz wszystkie realizacje bezpośrednio z bazy danych, również te z Year=0
                var dbService = IPlatformApplication.Current.Services.GetRequiredService<SledzSpecke.App.Services.Database.IDatabaseService>();
                var allDbRealizations = await dbService.QueryAsync<RealizedInternshipOldSMK>(
                    "SELECT * FROM RealizedInternshipOldSMK WHERE SpecializationId = ?", specialization.SpecializationId);

                System.Diagnostics.Debug.WriteLine($"WSZYSTKIE REALIZACJE W BAZIE DANYCH: {allDbRealizations.Count}");
                foreach (var r in allDbRealizations)
                {
                    System.Diagnostics.Debug.WriteLine($"  DB: ID={r.RealizedInternshipId}, Nazwa={r.InternshipName}, " +
                        $"Dni={r.DaysCount}, Rok={r.Year}, SpecID={r.SpecializationId}");
                }

                // Pobierz wszystkie realizacje staży dla lat odpowiadających aktualnemu modułowi + realizacje z Year=0
                var allRealizedInternships = new List<RealizedInternshipOldSMK>();

                // Najpierw dodaj realizacje z Year=0, które nie są przypisane do konkretnego roku
                var yearZeroRealizations = await dbService.QueryAsync<RealizedInternshipOldSMK>(
                    "SELECT * FROM RealizedInternshipOldSMK WHERE SpecializationId = ? AND Year = 0",
                    specialization.SpecializationId);

                System.Diagnostics.Debug.WriteLine($"Rok 0 (nieprzypisane): znaleziono {yearZeroRealizations.Count} realizacji");
                foreach (var r in yearZeroRealizations)
                {
                    System.Diagnostics.Debug.WriteLine($"  Rok 0 - Realizacja: ID={r.RealizedInternshipId}, " +
                        $"Nazwa={r.InternshipName}, Rok={r.Year}, Dni={r.DaysCount}");

                    // Dodaj realizacje z Year=0 do listy, bo mogą należeć do aktualnego modułu
                    // Będą widoczne dla wszystkich modułów
                    allRealizedInternships.Add(r);
                }

                // Następnie dodaj realizacje z odpowiednimi latami dla aktualnego modułu
                for (int year = startYear; year <= endYear; year++)
                {
                    var yearRealizations = await this.specializationService.GetRealizedInternshipsOldSMKAsync(year);
                    System.Diagnostics.Debug.WriteLine($"Rok {year}: znaleziono {yearRealizations.Count} realizacji");

                    foreach (var r in yearRealizations.Where(r => r.Year == year)) // Filtruj dokładnie po roku (bez Year=0)
                    {
                        System.Diagnostics.Debug.WriteLine($"  Rok {year} - Realizacja: ID={r.RealizedInternshipId}, " +
                            $"Nazwa={r.InternshipName}, Rok={r.Year}, Dni={r.DaysCount}");
                    }

                    allRealizedInternships.AddRange(yearRealizations.Where(r => r.Year == year));
                }

                // Sprawdź wszystkie realizacje przed filtrowaniem
                System.Diagnostics.Debug.WriteLine($"WSZYSTKIE REALIZACJE (Przed filtrowaniem): {allRealizedInternships.Count}");
                foreach (var r in allRealizedInternships)
                {
                    System.Diagnostics.Debug.WriteLine($"  Realizacja: ID={r.RealizedInternshipId}, Nazwa={r.InternshipName}, " +
                        $"Dni={r.DaysCount}, Rok={r.Year}, SpecID={r.SpecializationId}");
                }

                var viewModels = new List<InternshipStageViewModel>();
                foreach (var internship in internships)
                {
                    System.Diagnostics.Debug.WriteLine($"Przetwarzanie wymagania stażowego: {internship.InternshipName}");

                    // Filtruj realizacje dla tego konkretnego stażu po nazwie - z rozszerzonym logowaniem
                    var realizationsForThisInternship = allRealizedInternships
                        .Where(r => {
                            // Przygotuj nazwy do lepszego logowania
                            string realizationName = r.InternshipName ?? "null";
                            string requirementName = internship.InternshipName ?? "null";

                            // Jeśli nazwa jest pusta lub "Staż bez nazwy", próbujemy dopasować ją z innymi danymi
                            if (string.IsNullOrEmpty(r.InternshipName) || r.InternshipName == "Staż bez nazwy")
                            {
                                // Logujemy potencjalne dopasowanie
                                System.Diagnostics.Debug.WriteLine($"  Próba dopasowania realizacji bez nazwy do: {requirementName}");

                                // Przypisujemy wszystkie realizacje bez nazwy do pierwszego stażu
                                // To rozwiązanie tymczasowe, aby pokazać dane
                                if (internships.IndexOf(internship) == 0)
                                {
                                    System.Diagnostics.Debug.WriteLine($"  Dopasowano realizację bez nazwy do pierwszego stażu: {requirementName}");
                                    return true;
                                }
                                return false;
                            }

                            // Standardowe porównanie nazw z logowaniem każdej próby
                            bool exactMatch = r.InternshipName != null &&
                                r.InternshipName.Equals(internship.InternshipName, StringComparison.OrdinalIgnoreCase);
                            bool realizationContainsRequirement = r.InternshipName != null &&
                                r.InternshipName.Contains(internship.InternshipName, StringComparison.OrdinalIgnoreCase);
                            bool requirementContainsRealization = internship.InternshipName != null &&
                                internship.InternshipName.Contains(r.InternshipName, StringComparison.OrdinalIgnoreCase);

                            // Dodatkowe porównanie: usuń spacje i znaki specjalne
                            string cleanRealizationName = realizationName
                                .Replace(" ", "").Replace("-", "").Replace("_", "").ToLowerInvariant();
                            string cleanRequirementName = requirementName
                                .Replace(" ", "").Replace("-", "").Replace("_", "").ToLowerInvariant();
                            bool fuzzyMatch = cleanRealizationName.Contains(cleanRequirementName) ||
                                             cleanRequirementName.Contains(cleanRealizationName);

                            bool matches = exactMatch || realizationContainsRequirement || requirementContainsRealization || fuzzyMatch;

                            System.Diagnostics.Debug.WriteLine($"  Porównanie: '{realizationName}' do '{requirementName}'");
                            System.Diagnostics.Debug.WriteLine($"    Dokładne dopasowanie: {exactMatch}");
                            System.Diagnostics.Debug.WriteLine($"    Realizacja zawiera wymaganie: {realizationContainsRequirement}");
                            System.Diagnostics.Debug.WriteLine($"    Wymaganie zawiera realizację: {requirementContainsRealization}");
                            System.Diagnostics.Debug.WriteLine($"    Rozmyte dopasowanie: {fuzzyMatch}");
                            System.Diagnostics.Debug.WriteLine($"    OSTATECZNY WYNIK: {matches}");

                            return matches;
                        })
                        .ToList();

                    System.Diagnostics.Debug.WriteLine($"Znaleziono {realizationsForThisInternship.Count} realizacji dla stażu {internship.InternshipName}");
                    foreach (var realization in realizationsForThisInternship)
                    {
                        System.Diagnostics.Debug.WriteLine($"  Dopasowana realizacja: {realization.InternshipName}, Dni: {realization.DaysCount}, " +
                            $"Rok: {realization.Year}, ID: {realization.RealizedInternshipId}");
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

                System.Diagnostics.Debug.WriteLine("===== ZAKOŃCZONO ŁADOWANIE DANYCH STAŻÓW OLD SMK =====");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BŁĄD podczas ładowania danych: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił błąd podczas ładowania danych. Spróbuj ponownie.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
                this.IsRefreshing = false;
                this.isLoading = false;
            }
        }
    }
}
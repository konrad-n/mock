using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Specializations
{
    /// <summary>
    /// ViewModel do inicjalizacji nowej specjalizacji z modułami.
    /// </summary>
    public class InitializeSpecializationViewModel : BaseViewModel
    {
        private readonly IDatabaseService databaseService;
        private readonly IDialogService dialogService;

        private ObservableCollection<SpecializationProgram> availableSpecializations;
        private SpecializationProgram selectedSpecialization;
        private SmkVersion smkVersion;
        private DateTime startDate = DateTime.Now;
        private bool isInitializing;
        private bool hasModules;
        private string statusMessage = string.Empty;

        public InitializeSpecializationViewModel(
            IDatabaseService databaseService,
            IDialogService dialogService)
        {
            this.databaseService = databaseService;
            this.dialogService = dialogService;

            // Inicjalizacja komend
            InitializeCommand = new AsyncRelayCommand(OnInitializeAsync, () => selectedSpecialization != null);
            CancelCommand = new AsyncRelayCommand(OnCancelAsync);

            // Inicjalizacja właściwości
            Title = "Inicjalizacja specjalizacji";
            AvailableSpecializations = new ObservableCollection<SpecializationProgram>();

            // Wczytanie dostępnych specjalizacji
            LoadSpecializationsAsync().ConfigureAwait(false);
        }

        // Właściwości
        public ObservableCollection<SpecializationProgram> AvailableSpecializations
        {
            get => availableSpecializations;
            set => SetProperty(ref availableSpecializations, value);
        }

        public SpecializationProgram SelectedSpecialization
        {
            get => selectedSpecialization;
            set
            {
                if (SetProperty(ref selectedSpecialization, value))
                {
                    HasModules = value?.HasModules ?? false;
                    ((AsyncRelayCommand)InitializeCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public SmkVersion SmkVersion
        {
            get => smkVersion;
            set => SetProperty(ref smkVersion, value);
        }

        public DateTime StartDate
        {
            get => startDate;
            set => SetProperty(ref startDate, value);
        }

        public bool IsInitializing
        {
            get => isInitializing;
            set => SetProperty(ref isInitializing, value);
        }

        public bool HasModules
        {
            get => hasModules;
            set => SetProperty(ref hasModules, value);
        }

        public string StatusMessage
        {
            get => statusMessage;
            set => SetProperty(ref statusMessage, value);
        }

        // Komendy
        public ICommand InitializeCommand { get; }
        public ICommand CancelCommand { get; }

        // Metody
        private async Task LoadSpecializationsAsync()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            try
            {
                // Pobierz wszystkie dostępne programy specjalizacji
                var programs = await databaseService.GetAllSpecializationProgramsAsync();

                AvailableSpecializations.Clear();

                foreach (var program in programs)
                {
                    AvailableSpecializations.Add(program);
                }

                // Jeśli nie ma dostępnych programów, wczytaj domyślne
                if (AvailableSpecializations.Count == 0)
                {
                    await LoadDefaultSpecializationsAsync();
                }

                // Wybierz pierwszą dostępną specjalizację
                if (AvailableSpecializations.Count > 0)
                {
                    SelectedSpecialization = AvailableSpecializations[0];
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd ładowania specjalizacji: {ex.Message}");
                StatusMessage = "Nie udało się załadować dostępnych specjalizacji.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadDefaultSpecializationsAsync()
        {
            try
            {
                // Wczytaj domyślne specjalizacje z zasobów aplikacji
                string[] codes = new[] { "internal_medicine", "cardiology", "psychiatry", "anesthesiology" };

                foreach (var code in codes)
                {
                    try
                    {
                        // Próba wczytania programu specjalizacji
                        var program = await SpecializationLoader.LoadSpecializationProgramAsync(code, SmkVersion);

                        if (program != null)
                        {
                            // Zapisz program w bazie danych
                            await databaseService.SaveSpecializationProgramAsync(program);

                            // Dodaj do listy dostępnych specjalizacji
                            AvailableSpecializations.Add(program);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Nie udało się załadować specjalizacji {code}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd ładowania domyślnych specjalizacji: {ex.Message}");
            }
        }

        private async Task OnInitializeAsync()
        {
            if (IsInitializing || SelectedSpecialization == null)
            {
                return;
            }

            IsInitializing = true;
            StatusMessage = "Inicjalizacja specjalizacji...";

            try
            {
                // Utwórz nową specjalizację
                var specialization = new Specialization
                {
                    Name = SelectedSpecialization.Name ?? string.Empty,
                    ProgramCode = SelectedSpecialization.Code ?? string.Empty,
                    StartDate = StartDate,
                    PlannedEndDate = CalculatePlannedEndDate(StartDate, SelectedSpecialization),
                    HasModules = SelectedSpecialization.HasModules,
                    ProgramStructure = SelectedSpecialization.Structure ?? string.Empty,
                };

                StatusMessage = "Zapisywanie specjalizacji...";

                // Zapisz specjalizację w bazie danych
                int specializationId = await databaseService.SaveSpecializationAsync(specialization);

                // Pobierz zapisaną specjalizację z ID
                specialization = await databaseService.GetSpecializationAsync(specializationId);

                if (specialization == null)
                {
                    throw new InvalidOperationException("Nie udało się zapisać specjalizacji.");
                }

                // Jeśli specjalizacja ma moduły, utwórz je
                if (specialization.HasModules)
                {
                    StatusMessage = "Tworzenie modułów...";

                    // Utwórz moduły specjalizacji
                    var modules = ModuleHelper.CreateModulesForSpecialization(
                        specialization.ProgramCode,
                        specialization.StartDate);

                    if (modules != null && modules.Count > 0)
                    {
                        // Ustaw ID specjalizacji dla modułów
                        foreach (var module in modules)
                        {
                            module.SpecializationId = specializationId;

                            // Zapisz moduł w bazie danych
                            await databaseService.SaveModuleAsync(module);
                        }

                        // Aktualizuj specjalizację o listę modułów
                        specialization.Modules = await databaseService.GetModulesAsync(specializationId);

                        // Ustaw pierwszy moduł jako aktualny
                        if (specialization.Modules.Count > 0)
                        {
                            specialization.CurrentModuleId = specialization.Modules[0].ModuleId;
                            await databaseService.UpdateSpecializationAsync(specialization);
                        }

                        // Inicjalizuj moduły z danymi z plików JSON
                        StatusMessage = "Inicjalizacja danych modułów...";
                    }
                }

                StatusMessage = "Specjalizacja została zainicjalizowana.";
                await dialogService.DisplayAlertAsync("Sukces", "Specjalizacja została pomyślnie zainicjalizowana.", "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd inicjalizacji specjalizacji: {ex.Message}");
                StatusMessage = "Nie udało się zainicjalizować specjalizacji.";
                await dialogService.DisplayAlertAsync("Błąd", "Nie udało się zainicjalizować specjalizacji.", "OK");
            }
            finally
            {
                IsInitializing = false;
            }
        }

        private DateTime CalculatePlannedEndDate(DateTime startDate, SpecializationProgram program)
        {
            // Oblicz datę zakończenia na podstawie programu specjalizacji
            int totalMonths = program.TotalDurationMonths;

            if (totalMonths <= 0)
            {
                // Jeśli brak informacji o czasie trwania, użyj domyślnej wartości 48 miesięcy (4 lata)
                totalMonths = 48;
            }

            return startDate.AddMonths(totalMonths);
        }

        private async Task OnCancelAsync()
        {
            // Powrót do poprzedniej strony
            await Shell.Current.GoToAsync("..");
        }
    }
}
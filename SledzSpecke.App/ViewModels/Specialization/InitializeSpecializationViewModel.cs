using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Helpers;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Specialization
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
            this.InitializeCommand = new AsyncRelayCommand(this.OnInitializeAsync, () => this.selectedSpecialization != null);
            this.CancelCommand = new AsyncRelayCommand(this.OnCancelAsync);

            // Inicjalizacja właściwości
            this.Title = "Inicjalizacja specjalizacji";
            this.AvailableSpecializations = new ObservableCollection<SpecializationProgram>();

            // Wczytanie dostępnych specjalizacji
            this.LoadSpecializationsAsync().ConfigureAwait(false);
        }

        // Właściwości
        public ObservableCollection<SpecializationProgram> AvailableSpecializations
        {
            get => this.availableSpecializations;
            set => this.SetProperty(ref this.availableSpecializations, value);
        }

        public SpecializationProgram SelectedSpecialization
        {
            get => this.selectedSpecialization;
            set
            {
                if (this.SetProperty(ref this.selectedSpecialization, value))
                {
                    this.HasModules = value?.HasModules ?? false;
                    ((AsyncRelayCommand)this.InitializeCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public SmkVersion SmkVersion
        {
            get => this.smkVersion;
            set => this.SetProperty(ref this.smkVersion, value);
        }

        public DateTime StartDate
        {
            get => this.startDate;
            set => this.SetProperty(ref this.startDate, value);
        }

        public bool IsInitializing
        {
            get => this.isInitializing;
            set => this.SetProperty(ref this.isInitializing, value);
        }

        public bool HasModules
        {
            get => this.hasModules;
            set => this.SetProperty(ref this.hasModules, value);
        }

        public string StatusMessage
        {
            get => this.statusMessage;
            set => this.SetProperty(ref this.statusMessage, value);
        }

        // Komendy
        public ICommand InitializeCommand { get; }
        public ICommand CancelCommand { get; }

        // Metody
        private async Task LoadSpecializationsAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Pobierz wszystkie dostępne programy specjalizacji
                var programs = await this.databaseService.GetAllSpecializationProgramsAsync();

                this.AvailableSpecializations.Clear();

                foreach (var program in programs)
                {
                    this.AvailableSpecializations.Add(program);
                }

                // Jeśli nie ma dostępnych programów, wczytaj domyślne
                if (this.AvailableSpecializations.Count == 0)
                {
                    await this.LoadDefaultSpecializationsAsync();
                }

                // Wybierz pierwszą dostępną specjalizację
                if (this.AvailableSpecializations.Count > 0)
                {
                    this.SelectedSpecialization = this.AvailableSpecializations[0];
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd ładowania specjalizacji: {ex.Message}");
                this.StatusMessage = "Nie udało się załadować dostępnych specjalizacji.";
            }
            finally
            {
                this.IsBusy = false;
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
                        var program = await SpecializationLoader.LoadSpecializationProgramAsync(code, this.SmkVersion);

                        if (program != null)
                        {
                            // Zapisz program w bazie danych
                            await this.databaseService.SaveSpecializationProgramAsync(program);

                            // Dodaj do listy dostępnych specjalizacji
                            this.AvailableSpecializations.Add(program);
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
            if (this.IsInitializing || this.SelectedSpecialization == null)
            {
                return;
            }

            this.IsInitializing = true;
            this.StatusMessage = "Inicjalizacja specjalizacji...";

            try
            {
                // Utwórz nową specjalizację
                var specialization = new Models.Specialization
                {
                    Name = this.SelectedSpecialization.Name,
                    ProgramCode = this.SelectedSpecialization.Code,
                    StartDate = this.StartDate,
                    PlannedEndDate = this.CalculatePlannedEndDate(this.StartDate, this.SelectedSpecialization),
                    HasModules = this.SelectedSpecialization.HasModules,
                    ProgramStructure = this.SelectedSpecialization.Structure,
                };

                this.StatusMessage = "Zapisywanie specjalizacji...";

                // Zapisz specjalizację w bazie danych
                int specializationId = await this.databaseService.SaveSpecializationAsync(specialization);

                // Pobierz zapisaną specjalizację z ID
                specialization = await this.databaseService.GetSpecializationAsync(specializationId);

                if (specialization == null)
                {
                    throw new InvalidOperationException("Nie udało się zapisać specjalizacji.");
                }

                // Jeśli specjalizacja ma moduły, utwórz je
                if (specialization.HasModules)
                {
                    this.StatusMessage = "Tworzenie modułów...";

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
                            await this.databaseService.SaveModuleAsync(module);
                        }

                        // Aktualizuj specjalizację o listę modułów
                        specialization.Modules = await this.databaseService.GetModulesAsync(specializationId);

                        // Ustaw pierwszy moduł jako aktualny
                        if (specialization.Modules.Count > 0)
                        {
                            specialization.CurrentModuleId = specialization.Modules[0].ModuleId;
                            await this.databaseService.UpdateSpecializationAsync(specialization);
                        }

                        // Inicjalizuj moduły z danymi z plików JSON
                        this.StatusMessage = "Inicjalizacja danych modułów...";
                        await ModuleHelper.InitializeModulesAsync(
                            this.databaseService,
                            specializationId,
                            specialization.Modules);
                    }
                }

                this.StatusMessage = "Specjalizacja została pomyślnie zainicjalizowana.";

                // Wyświetl komunikat o sukcesie
                await this.dialogService.DisplayAlertAsync(
                    "Sukces",
                    "Specjalizacja została pomyślnie zainicjalizowana.",
                    "OK");

                // Przekieruj do dashboardu
                await this.OnCancelAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd inicjalizacji specjalizacji: {ex.Message}");
                this.StatusMessage = $"Błąd inicjalizacji specjalizacji: {ex.Message}";

                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    $"Nie udało się zainicjalizować specjalizacji: {ex.Message}",
                    "OK");
            }
            finally
            {
                this.IsInitializing = false;
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
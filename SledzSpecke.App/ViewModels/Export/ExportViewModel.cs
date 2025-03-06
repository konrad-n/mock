using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Export;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Export
{
    public class ExportViewModel : BaseViewModel
    {
        private readonly IExportService exportService;
        private readonly ISpecializationService specializationService;
        private readonly IDialogService dialogService;
        private readonly IAuthService authService;

        private DateTime startDate;
        private DateTime endDate;
        private bool includeShifts;
        private bool includeProcedures;
        private bool includeInternships;
        private bool includeCourses;
        private bool includeSelfEducation;
        private bool includePublications;
        private bool includeAbsences;
        private bool includeEducationalActivities;
        private bool includeRecognitions;
        private bool formatForOldSmk;
        private bool hasModules;
        private bool isExporting;
        private string exportStatusMessage;
        private DateTime? lastExportDate;
        private ObservableCollection<ModuleInfo> availableModules;
        private ModuleInfo selectedModule;
        private bool canExport;
        private SmkVersion userSmkVersion; // Added to track user's SMK version

        public ExportViewModel(
            IExportService exportService,
            ISpecializationService specializationService,
            IDialogService dialogService,
            IAuthService authService)
        {
            this.exportService = exportService;
            this.specializationService = specializationService;
            this.dialogService = dialogService;
            this.authService = authService;

            // Ustawienie tytułu
            this.Title = "Eksport danych do SMK";

            // Inicjalizacja kolekcji
            this.AvailableModules = new ObservableCollection<ModuleInfo>();

            // Domyślnie zaznaczenie wszystkich opcji
            this.IncludeShifts = true;
            this.IncludeProcedures = true;
            this.IncludeInternships = true;
            this.IncludeCourses = true;
            this.IncludeSelfEducation = true;
            this.IncludePublications = true;
            this.IncludeAbsences = true;
            this.IncludeEducationalActivities = true;
            this.IncludeRecognitions = true;

            // Ustawienie domyślnego zakresu dat (ostatni miesiąc)
            this.EndDate = DateTime.Today;
            this.StartDate = this.EndDate.AddMonths(-1);

            // Inicjalizacja komend
            this.ExportCommand = new AsyncRelayCommand(this.OnExportAsync, this.CanExecuteExport);
            this.ShareLastExportCommand = new AsyncRelayCommand(this.OnShareLastExportAsync);
            this.SelectAllCommand = new RelayCommand(this.SelectAllOptions);
            this.DeselectAllCommand = new RelayCommand(this.DeselectAllOptions);
            this.SetLastMonthCommand = new RelayCommand(this.SetLastMonthRange);
            this.SetLastYearCommand = new RelayCommand(this.SetLastYearRange);
            this.SetAllTimeCommand = new RelayCommand(this.SetAllTimeRange);

            // Wczytanie danych
            this.LoadDataAsync().ConfigureAwait(false);
        }

        // Właściwości do bindowania
        public DateTime StartDate
        {
            get => this.startDate;
            set
            {
                if (this.SetProperty(ref this.startDate, value))
                {
                    this.ValidateInput();
                }
            }
        }

        public DateTime EndDate
        {
            get => this.endDate;
            set
            {
                if (this.SetProperty(ref this.endDate, value))
                {
                    this.ValidateInput();
                }
            }
        }

        public bool IncludeShifts
        {
            get => this.includeShifts;
            set => this.SetProperty(ref this.includeShifts, value);
        }

        public bool IncludeProcedures
        {
            get => this.includeProcedures;
            set => this.SetProperty(ref this.includeProcedures, value);
        }

        public bool IncludeInternships
        {
            get => this.includeInternships;
            set => this.SetProperty(ref this.includeInternships, value);
        }

        public bool IncludeCourses
        {
            get => this.includeCourses;
            set => this.SetProperty(ref this.includeCourses, value);
        }

        public bool IncludeSelfEducation
        {
            get => this.includeSelfEducation;
            set => this.SetProperty(ref this.includeSelfEducation, value);
        }

        public bool IncludePublications
        {
            get => this.includePublications;
            set => this.SetProperty(ref this.includePublications, value);
        }

        public bool IncludeAbsences
        {
            get => this.includeAbsences;
            set => this.SetProperty(ref this.includeAbsences, value);
        }

        public bool IncludeEducationalActivities
        {
            get => this.includeEducationalActivities;
            set => this.SetProperty(ref this.includeEducationalActivities, value);
        }

        public bool IncludeRecognitions
        {
            get => this.includeRecognitions;
            set => this.SetProperty(ref this.includeRecognitions, value);
        }

        public bool FormatForOldSmk
        {
            get => this.formatForOldSmk;
            set => this.SetProperty(ref this.formatForOldSmk, value);
        }

        public bool HasModules
        {
            get => this.hasModules;
            set => this.SetProperty(ref this.hasModules, value);
        }

        public bool IsExporting
        {
            get => this.isExporting;
            set
            {
                if (this.SetProperty(ref this.isExporting, value))
                {
                    ((AsyncRelayCommand)this.ExportCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public string ExportStatusMessage
        {
            get => this.exportStatusMessage;
            set => this.SetProperty(ref this.exportStatusMessage, value);
        }

        public DateTime? LastExportDate
        {
            get => this.lastExportDate;
            set => this.SetProperty(ref this.lastExportDate, value);
        }

        public ObservableCollection<ModuleInfo> AvailableModules
        {
            get => this.availableModules;
            set => this.SetProperty(ref this.availableModules, value);
        }

        public ModuleInfo SelectedModule
        {
            get => this.selectedModule;
            set => this.SetProperty(ref this.selectedModule, value);
        }

        public bool CanExport
        {
            get => this.canExport;
            set
            {
                if (this.SetProperty(ref this.canExport, value))
                {
                    ((AsyncRelayCommand)this.ExportCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public SmkVersion UserSmkVersion
        {
            get => this.userSmkVersion;
            private set => this.SetProperty(ref this.userSmkVersion, value);
        }

        public bool HasAnyOptionSelected =>
            this.IncludeShifts ||
            this.IncludeProcedures ||
            this.IncludeInternships ||
            this.IncludeCourses ||
            this.IncludeSelfEducation ||
            this.IncludePublications ||
            this.IncludeAbsences ||
            this.IncludeEducationalActivities ||
            this.IncludeRecognitions;

        public string FormattedLastExportDate => this.LastExportDate.HasValue
            ? this.LastExportDate.Value.ToString("dd.MM.yyyy HH:mm")
            : "Brak";

        // New property to display SMK version info in UI
        public string SmkVersionInfo => this.UserSmkVersion == SmkVersion.New
            ? "Format danych: Nowa wersja SMK"
            : "Format danych: Stara wersja SMK";

        // Komendy
        public ICommand ExportCommand { get; }
        public ICommand ShareLastExportCommand { get; }
        public ICommand SelectAllCommand { get; }
        public ICommand DeselectAllCommand { get; }
        public ICommand SetLastMonthCommand { get; }
        public ICommand SetLastYearCommand { get; }
        public ICommand SetAllTimeCommand { get; }

        // Metody
        private async Task LoadDataAsync()
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Sprawdzenie czy specjalizacja ma moduły
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie znaleziono aktywnej specjalizacji.",
                        "OK");
                    return;
                }

                this.HasModules = specialization.HasModules;

                // Wczytaj user settings dla formatu SMK
                var user = await this.authService.GetCurrentUserAsync();
                if (user != null)
                {
                    this.UserSmkVersion = user.SmkVersion;
                    // Ustaw format eksportu na podstawie wersji SMK użytkownika
                    this.FormatForOldSmk = user.SmkVersion == SmkVersion.Old;
                }

                // Wczytaj dostępne moduły, jeśli specjalizacja jest modułowa
                if (this.HasModules)
                {
                    // Pobierz moduły dla bieżącej specjalizacji
                    var modules = await this.specializationService.GetModulesAsync(specialization.SpecializationId);

                    this.AvailableModules.Clear();
                    this.AvailableModules.Add(new ModuleInfo { Id = 0, Name = "Wszystkie moduły" });

                    foreach (var module in modules)
                    {
                        this.AvailableModules.Add(new ModuleInfo
                        {
                            Id = module.ModuleId,
                            Name = module.Name,
                        });
                    }

                    // Wybierz opcję "Wszystkie moduły" jako domyślną
                    this.SelectedModule = this.AvailableModules[0];
                }

                // Wczytaj datę ostatniego eksportu
                this.LastExportDate = await this.exportService.GetLastExportDateAsync();

                // Walidacja wprowadzonych danych
                this.ValidateInput();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania danych eksportu: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem podczas ładowania danych. Spróbuj ponownie.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private void ValidateInput()
        {
            // Sprawdź czy zakres dat jest poprawny
            this.CanExport = this.StartDate <= this.EndDate && this.HasAnyOptionSelected;
        }

        private bool CanExecuteExport()
        {
            return this.CanExport && !this.IsExporting;
        }

        private async Task OnExportAsync()
        {
            if (this.IsExporting)
            {
                return;
            }

            this.IsExporting = true;
            this.ExportStatusMessage = "Przygotowywanie danych do eksportu...";

            try
            {
                // Przygotuj opcje eksportu
                var options = new ExportOptions
                {
                    StartDate = this.StartDate,
                    EndDate = this.EndDate,
                    IncludeShifts = this.IncludeShifts,
                    IncludeProcedures = this.IncludeProcedures,
                    IncludeInternships = this.IncludeInternships,
                    IncludeCourses = this.IncludeCourses,
                    IncludeSelfEducation = this.IncludeSelfEducation,
                    IncludePublications = this.IncludePublications,
                    IncludeAbsences = this.IncludeAbsences,
                    IncludeEducationalActivities = this.IncludeEducationalActivities,
                    IncludeRecognitions = this.IncludeRecognitions,
                    FormatForOldSMK = this.FormatForOldSmk,
                    ModuleId = this.HasModules && this.SelectedModule != null && this.SelectedModule.Id > 0
                        ? this.SelectedModule.Id
                        : null,
                };

                // Serializuj opcje do przekazania do strony podglądu
                string serializedOptions = System.Text.Json.JsonSerializer.Serialize(options);

                // Nawiguj do strony podglądu z opcjami eksportu
                await Shell.Current.GoToAsync($"ExportPreview?options={Uri.EscapeDataString(serializedOptions)}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas eksportu: {ex.Message}");
                this.ExportStatusMessage = "Wystąpił błąd podczas eksportu.";

                await this.dialogService.DisplayAlertAsync(
                    "Błąd eksportu",
                    $"Wystąpił problem podczas eksportu danych: {ex.Message}",
                    "OK");
            }
            finally
            {
                this.IsExporting = false;
            }
        }

        private async Task OnShareLastExportAsync()
        {
            try
            {
                // Pobierz ścieżkę do ostatniego pliku eksportu
                string filePath = await this.exportService.GetLastExportFilePathAsync();

                if (string.IsNullOrEmpty(filePath))
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Informacja",
                        "Brak dostępnego pliku eksportu. Wykonaj najpierw eksport danych.",
                        "OK");
                    return;
                }

                // Sprawdź czy plik istnieje
                if (!File.Exists(filePath))
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie znaleziono pliku eksportu. Mógł zostać przeniesiony lub usunięty.",
                        "OK");
                    return;
                }

                // Udostępnij plik
                bool result = await this.exportService.ShareExportFileAsync(filePath);

                if (!result)
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie udało się udostępnić pliku eksportu.",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas udostępniania pliku: {ex.Message}");

                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Wystąpił problem podczas udostępniania pliku eksportu.",
                    "OK");
            }
        }

        private void SelectAllOptions()
        {
            this.IncludeShifts = true;
            this.IncludeProcedures = true;
            this.IncludeInternships = true;
            this.IncludeCourses = true;
            this.IncludeSelfEducation = true;
            this.IncludePublications = true;
            this.IncludeAbsences = true;
            this.IncludeEducationalActivities = true;
            this.IncludeRecognitions = true;

            this.ValidateInput();
        }

        private void DeselectAllOptions()
        {
            this.IncludeShifts = false;
            this.IncludeProcedures = false;
            this.IncludeInternships = false;
            this.IncludeCourses = false;
            this.IncludeSelfEducation = false;
            this.IncludePublications = false;
            this.IncludeAbsences = false;
            this.IncludeEducationalActivities = false;
            this.IncludeRecognitions = false;

            this.ValidateInput();
        }

        private void SetLastMonthRange()
        {
            this.EndDate = DateTime.Today;
            this.StartDate = this.EndDate.AddMonths(-1);
        }

        private void SetLastYearRange()
        {
            this.EndDate = DateTime.Today;
            this.StartDate = this.EndDate.AddYears(-1);
        }

        private void SetAllTimeRange()
        {
            this.EndDate = DateTime.Today;

            // Ustaw początek na datę rozpoczęcia specjalizacji
            Task.Run(async () =>
            {
                try
                {
                    var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                    if (specialization != null)
                    {
                        // Przełącz na wątek UI, aby zaktualizować własność
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            this.StartDate = specialization.StartDate;
                        });
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Błąd podczas ustawiania zakresu dat: {ex.Message}");
                }
            });
        }
    }
}
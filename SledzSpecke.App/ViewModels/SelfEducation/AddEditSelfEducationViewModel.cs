using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.Specialization;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.SelfEducation
{
    [QueryProperty(nameof(SelfEducationId), "selfEducationId")]
    [QueryProperty(nameof(ModuleId), "moduleId")]
    public class AddEditSelfEducationViewModel : BaseViewModel
    {
        private readonly ISpecializationService specializationService;
        private readonly IDatabaseService databaseService;
        private readonly IDialogService dialogService;

        private int selfEducationId;
        private int? moduleId;
        private string title = string.Empty;
        private string type = string.Empty;
        private string publisher = string.Empty;
        private int year = DateTime.Now.Year;
        private bool canSave;
        private bool isOldSmkVersion;
        private bool requiresAcceptance;

        private ObservableCollection<string> availableTypes;
        private ObservableCollection<int> availableYears;
        private bool? hasModules;
        private string moduleInfo = string.Empty;

        public AddEditSelfEducationViewModel(
            ISpecializationService specializationService,
            IDatabaseService databaseService,
            IDialogService dialogService)
        {
            this.specializationService = specializationService;
            this.databaseService = databaseService;
            this.dialogService = dialogService;

            // Inicjalizacja tytułu
            this.Title = "Nowe samokształcenie";

            // Inicjalizacja komend
            this.SaveCommand = new AsyncRelayCommand(this.OnSaveAsync, () => this.CanSave);
            this.CancelCommand = new AsyncRelayCommand(this.OnCancelAsync);

            // Inicjalizacja kolekcji
            this.AvailableTypes = new ObservableCollection<string>
            {
                "Kongres/Konferencja",
                "Kurs/Szkolenie",
                "Sympozjum",
                "Warsztaty",
                "Publikacja",
                "Inne",
            };

            this.AvailableYears = new ObservableCollection<int>();
            int currentYear = DateTime.Now.Year;
            for (int i = currentYear - 10; i <= currentYear; i++)
            {
                this.AvailableYears.Add(i);
            }

            // Sprawdź wersję SMK
            this.CheckSmkVersionAsync().ConfigureAwait(false);
        }

        // Właściwości
        public int SelfEducationId
        {
            get => this.selfEducationId;
            set
            {
                if (this.SetProperty(ref this.selfEducationId, value) && value > 0)
                {
                    // Aktualizacja tytułu dla trybu edycji
                    this.Title = "Edytuj samokształcenie";

                    // Wczytanie danych samokształcenia
                    this.LoadSelfEducationAsync(value).ConfigureAwait(false);
                }
            }
        }

        public int? ModuleId
        {
            get => this.moduleId;
            set
            {
                if (this.SetProperty(ref this.moduleId, value) && value.HasValue)
                {
                    // Wczytaj dane o module
                    this.LoadModuleInfoAsync().ConfigureAwait(false);
                }
            }
        }

        public string SelfEducationTitle
        {
            get => this.title;
            set
            {
                this.SetProperty(ref this.title, value);
                this.ValidateInput();
            }
        }

        public string Type
        {
            get => this.type;
            set
            {
                this.SetProperty(ref this.type, value);
                this.ValidateInput();
            }
        }

        public string Publisher
        {
            get => this.publisher;
            set
            {
                this.SetProperty(ref this.publisher, value);
                this.ValidateInput();
            }
        }

        public int Year
        {
            get => this.year;
            set => this.SetProperty(ref this.year, value);
        }

        public bool CanSave
        {
            get => this.canSave;
            set => this.SetProperty(ref this.canSave, value);
        }

        public ObservableCollection<string> AvailableTypes
        {
            get => this.availableTypes;
            set => this.SetProperty(ref this.availableTypes, value);
        }

        public ObservableCollection<int> AvailableYears
        {
            get => this.availableYears;
            set => this.SetProperty(ref this.availableYears, value);
        }

        public string ModuleInfo
        {
            get => this.moduleInfo;
            set => this.SetProperty(ref this.moduleInfo, value);
        }

        // Dodane właściwości dla starego SMK
        public bool IsOldSmkVersion
        {
            get => this.isOldSmkVersion;
            set => this.SetProperty(ref this.isOldSmkVersion, value);
        }

        public bool RequiresAcceptance
        {
            get => this.requiresAcceptance;
            set => this.SetProperty(ref this.requiresAcceptance, value);
        }

        // Komendy
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        // Metody
        private async Task CheckSmkVersionAsync()
        {
            try
            {
                var user = await this.specializationService.GetCurrentUserAsync();
                this.IsOldSmkVersion = user?.SmkVersion == SmkVersion.Old;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas sprawdzania wersji SMK: {ex.Message}");
            }
        }

        private async Task LoadSelfEducationAsync(int selfEducationId)
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Wczytanie samokształcenia
                var selfEducation = await this.databaseService.GetSelfEducationAsync(selfEducationId);
                if (selfEducation == null)
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie znaleziono samokształcenia.",
                        "OK");
                    await this.OnCancelAsync();
                    return;
                }

                // Ustawienie moduleId na podstawie samokształcenia
                this.moduleId = selfEducation.ModuleId;

                // Ustawienie właściwości
                this.SelfEducationTitle = selfEducation.Title;
                this.Type = selfEducation.Type;
                this.Publisher = selfEducation.Publisher;
                this.Year = selfEducation.Year;

                // Wczytaj dane o module, jeśli istnieje
                if (selfEducation.ModuleId.HasValue)
                {
                    await this.LoadModuleInfoAsync();
                }

                // Wczytaj dodatkowe pola, jeśli jest to stara wersja SMK
                if (this.IsOldSmkVersion && !string.IsNullOrEmpty(selfEducation.AdditionalFields))
                {
                    try
                    {
                        var additionalFields = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(selfEducation.AdditionalFields);

                        if (additionalFields != null && additionalFields.TryGetValue("RequiresAcceptance", out object requiresAcceptance))
                        {
                            this.RequiresAcceptance = Convert.ToBoolean(requiresAcceptance);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Błąd parsowania pól dodatkowych: {ex.Message}");
                    }
                }

                // Walidacja
                this.ValidateInput();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd ładowania samokształcenia: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie udało się załadować danych samokształcenia.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task LoadModuleInfoAsync()
        {
            try
            {
                if (!this.moduleId.HasValue)
                {
                    this.ModuleInfo = string.Empty;
                    return;
                }

                // Sprawdź, czy specjalizacja ma moduły
                if (!this.hasModules.HasValue)
                {
                    var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                    this.hasModules = specialization?.HasModules ?? false;
                }

                // Jeśli specjalizacja ma moduły, pobierz dane modułu
                if (this.hasModules.Value)
                {
                    var module = await this.databaseService.GetModuleAsync(this.moduleId.Value);
                    if (module != null)
                    {
                        this.ModuleInfo = $"To samokształcenie będzie dodane do modułu: {module.Name}";
                    }
                }
                else
                {
                    this.ModuleInfo = string.Empty;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd ładowania informacji o module: {ex.Message}");
                this.ModuleInfo = string.Empty;
            }
        }

        private void ValidateInput()
        {
            // Sprawdź czy wszystkie wymagane pola są wypełnione
            this.CanSave = !string.IsNullOrWhiteSpace(this.SelfEducationTitle)
                        && !string.IsNullOrWhiteSpace(this.Type)
                        && !string.IsNullOrWhiteSpace(this.Publisher);
        }

        private async Task OnSaveAsync()
        {
            if (this.IsBusy || !this.CanSave)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Pobierz aktualną specjalizację
                var specialization = await this.specializationService.GetCurrentSpecializationAsync();
                if (specialization == null)
                {
                    throw new Exception("Nie znaleziono aktywnej specjalizacji.");
                }

                // Utwórz lub pobierz samokształcenie
                Models.SelfEducation selfEducation;
                if (this.SelfEducationId > 0)
                {
                    selfEducation = await this.databaseService.GetSelfEducationAsync(this.SelfEducationId);
                    if (selfEducation == null)
                    {
                        throw new Exception("Nie znaleziono samokształcenia do edycji.");
                    }

                    // Oznacz jako zmodyfikowane, jeśli było wcześniej zsynchronizowane
                    if (selfEducation.SyncStatus == SyncStatus.Synced)
                    {
                        selfEducation.SyncStatus = SyncStatus.Modified;
                    }
                }
                else
                {
                    selfEducation = new Models.SelfEducation
                    {
                        SpecializationId = specialization.SpecializationId,
                        SyncStatus = SyncStatus.NotSynced,
                    };
                }

                // Aktualizacja właściwości
                selfEducation.Title = this.SelfEducationTitle;
                selfEducation.Type = this.Type;
                selfEducation.Publisher = this.Publisher;
                selfEducation.Year = this.Year;

                // Dodaj pola specyficzne dla starej wersji SMK w AdditionalFields
                if (this.IsOldSmkVersion)
                {
                    var additionalFields = new Dictionary<string, object>();

                    if (!string.IsNullOrEmpty(selfEducation.AdditionalFields))
                    {
                        try
                        {
                            additionalFields = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(selfEducation.AdditionalFields);
                        }
                        catch
                        {
                            additionalFields = new Dictionary<string, object>();
                        }
                    }

                    // Dodaj informację o akceptacji
                    additionalFields["RequiresAcceptance"] = this.RequiresAcceptance;

                    // Serializuj z powrotem do właściwości AdditionalFields
                    selfEducation.AdditionalFields = System.Text.Json.JsonSerializer.Serialize(additionalFields);
                }

                // Ustaw moduł, jeśli specjalizacja ma moduły
                if (specialization.HasModules)
                {
                    // Jeśli przekazano moduleId, użyj go
                    if (this.moduleId.HasValue)
                    {
                        selfEducation.ModuleId = this.moduleId.Value;
                    }
                    // W przeciwnym razie użyj bieżącego modułu specjalizacji
                    else if (specialization.CurrentModuleId.HasValue)
                    {
                        selfEducation.ModuleId = specialization.CurrentModuleId.Value;
                    }
                }
                else
                {
                    selfEducation.ModuleId = null;
                }

                // Zapisz do bazy danych
                await this.databaseService.SaveSelfEducationAsync(selfEducation);

                await this.dialogService.DisplayAlertAsync(
                    "Sukces",
                    this.SelfEducationId > 0
                        ? "Samokształcenie zostało pomyślnie zaktualizowane."
                        : "Samokształcenie zostało pomyślnie dodane.",
                    "OK");

                // Powrót
                await this.OnCancelAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd zapisywania samokształcenia: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie udało się zapisać samokształcenia. Spróbuj ponownie.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task OnCancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
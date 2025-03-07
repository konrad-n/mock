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
    public class SelfEducationListViewModel : BaseViewModel
    {
        private readonly ISpecializationService specializationService;
        private readonly IDatabaseService databaseService;
        private readonly IDialogService dialogService;

        // Filtry
        private string searchText = string.Empty;
        private Models.SelfEducation selectedSelfEducation;

        // Dane
        private ObservableCollection<SelfEducationViewModel> selfEducationItems;
        private int? currentModuleId;
        private bool isOldSmkVersion;

        public SelfEducationListViewModel(
            ISpecializationService specializationService,
            IDatabaseService databaseService,
            IDialogService dialogService)
        {
            this.specializationService = specializationService;
            this.databaseService = databaseService;
            this.dialogService = dialogService;

            // Inicjalizacja komend
            this.RefreshCommand = new AsyncRelayCommand(this.LoadSelfEducationItemsAsync);
            this.FilterCommand = new AsyncRelayCommand(this.ApplyFiltersAsync);
            this.SelfEducationSelectedCommand = new AsyncRelayCommand(this.OnSelfEducationSelectedAsync);
            this.AddSelfEducationCommand = new AsyncRelayCommand(this.OnAddSelfEducationAsync);

            // Inicjalizacja właściwości
            this.Title = "Samokształcenie";
            this.SelfEducationItems = new ObservableCollection<SelfEducationViewModel>();

            // Sprawdź wersję SMK
            this.CheckSmkVersionAsync().ConfigureAwait(false);

            // Wczytanie danych
            this.LoadSelfEducationItemsAsync().ConfigureAwait(false);
        }

        // Właściwości
        public ObservableCollection<SelfEducationViewModel> SelfEducationItems
        {
            get => this.selfEducationItems;
            set => this.SetProperty(ref this.selfEducationItems, value);
        }

        public Models.SelfEducation SelectedSelfEducation
        {
            get => this.selectedSelfEducation;
            set => this.SetProperty(ref this.selectedSelfEducation, value);
        }

        public string SearchText
        {
            get => this.searchText;
            set => this.SetProperty(ref this.searchText, value);
        }

        public bool IsOldSmkVersion
        {
            get => this.isOldSmkVersion;
            set => this.SetProperty(ref this.isOldSmkVersion, value);
        }

        // Komendy
        public ICommand RefreshCommand { get; }

        public ICommand FilterCommand { get; }

        public ICommand SelfEducationSelectedCommand { get; }

        public ICommand AddSelfEducationCommand { get; }

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

        private async Task LoadSelfEducationItemsAsync()
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

                // Ustaw bieżący moduł
                if (specialization.CurrentModuleId.HasValue)
                {
                    this.currentModuleId = specialization.CurrentModuleId.Value;
                }
                else
                {
                    this.currentModuleId = null;
                }

                // Wyczyść istniejące elementy
                this.SelfEducationItems.Clear();

                // Pobierz elementy dla bieżącego modułu lub całej specjalizacji
                var items = await this.databaseService.GetSelfEducationItemsAsync(
                    specializationId: specialization.SpecializationId,
                    moduleId: this.currentModuleId);

                // Filtrowanie według wyszukiwanego tekstu
                if (!string.IsNullOrEmpty(this.SearchText))
                {
                    var searchLower = this.SearchText.ToLowerInvariant();
                    items = items.Where(i =>
                        i.Title.ToLowerInvariant().Contains(searchLower) ||
                        i.Type.ToLowerInvariant().Contains(searchLower) ||
                        i.Publisher.ToLowerInvariant().Contains(searchLower))
                        .ToList();
                }

                // Dodaj elementy do kolekcji
                foreach (var item in items.OrderByDescending(i => i.Year))
                {
                    var viewModel = new SelfEducationViewModel
                    {
                        SelfEducationId = item.SelfEducationId,
                        Title = item.Title,
                        Type = item.Type,
                        Publisher = item.Publisher,
                        Year = item.Year,
                        SyncStatus = item.SyncStatus,
                    };

                    // Sprawdź czy istnieje informacja o wymaganiu akceptacji (tylko dla starego SMK)
                    if (this.IsOldSmkVersion && !string.IsNullOrEmpty(item.AdditionalFields))
                    {
                        try
                        {
                            var additionalFields = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(item.AdditionalFields);
                            if (additionalFields != null && additionalFields.TryGetValue("RequiresAcceptance", out object requiresAcceptance))
                            {
                                viewModel.RequiresAcceptance = Convert.ToBoolean(requiresAcceptance);
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Błąd parsowania pól dodatkowych: {ex.Message}");
                        }
                    }

                    // Pobierz nazwę modułu
                    if (item.ModuleId.HasValue)
                    {
                        var module = await this.databaseService.GetModuleAsync(item.ModuleId.Value);
                        if (module != null)
                        {
                            viewModel.ModuleName = module.Name;
                        }
                    }

                    this.SelfEducationItems.Add(viewModel);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas ładowania elementów samokształcenia: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie udało się załadować elementów samokształcenia. Spróbuj ponownie.",
                    "OK");
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private async Task ApplyFiltersAsync()
        {
            await this.LoadSelfEducationItemsAsync();
        }

        private async Task OnSelfEducationSelectedAsync()
        {
            if (this.SelectedSelfEducation == null)
            {
                return;
            }

            // Nawigacja do strony szczegółów
            await Shell.Current.GoToAsync($"SelfEducationDetails?selfEducationId={this.SelectedSelfEducation.SelfEducationId}");

            // Resetuj zaznaczenie
            this.SelectedSelfEducation = null;
        }

        private async Task OnAddSelfEducationAsync()
        {
            // Pobierz identyfikator bieżącego modułu, aby przekazać go do strony dodawania
            var specialization = await this.specializationService.GetCurrentSpecializationAsync();
            int? moduleId = null;

            if (specialization != null && specialization.CurrentModuleId.HasValue)
            {
                moduleId = specialization.CurrentModuleId.Value;
            }

            // Nawigacja do strony dodawania
            if (moduleId.HasValue)
            {
                await Shell.Current.GoToAsync($"AddEditSelfEducation?moduleId={moduleId.Value}");
            }
            else
            {
                await Shell.Current.GoToAsync("AddEditSelfEducation");
            }
        }
    }
}
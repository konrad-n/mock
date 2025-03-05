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
    public class SelfEducationDetailsViewModel : BaseViewModel
    {
        private readonly IDatabaseService databaseService;
        private readonly IDialogService dialogService;
        private readonly ISpecializationService specializationService;

        private int selfEducationId;
        private Models.SelfEducation selfEducation;
        private string title;
        private string type;
        private string publisher;
        private int year;
        private string syncStatusText;
        private bool isNotSynced;
        private string additionalDetails;
        private string moduleInfo;
        private bool isOldSmkVersion;
        private bool requiresAcceptance;

        public SelfEducationDetailsViewModel(
            IDatabaseService databaseService,
            IDialogService dialogService,
            ISpecializationService specializationService)
        {
            this.databaseService = databaseService;
            this.dialogService = dialogService;
            this.specializationService = specializationService;

            // Inicjalizacja komend
            this.EditCommand = new AsyncRelayCommand(this.OnEditAsync);
            this.DeleteCommand = new AsyncRelayCommand(this.OnDeleteAsync);
            this.GoBackCommand = new AsyncRelayCommand(this.OnGoBackAsync);

            // Inicjalizacja właściwości
            this.Title = "Szczegóły samokształcenia";

            // Sprawdź wersję SMK
            this.CheckSmkVersionAsync().ConfigureAwait(false);
        }

        // Właściwości
        public int SelfEducationId
        {
            get => this.selfEducationId;
            set
            {
                this.SetProperty(ref this.selfEducationId, value);
                this.LoadSelfEducationAsync(value).ConfigureAwait(false);
            }
        }

        public Models.SelfEducation SelfEducation
        {
            get => this.selfEducation;
            set => this.SetProperty(ref this.selfEducation, value);
        }

        public string SelfEducationTitle
        {
            get => this.title;
            set => this.SetProperty(ref this.title, value);
        }

        public string Type
        {
            get => this.type;
            set => this.SetProperty(ref this.type, value);
        }

        public string Publisher
        {
            get => this.publisher;
            set => this.SetProperty(ref this.publisher, value);
        }

        public int Year
        {
            get => this.year;
            set => this.SetProperty(ref this.year, value);
        }

        public string SyncStatusText
        {
            get => this.syncStatusText;
            set => this.SetProperty(ref this.syncStatusText, value);
        }

        public bool IsNotSynced
        {
            get => this.isNotSynced;
            set => this.SetProperty(ref this.isNotSynced, value);
        }

        public string AdditionalDetails
        {
            get => this.additionalDetails;
            set => this.SetProperty(ref this.additionalDetails, value);
        }

        public string ModuleInfo
        {
            get => this.moduleInfo;
            set => this.SetProperty(ref this.moduleInfo, value);
        }

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
        public ICommand EditCommand { get; }

        public ICommand DeleteCommand { get; }

        public ICommand GoBackCommand { get; }

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
                this.SelfEducation = await this.databaseService.GetSelfEducationAsync(selfEducationId);
                if (this.SelfEducation == null)
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie znaleziono samokształcenia.",
                        "OK");
                    await this.OnGoBackAsync();
                    return;
                }

                // Ustawienie właściwości
                this.SelfEducationTitle = this.SelfEducation.Title;
                this.Type = this.SelfEducation.Type;
                this.Publisher = this.SelfEducation.Publisher;
                this.Year = this.SelfEducation.Year;

                // Ustawienie statusu synchronizacji
                this.IsNotSynced = this.SelfEducation.SyncStatus != SyncStatus.Synced;
                this.SyncStatusText = this.GetSyncStatusText(this.SelfEducation.SyncStatus);

                // Pobierz dodatkowe informacje o module, jeśli dostępne
                if (this.SelfEducation.ModuleId.HasValue && this.SelfEducation.ModuleId.Value > 0)
                {
                    var module = await this.databaseService.GetModuleAsync(this.SelfEducation.ModuleId.Value);
                    if (module != null)
                    {
                        this.ModuleInfo = $"Moduł: {module.Name}";
                    }
                    else
                    {
                        this.ModuleInfo = string.Empty;
                    }
                }
                else
                {
                    this.ModuleInfo = string.Empty;
                }

                // Wczytaj dodatkowe pola dla starej wersji SMK
                if (this.IsOldSmkVersion && !string.IsNullOrEmpty(this.SelfEducation.AdditionalFields))
                {
                    try
                    {
                        var additionalFields = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(this.SelfEducation.AdditionalFields);

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

                // Parsowanie dodatkowych pól
                if (!string.IsNullOrEmpty(this.SelfEducation.AdditionalFields))
                {
                    try
                    {
                        var additionalFields = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(
                            this.SelfEducation.AdditionalFields);

                        this.AdditionalDetails = string.Join("\n", additionalFields.Select(kv => $"{kv.Key}: {kv.Value}"));
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Błąd parsowania pól dodatkowych: {ex.Message}");
                        this.AdditionalDetails = string.Empty;
                    }
                }
                else
                {
                    this.AdditionalDetails = string.Empty;
                }
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

        private string GetSyncStatusText(SyncStatus status)
        {
            return status switch
            {
                SyncStatus.NotSynced => "Nie zsynchronizowano z SMK",
                SyncStatus.Synced => "Zsynchronizowano z SMK",
                SyncStatus.SyncFailed => "Synchronizacja nie powiodła się",
                SyncStatus.Modified => "Zsynchronizowano, ale potem zmodyfikowano",
                _ => "Nieznany status",
            };
        }

        private async Task OnEditAsync()
        {
            if (this.SelfEducation != null)
            {
                await Shell.Current.GoToAsync($"AddEditSelfEducation?selfEducationId={this.SelfEducation.SelfEducationId}");
            }
        }

        private async Task OnDeleteAsync()
        {
            if (this.SelfEducation == null)
            {
                return;
            }

            bool confirm = await this.dialogService.DisplayAlertAsync(
                "Potwierdź usunięcie",
                "Czy na pewno chcesz usunąć to samokształcenie? Tej operacji nie można cofnąć.",
                "Tak, usuń",
                "Anuluj");

            if (confirm)
            {
                try
                {
                    await this.databaseService.DeleteSelfEducationAsync(this.SelfEducation);
                    await this.dialogService.DisplayAlertAsync(
                        "Sukces",
                        "Samokształcenie zostało pomyślnie usunięte.",
                        "OK");
                    await this.OnGoBackAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Błąd podczas usuwania samokształcenia: {ex.Message}");
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie udało się usunąć samokształcenia. Spróbuj ponownie.",
                        "OK");
                }
            }
        }

        private async Task OnGoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
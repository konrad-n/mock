using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Internships
{
    [QueryProperty(nameof(InternshipId), "internshipId")]
    public class InternshipDetailsViewModel : BaseViewModel
    {
        private readonly IDatabaseService databaseService;
        private readonly IDialogService dialogService;

        private int internshipId;
        private Internship internship;
        private string internshipName;
        private string institutionName;
        private string departmentName;
        private DateTime startDate;
        private DateTime endDate;
        private int daysCount;
        private int year;
        private bool isCompleted;
        private bool isApproved;
        private string syncStatusText;
        private bool isNotSynced;
        private string moduleInfo;

        // Pola specyficzne dla starego SMK
        private bool isPartialRealization;
        private string supervisorName;
        private bool isOldSmkVersion;

        public InternshipDetailsViewModel(
            IDatabaseService databaseService,
            IDialogService dialogService)
        {
            this.databaseService = databaseService;
            this.dialogService = dialogService;

            // Inicjalizacja komend
            this.EditCommand = new AsyncRelayCommand(this.OnEditAsync);
            this.DeleteCommand = new AsyncRelayCommand(this.OnDeleteAsync);
            this.GoBackCommand = new AsyncRelayCommand(this.OnGoBackAsync);

            // Inicjalizacja właściwości
            this.Title = "Szczegóły stażu";

            // Sprawdzenie wersji SMK
            this.CheckSmkVersionAsync().ConfigureAwait(false);
        }

        // Właściwości
        public int InternshipId
        {
            get => this.internshipId;
            set
            {
                this.SetProperty(ref this.internshipId, value);
                this.LoadInternshipAsync(value).ConfigureAwait(false);
            }
        }

        public Internship Internship
        {
            get => this.internship;
            set => this.SetProperty(ref this.internship, value);
        }

        public string InternshipName
        {
            get => this.internshipName;
            set => this.SetProperty(ref this.internshipName, value);
        }

        public string InstitutionName
        {
            get => this.institutionName;
            set => this.SetProperty(ref this.institutionName, value);
        }

        public string DepartmentName
        {
            get => this.departmentName;
            set => this.SetProperty(ref this.departmentName, value);
        }

        public DateTime StartDate
        {
            get => this.startDate;
            set => this.SetProperty(ref this.startDate, value);
        }

        public DateTime EndDate
        {
            get => this.endDate;
            set => this.SetProperty(ref this.endDate, value);
        }

        public int DaysCount
        {
            get => this.daysCount;
            set => this.SetProperty(ref this.daysCount, value);
        }

        public int Year
        {
            get => this.year;
            set => this.SetProperty(ref this.year, value);
        }

        public bool IsCompleted
        {
            get => this.isCompleted;
            set => this.SetProperty(ref this.isCompleted, value);
        }

        public bool IsApproved
        {
            get => this.isApproved;
            set => this.SetProperty(ref this.isApproved, value);
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

        public string ModuleInfo
        {
            get => this.moduleInfo;
            set => this.SetProperty(ref this.moduleInfo, value);
        }

        // Właściwości specyficzne dla starego SMK
        public bool IsPartialRealization
        {
            get => this.isPartialRealization;
            set => this.SetProperty(ref this.isPartialRealization, value);
        }

        public string SupervisorName
        {
            get => this.supervisorName;
            set => this.SetProperty(ref this.supervisorName, value);
        }

        public bool IsOldSmkVersion
        {
            get => this.isOldSmkVersion;
            set => this.SetProperty(ref this.isOldSmkVersion, value);
        }

        public string DateRange => $"{this.startDate:d} - {this.endDate:d}";

        public string Status
        {
            get
            {
                string status = string.Empty;

                if (this.isApproved)
                {
                    status = "Zatwierdzony";
                }
                else if (this.isCompleted)
                {
                    status = "Ukończony";
                }
                else
                {
                    status = "W trakcie";
                }

                // Dodaj informację o realizacji częściowej w starym SMK
                if (this.IsOldSmkVersion && this.IsPartialRealization)
                {
                    status += " (realizacja częściowa)";
                }

                return status;
            }
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
                var user = await this.databaseService.GetCurrentUserAsync();
                this.IsOldSmkVersion = user?.SmkVersion == SmkVersion.Old;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas sprawdzania wersji SMK: {ex.Message}");
            }
        }

        private async Task LoadInternshipAsync(int internshipId)
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Wczytanie stażu
                this.Internship = await this.databaseService.GetInternshipAsync(internshipId);
                if (this.Internship == null)
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie znaleziono stażu.",
                        "OK");
                    await this.OnGoBackAsync();
                    return;
                }

                // Ustawienie właściwości
                this.InternshipName = this.Internship.InternshipName;
                this.InstitutionName = this.Internship.InstitutionName;
                this.DepartmentName = this.Internship.DepartmentName;
                this.StartDate = this.Internship.StartDate;
                this.EndDate = this.Internship.EndDate;
                this.DaysCount = this.Internship.DaysCount;
                this.Year = this.Internship.Year;
                this.IsCompleted = this.Internship.IsCompleted;
                this.IsApproved = this.Internship.IsApproved;

                // Właściwości specyficzne dla starego SMK
                this.IsPartialRealization = this.Internship.IsPartialRealization;
                this.SupervisorName = this.Internship.SupervisorName ?? string.Empty;

                // Ustawienie statusu synchronizacji
                this.IsNotSynced = this.Internship.SyncStatus != SyncStatus.Synced;
                this.SyncStatusText = this.GetSyncStatusText(this.Internship.SyncStatus);

                // Pobierz dodatkowe informacje o module, jeśli dostępne
                if (this.Internship.ModuleId.HasValue && this.Internship.ModuleId.Value > 0)
                {
                    var module = await this.databaseService.GetModuleAsync(this.Internship.ModuleId.Value);
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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd ładowania stażu: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie udało się załadować danych stażu.",
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
            if (this.Internship != null)
            {
                await Shell.Current.GoToAsync($"AddEditInternship?internshipId={this.Internship.InternshipId}");
            }
        }

        private async Task OnDeleteAsync()
        {
            if (this.Internship == null)
            {
                return;
            }

            bool confirm = await this.dialogService.DisplayAlertAsync(
                "Potwierdź usunięcie",
                "Czy na pewno chcesz usunąć ten staż? Tej operacji nie można cofnąć.",
                "Tak, usuń",
                "Anuluj");

            if (confirm)
            {
                try
                {
                    await this.databaseService.DeleteInternshipAsync(this.Internship);
                    await this.dialogService.DisplayAlertAsync(
                        "Sukces",
                        "Staż został pomyślnie usunięty.",
                        "OK");
                    await this.OnGoBackAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Błąd podczas usuwania stażu: {ex.Message}");
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie udało się usunąć stażu. Spróbuj ponownie.",
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
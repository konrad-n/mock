using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.Services.SmkStrategy;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.MedicalShifts
{
    [QueryProperty(nameof(ShiftId), "shiftId")]
    public partial class MedicalShiftDetailsViewModel : BaseViewModel
    {
        private readonly IDatabaseService databaseService;
        private readonly ISmkVersionStrategy smkStrategy;
        private readonly IDialogService dialogService;

        private int shiftId;
        private MedicalShift shift;
        private string internshipName;
        private string internshipInstitution;
        private string durationText;
        private string syncStatusText;
        private bool isNotSynced;
        private bool isOldSMKVersion;
        private string oldSMKField1;
        private string oldSMKField2;

        public MedicalShiftDetailsViewModel(
            IDatabaseService databaseService,
            ISmkVersionStrategy smkStrategy,
            IDialogService dialogService)
        {
            this.databaseService = databaseService;
            this.smkStrategy = smkStrategy;
            this.dialogService = dialogService;

            // Initialize commands
            this.EditCommand = new AsyncRelayCommand(this.OnEditAsync);
            this.DeleteCommand = new AsyncRelayCommand(this.OnDeleteAsync);
            this.GoBackCommand = new AsyncRelayCommand(this.OnGoBackAsync);

            // Initialize properties
            this.Title = "Szczegóły dyżuru";
        }

        // Properties
        public int ShiftId
        {
            get => this.shiftId;
            set
            {
                this.SetProperty(ref this.shiftId, value);
                this.LoadShiftAsync(value);
            }
        }

        public MedicalShift Shift
        {
            get => this.shift;
            set => this.SetProperty(ref this.shift, value);
        }

        public string InternshipName
        {
            get => this.internshipName;
            set => this.SetProperty(ref this.internshipName, value);
        }

        public string InternshipInstitution
        {
            get => this.internshipInstitution;
            set => this.SetProperty(ref this.internshipInstitution, value);
        }

        public string DurationText
        {
            get => this.durationText;
            set => this.SetProperty(ref this.durationText, value);
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

        public bool IsOldSMKVersion
        {
            get => this.isOldSMKVersion;
            set => this.SetProperty(ref this.isOldSMKVersion, value);
        }

        public string OldSMKField1
        {
            get => this.oldSMKField1;
            set => this.SetProperty(ref this.oldSMKField1, value);
        }

        public string OldSMKField2
        {
            get => this.oldSMKField2;
            set => this.SetProperty(ref this.oldSMKField2, value);
        }

        // Commands
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand GoBackCommand { get; }

        // Methods
        private async Task LoadShiftAsync(int shiftId)
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Load shift
                this.Shift = await this.databaseService.GetMedicalShiftAsync(shiftId);
                if (this.Shift == null)
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie znaleziono dyżuru.",
                        "OK");
                    await this.OnGoBackAsync();
                    return;
                }

                // Format duration
                this.DurationText = $"{this.Shift.Hours} godzin {this.Shift.Minutes} minut";

                // Set sync status
                this.IsNotSynced = this.Shift.SyncStatus != SyncStatus.Synced;
                this.SyncStatusText = this.GetSyncStatusText(this.Shift.SyncStatus);

                // Load internship data
                if (this.Shift.InternshipId > 0)
                {
                    var internship = await this.databaseService.GetInternshipAsync(this.Shift.InternshipId);
                    if (internship != null)
                    {
                        this.InternshipName = internship.InternshipName;
                        this.InternshipInstitution = internship.InstitutionName;
                    }
                }

                // Check if old SMK version and parse additional fields
                this.IsOldSMKVersion = this.smkStrategy.GetType().Name.Contains("Old");
                if (this.IsOldSMKVersion && !string.IsNullOrEmpty(this.Shift.AdditionalFields))
                {
                    try
                    {
                        var additionalFields = this.smkStrategy.ParseAdditionalFields(this.Shift.AdditionalFields);

                        if (additionalFields.TryGetValue("OldSMKField1", out object field1))
                        {
                            this.OldSMKField1 = field1?.ToString() ?? string.Empty;
                        }

                        if (additionalFields.TryGetValue("OldSMKField2", out object field2))
                        {
                            this.OldSMKField2 = field2?.ToString() ?? string.Empty;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error parsing additional fields: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading shift: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie udało się załadować szczegółów dyżuru.",
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
                _ => "Nieznany status"
            };
        }

        private async Task OnEditAsync()
        {
            // Navigate to edit page
            if (this.Shift != null)
            {
                await Shell.Current.GoToAsync($"AddEditMedicalShift?shiftId={this.Shift.ShiftId}");
            }
        }

        private async Task OnDeleteAsync()
        {
            if (this.Shift == null)
            {
                return;
            }

            bool confirm = await this.dialogService.DisplayAlertAsync(
                "Potwierdź usunięcie",
                "Czy na pewno chcesz usunąć ten dyżur? Tej operacji nie można cofnąć.",
                "Tak, usuń",
                "Anuluj");

            if (confirm)
            {
                try
                {
                    await this.databaseService.DeleteMedicalShiftAsync(this.Shift);
                    await this.dialogService.DisplayAlertAsync(
                        "Sukces",
                        "Dyżur został pomyślnie usunięty.",
                        "OK");
                    await this.OnGoBackAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error deleting shift: {ex.Message}");
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie udało się usunąć dyżuru. Spróbuj ponownie.",
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
using System.Text.Json;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Dialog;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Procedures
{
    [QueryProperty(nameof(ProcedureId), "procedureId")]
    public class ProcedureDetailsViewModel : BaseViewModel
    {
        private readonly IDatabaseService databaseService;
        private readonly IDialogService dialogService;

        private int procedureId;
        private Procedure procedure;
        private string code = string.Empty;
        private string operatorCode = string.Empty;
        private DateTime date;
        private string location = string.Empty;
        private string patientInitials = string.Empty;
        private string patientGender = string.Empty;
        private string assistantData = string.Empty;
        private string procedureGroup = string.Empty;
        private string status = string.Empty;
        private string performingPerson = string.Empty;
        private string syncStatusText = string.Empty;
        private bool isNotSynced;
        private string internshipName = string.Empty;
        private string internshipInstitution = string.Empty;
        private string moduleInfo = string.Empty;
        private bool isOldSmkVersion; private int year;

        public ProcedureDetailsViewModel(
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
            this.Title = "Szczegóły procedury";

            // Sprawdzenie wersji SMK (można pobierać z ustawień lub usługi)
            this.isOldSmkVersion = false; // Domyślnie nowy SMK, faktyczna wartość będzie ustawiana w aplikacji
        }

        // Właściwości
        public int ProcedureId
        {
            get => this.procedureId;
            set
            {
                this.SetProperty(ref this.procedureId, value);
                this.LoadProcedureAsync(value).ConfigureAwait(false);
            }
        }

        public Procedure Procedure
        {
            get => this.procedure;
            set => this.SetProperty(ref this.procedure, value);
        }

        public string Code
        {
            get => this.code;
            set => this.SetProperty(ref this.code, value);
        }

        public string OperatorCode
        {
            get => this.operatorCode;
            set => this.SetProperty(ref this.operatorCode, value);
        }

        public DateTime Date
        {
            get => this.date;
            set => this.SetProperty(ref this.date, value);
        }

        public string Location
        {
            get => this.location;
            set => this.SetProperty(ref this.location, value);
        }

        public string PatientInitials
        {
            get => this.patientInitials;
            set => this.SetProperty(ref this.patientInitials, value);
        }

        public string PatientGender
        {
            get => this.patientGender;
            set => this.SetProperty(ref this.patientGender, value);
        }

        public int Year
        {
            get => this.year;
            set => this.SetProperty(ref this.year, value);
        }

        public string AssistantData
        {
            get => this.assistantData;
            set => this.SetProperty(ref this.assistantData, value);
        }

        public string ProcedureGroup
        {
            get => this.procedureGroup;
            set => this.SetProperty(ref this.procedureGroup, value);
        }

        public string Status
        {
            get => this.status;
            set => this.SetProperty(ref this.status, value);
        }

        public string PerformingPerson
        {
            get => this.performingPerson;
            set => this.SetProperty(ref this.performingPerson, value);
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

        public string FormattedDate => this.Date.ToString("d");

        public string PatientInfo
        {
            get
            {
                if (!string.IsNullOrEmpty(this.PatientInitials))
                {
                    string info = $"Pacjent: {this.PatientInitials}";

                    if (!string.IsNullOrEmpty(this.PatientGender))
                    {
                        info += $" ({this.PatientGender})";
                    }

                    return info;
                }

                return string.Empty;
            }
        }

        public string OperatorTypeText
        {
            get
            {
                return this.OperatorCode switch
                {
                    "A" => "Operator",
                    "B" => "Asysta",
                    _ => this.OperatorCode,
                };
            }
        }

        // Komendy
        public ICommand EditCommand { get; }

        public ICommand DeleteCommand { get; }

        public ICommand GoBackCommand { get; }

        // Metody
        private async Task LoadProcedureAsync(int procedureId)
        {
            if (this.IsBusy)
            {
                return;
            }

            this.IsBusy = true;

            try
            {
                // Wczytanie procedury
                this.Procedure = await this.databaseService.GetProcedureAsync(procedureId);
                if (this.Procedure == null)
                {
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie znaleziono procedury.",
                        "OK");
                    await this.OnGoBackAsync();
                    return;
                }

                // Ustawienie właściwości
                this.Code = this.Procedure.Code;
                this.OperatorCode = this.Procedure.OperatorCode;
                this.Date = this.Procedure.Date;
                this.Location = this.Procedure.Location;
                this.PatientInitials = this.Procedure.PatientInitials;
                this.PatientGender = this.Procedure.PatientGender;
                this.AssistantData = this.Procedure.AssistantData;
                this.ProcedureGroup = this.Procedure.ProcedureGroup;
                this.Status = this.Procedure.Status;
                this.PerformingPerson = this.Procedure.PerformingPerson;
                this.Year = this.Procedure.Year;

                // Ustawienie statusu synchronizacji
                this.IsNotSynced = this.Procedure.SyncStatus != SyncStatus.Synced;
                this.SyncStatusText = this.GetSyncStatusText(this.Procedure.SyncStatus);

                // Wczytaj informacje o stażu
                if (this.Procedure.InternshipId > 0)
                {
                    var internship = await this.databaseService.GetInternshipAsync(this.Procedure.InternshipId);
                    if (internship != null)
                    {
                        this.InternshipName = internship.InternshipName;
                        this.InternshipInstitution = internship.InstitutionName;

                        // Wczytaj informacje o module, jeśli staż jest przypisany do modułu
                        if (internship.ModuleId.HasValue && internship.ModuleId.Value > 0)
                        {
                            var module = await this.databaseService.GetModuleAsync(internship.ModuleId.Value);
                            if (module != null)
                            {
                                this.ModuleInfo = $"Moduł: {module.Name}";
                            }
                        }
                    }
                }

                // Parsowanie dodatkowych pól
                if (!string.IsNullOrEmpty(this.Procedure.AdditionalFields))
                {
                    try
                    {
                        var additionalFields = JsonSerializer.Deserialize<Dictionary<string, object>>(
                            this.Procedure.AdditionalFields);

                        // Odczytywanie specyficznych pól z danych dodatkowych
                        // Można dodać więcej pól w miarę potrzeb
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Błąd parsowania pól dodatkowych: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd ładowania procedury: {ex.Message}");
                await this.dialogService.DisplayAlertAsync(
                    "Błąd",
                    "Nie udało się załadować danych procedury.",
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
            if (this.Procedure != null)
            {
                await Shell.Current.GoToAsync($"AddEditProcedure?procedureId={this.Procedure.ProcedureId}");
            }
        }

        private async Task OnDeleteAsync()
        {
            if (this.Procedure == null)
            {
                return;
            }

            bool confirm = await this.dialogService.DisplayAlertAsync(
                "Potwierdź usunięcie",
                "Czy na pewno chcesz usunąć tę procedurę? Tej operacji nie można cofnąć.",
                "Tak, usuń",
                "Anuluj");

            if (confirm)
            {
                try
                {
                    await this.databaseService.DeleteProcedureAsync(this.Procedure);
                    await this.dialogService.DisplayAlertAsync(
                        "Sukces",
                        "Procedura została pomyślnie usunięta.",
                        "OK");
                    await this.OnGoBackAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Błąd podczas usuwania procedury: {ex.Message}");
                    await this.dialogService.DisplayAlertAsync(
                        "Błąd",
                        "Nie udało się usunąć procedury. Spróbuj ponownie.",
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
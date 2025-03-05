using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Procedures
{
    /// <summary>
    /// ViewModel reprezentujący pojedynczą procedurę w interfejsie użytkownika.
    /// </summary>
    public class ProcedureViewModel : BaseViewModel
    {
        private int procedureId;
        private int internshipId;
        private DateTime date;
        private string code = string.Empty;
        private string operatorCode = string.Empty;
        private string location = string.Empty;
        private string patientInitials = string.Empty;
        private string patientGender = string.Empty;
        private string assistantData = string.Empty;
        private string procedureGroup = string.Empty;
        private string status = string.Empty;
        private string performingPerson = string.Empty;
        private SyncStatus syncStatus;
        private string syncStatusText = string.Empty;
        private string internshipName = string.Empty;
        private string moduleName = string.Empty;
        private ModuleType moduleType;
        private string procedureName = string.Empty;
        private string patientInfo = string.Empty;
        private string groupName = string.Empty;

        /// <summary>
        /// Pobiera lub ustawia identyfikator procedury.
        /// </summary>
        public int ProcedureId
        {
            get => this.procedureId;
            set => this.SetProperty(ref this.procedureId, value);
        }

        /// <summary>
        /// Pobiera lub ustawia identyfikator stażu.
        /// </summary>
        public int InternshipId
        {
            get => this.internshipId;
            set => this.SetProperty(ref this.internshipId, value);
        }

        /// <summary>
        /// Pobiera lub ustawia datę wykonania procedury.
        /// </summary>
        public DateTime Date
        {
            get => this.date;
            set => this.SetProperty(ref this.date, value);
        }

        /// <summary>
        /// Pobiera lub ustawia kod procedury.
        /// </summary>
        public string Code
        {
            get => this.code;
            set => this.SetProperty(ref this.code, value);
        }

        /// <summary>
        /// Pobiera lub ustawia kod operatora (A - operator, B - asysta).
        /// </summary>
        public string OperatorCode
        {
            get => this.operatorCode;
            set => this.SetProperty(ref this.operatorCode, value);
        }

        /// <summary>
        /// Pobiera lub ustawia miejsce wykonania procedury.
        /// </summary>
        public string Location
        {
            get => this.location;
            set => this.SetProperty(ref this.location, value);
        }

        /// <summary>
        /// Pobiera lub ustawia inicjały pacjenta.
        /// </summary>
        public string PatientInitials
        {
            get => this.patientInitials;
            set => this.SetProperty(ref this.patientInitials, value);
        }

        /// <summary>
        /// Pobiera lub ustawia płeć pacjenta.
        /// </summary>
        public string PatientGender
        {
            get => this.patientGender;
            set => this.SetProperty(ref this.patientGender, value);
        }

        /// <summary>
        /// Pobiera lub ustawia dane asysty.
        /// </summary>
        public string AssistantData
        {
            get => this.assistantData;
            set => this.SetProperty(ref this.assistantData, value);
        }

        /// <summary>
        /// Pobiera lub ustawia grupę procedur.
        /// </summary>
        public string ProcedureGroup
        {
            get => this.procedureGroup;
            set => this.SetProperty(ref this.procedureGroup, value);
        }

        /// <summary>
        /// Pobiera lub ustawia status procedury.
        /// </summary>
        public string Status
        {
            get => this.status;
            set => this.SetProperty(ref this.status, value);
        }

        /// <summary>
        /// Pobiera lub ustawia osobę wykonującą procedurę (dla starej wersji SMK).
        /// </summary>
        public string PerformingPerson
        {
            get => this.performingPerson;
            set => this.SetProperty(ref this.performingPerson, value);
        }

        /// <summary>
        /// Pobiera lub ustawia status synchronizacji z systemem SMK.
        /// </summary>
        public SyncStatus SyncStatus
        {
            get => this.syncStatus;
            set
            {
                if (this.SetProperty(ref this.syncStatus, value))
                {
                    this.SyncStatusText = this.GetSyncStatusText(value);
                }
            }
        }

        /// <summary>
        /// Pobiera lub ustawia tekstowy opis statusu synchronizacji.
        /// </summary>
        public string SyncStatusText
        {
            get => this.syncStatusText;
            set => this.SetProperty(ref this.syncStatusText, value);
        }

        /// <summary>
        /// Pobiera lub ustawia nazwę stażu, podczas którego wykonano procedurę.
        /// </summary>
        public string InternshipName
        {
            get => this.internshipName;
            set => this.SetProperty(ref this.internshipName, value);
        }

        /// <summary>
        /// Pobiera lub ustawia nazwę modułu, do którego należy procedura.
        /// </summary>
        public string ModuleName
        {
            get => this.moduleName;
            set => this.SetProperty(ref this.moduleName, value);
        }

        /// <summary>
        /// Pobiera lub ustawia typ modułu, do którego należy procedura.
        /// </summary>
        public ModuleType ModuleType
        {
            get => this.moduleType;
            set => this.SetProperty(ref this.moduleType, value);
        }

        /// <summary>
        /// Pobiera sformatowaną datę wykonania procedury.
        /// </summary>
        public string FormattedDate
        {
            get => this.date.ToString("d");
        }

        /// <summary>
        /// Pobiera informacje o pacjencie w sformatowanej postaci.
        /// </summary>
        public string PatientInfo
        {
            get => this.patientInfo;
            set => this.SetProperty(ref this.patientInfo, value);
        }

        /// <summary>
        /// Pobiera lub ustawia nazwę procedury.
        /// </summary>
        public string ProcedureName
        {
            get => this.procedureName;
            set => this.SetProperty(ref this.procedureName, value);
        }

        /// <summary>
        /// Pobiera lub ustawia nazwę grupy procedur.
        /// </summary>
        public string GroupName
        {
            get => this.groupName;
            set => this.SetProperty(ref this.groupName, value);
        }

        /// <summary>
        /// Konwertuje obiekt Procedure do ProcedureViewModel.
        /// </summary>
        /// <param name="procedure">Obiekt procedury z modelu danych.</param>
        /// <param name="internshipName">Opcjonalna nazwa stażu.</param>
        /// <param name="moduleName">Opcjonalna nazwa modułu.</param>
        /// <returns>Obiekt ViewModel procedury.</returns>
        public static ProcedureViewModel FromModel(Procedure procedure, string internshipName = null, string moduleName = null, ModuleType moduleType = ModuleType.Basic)
        {
            ArgumentNullException.ThrowIfNull(procedure, nameof(procedure));

            return new ProcedureViewModel
            {
                ProcedureId = procedure.ProcedureId,
                InternshipId = procedure.InternshipId,
                Date = procedure.Date,
                Code = procedure.Code,
                OperatorCode = procedure.OperatorCode,
                Location = procedure.Location,
                PatientInitials = procedure.PatientInitials,
                PatientGender = procedure.PatientGender,
                AssistantData = procedure.AssistantData,
                ProcedureGroup = procedure.ProcedureGroup,
                Status = procedure.Status,
                PerformingPerson = procedure.PerformingPerson,
                SyncStatus = procedure.SyncStatus,
                InternshipName = internshipName ?? string.Empty,
                ModuleName = moduleName ?? string.Empty,
                ModuleType = moduleType,
                ProcedureName = procedure.Code,
                PatientInfo = $"{procedure.PatientInitials} ({procedure.PatientGender})",
                GroupName = procedure.ProcedureGroup
            };
        }

        /// <summary>
        /// Konwertuje ViewModel na model danych Procedure.
        /// </summary>
        /// <param name="internshipId">Identyfikator stażu, do którego należy procedura.</param>
        /// <returns>Obiekt modelu procedury.</returns>
        public Procedure ToModel(int internshipId)
        {
            return new Procedure
            {
                ProcedureId = this.ProcedureId,
                InternshipId = internshipId,
                Date = this.Date,
                Code = this.Code,
                OperatorCode = this.OperatorCode,
                Location = this.Location,
                PatientInitials = this.PatientInitials,
                PatientGender = this.PatientGender,
                AssistantData = this.AssistantData,
                ProcedureGroup = this.ProcedureGroup,
                Status = this.Status,
                PerformingPerson = this.PerformingPerson,
                SyncStatus = this.SyncStatus,
            };
        }

        /// <summary>
        /// Zwraca tekstowy opis dla danego statusu synchronizacji.
        /// </summary>
        /// <param name="status">Status synchronizacji.</param>
        /// <returns>Opis tekstowy statusu.</returns>
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
    }
}
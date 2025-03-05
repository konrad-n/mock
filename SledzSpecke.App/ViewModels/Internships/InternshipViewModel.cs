using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Internships
{
    /// <summary>
    /// ViewModel reprezentujący pojedynczy staż w interfejsie użytkownika.
    /// </summary>
    public class InternshipViewModel : BaseViewModel
    {
        private int internshipId;
        private string internshipName = string.Empty;
        private string institutionName = string.Empty;
        private string departmentName = string.Empty;
        private DateTime startDate;
        private DateTime endDate;
        private int daysCount;
        private int year;
        private bool isCompleted;
        private bool isApproved;
        private bool isBasic;
        private SyncStatus syncStatus;
        private string syncStatusText = string.Empty;
        private string moduleName = string.Empty;
        private ModuleType moduleType;

        // Pola specyficzne dla starego SMK
        private bool isPartialRealization;
        private string supervisorName = string.Empty;
        private bool isOldSmkVersion;
        private string status = string.Empty;
        private string dateRange = string.Empty;

        /// <summary>
        /// Pobiera lub ustawia identyfikator stażu.
        /// </summary>
        public int InternshipId
        {
            get => this.internshipId;
            set => this.SetProperty(ref this.internshipId, value);
        }

        /// <summary>
        /// Pobiera lub ustawia nazwę stażu.
        /// </summary>
        public string InternshipName
        {
            get => this.internshipName;
            set => this.SetProperty(ref this.internshipName, value);
        }

        /// <summary>
        /// Pobiera lub ustawia nazwę instytucji.
        /// </summary>
        public string InstitutionName
        {
            get => this.institutionName;
            set => this.SetProperty(ref this.institutionName, value);
        }

        /// <summary>
        /// Pobiera lub ustawia nazwę oddziału.
        /// </summary>
        public string DepartmentName
        {
            get => this.departmentName;
            set => this.SetProperty(ref this.departmentName, value);
        }

        /// <summary>
        /// Pobiera lub ustawia datę rozpoczęcia stażu.
        /// </summary>
        public DateTime StartDate
        {
            get => this.startDate;
            set => this.SetProperty(ref this.startDate, value);
        }

        /// <summary>
        /// Pobiera lub ustawia datę zakończenia stażu.
        /// </summary>
        public DateTime EndDate
        {
            get => this.endDate;
            set => this.SetProperty(ref this.endDate, value);
        }

        /// <summary>
        /// Pobiera lub ustawia liczbę dni stażu.
        /// </summary>
        public int DaysCount
        {
            get => this.daysCount;
            set => this.SetProperty(ref this.daysCount, value);
        }

        /// <summary>
        /// Pobiera lub ustawia rok szkolenia.
        /// </summary>
        public int Year
        {
            get => this.year;
            set => this.SetProperty(ref this.year, value);
        }

        /// <summary>
        /// Pobiera lub ustawia wartość wskazującą, czy staż jest ukończony.
        /// </summary>
        public bool IsCompleted
        {
            get => this.isCompleted;
            set => this.SetProperty(ref this.isCompleted, value);
        }

        /// <summary>
        /// Pobiera lub ustawia wartość wskazującą, czy staż jest zatwierdzony.
        /// </summary>
        public bool IsApproved
        {
            get => this.isApproved;
            set => this.SetProperty(ref this.isApproved, value);
        }

        /// <summary>
        /// Pobiera lub ustawia wartość wskazującą, czy to staż podstawowy.
        /// </summary>
        public bool IsBasic
        {
            get => this.isBasic;
            set => this.SetProperty(ref this.isBasic, value);
        }

        /// <summary>
        /// Pobiera lub ustawia wartość wskazującą, czy to realizacja częściowa stażu (stary SMK).
        /// </summary>
        public bool IsPartialRealization
        {
            get => this.isPartialRealization;
            set => this.SetProperty(ref this.isPartialRealization, value);
        }

        /// <summary>
        /// Pobiera lub ustawia kierownika stażu (stary SMK).
        /// </summary>
        public string SupervisorName
        {
            get => this.supervisorName;
            set => this.SetProperty(ref this.supervisorName, value);
        }

        /// <summary>
        /// Pobiera lub ustawia wartość wskazującą, czy używana jest stara wersja SMK.
        /// </summary>
        public bool IsOldSmkVersion
        {
            get => this.isOldSmkVersion;
            set => this.SetProperty(ref this.isOldSmkVersion, value);
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
        /// Pobiera lub ustawia nazwę modułu, do którego należy staż.
        /// </summary>
        public string ModuleName
        {
            get => this.moduleName;
            set => this.SetProperty(ref this.moduleName, value);
        }

        /// <summary>
        /// Pobiera lub ustawia typ modułu, do którego należy staż.
        /// </summary>
        public ModuleType ModuleType
        {
            get => this.moduleType;
            set => this.SetProperty(ref this.moduleType, value);
        }

        /// <summary>
        /// Pobiera sformatowany zakres dat stażu.
        /// </summary>
        public string DateRange
        {
            get => this.dateRange;
            set => this.SetProperty(ref this.dateRange, value);
        }

        /// <summary>
        /// Pobiera sformatowany status stażu.
        /// </summary>
        public string Status
        {
            get => this.status;
            set => this.SetProperty(ref this.status, value);
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

        /// <summary>
        /// Konwertuje obiekt Internship do InternshipViewModel.
        /// </summary>
        /// <param name="internship">Obiekt stażu z modelu danych.</param>
        /// <param name="moduleName">Opcjonalna nazwa modułu.</param>
        /// <param name="moduleType">Typ modułu.</param>
        /// <param name="isOldSmk">Czy używana jest stara wersja SMK.</param>
        /// <returns>Obiekt ViewModel stażu.</returns>
        public static InternshipViewModel FromModel(Internship internship, string moduleName = null, ModuleType moduleType = ModuleType.Basic, bool isOldSmk = false)
        {
            ArgumentNullException.ThrowIfNull(internship, nameof(internship));

            return new InternshipViewModel
            {
                InternshipId = internship.InternshipId,
                InternshipName = internship.InternshipName,
                InstitutionName = internship.InstitutionName,
                DepartmentName = internship.DepartmentName,
                StartDate = internship.StartDate,
                EndDate = internship.EndDate,
                DaysCount = internship.DaysCount,
                Year = internship.Year,
                IsCompleted = internship.IsCompleted,
                IsApproved = internship.IsApproved,
                IsBasic = internship.IsBasic,
                IsPartialRealization = internship.IsPartialRealization,
                SupervisorName = internship.SupervisorName,
                SyncStatus = internship.SyncStatus,
                ModuleName = moduleName ?? string.Empty,
                ModuleType = moduleType,
                IsOldSmkVersion = isOldSmk,
            };
        }

        /// <summary>
        /// Konwertuje ViewModel na model danych Internship.
        /// </summary>
        /// <param name="specializationId">Identyfikator specjalizacji.</param>
        /// <param name="moduleId">Opcjonalny identyfikator modułu.</param>
        /// <returns>Obiekt modelu stażu.</returns>
        public Internship ToModel(int specializationId, int? moduleId = null)
        {
            return new Internship
            {
                InternshipId = this.InternshipId,
                SpecializationId = specializationId,
                ModuleId = moduleId,
                InternshipName = this.InternshipName,
                InstitutionName = this.InstitutionName,
                DepartmentName = this.DepartmentName,
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                DaysCount = this.DaysCount,
                Year = this.Year,
                IsCompleted = this.IsCompleted,
                IsApproved = this.IsApproved,
                SyncStatus = this.SyncStatus,
                // Pola specyficzne dla starego SMK
                IsPartialRealization = this.IsPartialRealization,
                SupervisorName = this.SupervisorName
            };
        }
    }
}
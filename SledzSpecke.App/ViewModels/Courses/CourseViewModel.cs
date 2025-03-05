using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.ViewModels.Base;

namespace SledzSpecke.App.ViewModels.Courses
{
    /// <summary>
    /// ViewModel reprezentujący pojedynczy kurs w interfejsie użytkownika.
    /// </summary>
    public class CourseViewModel : BaseViewModel
    {
        private int courseId;
        private string courseType = string.Empty;
        private string courseName = string.Empty;
        private string courseNumber = string.Empty;
        private string institutionName = string.Empty;
        private DateTime completionDate;
        private int year;
        private int courseSequenceNumber;
        private bool hasCertificate;
        private string certificateNumber = string.Empty;
        private DateTime? certificateDate;
        private SyncStatus syncStatus;
        private string syncStatusText = string.Empty;
        private string moduleName = string.Empty;
        private ModuleType moduleType;

        /// <summary>
        /// Pobiera lub ustawia identyfikator kursu.
        /// </summary>
        public int CourseId
        {
            get => this.courseId;
            set => this.SetProperty(ref this.courseId, value);
        }

        /// <summary>
        /// Pobiera lub ustawia typ kursu.
        /// </summary>
        public string CourseType
        {
            get => this.courseType;
            set => this.SetProperty(ref this.courseType, value);
        }

        /// <summary>
        /// Pobiera lub ustawia nazwę kursu.
        /// </summary>
        public string CourseName
        {
            get => this.courseName;
            set => this.SetProperty(ref this.courseName, value);
        }

        /// <summary>
        /// Pobiera lub ustawia numer kursu.
        /// </summary>
        public string CourseNumber
        {
            get => this.courseNumber;
            set => this.SetProperty(ref this.courseNumber, value);
        }

        /// <summary>
        /// Pobiera lub ustawia nazwę instytucji prowadzącej.
        /// </summary>
        public string InstitutionName
        {
            get => this.institutionName;
            set => this.SetProperty(ref this.institutionName, value);
        }

        /// <summary>
        /// Pobiera lub ustawia datę ukończenia kursu.
        /// </summary>
        public DateTime CompletionDate
        {
            get => this.completionDate;
            set => this.SetProperty(ref this.completionDate, value);
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
        /// Pobiera lub ustawia numer kolejny kursu.
        /// </summary>
        public int CourseSequenceNumber
        {
            get => this.courseSequenceNumber;
            set => this.SetProperty(ref this.courseSequenceNumber, value);
        }

        /// <summary>
        /// Pobiera lub ustawia wartość wskazującą, czy kurs posiada certyfikat.
        /// </summary>
        public bool HasCertificate
        {
            get => this.hasCertificate;
            set => this.SetProperty(ref this.hasCertificate, value);
        }

        /// <summary>
        /// Pobiera lub ustawia numer certyfikatu.
        /// </summary>
        public string CertificateNumber
        {
            get => this.certificateNumber;
            set => this.SetProperty(ref this.certificateNumber, value);
        }

        /// <summary>
        /// Pobiera lub ustawia datę certyfikatu.
        /// </summary>
        public DateTime? CertificateDate
        {
            get => this.certificateDate;
            set => this.SetProperty(ref this.certificateDate, value);
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
        /// Pobiera lub ustawia nazwę modułu, do którego należy kurs.
        /// </summary>
        public string ModuleName
        {
            get => this.moduleName;
            set => this.SetProperty(ref this.moduleName, value);
        }

        /// <summary>
        /// Pobiera lub ustawia typ modułu, do którego należy kurs.
        /// </summary>
        public ModuleType ModuleType
        {
            get => this.moduleType;
            set => this.SetProperty(ref this.moduleType, value);
        }

        /// <summary>
        /// Pobiera sformatowaną datę ukończenia kursu.
        /// </summary>
        public string FormattedCompletionDate
        {
            get => this.completionDate.ToString("d");
        }

        /// <summary>
        /// Pobiera sformatowane informacje o certyfikacie.
        /// </summary>
        public string CertificateInfo
        {
            get
            {
                if (this.hasCertificate)
                {
                    string info = "Certyfikat: " + this.certificateNumber;
                    if (this.certificateDate.HasValue)
                    {
                        info += " z dnia " + this.certificateDate.Value.ToString("d");
                    }

                    return info;
                }

                return "Brak certyfikatu";
            }
        }

        /// <summary>
        /// Konwertuje obiekt Course do CourseViewModel.
        /// </summary>
        /// <param name="course">Obiekt kursu z modelu danych.</param>
        /// <param name="moduleName">Opcjonalna nazwa modułu.</param>
        /// <returns>Obiekt ViewModel kursu.</returns>
        public static CourseViewModel FromModel(Course course, string moduleName = null, ModuleType moduleType = ModuleType.Basic)
        {
            ArgumentNullException.ThrowIfNull(course, nameof(course));

            return new CourseViewModel
            {
                CourseId = course.CourseId,
                CourseType = course.CourseType,
                CourseName = course.CourseName,
                CourseNumber = course.CourseNumber,
                InstitutionName = course.InstitutionName,
                CompletionDate = course.CompletionDate,
                Year = course.Year,
                CourseSequenceNumber = course.CourseSequenceNumber,
                HasCertificate = course.HasCertificate,
                CertificateNumber = course.CertificateNumber,
                CertificateDate = course.CertificateDate,
                SyncStatus = course.SyncStatus,
                ModuleName = moduleName ?? string.Empty,
                ModuleType = moduleType,
            };
        }

        /// <summary>
        /// Konwertuje ViewModel na model danych Course.
        /// </summary>
        /// <param name="specializationId">Identyfikator specjalizacji.</param>
        /// <param name="moduleId">Opcjonalny identyfikator modułu.</param>
        /// <returns>Obiekt modelu kursu.</returns>
        public Course ToModel(int specializationId, int? moduleId = null)
        {
            return new Course
            {
                CourseId = this.CourseId,
                SpecializationId = specializationId,
                ModuleId = moduleId,
                CourseType = this.CourseType ?? string.Empty,
                CourseName = this.CourseName ?? string.Empty,
                CourseNumber = this.CourseNumber ?? string.Empty,
                InstitutionName = this.InstitutionName ?? string.Empty,
                CompletionDate = this.CompletionDate,
                Year = this.Year,
                CourseSequenceNumber = this.CourseSequenceNumber,
                HasCertificate = this.HasCertificate,
                CertificateNumber = this.CertificateNumber ?? string.Empty,
                CertificateDate = this.CertificateDate,
                SyncStatus = this.SyncStatus
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
using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!
    public class Course
    {
        [PrimaryKey]
        [AutoIncrement]
        public int CourseId { get; set; }

        [Indexed]
        public int SpecializationId { get; set; }

        // Dodane pole dla modułu
        [Indexed]
        public int? ModuleId { get; set; }

        [MaxLength(50)]
        public string CourseType { get; set; }

        [MaxLength(100)]
        public string CourseName { get; set; }

        [MaxLength(20)]
        public string CourseNumber { get; set; }

        [MaxLength(100)]
        public string InstitutionName { get; set; }

        public DateTime CompletionDate { get; set; }

        public int Year { get; set; }

        public int CourseSequenceNumber { get; set; }

        // Pola dla certyfikatów
        public bool HasCertificate { get; set; }

        public string CertificateNumber { get; set; }

        public DateTime? CertificateDate { get; set; }

        // Nowe pole dla rodzaju uznania kursu (stary SMK)
        [MaxLength(100)]
        public string RecognitionType { get; set; }

        // Czy kurs wymaga akceptacji kierownika (stary SMK)
        public bool RequiresApproval { get; set; }

        // Nowe pole dla daty wystawienia certyfikatu (stary SMK)
        public DateTime? CertificateIssueDate { get; set; }

        public SyncStatus SyncStatus { get; set; }

        public string AdditionalFields { get; set; } // JSON
    }
}
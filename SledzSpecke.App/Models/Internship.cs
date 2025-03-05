using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class Internship
    {
        [PrimaryKey]
        [AutoIncrement]
        public int InternshipId { get; set; }

        [Indexed]
        public int SpecializationId { get; set; }

        // Dodane pole dla modułu
        [Indexed]
        public int? ModuleId { get; set; }

        [MaxLength(100)]
        public string? InstitutionName { get; set; }

        [MaxLength(100)]
        public string? DepartmentName { get; set; }

        [MaxLength(100)]
        public string? InternshipName { get; set; }

        public int Year { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int DaysCount { get; set; }

        // Status tracking
        public bool IsCompleted { get; set; }

        public bool IsApproved { get; set; }

        // Recognition fields
        public bool IsRecognition { get; set; }

        public string? RecognitionReason { get; set; }

        public int RecognitionDaysReduction { get; set; }

        // Pola specyficzne dla starego SMK
        public bool IsPartialRealization { get; set; }  // Oznaczenie stażu jako "realizacja częściowa"

        public string? SupervisorName { get; set; }  // Kierownik stażu (odpowiednik OldSMKField1)

        // Określa, czy jest to staż podstawowy
        public bool IsBasic { get; set; }

        public SyncStatus SyncStatus { get; set; }

        public string? AdditionalFields { get; set; } // JSON
    }
}
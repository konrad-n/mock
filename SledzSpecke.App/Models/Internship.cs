using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!
    public class Internship
    {
        [PrimaryKey]
        [AutoIncrement]
        public int InternshipId { get; set; }

        [Indexed]
        public int SpecializationId { get; set; }

        [Indexed]
        public int? ModuleId { get; set; }

        [MaxLength(100)]
        public string InstitutionName { get; set; }

        [MaxLength(100)]
        public string DepartmentName { get; set; }

        [MaxLength(100)]
        public string InternshipName { get; set; }

        public int Year { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int DaysCount { get; set; }

        public bool IsCompleted { get; set; }

        public bool IsApproved { get; set; }

        public bool IsRecognition { get; set; }

        public string RecognitionReason { get; set; }

        public int RecognitionDaysReduction { get; set; }

        public bool IsPartialRealization { get; set; }

        public string SupervisorName { get; set; }

        public SyncStatus SyncStatus { get; set; }

        public string AdditionalFields { get; set; }
    }
}
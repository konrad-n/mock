using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class MedicalShift
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ShiftId { get; set; }

        [Indexed]
        public int InternshipId { get; set; } // Powiązanie ze stażem

        public DateTime Date { get; set; }

        public int Hours { get; set; }

        public int Minutes { get; set; }

        [MaxLength(100)]
        public string Location { get; set; }

        public int Year { get; set; }

        public SyncStatus SyncStatus { get; set; }

        public string AdditionalFields { get; set; } // JSON
    }
}

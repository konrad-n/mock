using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class Procedure
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ProcedureId { get; set; }

        [Indexed]
        public int InternshipId { get; set; } // Powiązanie ze stażem

        public DateTime Date { get; set; }

        public int Year { get; set; } // Rok szkolenia - wymagany w starym SMK

        [MaxLength(20)]
        public string Code { get; set; } // "A - operator" lub "B - asysta"

        [MaxLength(100)]
        public string PerformingPerson { get; set; } // Osoba wykonująca - wymagana w starym SMK

        [MaxLength(100)]
        public string Location { get; set; }

        [MaxLength(10)]
        public string PatientInitials { get; set; }

        [MaxLength(1)]
        public string PatientGender { get; set; }

        public string AssistantData { get; set; } // Dane osoby wykonującej I i II asystę

        public string ProcedureGroup { get; set; } // Procedura z grupy

        [MaxLength(20)]
        public string Status { get; set; }

        public SyncStatus SyncStatus { get; set; }

        public string AdditionalFields { get; set; } // JSON
    }
}

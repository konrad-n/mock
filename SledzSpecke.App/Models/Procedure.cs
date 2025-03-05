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
        public string Code { get; set; } = string.Empty;

        [MaxLength(10)]
        public string OperatorCode { get; set; } = string.Empty; // A lub B - zachowujemy dla wewnętrznej logiki

        [MaxLength(100)]
        public string PerformingPerson { get; set; } = string.Empty; // Osoba wykonująca - wymagana w starym SMK

        [MaxLength(100)]
        public string Location { get; set; } = string.Empty;

        [MaxLength(10)]
        public string? PatientInitials { get; set; }

        [MaxLength(1)]
        public string? PatientGender { get; set; }

        public string? AssistantData { get; set; } // Dane osoby wykonującej I i II asystę

        public string ProcedureGroup { get; set; } = string.Empty; // Procedura z grupy

        [MaxLength(20)]
        public string Status { get; set; } = "Nowa";

        public SyncStatus SyncStatus { get; set; }

        public string? AdditionalFields { get; set; } // JSON
    }
}

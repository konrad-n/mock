using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class RealizedProcedureOldSMK : RealizedProcedureBase
    {
        public int Year { get; set; } // Rok specjalizacji

        [MaxLength(20)]
        public string Code { get; set; } // A - operator lub B - asysta

        [MaxLength(100)]
        public string PerformingPerson { get; set; } // Osoba wykonująca

        [MaxLength(100)]
        public string Location { get; set; } // Miejsce wykonania

        [Indexed]
        public int InternshipId { get; set; } // Powiązanie ze stażem

        [MaxLength(100)]
        public string InternshipName { get; set; } // Nazwa stażu

        [MaxLength(10)]
        public string PatientInitials { get; set; } // Inicjały pacjenta

        [MaxLength(1)]
        public string PatientGender { get; set; } // K lub M

        public string AssistantData { get; set; } // Dane osoby wykonującej I i II asystę

        public string ProcedureGroup { get; set; } // Procedura z grupy
    }
}
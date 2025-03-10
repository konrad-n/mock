using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class RealizedProcedureNewSMK : RealizedProcedureBase
    {
        [Indexed]
        public int ProcedureRequirementId { get; set; } // Powiązanie z wymaganiem procedury z JSON

        [Indexed]
        public int? ModuleId { get; set; } // Powiązanie z modułem

        public int CountA { get; set; } // Liczba wykonanych samodzielnie
        public int CountB { get; set; } // Liczba wykonanych jako asysta

        public DateTime StartDate { get; set; } // Data od
        public DateTime EndDate { get; set; } // Data do

        // Dodatkowe informacje o procedurze (nie przechowywane w bazie, ale ładowane z JSON)
        [Ignore]
        public string ProcedureName { get; set; }

        [Ignore]
        public string InternshipName { get; set; }
    }
}
using System;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class RealizedMedicalShiftNewSMK : RealizedMedicalShiftBase
    {
        [Indexed]
        public int InternshipRequirementId { get; set; } // Powiązanie z wymaganiem stażowym z JSON

        // Dodane pole dla modułu
        [Indexed]
        public int? ModuleId { get; set; } // Powiązanie z modułem

        public DateTime StartDate { get; set; } // Data od
        public DateTime EndDate { get; set; } // Data do

        // Dodatkowe informacje o stażu (nie przechowywane w bazie, ale ładowane z JSON)
        [Ignore]
        public string InternshipName { get; set; }
    }
}
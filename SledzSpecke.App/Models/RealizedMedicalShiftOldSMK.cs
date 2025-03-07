using System;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class RealizedMedicalShiftOldSMK : RealizedMedicalShiftBase
    {
        public int Year { get; set; } // Rok szkolenia
        public DateTime StartDate { get; set; } // Data rozpoczęcia

        [MaxLength(100)]
        public string Location { get; set; } // Nazwa komórki organizacyjnej
    }
}
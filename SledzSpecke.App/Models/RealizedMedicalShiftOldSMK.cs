using System;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class RealizedMedicalShiftOldSMK : RealizedMedicalShiftBase
    {
        public int Year { get; set; }
        public DateTime StartDate { get; set; }

        [MaxLength(100)]
        public string Location { get; set; }
    }
}
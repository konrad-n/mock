using SQLite;
using System;

namespace SledzSpecke.Core.Models
{
    [Table("Absences")]
    public class Absence
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int DurationDays { get; set; }

        public AbsenceType Type { get; set; }

        public string Description { get; set; }

        public bool AffectsSpecializationLength { get; set; }

        public bool IsApproved { get; set; }

        public string DocumentReference { get; set; }

        public int SpecializationId { get; set; }

        public int Year { get; set; }
    }

    public enum AbsenceType
    {
        SickLeave,            // L4
        VacationLeave,        // Urlop wypoczynkowy
        SelfEducationLeave,   // Urlop szkoleniowy (6 dni rocznie)
        MaternityLeave,       // Urlop macierzyński
        ParentalLeave,        // Urlop rodzicielski
        SpecialLeave,         // Urlop okolicznościowy
        UnpaidLeave,          // Urlop bezpłatny
        Other                 // Inne
    }
}
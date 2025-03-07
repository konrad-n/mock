using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class Module
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ModuleId { get; set; }

        [Indexed]
        public int SpecializationId { get; set; }

        public ModuleType Type { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Structure { get; set; } // JSON struktura modułu

        // Statystyki postępu - staże
        public int CompletedInternships { get; set; }

        public int TotalInternships { get; set; }

        // Statystyki postępu - kursy
        public int CompletedCourses { get; set; }

        public int TotalCourses { get; set; }

        // Statystyki postępu - procedury
        public int CompletedProceduresA { get; set; }

        public int TotalProceduresA { get; set; }

        public int CompletedProceduresB { get; set; }

        public int TotalProceduresB { get; set; }

        // Statystyki postępu - dyżury medyczne (NOWE POLA)
        public int CompletedShiftHours { get; set; }

        public int RequiredShiftHours { get; set; }

        public double WeeklyShiftHours { get; set; }

        // Statystyki postępu - samokształcenie (NOWE POLA)
        public int CompletedSelfEducationDays { get; set; }

        public int TotalSelfEducationDays { get; set; }
    }
}
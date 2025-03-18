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

        [Indexed]
        public SmkVersion SmkVersion { get; set; }

        public string Version { get; set; } // np. "CMKP 2014" lub "CMKP 2023"

        [MaxLength(100)]
        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Structure { get; set; }

        public int CompletedInternships { get; set; }

        public int TotalInternships { get; set; }

        public int CompletedCourses { get; set; }

        public int TotalCourses { get; set; }

        public int CompletedProceduresA { get; set; }

        public int TotalProceduresA { get; set; }

        public int CompletedProceduresB { get; set; }

        public int TotalProceduresB { get; set; }

        public int CompletedShiftHours { get; set; }

        public int RequiredShiftHours { get; set; }

        public double WeeklyShiftHours { get; set; }

        public int CompletedSelfEducationDays { get; set; }

        public int TotalSelfEducationDays { get; set; }
    }
}
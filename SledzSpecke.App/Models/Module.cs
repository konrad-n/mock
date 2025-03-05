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
        public string? Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string? Structure { get; set; } // JSON struktura modułu

        // Statystyki postępu
        public int CompletedInternships { get; set; }

        public int TotalInternships { get; set; }

        public int CompletedCourses { get; set; }

        public int TotalCourses { get; set; }

        public int CompletedProceduresA { get; set; }

        public int TotalProceduresA { get; set; }

        public int CompletedProceduresB { get; set; }

        public int TotalProceduresB { get; set; }
    }
}

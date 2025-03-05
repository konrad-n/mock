using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class SpecializationProgram
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ProgramId { get; set; }

        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(20)]
        public string? Code { get; set; }

        public string? Structure { get; set; } // JSON zawierający całą oryginalną strukturę programu

        public SmkVersion SmkVersion { get; set; }

        public bool HasModules { get; set; }

        // Program modułu podstawowego (jeśli specjalizacja ma moduły)
        public string? BasicModuleCode { get; set; }

        // Domyślna długość modułu podstawowego w miesiącach (najczęściej 24)
        public int BasicModuleDurationMonths { get; set; }

        // Domyślna długość całej specjalizacji w miesiącach
        public int TotalDurationMonths { get; set; }
    }
}

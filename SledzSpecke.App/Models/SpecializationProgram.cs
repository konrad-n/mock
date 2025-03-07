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
        public string Name { get; set; }

        [MaxLength(20)]
        public string Code { get; set; }

        public string Version { get; set; }

        public string Structure { get; set; } // JSON zawierający całą oryginalną strukturę programu

        public SmkVersion SmkVersion { get; set; }

        // Program może mieć lub nie mieć modułu podstawowego
        public bool HasBasicModule { get; set; }

        // Kod modułu podstawowego (jeśli występuje)
        public string BasicModuleCode { get; set; }

        // Kod modułu specjalistycznego (zawsze występuje)
        public string SpecialisticModuleCode { get; set; }

        // Domyślna długość całej specjalizacji
        public TotalDuration Duration { get; set; }

        // Całkowita liczba dni roboczych
        public int TotalWorkingDays { get; set; }
    }
}

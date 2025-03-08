using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!

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

        // Podstawowe typy do przechowywania w bazie danych
        public int DurationYears { get; set; }
        public int DurationMonths { get; set; }
        public int DurationDays { get; set; }

        // Całkowita liczba dni roboczych
        public int TotalWorkingDays { get; set; }

        // Domyślna długość całej specjalizacji
        [Ignore]
        public TotalDuration Duration
        {
            get
            {
                return new TotalDuration
                {
                    Years = this.DurationYears,
                    Months = this.DurationMonths,
                    Days = this.DurationDays
                };
            }
            set
            {
                if (value != null)
                {
                    this.DurationYears = value.Years;
                    this.DurationMonths = value.Months;
                    this.DurationDays = value.Days;
                }
            }
        }
    }
}
using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class Absence
    {
        [PrimaryKey]
        [AutoIncrement]
        public int AbsenceId { get; set; }

        [Indexed]
        public int SpecializationId { get; set; }

        public AbsenceType Type { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string? Description { get; set; }

        public SyncStatus SyncStatus { get; set; }

        public string? AdditionalFields { get; set; } // JSON

        // Liczba dni (automatycznie obliczana)
        [Ignore]
        public int DaysCount => (this.EndDate - this.StartDate).Days + 1;

        // Czy wpływa na przedłużenie specjalizacji
        [Ignore]
        public bool ExtendsSpecialization =>
            this.Type == AbsenceType.Sick ||
            this.Type == AbsenceType.Maternity ||
            this.Type == AbsenceType.Paternity;
    }
}

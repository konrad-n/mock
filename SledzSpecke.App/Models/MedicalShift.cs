using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!

    /// <summary>
    /// Rozszerzenie modelu MedicalShift o pola związane z zatwierdzaniem
    /// </summary>
    public class MedicalShift
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ShiftId { get; set; }

        [Indexed]
        public int InternshipId { get; set; } // Powiązanie ze stażem

        public DateTime Date { get; set; }

        public int Hours { get; set; }

        public int Minutes { get; set; }

        [MaxLength(100)]
        public string Location { get; set; }

        public int Year { get; set; }

        public SyncStatus SyncStatus { get; set; }

        public string AdditionalFields { get; set; } // JSON

        // Nowe pola związane z zatwierdzaniem

        /// <summary>
        /// Data zatwierdzenia dyżuru
        /// </summary>
        public DateTime? ApprovalDate { get; set; }

        /// <summary>
        /// Imię i nazwisko osoby zatwierdzającej
        /// </summary>
        [MaxLength(100)]
        public string ApproverName { get; set; }

        /// <summary>
        /// Funkcja osoby zatwierdzającej (np. Kierownik Specjalizacji)
        /// </summary>
        [MaxLength(100)]
        public string ApproverRole { get; set; }

        /// <summary>
        /// Czy dyżur został zatwierdzony
        /// </summary>
        public bool IsApproved
        {
            get
            {
                return this.SyncStatus == SyncStatus.Synced && this.ApprovalDate.HasValue;
            }
        }

        /// <summary>
        /// Czy dyżur może być usunięty (tylko niezatwierdzone)
        /// </summary>
        public bool CanBeDeleted
        {
            get
            {
                return !this.IsApproved;
            }
        }
    }
}
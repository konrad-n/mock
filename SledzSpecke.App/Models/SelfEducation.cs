using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class SelfEducation
    {
        [PrimaryKey]
        [AutoIncrement]
        public int SelfEducationId { get; set; }

        [Indexed]
        public int SpecializationId { get; set; }

        // Dodane pole dla modułu
        [Indexed]
        public int? ModuleId { get; set; }

        public int Year { get; set; }

        [MaxLength(50)]
        public string? Type { get; set; }

        [MaxLength(100)]
        public string? Title { get; set; }

        [MaxLength(100)]
        public string? Publisher { get; set; }

        public SyncStatus SyncStatus { get; set; }

        public string? AdditionalFields { get; set; } // JSON
    }
}

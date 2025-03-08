using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!

    public class EducationalActivity
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ActivityId { get; set; }

        [Indexed]
        public int SpecializationId { get; set; }

        [Indexed]
        public int? ModuleId { get; set; }

        public EducationalActivityType Type { get; set; }

        [MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public SyncStatus SyncStatus { get; set; }
    }
}

using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class Recognition
    {
        [PrimaryKey]
        [AutoIncrement]
        public int RecognitionId { get; set; }

        [Indexed]
        public int SpecializationId { get; set; }

        public RecognitionType Type { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int DaysReduction { get; set; } // O ile dni skraca specjalizację

        public SyncStatus SyncStatus { get; set; }
    }
}

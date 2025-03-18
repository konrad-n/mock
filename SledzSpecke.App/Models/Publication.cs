using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public class Publication
    {
        [PrimaryKey]
        [AutoIncrement]
        public int PublicationId { get; set; }

        [Indexed]
        public int SpecializationId { get; set; }

        [Indexed]
        public int? ModuleId { get; set; }

        public string Description { get; set; }

        public string FilePath { get; set; }

        public SyncStatus SyncStatus { get; set; }

        public string AdditionalFields { get; set; }
    }
}

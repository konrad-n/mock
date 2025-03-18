using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public abstract class RealizedProcedureBase
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ProcedureId { get; set; }

        [Indexed]
        public int SpecializationId { get; set; }

        public DateTime Date { get; set; }
        public SyncStatus SyncStatus { get; set; }
        public string AdditionalFields { get; set; }
    }
}
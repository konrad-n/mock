using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public abstract class RealizedMedicalShiftBase
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ShiftId { get; set; }

        [Indexed]
        public int SpecializationId { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public SyncStatus SyncStatus { get; set; }

        [Ignore]
        public string FormattedTime => $"{this.Hours} godz. {this.Minutes} min.";
    }
}
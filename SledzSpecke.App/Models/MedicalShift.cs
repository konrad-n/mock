using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    // ZAKAZ MODYFIKACJI!!!! JEST TO MODEL 1 DO 1 Z JSON!!!
    public class MedicalShift
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ShiftId { get; set; }

        [Indexed]
        public int InternshipId { get; set; }

        public DateTime Date { get; set; }

        public int Hours { get; set; }

        public int Minutes { get; set; }

        [MaxLength(100)]
        public string Location { get; set; }

        public int Year { get; set; }

        public SyncStatus SyncStatus { get; set; }

        public string AdditionalFields { get; set; }

        public DateTime? ApprovalDate { get; set; }

        [MaxLength(100)]
        public string ApproverName { get; set; }

        [MaxLength(100)]
        public string ApproverRole { get; set; }

        public bool IsApproved
        {
            get
            {
                return this.SyncStatus == SyncStatus.Synced && this.ApprovalDate.HasValue;
            }
        }
        public bool CanBeDeleted
        {
            get
            {
                return !this.IsApproved;
            }
        }
    }
}
using SledzSpecke.App.Models.Enums;
using SQLite;

namespace SledzSpecke.App.Models
{
    public abstract class RealizedInternshipBase
    {
        [PrimaryKey]
        [AutoIncrement]
        public int RealizedInternshipId { get; set; }

        [Indexed]
        public int SpecializationId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int DaysCount { get; set; }

        public SyncStatus SyncStatus { get; set; }

        [MaxLength(100)]
        public string InstitutionName { get; set; }

        [MaxLength(100)]
        public string DepartmentName { get; set; }

        [MaxLength(100)]
        public string SupervisorName { get; set; }

        public string AdditionalFields { get; set; }

        [Ignore]
        public string DateRange => $"{this.StartDate:dd.MM.yyyy} - {this.EndDate:dd.MM.yyyy}";
    }
}
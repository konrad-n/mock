using System;
using SledzSpecke.Core.Models.Enums;
using SQLite;

namespace SledzSpecke.Core.Models
{
    [Table("DutyShifts")]
    public class DutyShift
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public double DurationHours { get; set; }

        [Ignore]
        public int DurationHoursInt => (int)Math.Floor(this.DurationHours);

        [Ignore]
        public int DurationMinutes => (int)Math.Round((this.DurationHours - this.DurationHoursInt) * 60);

        public DutyType Type { get; set; }

        public string Location { get; set; }

        public string SupervisorName { get; set; }

        public string Notes { get; set; }

        [Indexed]
        public int? InternshipId { get; set; }

        public int SpecializationId { get; set; }
        public string DepartmentName { get; set; }
    }
}
using SQLite;
using System;
using System.ComponentModel.DataAnnotations;

namespace SledzSpecke.Core.Models
{
    [Table("DutyShifts")]
    public class DutyShift
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        public double DurationHours { get; set; }

        public DutyType Type { get; set; }

        public string Location { get; set; }

        public string SupervisorName { get; set; }

        public string Notes { get; set; }

        [Indexed]
        public int? InternshipId { get; set; }

        public int SpecializationId { get; set; }
    }
}
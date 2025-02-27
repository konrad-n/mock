using System;

namespace SledzSpecke.Core.Models
{
    public class DutyShift
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double DurationHours { get; set; }
        public DutyType Type { get; set; }
        public string Location { get; set; }
        public string SupervisorName { get; set; }
        public string Notes { get; set; }
        public int? InternshipId { get; set; }
    }

    public enum DutyType
    {
        Accompanied,
        Independent
    }
}

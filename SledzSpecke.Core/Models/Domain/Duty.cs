using SledzSpecke.Core.Models.Enums;
using System;

namespace SledzSpecke.Core.Models.Domain
{
    public class Duty : BaseEntity
    {
        public int UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Location { get; set; }
        public DutyType Type { get; set; }
        public int? SupervisorId { get; set; }
        public string Notes { get; set; }
        public bool IsConfirmed { get; set; }
        public decimal DurationInHours { get; set; }

        // Właściwości nawigacyjne
        public User User { get; set; }
        public User Supervisor { get; set; }
    }
}

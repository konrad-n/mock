using System.Collections.Generic;

namespace SledzSpecke.Core.Models.Domain
{
    public class DutyStatistics
    {
        public decimal TotalHours { get; set; }
        public decimal SupervisedHours { get; set; }
        public decimal EmergencyHours { get; set; }
        public decimal WeekendHours { get; set; }
        public decimal RemainingHours { get; set; }
        public Dictionary<string, decimal> HoursByMonth { get; set; }
    }
}

using SledzSpecke.Core.Models.Enums;
using System;

namespace SledzSpecke.Core.Models.Dto
{
    public class DutyDto
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Location { get; set; }
        public DutyType Type { get; set; }
        public int? SupervisorId { get; set; }
        public string Notes { get; set; }
    }
}

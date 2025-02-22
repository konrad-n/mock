using System;

namespace SledzSpecke.Core.Models.Dto
{
    public class InternshipDto
    {
        public int DefinitionId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Location { get; set; }
        public int? SupervisorId { get; set; }
        public string Notes { get; set; }
    }
}

using System;
namespace SledzSpecke.Core.Models.Dto
{
    public class CourseDto
    {
        public int DefinitionId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Location { get; set; }
        public string Organizer { get; set; }
        public string Notes { get; set; }
    }
}

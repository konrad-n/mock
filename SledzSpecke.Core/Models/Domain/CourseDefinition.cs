using System.Collections.Generic;

namespace SledzSpecke.Core.Models.Domain
{
    public class CourseDefinition : BaseEntity
    {
        public int SpecializationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DurationInHours { get; set; }
        public bool IsRequired { get; set; }
        public bool CanBeRemote { get; set; }
        public int RecommendedYear { get; set; }
        public string Requirements { get; set; }
        public string LearningObjectives { get; set; }

        // Właściwości nawigacyjne
        public Specialization Specialization { get; set; }
        public ICollection<Course> Courses { get; set; }
    }
}

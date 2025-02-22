using System;
using System.Collections.Generic;

namespace SledzSpecke.Core.Models.Domain
{
    public class Specialization : BaseEntity
    {
        public string Name { get; set; }
        public int DurationInWeeks { get; set; }
        public string ProgramVersion { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string Requirements { get; set; }
        public double MinimumDutyHours { get; set; }
        public string Description { get; set; }

        // Właściwości nawigacyjne
        public ICollection<User> Users { get; set; }
        public ICollection<Course> RequiredCourses { get; set; }
        public ICollection<Internship> RequiredInternships { get; set; }
    }
}
